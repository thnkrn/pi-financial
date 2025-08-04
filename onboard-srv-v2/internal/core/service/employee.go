package service

import (
	"context"
	"fmt"

	client "github.com/pi-financial/onboard-srv-v2/internal/client/dto"
	"github.com/pi-financial/onboard-srv-v2/internal/core/port"
)

type employeeService struct {
	EmployeeClient port.EmployeeClient
	Log            port.Logger
}

func NewEmployeeService(
	EmployeeClient port.EmployeeClient,
	Log port.Logger) port.EmployeeService {
	return &employeeService{
		EmployeeClient: EmployeeClient,
		Log:            Log,
	}
}

func (s *employeeService) GetEmployeeInfoById(ctx context.Context, employeeId string) (_ *client.EmployeeInfo, err error) {
	defer func() {
		if err != nil {
			err = fmt.Errorf("in GetEmployeeInfoById: %w", err)
		}
	}()

	defaultToEmpty := func(s *string) string {
		if s != nil {
			return *s
		}
		return ""
	}

	resp, err := s.EmployeeClient.GetEmployeeInfoById(ctx, employeeId)
	if err != nil {
		return nil, fmt.Errorf("get employee info for %q: %w", employeeId, err)
	}

	if resp == nil {
		return nil, nil
	}

	return &client.EmployeeInfo{
		Id:            defaultToEmpty(resp.Id.Get()),
		DivisionCode:  defaultToEmpty(resp.DivisionCode.Get()),
		NameTh:        defaultToEmpty(resp.NameTh.Get()),
		NameEn:        defaultToEmpty(resp.NameEn.Get()),
		EffectiveDate: resp.EffectiveDate.Get(),
		CloseDate:     resp.CloseDate.Get(),
		TeamId:        resp.TeamId.Get(),
		Email:         resp.Email.Get(),
	}, nil
}
