package service

import (
	"context"
	"errors"
	"testing"

	"github.com/google/uuid"
	"github.com/pi-financial/user-srv-v2/internal/domain"
	"github.com/pi-financial/user-srv-v2/internal/dto"
	mockclient "github.com/pi-financial/user-srv-v2/mock/driver/client"
	mockvendor "github.com/pi-financial/user-srv-v2/mock/mock_vendor"
	mockrepository "github.com/pi-financial/user-srv-v2/mock/repository"
	"github.com/stretchr/testify/suite"
	"go.uber.org/mock/gomock"
)

type AddressTestSuite struct {
	suite.Suite
	mockAddressRepo  *mockrepository.MockAddressRepository
	mockItDataClient *mockclient.MockItDataClient
	mockUserInfoRepo *mockrepository.MockUserInfoRepository
	mockLogger       *mockvendor.MockLogger
	service          AddressService
	ctx              context.Context
}

func (s *AddressTestSuite) SetupTest() {
	ctrl := gomock.NewController(s.T())
	defer ctrl.Finish()

	s.mockAddressRepo = mockrepository.NewMockAddressRepository(ctrl)
	s.mockItDataClient = mockclient.NewMockItDataClient(ctrl)
	s.mockUserInfoRepo = mockrepository.NewMockUserInfoRepository(ctrl)
	s.mockLogger = mockvendor.NewMockLogger(ctrl)
	s.service = AddressService{
		AddressRepo:  s.mockAddressRepo,
		ItDataClient: s.mockItDataClient,
		UserInfoRepo: s.mockUserInfoRepo,
		Log:          s.mockLogger,
	}
	s.ctx = context.Background()
}

func TestAddressService(t *testing.T) {
	suite.Run(t, new(AddressTestSuite))
}

func (s *AddressTestSuite) TestGetAddress() {
	id := uuid.New()
	testCases := []struct {
		name     string
		userId   string
		setup    func()
		expected *dto.Address
		wantErr  bool
	}{
		{
			name:   "should return ErrAddressNotFound when no record",
			userId: "notfound",
			setup: func() {
				s.mockAddressRepo.EXPECT().FindByUserId(s.ctx, "notfound").Return(nil, errors.New("db error"))
			},
			expected: nil,
			wantErr:  true,
		}, {
			name:   "should return data when has a record",
			userId: "found",
			setup: func() {
				mockData := &domain.Address{
					Id:           id,
					Place:        "place",
					HomeNo:       "home",
					Town:         "town",
					Building:     "building",
					Village:      "village",
					Floor:        "floor",
					Soi:          "soi",
					Road:         "road",
					SubDistrict:  "SubDistrict",
					District:     "district",
					Province:     "province",
					Country:      "country",
					ZipCode:      "zipcode",
					CountryCode:  "country_code",
					ProvinceCode: "province_code",
				}
				s.mockAddressRepo.EXPECT().FindByUserId(s.ctx, "found").Return(mockData, nil)
			},
			expected: &dto.Address{
				Place:        "place",
				HomeNo:       "home",
				Town:         "town",
				Building:     "building",
				Village:      "village",
				Floor:        "floor",
				Soi:          "soi",
				Road:         "road",
				SubDistrict:  "SubDistrict",
				District:     "district",
				Province:     "province",
				Country:      "country",
				ZipCode:      "zipcode",
				CountryCode:  "country_code",
				ProvinceCode: "province_code",
			},
			wantErr: false,
		},
	}

	for _, tc := range testCases {
		s.Run(tc.name, func() {
			tc.setup()

			result, err := s.service.GetAddressByUserId(s.ctx, tc.userId)

			if tc.wantErr {
				s.Error(err)
			} else {
				s.NoError(err)
				s.Equal(tc.expected, result)
			}
		})
	}
}

func (s *AddressTestSuite) TestUpsertAddress() {
	testCases := []struct {
		name     string
		userId   string
		address  *dto.Address
		setup    func()
		expected error
		wantErr  bool
	}{
		{
			name:   "should create a new address when address record not found",
			userId: "ddb811a0-5e53-4cd7-935e-bc9ed42cace0",
			address: &dto.Address{
				Place:        "place",
				HomeNo:       "home",
				Town:         "town",
				Building:     "building",
				Village:      "village",
				Floor:        "floor",
				Soi:          "soi",
				Road:         "road",
				SubDistrict:  "SubDistrict",
				District:     "district",
				Province:     "province",
				Country:      "country",
				ZipCode:      "zipcode",
				CountryCode:  "country_code",
				ProvinceCode: "province_code",
			},
			setup: func() {
				s.mockAddressRepo.EXPECT().FindByUserId(s.ctx, "ddb811a0-5e53-4cd7-935e-bc9ed42cace0").Return(nil, errors.New("no record"))
				s.mockAddressRepo.EXPECT().Create(s.ctx, gomock.Any()).Return(nil)
			},
			expected: nil,
			wantErr:  false,
		},
		{
			name:   "should update address when has address record",
			userId: "ddb811a0-5e53-4cd7-935e-bc9ed42cace0",
			address: &dto.Address{
				Place:        "place",
				HomeNo:       "home",
				Town:         "town",
				Building:     "building",
				Village:      "village",
				Floor:        "floor",
				Soi:          "soi",
				Road:         "road",
				SubDistrict:  "SubDistrict",
				District:     "district",
				Province:     "province",
				Country:      "country",
				ZipCode:      "zipcode",
				CountryCode:  "country_code",
				ProvinceCode: "province_code",
			},
			setup: func() {
				mockData := &domain.Address{
					Id:           uuid.New(),
					UserId:       uuid.New(),
					Place:        "place",
					HomeNo:       "home",
					Town:         "town",
					Building:     "building",
					Village:      "village",
					Floor:        "floor",
					Soi:          "soi",
					Road:         "road",
					SubDistrict:  "SubDistrict",
					District:     "district",
					Province:     "province",
					Country:      "country",
					ZipCode:      "zipcode",
					CountryCode:  "country_code",
					ProvinceCode: "province_code",
				}
				s.mockAddressRepo.EXPECT().FindByUserId(s.ctx, gomock.Any()).Return(mockData, nil)
				s.mockAddressRepo.EXPECT().Update(s.ctx, gomock.Any()).Return(nil)
			},
			expected: nil,
			wantErr:  false,
		},
	}

	for _, tc := range testCases {
		s.Run(tc.name, func() {
			tc.setup()
			err := s.service.UpsertAddress(s.ctx, tc.userId, tc.address)
			if tc.wantErr {
				s.Error(err)
			} else {
				s.NoError(err)
				s.Equal(tc.expected, err)
			}
		})
	}
}
