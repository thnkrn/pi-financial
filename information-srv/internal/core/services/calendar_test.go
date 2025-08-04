package services

import (
	"context"
	"testing"
	"time"

	"github.com/stretchr/testify/suite"
	"go.uber.org/mock/gomock"

	"github.com/pi-financial/information-srv/internal/core/domain/calendar"
	mockLog "github.com/pi-financial/information-srv/mocks/log_mock"
	mockPorts "github.com/pi-financial/information-srv/mocks/ports_mock"
)

type HolidaysSuite struct {
	suite.Suite
	ctx              context.Context
	service          CalendarService
	mockLogger       *mockLog.MockLogger
	mockCacheRepo    *mockPorts.MockCacheRepository
	mockCalendarRepo *mockPorts.MockCalendarRepository
}

func (s *HolidaysSuite) SetupTest() {
	ctrl := gomock.NewController(s.T())
	defer ctrl.Finish()

	s.ctx = context.Background()

	// Mock Logger
	s.mockLogger = mockLog.NewMockLogger(ctrl)
	s.mockLogger.EXPECT().Debug(gomock.Any()).AnyTimes()
	s.mockLogger.EXPECT().Error(gomock.Any()).AnyTimes()
	s.mockLogger.EXPECT().Fatal(gomock.Any()).AnyTimes()
	s.mockLogger.EXPECT().Info(gomock.Any()).AnyTimes()
	s.mockLogger.EXPECT().Warn(gomock.Any()).AnyTimes()

	// Mock Redis
	s.mockCacheRepo = mockPorts.NewMockCacheRepository(ctrl)
	s.mockCacheRepo.EXPECT().Get(gomock.Any()).Return("", nil).AnyTimes()
	s.mockCacheRepo.EXPECT().Set(gomock.Any(), gomock.Any(), gomock.Any()).Return(nil).AnyTimes()

	// Mock Calendar Repository
	s.mockCalendarRepo = mockPorts.NewMockCalendarRepository(ctrl)
	date1 := calendar.Holiday{
		Name: "Holiday-1",
		Date: "2025-01-01",
	}
	date2 := calendar.Holiday{
		Name: "Holiday-2",
		Date: "2025-01-06",
	}
	s.mockCalendarRepo.EXPECT().
		GetHolidays(gomock.Any(), gomock.Any()).
		Return([]calendar.Holiday{date1, date2}, nil).
		AnyTimes()

	s.service = *NewCalendarService(s.mockCalendarRepo, s.mockCacheRepo, s.mockLogger)
}

func TestHolidaysSuite(t *testing.T) {
	suite.Run(t, new(HolidaysSuite))
}

func (t *HolidaysSuite) Test_GetHolidays() {
	t.Run("ShouldGetSingleRecord", func() {
		holidays, _ := t.service.GetHolidays(t.ctx, 2025)
		t.Equal(2, len(holidays))
	})

}

func (t *HolidaysSuite) Test_GetNextBusinessDay() {
	t.Run("ShouldReturnNextDay", func() {
		inputDate, _ := time.Parse("2006-01-02", "2025-01-02")
		expected, _ := time.Parse("2006-01-02", "2025-01-03")
		actual, _ := t.service.GetNextBusinessDay(t.ctx, inputDate)
		t.Equal(expected, *actual)
	})

	t.Run("ShouldSkipHoliday", func() {
		inputDate, _ := time.Parse("2006-01-02", "2024-12-31")
		expected, _ := time.Parse("2006-01-02", "2025-01-02")
		actual, _ := t.service.GetNextBusinessDay(t.ctx, inputDate)
		t.Equal(expected, *actual)
	})

	t.Run("ShouldSkipWeekend", func() {
		inputDate, _ := time.Parse("2006-01-02", "2025-01-03")
		expected, _ := time.Parse("2006-01-02", "2025-01-07")
		actual, _ := t.service.GetNextBusinessDay(t.ctx, inputDate)
		t.Equal(expected, *actual)
	})
}
func (t *HolidaysSuite) Test_IsHoliday() {
	t.Run("ValidHoliday", func() {
		date, _ := time.Parse("2006-01-02", "2025-01-01")
		isHoliday, _ := t.service.IsHoliday(t.ctx, date)
		t.Equal(true, isHoliday)
	})

	t.Run("InvalidHoliday", func() {
		date, _ := time.Parse("2006-01-02", "2025-01-02")
		isHoliday, _ := t.service.IsHoliday(t.ctx, date)
		t.Equal(false, isHoliday)
	})

	t.Run("Weekend", func() {
		date, _ := time.Parse("2006-01-02", "2025-01-04")
		isHoliday, _ := t.service.IsHoliday(t.ctx, date)
		t.Equal(true, isHoliday)
	})
}
