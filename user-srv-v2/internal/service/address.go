package service

import (
	"context"
	"errors"
	"fmt"

	"github.com/google/uuid"
	"github.com/pi-financial/go-common/logger"
	goclient "github.com/pi-financial/it-data-api-client/go-client"
	constants "github.com/pi-financial/user-srv-v2/const"
	"github.com/pi-financial/user-srv-v2/internal/domain"
	clientinterfaces "github.com/pi-financial/user-srv-v2/internal/driver/client/interfaces"
	"github.com/pi-financial/user-srv-v2/internal/dto"
	repointerface "github.com/pi-financial/user-srv-v2/internal/repository/interfaces"
	"github.com/pi-financial/user-srv-v2/internal/service/interfaces"
	"github.com/samber/lo"
	"gorm.io/gorm"
)

type AddressService struct {
	AddressRepo  repointerface.AddressRepository
	ItDataClient clientinterfaces.ItDataClient
	UserInfoRepo repointerface.UserInfoRepository
	Log          logger.Logger
}

func NewAddressService(addressRepo repointerface.AddressRepository, itDataClient clientinterfaces.ItDataClient, userInfoRepo repointerface.UserInfoRepository, log logger.Logger) interfaces.AddressService {
	return &AddressService{
		AddressRepo:  addressRepo,
		ItDataClient: itDataClient,
		UserInfoRepo: userInfoRepo,
		Log:          log,
	}
}

// GetAddressByUserId retrieves the address for a given user ID or create one if it does not exist.
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - userId: User ID to retrieve the address for
//
// Returns:
//   - *dto.Address: Address details for the user, or nil if not found
//   - error: Error if fetching fails (e.g., user not found, database error, IT Data service error)
//
// Implementation:
//  1. Calls AddressRepo.FindByUserId to check if the address exists in the repository.
//  2. If found, returns the address as a dto.Address.
//  3. If not found, create a new one:
//     - Calls UserInfoRepo.FindById to get user information using the user ID.
//     - Calls ItDataClient.GetAddress to fetch the address from the IT Data service using the user's citizen ID.
//     - Calls AddressRepo.UpsertByUserId if the address type "1" is found.
//     - Returns the created dto.Address.
//
// Error cases:
//   - Returns constants.ErrFindingUserInfo if the user information cannot be found
//   - Returns constants.ErrItDataSrvGetAddress if the IT Data service fails to retrieve the address
//   - Returns constants.ErrAddressNotFound if the address type "1" is not found in the IT Data service response
func (a AddressService) GetAddressByUserId(ctx context.Context, userId string) (*dto.Address, error) {

	if address, err := a.AddressRepo.FindByUserId(ctx, userId); err == nil {
		return &dto.Address{
			Place:        address.Place,
			HomeNo:       address.HomeNo,
			Town:         address.Town,
			Building:     address.Building,
			Village:      address.Village,
			Floor:        address.Floor,
			Soi:          address.Soi,
			Road:         address.Road,
			SubDistrict:  address.SubDistrict,
			District:     address.District,
			Province:     address.Province,
			Country:      address.Country,
			ZipCode:      address.ZipCode,
			CountryCode:  address.CountryCode,
			ProvinceCode: address.ProvinceCode,
		}, nil
	} else if !errors.Is(err, gorm.ErrRecordNotFound) {
		return nil, err
	}

	// Try to get address from IT Data service
	userInfo, err := a.UserInfoRepo.FindById(ctx, userId)
	if err != nil {
		return nil, constants.ErrFindingUserInfo
	}

	address, err := a.ItDataClient.GetAddress(ctx, &userInfo.CitizenId, nil)
	if err != nil {
		return nil, constants.ErrItDataSrvGetAddress
	}

	addressType1, ok := lo.Find(address, func(add goclient.DatumAddrInfoModel) bool {
		return add.GetAddrtype() == "1"
	})
	if !ok {
		return nil, constants.ErrAddressNotFound
	}

	if err := a.AddressRepo.UpsertByUserId(ctx, uuid.MustParse(userId), &domain.Address{
		UserId:       uuid.MustParse(userId),
		Place:        addressType1.GetAddr1(),
		HomeNo:       addressType1.GetHomeno(),
		Town:         addressType1.GetTown(),
		Building:     addressType1.GetBuilding(),
		Village:      addressType1.GetVillage(),
		Floor:        addressType1.GetFloor(),
		Soi:          addressType1.GetSoi(),
		Road:         addressType1.GetRoad(),
		SubDistrict:  addressType1.GetSubdistrict(),
		District:     addressType1.GetDistrict(),
		Province:     addressType1.GetProvincedesc(),
		Country:      addressType1.GetCountrydesc(),
		ZipCode:      addressType1.GetZipcode(),
		CountryCode:  addressType1.GetCtycode(),
		ProvinceCode: addressType1.GetProvcode(),
	}); err != nil {
		a.Log.Error(fmt.Sprintf("Error upserting address for user %s with citizen id %s: %v", userId, userInfo.CitizenId, err))
		return nil, err
	}

	return &dto.Address{
		Place:        addressType1.GetAddr1(),
		HomeNo:       addressType1.GetHomeno(),
		Town:         addressType1.GetTown(),
		Building:     addressType1.GetBuilding(),
		Village:      addressType1.GetVillage(),
		Floor:        addressType1.GetFloor(),
		Soi:          addressType1.GetSoi(),
		Road:         addressType1.GetRoad(),
		SubDistrict:  addressType1.GetSubdistrict(),
		District:     addressType1.GetDistrict(),
		Province:     addressType1.GetProvincedesc(),
		Country:      addressType1.GetCountrydesc(),
		ZipCode:      addressType1.GetZipcode(),
		CountryCode:  addressType1.GetCtycode(),
		ProvinceCode: addressType1.GetProvcode(),
	}, nil
}

// UpsertAddress (upserts) updates or creates an address for a user based on the provided user ID and address details.
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - userId: User ID to associate the address with
//   - address: Address details to be updated or created
//
// Returns:
//   - error: Error if the operation fails (e.g., database error, parsing error
//
// Implementation:
//  1. Calls AddressRepo.FindByUserId to check if an address already exists for the user ID.
//  2. If an address exists, it updates the existing address with the new details.
//  3. If no address exists, it creates a new address with a new UUID.
//
// Error cases:
//   - Returns error if the user ID cannot be parsed to a UUID
//   - Returns error if the database operation fails (e.g., create or update fails)
func (a AddressService) UpsertAddress(ctx context.Context, userId string, address *dto.Address) error {
	addressResult, _ := a.AddressRepo.FindByUserId(ctx, userId)
	parsedUserId, err := uuid.Parse(userId)
	if err != nil {
		return err
	}
	data := domain.Address{
		UserId:       parsedUserId,
		Place:        address.Place,
		HomeNo:       address.HomeNo,
		Town:         address.Town,
		Building:     address.Building,
		Village:      address.Village,
		Floor:        address.Floor,
		Soi:          address.Soi,
		Road:         address.Road,
		SubDistrict:  address.SubDistrict,
		District:     address.District,
		Province:     address.Province,
		Country:      address.Country,
		ZipCode:      address.ZipCode,
		CountryCode:  address.CountryCode,
		ProvinceCode: address.ProvinceCode,
	}
	if addressResult == nil {
		newAddressId := uuid.New()
		data.Id = newAddressId
		err = a.AddressRepo.Create(ctx, &data)

	} else {
		data.Id = addressResult.Id
		err = a.AddressRepo.Update(ctx, &data)
	}
	return err
}
