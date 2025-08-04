package services

import (
	"context"

	"github.com/pi-financial/information-srv/internal/core/domain/address"
	"github.com/pi-financial/information-srv/internal/core/ports"
	"github.com/pi-financial/information-srv/internal/driver/log"
)

type AddressService struct {
	cache  ports.CacheRepository
	logger log.Logger
	repo   ports.AddressRepository
}

func NewAddressService(repo ports.AddressRepository, cache ports.CacheRepository, logger log.Logger) *AddressService {
	return &AddressService{
		cache,
		logger,
		repo,
	}
}

func (srv *AddressService) GetProvinces(ctx context.Context) ([]address.Province, error) {
	provinces, err := srv.repo.GetProvinces(ctx)
	if err != nil {
		return nil, err
	}
	return provinces, nil
}

func (srv *AddressService) GetAddresses(ctx context.Context) ([]address.Address, error) {
	addresses, err := srv.repo.GetAddresses(ctx)
	if err != nil {
		return nil, err
	}
	return addresses, nil
}

func (srv *AddressService) GetAddressesByZipCode(ctx context.Context, zipCode int) ([]address.Address, error) {
	addresses, err := srv.repo.GetAddressesByZipCode(ctx, zipCode)
	if err != nil {
		return nil, err
	}

	return addresses, nil
}

func (srv *AddressService) GetAddressesByProvince(ctx context.Context, province string, lang string) ([]address.Address, error) {
	addresses, err := srv.repo.GetAddressesByProvince(ctx, province, lang)
	if err != nil {
		return nil, err
	}

	return addresses, nil
}
