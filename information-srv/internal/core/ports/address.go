package ports

import (
	"context"

	"github.com/pi-financial/information-srv/internal/core/domain/address"
)

type AddressRepository interface {
	GetProvinces(ctx context.Context) ([]address.Province, error)
	GetAddresses(ctx context.Context) ([]address.Address, error)
	GetAddressesByZipCode(ctx context.Context, zipCode int) ([]address.Address, error)
	GetAddressesByProvince(ctx context.Context, province string, lang string) ([]address.Address, error)
}

type AddressService interface {
	GetProvinces(ctx context.Context) ([]address.Province, error)
	GetAddresses(ctx context.Context) ([]address.Address, error)
	GetAddressesByZipCode(ctx context.Context, zipCode int) ([]address.Address, error)
	GetAddressesByProvince(ctx context.Context, province string, lang string) ([]address.Address, error)
}
