package error

type BadRequestError struct {
	err error
}

func (e *BadRequestError) Error() string {
	return e.err.Error()
}

func NewBadRequestError(err error) error {
	return &BadRequestError{err}
}
