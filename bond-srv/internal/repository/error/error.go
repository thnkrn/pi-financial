package error

type InternalServiceError struct {
	err error
}

func (e *InternalServiceError) Error() string {
	return e.err.Error()
}

func NewInternalServiceError(err error) error {
	return &InternalServiceError{err}
}
