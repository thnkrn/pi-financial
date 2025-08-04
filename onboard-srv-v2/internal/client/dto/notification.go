package client

import "github.com/pi-financial/onboard-srv-v2/internal/handler/dto"

type NotificationTicket struct {
	TicketId string `json:"id"`
}

type SendEmailRequest struct {
	UserId       string     `json:"userId"`
	CustomerCode string     `json:"customerCode"`
	Recipents    []string   `json:"recipents"`
	TemplateId   int64      `json:"templateId"`
	Language     dto.Locale `json:"language"`
	TitlePayload []string   `json:"titlePayload"`
	BodyPayload  []string   `json:"bodyPayload"`
}

type CustomerCode string
type MarketingId string

type SendEmailRequestData struct {
	UserId       string
	CustomerCode string
	Recipents    []string
	TemplateId   int64
	Language     dto.Locale
	TitlePayload []string
	BodyPayload  SendEmailRequestBodyPayloadData
}

type SendEmailRequestBodyPayloadData struct {
	CustomerFirstName string
	CustomerFullName  string
	CustomerMobileNo  string
	MT5Set            string
	MT5Tfex           string
	MT4               string
	MarketingId       string
	EmployeeFullName  string
	MarketingEmail    string
}
