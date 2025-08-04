package service

import (
	"context"
	"errors"
	"strings"
	"time"

	"fmt"

	"github.com/google/uuid"
	commondatabase "github.com/pi-financial/go-common/database"
	goclient "github.com/pi-financial/it-data-api-client/go-client"
	constants "github.com/pi-financial/user-srv-v2/const"
	"github.com/pi-financial/user-srv-v2/internal/domain"
	clientinterfaces "github.com/pi-financial/user-srv-v2/internal/driver/client/interfaces"
	"github.com/pi-financial/user-srv-v2/internal/dto"
	repoiface "github.com/pi-financial/user-srv-v2/internal/repository/interfaces"
	"github.com/pi-financial/user-srv-v2/internal/service/interfaces"
	"github.com/samber/lo"
	"gorm.io/gorm"
)

var ErrNoChange = errors.New("no changes detected")

type ChangeRequestService struct {
	ChangeRequestRepo     repoiface.ChangeRequestRepository
	AuditLogRepo          repoiface.AuditLogRepository
	ChangeRequestInfoRepo repoiface.ChangeRequestInfoRepository
	UserInfoRepo          repoiface.UserInfoRepository
	ItDataClient          clientinterfaces.ItDataClient
	DopaClient            clientinterfaces.DopaClient
	DocumentRepo          repoiface.DocumentRepository
	S3Client              clientinterfaces.S3Client
}

func NewChangeRequestService(
	changeRequestRepo repoiface.ChangeRequestRepository,
	auditLogRepo repoiface.AuditLogRepository,
	changeRequestInfoRepo repoiface.ChangeRequestInfoRepository,
	userInfoRepo repoiface.UserInfoRepository,
	itDataClient clientinterfaces.ItDataClient,
	dopaClient clientinterfaces.DopaClient,
	documentRepo repoiface.DocumentRepository,
	s3Client clientinterfaces.S3Client,
) interfaces.ChangeRequestService {
	return &ChangeRequestService{
		ChangeRequestRepo:     changeRequestRepo,
		AuditLogRepo:          auditLogRepo,
		ChangeRequestInfoRepo: changeRequestInfoRepo,
		UserInfoRepo:          userInfoRepo,
		ItDataClient:          itDataClient,
		DopaClient:            dopaClient,
		DocumentRepo:          documentRepo,
		S3Client:              s3Client,
	}
}

// ProcessChangeRequest processes a change request, checks for changes, and adds an audit log.
func (s *ChangeRequestService) ProcessChangeRequest(ctx context.Context, req dto.CreateChangeRequireInfoRequest) error {

	// Process change request infos and validate if there are any changes
	changeRequestInfos, changeRequestInfoErr := s.processChangeRequestInfos(ctx, req)
	if changeRequestInfoErr != nil {
		return changeRequestInfoErr
	}

	changeRequest := domain.ChangeRequest{
		UserId:    uuid.MustParse(req.UserID),
		InfoType:  domain.ChangeRequestInfoType(req.InfoType),
		Status:    domain.PendingStatus,
		MakerId:   req.MakerID,
		MakerName: req.MakerName,
	}

	changeRequestId, err := s.ChangeRequestRepo.Create(ctx, &changeRequest)
	if err != nil {
		return err
	}

	for i := range changeRequestInfos {
		changeRequestInfos[i].ChangeRequestId = *changeRequestId
	}

	if len(changeRequestInfos) != 0 {
		if err := s.ChangeRequestInfoRepo.CreateBatch(ctx, changeRequestInfos); err != nil {
			return err
		}
	}

	auditLog := &domain.AuditLog{
		ChangeRequestId: *changeRequestId,
		Action:          domain.CreateAction,
		Actor:           req.MakerName,
	}
	if _, err := s.AuditLogRepo.Create(ctx, auditLog); err != nil {
		return err
	}

	return nil
}

func (s *ChangeRequestService) InsertAuditAction(ctx context.Context, changeRequestId uuid.UUID, req dto.AuditAction) (*uuid.UUID, error) {
	//find change request
	existed, err := s.ChangeRequestRepo.FindById(ctx, changeRequestId)
	if err != nil {
		return nil, err
	}
	if existed == nil {
		return nil, ErrNoChange
	}

	request := &domain.AuditLog{
		ChangeRequestId: changeRequestId,
		Action:          req.Action,
		Actor:           req.CheckerId,
		Note:            req.Note,
	}
	id, err := s.AuditLogRepo.Create(ctx, request)
	if err != nil {
		return nil, err
	}
	existed.Status = domain.ChangeRequestStatus(req.Action)
	existed.CheckerId = req.CheckerId
	existed.CheckerName = req.CheckerName
	if err := s.ChangeRequestRepo.Update(ctx, existed); err != nil {
		return nil, err
	}

	if existed.Status == domain.ApprovedStatus && (existed.InfoType == domain.Signature || existed.InfoType == domain.IdCardInfo || existed.InfoType == domain.BankAccountInfo) {
		var fileName string
		switch existed.InfoType {
		case domain.Signature:
			fileName = "signatureImage"
		case domain.IdCardInfo:
			fileName = "citizenImage"
		case domain.BankAccountInfo:
			fileName = "bookBank"
		default:
			fileName = ""
		}

		changeRequestInfo, err := s.ChangeRequestInfoRepo.FindByChangeRequestIdAndFieldName(ctx, changeRequestId, &fileName)
		if err != nil {
			return nil, err
		}

		sourceBucket := "change-request-temp-doc"
		destBucket := "onboard-document"
		if err := s.S3Client.CopyObject(ctx, changeRequestInfo.ChangeValue, sourceBucket, destBucket); err != nil {
			return nil, err
		}

		// create documentType var base on existed.InfoType
		var documentType domain.DocumentType
		switch existed.InfoType {
		case domain.Signature:
			documentType = domain.DocumentTypeSignature
		case domain.IdCardInfo:
			documentType = domain.DocumentTypeCitizenCard
		case domain.BankAccountInfo:
			documentType = domain.DocumentTypeBookBank
		default:
			documentType = ""
		}

		if err := s.DocumentRepo.Create(ctx, &domain.Document{
			UserId:       existed.UserId,
			DocumentType: documentType,
			FileUrl:      "https://" + destBucket + ".s3.ap-southeast-1.amazonaws.com/" + changeRequestInfo.ChangeValue,
			FileName:     changeRequestInfo.ChangeValue,
		}); err != nil {
			return nil, err
		}
	}

	return id, nil
}

func (s *ChangeRequestService) processChangeRequestInfos(ctx context.Context, req dto.CreateChangeRequireInfoRequest) ([]domain.ChangeRequestInfo, error) {
	userInfo, err := s.UserInfoRepo.FindById(ctx, req.UserID)
	if err != nil {
		return nil, err
	}

	changeRequestInfos := []domain.ChangeRequestInfo{}

	if req.InfoType == domain.Signature {
		return changeRequestInfos, nil
	}

	switch req.InfoType {
	case domain.ContactInfo, domain.IdCardInfo:
		var (
			citizenId *string
			firstName *string
			lastName  *string
			birthDay  *string
			laserCode *string
		)
		customerInfo, err := s.ItDataClient.GetCustomerInfo(ctx, &userInfo.CitizenId, nil)
		if err != nil || len(customerInfo) == 0 {
			return nil, constants.ErrFindingUserInfo
		}

		changeRequestInfos = lo.Map(req.Infos, func(item dto.ChangeRequestInfo, _ int) domain.ChangeRequestInfo {
			switch item.FieldName {
			case "citizenId":
				citizenId = &item.FieldValue
			case "firstNameTh":
				firstName = &item.FieldValue
			case "lastNameTh":
				lastName = &item.FieldValue
			case "dateOfBirth":
				birthDay = &item.FieldValue
			case "laserCode":
				laserCode = &item.FieldValue
			}
			return domain.ChangeRequestInfo{
				FieldName:    item.FieldName,
				CurrentValue: getCurrentValueFromItCustomerInfo(item, customerInfo[0], userInfo),
				ChangeValue:  item.FieldValue,
			}
		})

		if req.InfoType == domain.IdCardInfo {
			resp, verifyErr := s.DopaClient.VerifyByLaserCode(ctx, citizenId, firstName, lastName, birthDay, laserCode)
			if verifyErr != nil || (resp != nil && (strings.Contains(*resp.Desc, "บัตรหมดอายุ") || *resp.Code != "0")) {
				return nil, constants.ErrDopaVerify
			}
		}
	case domain.IdCardAddressInfo, domain.CurrentAddress, domain.WorkplaceAddress:
		addressInfo, err := s.ItDataClient.GetAddress(ctx, &userInfo.CitizenId, nil)
		if err != nil || len(addressInfo) == 0 {
			return nil, constants.ErrAddressNotFound
		}
		idCardAddress, _ := lo.Find(addressInfo, func(item goclient.DatumAddrInfoModel) bool {
			return item.GetAddrtype() == "2"
		})
		currentAddress, _ := lo.Find(addressInfo, func(item goclient.DatumAddrInfoModel) bool {
			return item.GetAddrtype() == "1"
		})
		workplaceAddress, _ := lo.Find(addressInfo, func(item goclient.DatumAddrInfoModel) bool {
			return item.GetAddrtype() == "4"
		})

		sourceAddress := idCardAddress
		switch req.InfoType {
		case domain.CurrentAddress:
			sourceAddress = currentAddress
		case domain.WorkplaceAddress:
			sourceAddress = workplaceAddress
		}

		changeRequestInfos = lo.Map(req.Infos, func(item dto.ChangeRequestInfo, _ int) domain.ChangeRequestInfo {
			return domain.ChangeRequestInfo{
				FieldName:    item.FieldName,
				CurrentValue: getCurrentValueFromItAddressInfo(item, sourceAddress),
				ChangeValue:  item.FieldValue,
			}
		})
	case domain.IncomeSourceAndInvestmentPurpose, domain.Occupation:
		customerInfoOthers, err := s.ItDataClient.GetCustomerInfoOthers(ctx, &userInfo.CitizenId, nil)
		if err != nil || len(customerInfoOthers) == 0 {
			return nil, constants.ErrFindingUserInfo
		}
		kycInfo, err := s.ItDataClient.GetKyc(ctx, &userInfo.CitizenId, nil)
		if err != nil || len(kycInfo) == 0 {
			return nil, constants.ErrKycNotFound
		}
		changeRequestInfos = lo.Map(req.Infos, func(item dto.ChangeRequestInfo, _ int) domain.ChangeRequestInfo {
			return domain.ChangeRequestInfo{
				FieldName:    item.FieldName,
				CurrentValue: getCurrentValueFromItCustomerInfoOthers(item, customerInfoOthers[0], kycInfo[0]),
				ChangeValue:  item.FieldValue,
			}
		})
	case domain.Declaration:
		kycInfo, err := s.ItDataClient.GetKyc(ctx, &userInfo.CitizenId, nil)
		if err != nil || len(kycInfo) == 0 {
			return nil, constants.ErrKycNotFound
		}
		changeRequestInfos = lo.Map(req.Infos, func(item dto.ChangeRequestInfo, _ int) domain.ChangeRequestInfo {
			return domain.ChangeRequestInfo{
				FieldName:    item.FieldName,
				CurrentValue: getCurrentValueFromItKyc(item, kycInfo[0]),
				ChangeValue:  item.FieldValue,
			}
		})
	case domain.SuitabilityTestResult:
		freewillAccount, _ := lo.Find(userInfo.Accounts, func(item domain.UserAccount) bool {
			return item.UserAccountType == domain.Freewill
		})
		suittestChoice, err := s.ItDataClient.GetSuitChoice(ctx, nil, &freewillAccount.Id)
		if err != nil || len(suittestChoice) == 0 {
			return nil, constants.ErrItDataSrvGetSuitTest
		}
		changeRequestInfos = lo.Map(req.Infos, func(item dto.ChangeRequestInfo, _ int) domain.ChangeRequestInfo {
			return domain.ChangeRequestInfo{
				FieldName:    item.FieldName,
				CurrentValue: getCurrentValueFromItSuitabilityTest(item, suittestChoice),
				ChangeValue:  item.FieldValue,
			}
		})
	case domain.BankAccountInfo:
		freewillAccount, _ := lo.Find(userInfo.Accounts, func(item domain.UserAccount) bool {
			return item.UserAccountType == domain.Freewill
		})
		bankAccounts, err := s.ItDataClient.GetAtsBankAccounts(ctx, freewillAccount.Id)
		if err != nil || len(bankAccounts) == 0 {
			return nil, constants.ErrItDataSrvGetAtsBankAccounts
		}
		changeRequestInfos = lo.Map(req.Infos, func(item dto.ChangeRequestInfo, _ int) domain.ChangeRequestInfo {
			return domain.ChangeRequestInfo{
				FieldName:    item.FieldName,
				CurrentValue: getCurrentValueFromItBankAccount(item, bankAccounts[0]),
				ChangeValue:  item.FieldValue,
			}
		})
	default:
		return nil, fmt.Errorf("unsupported info type: %s", req.InfoType)
	}

	// Check if at least one current value is different from change value
	hasChanges := lo.SomeBy(changeRequestInfos, func(item domain.ChangeRequestInfo) bool {
		return item.CurrentValue != item.ChangeValue
	})

	if !hasChanges {
		return nil, errors.New("no changes detected: all current values match change values")
	}
	return changeRequestInfos, nil
}

func getCurrentValueFromItCustomerInfo(item dto.ChangeRequestInfo, customerInfo goclient.DatumCustInfoV2, userInfo *domain.UserInfo) string {
	switch item.FieldName {
	// Customer Info
	case "phone":
		return userInfo.PhoneNumber
	case "email":
		return customerInfo.GetEmail()
	// Id Card Info
	case "citizenId":
		return customerInfo.GetCardid()
	case "titleEn":
		return customerInfo.GetEtitle()
	case "firstNameEn":
		return customerInfo.GetEname()
	case "lastNameEn":
		return customerInfo.GetEsurname()
	case "titleTh":
		return customerInfo.GetTtitle()
	case "firstNameTh":
		return customerInfo.GetTname()
	case "lastNameTh":
		return customerInfo.GetTsurname()
	case "dateOfBirth":
		return customerInfo.GetBirthday()
	case "idCardExpiredDate":
		return customerInfo.GetCardexpire()
	case "laserCode":
		return customerInfo.GetLasercode()
	}

	return ""
}

func getCurrentValueFromItAddressInfo(item dto.ChangeRequestInfo, addressInfo goclient.DatumAddrInfoModel) string {
	switch item.FieldName {
	case "houseNo":
		return addressInfo.GetHomeno()
	case "moo":
		return addressInfo.GetTown()

	case "building":
		return addressInfo.GetBuilding()
	case "village":
		return addressInfo.GetVillage()
	case "soi":
		return addressInfo.GetSoi()
	case "road":
		return addressInfo.GetRoad()
	case "subDistrict":
		return addressInfo.GetSubdistrict()
	case "district":
		return addressInfo.GetDistrict()
	case "province":
		return addressInfo.GetProvincedesc()
	case "postalCode":
		return addressInfo.GetZipcode()
	}

	return ""
}

func getCurrentValueFromItCustomerInfoOthers(item dto.ChangeRequestInfo, customerInfoOthers goclient.DatasCustInfoOthers, kycInfo goclient.KycDetail) string {
	switch item.FieldName {
	case "occupation":
		return kycInfo.GetOccupationcode()
	case "jobTitle":
		return customerInfoOthers.GetPosition()
	case "income":
		return customerInfoOthers.GetIncome()
	case "incomeSource":
		return customerInfoOthers.GetInvestsource()
	case "investmentPurpose":
		return customerInfoOthers.GetInvestpurpose()
	}

	return ""
}

func getCurrentValueFromItKyc(item dto.ChangeRequestInfo, kycInfo goclient.KycDetail) string {
	switch item.FieldName {
	case "politicalFlag":
		return kycInfo.GetPoliticalflag()
	case "launderFlag":
		return kycInfo.GetLaunderflag()
	case "deniedTransactionFlag":
		return kycInfo.GetPersonfinanceflag()
	}

	return ""
}

func getCurrentValueFromItSuitabilityTest(req dto.ChangeRequestInfo, suittestChoice []goclient.SuitChoiceDetail) string {
	questionAnswer, ok := lo.Find(suittestChoice, func(item goclient.SuitChoiceDetail) bool {
		return item.GetQuestioncode() == req.FieldName
	})

	if !ok {
		return ""
	}

	return questionAnswer.GetChoicecode()
}

func getCurrentValueFromItBankAccount(req dto.ChangeRequestInfo, bankAccount goclient.AtsInfoDetail) string {
	switch req.FieldName {
	case "bankAccountNo":
		return bankAccount.GetBankaccno()
	case "bankCode":
		return bankAccount.GetBankcode()
	case "bankBranchCode":
		return bankAccount.GetBankbranchcode()
	}

	return ""
}

func (s *ChangeRequestService) GetChangeRequest(ctx context.Context, req dto.GetChangeRequestParams) (*dto.GetChangeRequestResponse, error) {
	filters := &domain.ChangeRequest{
		InfoType: req.InfoType,
		Status:   req.Status,
	}
	if req.Date != "" {
		createdAt, err := time.Parse(time.DateOnly, req.Date)
		if err != nil {
			return nil, err
		}
		filters.CreatedAt = createdAt
	}

	data, err := s.ChangeRequestRepo.FindByWithPagination(ctx, filters, commondatabase.PaginationParams{
		Page:  req.Page,
		Limit: req.Limit,
	})
	if err != nil {
		return nil, err
	}

	return &dto.GetChangeRequestResponse{
		Limit:      data.Limit,
		Page:       data.Page,
		ItemCount:  data.ItemCount,
		TotalPages: data.TotalPages,
		TotalItems: data.TotalItems,
		Items: lo.Map(data.Items, func(item domain.ChangeRequest, _ int) dto.ChangeRequest {
			return s.mapChangeRequest(&item, nil)
		}),
	}, nil
}

func (s *ChangeRequestService) GetChangeRequestById(ctx context.Context, id uuid.UUID) (*dto.GetChangeRequestByIdResponse, error) {
	changeRequest, err := s.ChangeRequestRepo.FindById(ctx, id)
	if err != nil {
		if errors.Is(err, gorm.ErrRecordNotFound) {
			return nil, constants.ErrChangeRequestNotFound
		}

		return nil, err
	}

	infos, err := s.ChangeRequestInfoRepo.GetByChangeRequestId(ctx, changeRequest.Id)
	if err != nil {
		return nil, err
	}

	return &dto.GetChangeRequestByIdResponse{
		ChangeRequest: s.mapChangeRequest(changeRequest, infos),
	}, nil
}

func (s *ChangeRequestService) GetChangeRequestAction(ctx context.Context, changeRequestId uuid.UUID, req dto.GetChangeRequestActionParams) ([]dto.GetChangeRequestActionResponse, error) {
	filters := &domain.AuditLog{
		Action: req.Action,
	}
	if req.Date != "" {
		actionTime, err := time.Parse(time.DateOnly, req.Date)
		if err != nil {
			return nil, err
		}
		filters.ActionTime = actionTime
	}

	actions, err := s.AuditLogRepo.FindByChangeRequestId(ctx, changeRequestId, filters)
	if err != nil {
		return nil, err
	}

	return lo.Map(actions, func(item domain.AuditLog, _ int) dto.GetChangeRequestActionResponse {
		return dto.GetChangeRequestActionResponse{
			AuditLog: dto.AuditLog{
				Id:        item.Id.String(),
				InfoType:  item.ChangeRequest.InfoType.String(),
				Maker:     item.Actor,
				Action:    item.Action.String(),
				Note:      item.Note,
				CreatedAt: item.ActionTime.Format(time.RFC3339),
			},
		}
	}), nil
}

func (s *ChangeRequestService) mapChangeRequest(changeRequest *domain.ChangeRequest, infos []domain.ChangeRequestInfo) dto.ChangeRequest {
	return dto.ChangeRequest{
		Id:       changeRequest.Id.String(),
		UserId:   changeRequest.UserId.String(),
		Status:   changeRequest.Status.String(),
		InfoType: changeRequest.InfoType.String(),
		Infos: lo.Map(infos, func(info domain.ChangeRequestInfo, _ int) dto.ChangeRequestInfo {
			return dto.ChangeRequestInfo{
				FieldName:  info.FieldName,
				FieldValue: info.ChangeValue,
			}
		}),
		CustomerCodes: lo.Map(changeRequest.UserInfo.Accounts, func(item domain.UserAccount, _ int) string {
			return item.Id
		}),
		CitizenId: changeRequest.UserInfo.CitizenId,
		Maker:     changeRequest.MakerName,
		Checker:   changeRequest.CheckerName,
		CreatedAt: changeRequest.CreatedAt.Format(time.DateTime),
	}
}
