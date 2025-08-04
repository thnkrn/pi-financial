package service

import (
	"context"
	"fmt"

	client "github.com/pi-financial/onboard-srv-v2/internal/client/dto"
	"github.com/pi-financial/onboard-srv-v2/internal/core/port"
)

type userSrvV2Service struct {
	UserSrvV2Client port.UserSrvV2Client
	Log             port.Logger
}

func NewUserSrvV2Service(
	UserSrvV2Client port.UserSrvV2Client,
	Log port.Logger) port.UserSrvV2Service {
	return &userSrvV2Service{
		UserSrvV2Client: UserSrvV2Client,
		Log:             Log,
	}
}

func (s *userSrvV2Service) GetTradingAccountWithMarketingInfoByCustomerCodes(ctx context.Context, customerCodes []string) (_ []client.TradingAccountsMarketingInfo, err error) {
	defer func() {
		if err != nil {
			err = fmt.Errorf("in GetTradingAccountWithMarketingInfoByCustomerCodes: %w", err)
		}
	}()

	defaultToEmpty := func(s *string) string {
		if s != nil {
			return *s
		}
		return ""
	}

	resp, err := s.UserSrvV2Client.GetTradingAccountWithMarketingInfoByCustomerCodes(ctx, customerCodes)
	if err != nil {
		return nil, fmt.Errorf("get trading accounts with marketing id for customer codes %q: %w", customerCodes, err)
	}

	result := []client.TradingAccountsMarketingInfo{}
	for _, item := range resp {
		result = append(result, client.TradingAccountsMarketingInfo{
			Id:               defaultToEmpty(item.Id),
			TradingAccountNo: defaultToEmpty(item.TradingAccountNo),
			AccountType:      defaultToEmpty(item.AccountType),
			AccountTypeCode:  defaultToEmpty(item.AccountTypeCode),
			ExchangeMarketId: defaultToEmpty(item.ExchangeMarketId),
			MarketingId:      defaultToEmpty(item.MarketingId),
			EndDate:          defaultToEmpty(item.EndDate),
		})
	}

	return result, nil
}

func (s *userSrvV2Service) GetUserInfoByCustomerCode(ctx context.Context, customerCode string) (_ []client.UserInfo, err error) {
	defer func() {
		if err != nil {
			err = fmt.Errorf("in GetUserInfoByCustomerCode: %w", err)
		}
	}()

	defaultToEmpty := func(s *string) string {
		if s != nil {
			return *s
		}
		return ""
	}

	defaultToFalse := func(b *bool) bool {
		if b != nil {
			return *b
		}
		return false
	}

	resp, err := s.UserSrvV2Client.GetUserInfoByCustomerCode(ctx, customerCode)
	if err != nil {
		return nil, fmt.Errorf("get user info for customer codes %q: %w", customerCode, err)
	}

	result := []client.UserInfo{}
	for _, item := range resp {

		devices := []client.UserInfoDevice{}
		for _, d := range item.Devices {

			notificationPreferences := client.UserInfoNotificationPreference{}
			if d.NotificationPreference != nil {
				notificationPreferences = client.UserInfoNotificationPreference{
					Important: defaultToFalse(d.NotificationPreference.Important),
					Market:    defaultToFalse(d.NotificationPreference.Market),
					Order:     defaultToFalse(d.NotificationPreference.Order),
					Portfolio: defaultToFalse(d.NotificationPreference.Portfolio),
					Wallet:    defaultToFalse(d.NotificationPreference.Wallet),
				}
			}

			devices = append(devices, client.UserInfoDevice{
				DeviceId:               defaultToEmpty(d.DeviceId),
				DeviceIdentifier:       defaultToEmpty(d.DeviceIdentifier),
				DeviceToken:            defaultToEmpty(d.DeviceToken),
				Language:               defaultToEmpty(d.Language),
				NotificationPreference: notificationPreferences,
				Platform:               defaultToEmpty(d.Platform),
			})
		}

		result = append(result, client.UserInfo{
			CitizenId:       defaultToEmpty(item.CitizenId),
			CustCodes:       item.CustCodes,
			DateOfBirth:     defaultToEmpty(item.DateOfBirth),
			Devices:         devices,
			Email:           defaultToEmpty(item.Email),
			FirstnameEn:     defaultToEmpty(item.FirstnameEn),
			FirstnameTh:     defaultToEmpty(item.FirstnameTh),
			Id:              defaultToEmpty(item.Id),
			LastnameEn:      defaultToEmpty(item.LastnameEn),
			LastnameTh:      defaultToEmpty(item.LastnameTh),
			PhoneNumber:     defaultToEmpty(item.PhoneNumber),
			TradingAccounts: item.TradingAccounts,
			WealthType:      defaultToEmpty(item.WealthType),
		})
	}

	return result, nil
}
