package dto

type ChannelResult[T any] struct {
	Data      T
	Err       error
	Reference string
}
