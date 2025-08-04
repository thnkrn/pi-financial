package error

type BusinessError struct {
	err error
}

func (e *BusinessError) Error() string {
	return e.err.Error()
}

func NewBusinessError(err error) error {
	return &BusinessError{err}
}

type NotFoundError struct {
	err error
}

func (e *NotFoundError) Error() string {
	return e.err.Error()
}

func NewNotFoundError(err error) error {
	return &NotFoundError{err}
}
