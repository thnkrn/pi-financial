package repository

import (
	"context"

	"github.com/google/uuid"
	"github.com/jinzhu/copier"
	"gorm.io/gorm"

	domain "github.com/pi-financial/go-boilerplate/internal/domain"
	interfaces "github.com/pi-financial/go-boilerplate/internal/repository/interfaces"
)

type PExample struct {
	ID   uuid.UUID `gorm:"type:UUID;primaryKey"`
	Name string    `gorm:"not null"`
	Age  string    `gorm:"not null"`
	VersionModel
}

func NewPExample(e domain.Example) PExample {
	var pExample PExample
	copier.Copy(&pExample, &e)

	return pExample
}

func (e *PExample) ToExample() domain.Example {
	model := domain.NewExample(e.ID, e.Name, e.Age)
	model.SetVersion(e.Versioning)

	return model
}

type exampleDatabase struct {
	DB *gorm.DB
}

func NewExampleRepository(DB *gorm.DB) interfaces.ExampleRepository {
	return &exampleDatabase{DB}
}

func (e *exampleDatabase) FindAll(ctx context.Context) ([]domain.Example, error) {
	var pExamples []PExample
	tx := e.DB.Find(&pExamples)

	examples := make([]domain.Example, len(pExamples))
	for i, v := range pExamples {
		examples[i] = v.ToExample()
	}

	return examples, tx.Error
}

func (e *exampleDatabase) FindByID(ctx context.Context, id string) (domain.Example, error) {
	var pExample PExample
	tx := e.DB.Where("id = ?", id).Find(&pExample)

	return pExample.ToExample(), tx.Error
}

func (e *exampleDatabase) Create(ctx context.Context, example domain.Example) (domain.Example, error) {
	pExample := NewPExample(example)
	tx := e.DB.Create(pExample)

	return example, tx.Error
}

func (e *exampleDatabase) Delete(ctx context.Context, example domain.Example) error {
	pExample := NewPExample(example)
	tx := e.DB.Delete(pExample)

	return tx.Error
}

func (e *exampleDatabase) UpdateByID(ctx context.Context, id string, example domain.Example) (domain.Example, error) {
	pExample := NewPExample(example)

	// NOTE: without optimistic lock
	// tx := e.DB.Model(&example).Where("id = ?", id).Updates(pExample)
	// if tx.Error != nil {
	// 	return example, tx.Error
	// }

	err := UpdateWithLock(e.DB, &pExample)
	if err != nil {
		return pExample.ToExample(), err
	}

	tx := e.DB.Where("id = ?", id).Find(&pExample)

	return pExample.ToExample(), tx.Error
}
