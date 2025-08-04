package user_v2_repository

import (
	"context"
	"errors"

	constants "github.com/pi-financial/pi-sso-v2/const"
	"github.com/pi-financial/pi-sso-v2/internal/domain"
	"github.com/pi-financial/pi-sso-v2/internal/log"
	userv2service "github.com/pi-financial/user-srv-v2/client"
	"github.com/samber/lo"
	"go.uber.org/zap"
)

type UserV2Repository struct {
	userAPIClientV2 userv2service.APIClient
	logger          log.Logger
}

func NewUserV2Repository(logger log.Logger, userAPIClientV2 userv2service.APIClient) *UserV2Repository {
	return &UserV2Repository{
		userAPIClientV2: userAPIClientV2,
		logger:          logger,
	}
}

func (r *UserV2Repository) Sync(ctx context.Context, customerCode string, syncType SyncType) error {
	_, _, err := r.userAPIClientV2.UserAPI.InternalV1UsersSyncPost(ctx).CustomerCode(customerCode).SyncType(string(syncType)).Execute()
	if err != nil {
		r.logger.Error(ctx, "userRepository.Sync Error Sync: %v\n", zap.Error(err))
		return err
	}

	return nil
}

func (r *UserV2Repository) GetUserByCustomerCode(ctx context.Context, customerCode string) (*userv2service.DtoUserInfo, error) {
	user, _, err := r.userAPIClientV2.UserAPI.InternalV1UsersGet(ctx).AccountId(customerCode).Execute()
	if err != nil {
		r.logger.Error(ctx, "userRepository.GetUserByCustomerCode Error Get: %v\n", zap.Error(err))
		return nil, err
	}

	if len(user.Data) == 0 {
		return nil, errors.New("user not found")
	}

	return &user.Data[0], nil
}

func (r *UserV2Repository) CreateUserAccount(ctx context.Context, userId, customerCode string) error {
	defaultStatus := userv2service.NormalUserAccountStatus

	_, _, err := r.userAPIClientV2.UserAccountAPI.InternalV1UserAccountPost(ctx).UserId(userId).DtoLinkUserAccountRequest(userv2service.DtoLinkUserAccountRequest{
		Status:          &defaultStatus,
		UserAccountId:   customerCode,
		UserAccountType: userv2service.DomainUserAccountType("Freewill"),
	}).Execute()
	if err != nil {
		r.logger.Error(ctx, "userRepository.CreateUserAccount Error Create: %v\n", zap.Error(err))
		return err
	}

	return nil
}

// FindById ค้นหาผู้ใช้ด้วย Id
func (r *UserV2Repository) FindById(Id string) (*domain.User, error) {
	req := r.userAPIClientV2.UserAPI.InternalV1UsersGet(context.Background()).Ids(Id)

	resp, _, err := r.userAPIClientV2.UserAPI.InternalV1UsersGetExecute(req)

	if err != nil {
		return nil, err
	}

	if len(resp.Data) == 0 {
		return nil, constants.ErrUserNotFound
	}

	return &domain.User{
		Id:          resp.Data[0].GetId(),
		IdCardNo:    resp.Data[0].CitizenId,
		FirstNameTh: resp.Data[0].FirstnameTh,
		LastNameTh:  resp.Data[0].LastnameTh,
		FirstNameEn: resp.Data[0].FirstnameEn,
		LastNameEn:  resp.Data[0].LastnameEn,
		Email:       resp.Data[0].Email,
		Phone:       resp.Data[0].PhoneNumber,
		Birthday:    resp.Data[0].DateOfBirth,
	}, nil
}

// FindByPhoneNumber ค้นหาผู้ใช้ด้วย Id
func (r *UserV2Repository) FindByPhoneNumber(ctx context.Context, phoneNumber string) (*domain.User, error) {
	req := r.userAPIClientV2.UserAPI.InternalV1UsersGet(context.Background()).PhoneNumber(phoneNumber)

	resp, _, err := r.userAPIClientV2.UserAPI.InternalV1UsersGetExecute(req)

	if len(resp.Data) == 0 || err != nil {
		return nil, constants.ErrPhoneNotFound
	}

	return &domain.User{
		Id:          resp.Data[0].GetId(),
		IdCardNo:    resp.Data[0].CitizenId,
		FirstNameTh: resp.Data[0].FirstnameTh,
		LastNameTh:  resp.Data[0].LastnameTh,
		FirstNameEn: resp.Data[0].FirstnameEn,
		LastNameEn:  resp.Data[0].LastnameEn,
		Email:       resp.Data[0].Email,
		Phone:       resp.Data[0].PhoneNumber,
	}, nil
}

// FindByEmail ค้นหาผู้ใช้ด้วย Email
func (r *UserV2Repository) FindByEmail(email *string) (*domain.User, error) {
	if email == nil {
		return nil, errors.New("Email is required")
	}
	req := r.userAPIClientV2.UserAPI.InternalV1UsersGet(context.Background()).Email(*email)

	resp, _, err := r.userAPIClientV2.UserAPI.InternalV1UsersGetExecute(req)

	if err != nil || len(resp.Data) == 0 {
		return nil, err
	}

	return &domain.User{
		Id:          resp.Data[0].GetId(),
		IdCardNo:    resp.Data[0].CitizenId,
		FirstNameTh: resp.Data[0].FirstnameTh,
		LastNameTh:  resp.Data[0].LastnameTh,
		FirstNameEn: resp.Data[0].FirstnameEn,
		LastNameEn:  resp.Data[0].LastnameEn,
		Email:       resp.Data[0].Email,
		Phone:       resp.Data[0].PhoneNumber,
	}, nil
}

func (r *UserV2Repository) FindUserIdByCustCode(ctx context.Context, custCode string) (*domain.User, error) {
	req := r.userAPIClientV2.UserAPI.InternalV1UsersGet(context.Background()).AccountId(custCode)

	resp, _, err := r.userAPIClientV2.UserAPI.InternalV1UsersGetExecute(req)

	if err != nil || len(resp.Data) == 0 {
		return nil, constants.ErrUserNotFound
	}

	return &domain.User{
		Id: resp.Data[0].GetId(),
	}, nil
}

func (r *UserV2Repository) FindByIdCardNo(ctx context.Context, idCardNo string) (*domain.User, error) {
	req := r.userAPIClientV2.UserAPI.InternalV1UsersGet(context.Background()).CitizenId(idCardNo)

	resp, _, err := r.userAPIClientV2.UserAPI.InternalV1UsersGetExecute(req)

	if err != nil || len(resp.Data) == 0 {
		return nil, constants.ErrUserNotFound
	}

	return &domain.User{
		Id:          resp.Data[0].GetId(),
		IdCardNo:    resp.Data[0].CitizenId,
		FirstNameTh: resp.Data[0].FirstnameTh,
		LastNameTh:  resp.Data[0].LastnameTh,
		FirstNameEn: resp.Data[0].FirstnameEn,
		LastNameEn:  resp.Data[0].LastnameEn,
		Email:       resp.Data[0].Email,
		Phone:       resp.Data[0].PhoneNumber,
	}, nil
}

func (r *UserV2Repository) CreateUserInfo(ctx context.Context, email *string, phoneNumber *string, idCardNo *string, firstnameTh, lastnameTh, firstnameEn, lastnameEn, wealthType *string) (*domain.User, error) {
	body := userv2service.DtoCreateUserInfoRequest{}
	if lo.IsNotNil(email) {
		body.Email = *email
	}
	if lo.IsNotNil(phoneNumber) {
		body.PhoneNumber = *phoneNumber
	}
	if lo.IsNotNil(idCardNo) {
		body.CitizenId = *idCardNo
	}
	if lo.IsNotNil(firstnameTh) {
		body.FirstnameTh = *firstnameTh
	}
	if lo.IsNotNil(lastnameTh) {
		body.LastnameTh = *lastnameTh
	}
	if lo.IsNotNil(firstnameEn) {
		body.FirstnameEn = *firstnameEn
	}
	if lo.IsNotNil(lastnameEn) {
		body.LastnameEn = *lastnameEn
	}
	if lo.IsNotNil(wealthType) {
		body.WealthType = *wealthType
	}

	req := r.userAPIClientV2.UserAPI.InternalV1UsersPost(context.Background()).DtoCreateUserInfoRequest(body)

	resp, status, err := r.userAPIClientV2.UserAPI.InternalV1UsersPostExecute(req)
	if err != nil || status.StatusCode != 200 {
		r.logger.Error(ctx, "userRepository.CreateUserInfo Error CreateUserInfo: %v\n", zap.Error(err))
		return nil, err
	}

	return &domain.User{
		Id: resp.Data.GetId(),
		//More fields
	}, nil
}

// UpdateInfoUser update information of user
// Birthday format: "2006-01-02"
func (r *UserV2Repository) UpdateInfoUser(ctx context.Context, id, idCardNo, firstNameTh, lastNameTh, firstNameEn, lastNameEn, birthday, email, phoneNumber *string) error {

	req := r.userAPIClientV2.UserAPI.InternalV1UsersPatch(context.Background()).UserId(*id).DtoPatchUserInfoRequest(userv2service.DtoPatchUserInfoRequest{
		CitizenId:   idCardNo,
		FirstnameTh: firstNameTh,
		LastnameTh:  lastNameTh,
		FirstnameEn: firstNameEn,
		LastnameEn:  lastNameEn,
		Email:       email,
		PhoneNumber: phoneNumber,
		DateOfBirth: birthday,
	})

	_, _, err := r.userAPIClientV2.UserAPI.InternalV1UsersPatchExecute(req)

	if err != nil {
		r.logger.Error(ctx, "userRepository.UpdateInfoUser Error Failed to update info", zap.Error(err))

		return err
	}

	return nil
}
