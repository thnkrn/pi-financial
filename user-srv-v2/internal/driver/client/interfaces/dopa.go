package interfaces

import (
	"context"

	goclient "github.com/pi-financial/dopa-client/go-client"
)

type DopaClient interface {
	VerifyByLaserCode(ctx context.Context, citizenId, firstName, lastName, birthDay, laserCode *string) (*goclient.CheckCardByLaserResponse, error)
}
