package domain

import (
	"github.com/google/uuid"
	"gorm.io/gorm"
)

type Example struct {
	gorm.Model
	ID         uuid.UUID
	Name       string
	Age        string
	Versioning int
}

func NewExample(id uuid.UUID, name, age string) Example {
	return Example{
		ID:         id,
		Name:       name,
		Age:        age,
		Versioning: 1,
	}
}

func (u *Example) Version() int {
	return u.Versioning
}

func (u *Example) SetVersion(i int) {
	u.Versioning = i
}
