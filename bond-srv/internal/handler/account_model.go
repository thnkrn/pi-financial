package handler

import (
	domain "github.com/pi-financial/bond-srv/internal/domain/account"
	utils "github.com/pi-financial/bond-srv/utils"
)

type AccountOverviewResponse struct {
	AccountsOverview      []AccountResponse       `json:"accountsOverview"`
	FailedToFetchAccounts []FailedAccountResponse `json:"failedToFetchAccounts"`
	HasFailedAccounts     bool                    `json:"hasFailedAccounts"`
}

type AccountResponse struct {
	AccountLimit            utils.Decimal `json:"accountLimit"`
	NetAssetValue           utils.Decimal `json:"netAssetValue"`
	MarketValue             utils.Decimal `json:"marketValue"`
	Cost                    utils.Decimal `json:"cost"`
	Gain                    utils.Decimal `json:"gain"`
	GainPercentage          utils.Decimal `json:"gainPercentage"`
	CashBalance             utils.Decimal `json:"cashBalance"`
	LineAvailable           utils.Decimal `json:"lineAvailable"`
	TradingAccountNoDisplay string        `json:"tradingAccountNoDisplay"`
	TradingAccountNo        string        `json:"tradingAccountNo"`
	Currency                string        `json:"currency"`
	AccountId               string        `json:"accountId"`
	HasBondTradingAccount   bool          `json:"hasBondTradingAccount"`
}

type FailedAccountResponse struct {
	AccountId               string `json:"accountId"`               //custCode
	TradingAccountNoDisplay string `json:"tradingAccountNoDisplay"` //custCodesuffix
	TradingAccountNo        string `json:"tradingAccountNo"`        //custCode-suffix
	CustCode                string `json:"custCode"`
	Error                   string `json:"error"`
}

type AccountSummaryResponse struct {
	Currency                    string             `json:"currency"`
	TotalAmount                 utils.Decimal      `json:"totalAmount"`
	MarketValue                 utils.Decimal      `json:"marketValue"`
	CostValue                   utils.Decimal      `json:"costValue"`
	Gain                        utils.Decimal      `json:"gain"`
	GainPercentage              utils.Decimal      `json:"gainPercentage"`
	AccruedInterestToBeCredited utils.Decimal      `json:"accruedInterestToBeCredited"`
	OutstandingAccruedInterest  utils.Decimal      `json:"outstandingAccruedInterest"`
	Positions                   []PositionResponse `json:"positions"`
}

type PositionResponse struct {
	MaturityDate          utils.DateTimeMS `json:"maturityDate"`
	AsOfDate              utils.DateTimeMS `json:"asOfDate"`
	MarketValue           utils.Decimal    `json:"marketValue"`
	AvailableUnit         utils.Decimal    `json:"availableUnit"`
	DailyChange           utils.Decimal    `json:"dailyChange"`
	DailyChangePercentage utils.Decimal    `json:"dailyChangePercentage"`
	MarketPrice           utils.Decimal    `json:"marketPrice"`
	GainPercentage        utils.Decimal    `json:"gainPercentage"`
	Gain                  utils.Decimal    `json:"gain"`
	AccruedInterest       utils.Decimal    `json:"accruedInterest"`
	CostPrice             utils.Decimal    `json:"costPrice"`
	CostValue             utils.Decimal    `json:"costValue"`
	CleanPrice            utils.Decimal    `json:"cleanPrice"`
	YieldPricePercentage  utils.Decimal    `json:"yieldPrice"`
	Currency              string           `json:"currency"`
	Symbol                string           `json:"symbol"`
	AssetType             string           `json:"assetType"`
}

func NewAccountOverviewResponse(accountsOverview map[string]domain.AccountSummary) *AccountOverviewResponse {
	accountRes := []AccountResponse{}
	faliedAccountRes := []FailedAccountResponse{}
	hasFailedAccounts := false
	for custCode, acc := range accountsOverview {
		if acc.Error == "" {
			accountOverview := AccountResponse{
				AccountId:               acc.AccountId,
				TradingAccountNoDisplay: acc.AccountNo,
				TradingAccountNo:        acc.TradingAccountNo,
				Currency:                acc.Currency,
				NetAssetValue:           utils.Decimal{Value: &acc.NetAssetValue},
				MarketValue:             utils.Decimal{Value: &acc.MarketValue},
				Cost:                    utils.Decimal{Value: &acc.CostValue},
				Gain:                    utils.Decimal{Value: &acc.Gain},
				GainPercentage:          utils.Decimal{Value: &acc.GainPercentage},
				AccountLimit:            utils.Decimal{Value: &acc.AccountLimit},
				CashBalance:             utils.Decimal{Value: &acc.CashBalance},
				LineAvailable:           utils.Decimal{Value: &acc.LineAvailable},
				HasBondTradingAccount:   acc.HasBondTradingAccount,
			}
			accountRes = append(accountRes, accountOverview)
			continue
		}

		accfaliedAccountRes := FailedAccountResponse{
			AccountId:               acc.AccountId,
			TradingAccountNoDisplay: acc.AccountNo,
			TradingAccountNo:        acc.TradingAccountNo,
			CustCode:                custCode,
			Error:                   acc.Error,
		}
		faliedAccountRes = append(faliedAccountRes, accfaliedAccountRes)
		hasFailedAccounts = true
	}

	accountOverview := AccountOverviewResponse{
		AccountsOverview:      accountRes,
		FailedToFetchAccounts: faliedAccountRes,
		HasFailedAccounts:     hasFailedAccounts,
	}

	return &accountOverview
}

func NewAccountSummaryResponse(accountSum domain.AccountSummary) *AccountSummaryResponse {
	var positionRes []PositionResponse
	for _, pos := range accountSum.Position {
		position := PositionResponse{
			Symbol:                string(pos.Symbol),
			AssetType:             "Bond",
			MarketPrice:           utils.Decimal{Value: &pos.MarketPrice},
			Currency:              pos.Currency,
			DailyChange:           utils.Decimal{Value: &pos.DailyChange},
			DailyChangePercentage: utils.Decimal{Value: &pos.DailyChangePercentage},
			MaturityDate:          utils.DateTimeMS(pos.MaturityDate),
			AsOfDate:              utils.DateTimeMS(pos.AsOfDate),
			MarketValue:           utils.Decimal{Value: &pos.MarketValue},
			AvailableUnit:         utils.Decimal{Value: &pos.AvailableUnit},
			CostPrice:             utils.Decimal{Value: &pos.CostPrice},
			CostValue:             utils.Decimal{Value: &pos.CostValue},
			CleanPrice:            utils.Decimal{Value: &pos.CleanPrice},
			YieldPricePercentage:  utils.Decimal{Value: &pos.YieldPricePercentage},
			AccruedInterest:       utils.Decimal{Value: &pos.AccruedInterest},
			Gain:                  utils.Decimal{Value: &pos.Gain},
			GainPercentage:        utils.Decimal{Value: &pos.GainPercentage},
		}
		positionRes = append(positionRes, position)
	}

	return &AccountSummaryResponse{
		Currency:                    accountSum.Currency,
		TotalAmount:                 utils.Decimal{Value: &accountSum.NetAssetValue},
		MarketValue:                 utils.Decimal{Value: &accountSum.MarketValue},
		CostValue:                   utils.Decimal{Value: &accountSum.CostValue},
		Gain:                        utils.Decimal{Value: &accountSum.Gain},
		GainPercentage:              utils.Decimal{Value: &accountSum.GainPercentage},
		AccruedInterestToBeCredited: utils.Decimal{Value: &accountSum.AccruedInterestToBeCredited},
		OutstandingAccruedInterest:  utils.Decimal{Value: &accountSum.OutstandingAccruedInterest},
		Positions:                   positionRes,
	}
}
