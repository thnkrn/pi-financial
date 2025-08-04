package example

import (
	"net/http"

	"github.com/google/uuid"
	"github.com/labstack/echo/v4"

	domain "github.com/pi-financial/go-boilerplate/internal/domain"
	interfaces "github.com/pi-financial/go-boilerplate/internal/service/interfaces"
)

type ExampleHandler struct {
	exampleService interfaces.ExampleService
}

func NewExampleHandler(exampleService interfaces.ExampleService) *ExampleHandler {
	return &ExampleHandler{
		exampleService,
	}
}

func (e *ExampleHandler) FindAll(c echo.Context) error {
	users, err := e.exampleService.FindAll(c.Request().Context())

	if err != nil {
		return err
	} else {
		return c.JSON(http.StatusOK, NewExamplesResponse(users))
	}
}

func (e *ExampleHandler) FindByID(c echo.Context) error {
	paramsId := c.Param("id")

	user, err := e.exampleService.FindByID(c.Request().Context(), paramsId)
	if err != nil {
		return err
	} else {
		return c.JSON(http.StatusOK, NewExampleResponse(user))
	}
}

func (e *ExampleHandler) Create(c echo.Context) error {
	var request ExampleRequest

	if err := c.Bind(&request); err != nil {
		return err
	}

	userData := domain.NewExample(uuid.New(), request.Name, request.Age)
	user, err := e.exampleService.Create(c.Request().Context(), userData)
	if err != nil {
		return err
	} else {
		return c.JSON(http.StatusOK, NewExampleResponse(user))
	}
}

func (e *ExampleHandler) Delete(c echo.Context) error {
	paramsId := c.Param("id")
	user, err := e.exampleService.FindByID(c.Request().Context(), paramsId)
	if err != nil {
		return err
	}

	err = e.exampleService.Delete(c.Request().Context(), user)
	if err != nil {
		return err
	} else {
		return c.NoContent(http.StatusNoContent)
	}
}

func (e *ExampleHandler) Update(c echo.Context) error {
	var request ExampleRequest

	paramsId := c.Param("id")

	if err := c.Bind(&request); err != nil {
		return err
	}

	foundedUser, err := e.exampleService.FindByID(c.Request().Context(), paramsId)
	if err != nil {
		return err
	}

	userData := domain.NewExample(foundedUser.ID, request.Name, request.Age)
	userData.SetVersion(foundedUser.Versioning)

	user, err := e.exampleService.UpdateByID(c.Request().Context(), paramsId, userData)
	if err != nil {
		return err
	} else {
		return c.JSON(http.StatusOK, NewExampleResponse(user))
	}
}
