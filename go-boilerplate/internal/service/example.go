package service

import (
	"context"
	"errors"

	domain "github.com/pi-financial/go-boilerplate/internal/domain"
	iRepository "github.com/pi-financial/go-boilerplate/internal/repository/interfaces"
	iService "github.com/pi-financial/go-boilerplate/internal/service/interfaces"
)

type exampleService struct {
	exampleRepo iRepository.ExampleRepository
}

func NewExampleService(exampleRepo iRepository.ExampleRepository) iService.ExampleService {
	return &exampleService{
		exampleRepo,
	}
}

func (e *exampleService) FindAll(ctx context.Context) ([]domain.Example, error) {
	examples, err := e.exampleRepo.FindAll(ctx)
	if err == nil && len(examples) == 0 {
		return examples, errors.New("users not found")
	}
	return examples, err
}

func (e *exampleService) FindByID(ctx context.Context, id string) (domain.Example, error) {
	example, err := e.exampleRepo.FindByID(ctx, id)
	if err == nil && example == (domain.Example{}) {
		return example, errors.New("users not found")
	}

	return example, err
}

func (e *exampleService) Create(ctx context.Context, example domain.Example) (domain.Example, error) {
	example, err := e.exampleRepo.Create(ctx, example)

	return example, err
}

func (e *exampleService) Delete(ctx context.Context, example domain.Example) error {
	err := e.exampleRepo.Delete(ctx, example)

	return err
}

func (e *exampleService) UpdateByID(ctx context.Context, id string, example domain.Example) (domain.Example, error) {
	example, err := e.exampleRepo.UpdateByID(ctx, id, example)

	return example, err
}
