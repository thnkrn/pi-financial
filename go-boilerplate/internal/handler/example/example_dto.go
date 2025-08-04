package example

import (
	"github.com/google/uuid"

	domain "github.com/pi-financial/go-boilerplate/internal/domain"
)

type ExampleRequest struct {
	Name string `json:"name" binding:"required"`
	Age  string `json:"age" binding:"required"`
}

type ExampleResponse struct {
	ID   uuid.UUID `json:"id"`
	Name string    `json:"name"`
	Age  string    `json:"age"`
}

func NewExampleResponse(example domain.Example) *ExampleResponse {
	response := ExampleResponse{
		ID:   example.ID,
		Name: example.Name,
		Age:  example.Age,
	}

	return &response
}

func NewExamplesResponse(examples []domain.Example) *[]ExampleResponse {
	response := make([]ExampleResponse, len(examples))

	for i, v := range examples {
		response[i] = ExampleResponse{
			ID:   v.ID,
			Name: v.Name,
			Age:  v.Age,
		}
	}

	return &response
}
