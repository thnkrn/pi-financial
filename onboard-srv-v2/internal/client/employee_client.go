package client

import (
	"context"
	"fmt"
	"net/url"

	goclient "github.com/pi-financial/employee-srv/go-client"
	"github.com/pi-financial/onboard-srv-v2/config"
	"github.com/pi-financial/onboard-srv-v2/internal/core/port"
)

type employeeClient struct {
	Log         port.Logger
	Config      config.Config
	EmployeeSrv *goclient.APIClient
}

func NewEmployeeClient(log port.Logger, config config.Config) port.EmployeeClient {
	host, err := url.Parse(config.Client.EmployeeSrvHost)
	log.Info(fmt.Sprintf("Employee service host: %s", host.String()))
	if err != nil {
		panic(err)
	}

	return &employeeClient{
		Log:    log,
		Config: config,
		EmployeeSrv: goclient.NewAPIClient(&goclient.Configuration{
			Scheme: "http",
			Servers: []goclient.ServerConfiguration{
				{
					URL: host.String(),
				},
			},
		}),
	}
}

func (c *employeeClient) GetEmployeeInfoById(ctx context.Context, employeeId string) (_ *goclient.PiEmployeeServiceDomainAggregatesModelEmployeeInfoAggregateEmployeeInfo, err error) {
	defer func() {
		if err != nil {
			err = fmt.Errorf("in GetEmployeeInfoById from employee-srv: %w", err)
		}
	}()

	resp, _, err := c.EmployeeSrv.EmployeeAPI.InternalGetEmployeeInfoById(ctx, employeeId).Execute()
	if err != nil {
		return nil, fmt.Errorf("get employee info for %q: %w", employeeId, err)
	}

	return resp.Data, nil
}
