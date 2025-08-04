package error

type ExternalServiceError struct {
	err error
}

func (e *ExternalServiceError) Error() string {
	return e.err.Error()
}

func NewExternalServiceError(err error) error {
	return &ExternalServiceError{err}
}

type ValidationError struct {
	err error
}

func (e *ValidationError) Error() string {
	return e.err.Error()
}

func NewValidationError(err error) error {
	return &ValidationError{err}
}
