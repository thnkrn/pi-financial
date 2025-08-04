package dto

type Watchlist struct {
	Id       string `json:"id"`
	Venue    string `json:"venue"`
	Symbol   string `json:"symbol"`
	Sequence int    `json:"sequence"`
}

type (
	OptWatchlistRequest struct {
		Venue  string `json:"venue" validate:"required"`
		Opt    string `json:"opt" validate:"required,oneof=add delete"`
		Symbol string `json:"symbol" validate:"required"`
	}
	OptWatchlistResponse struct {
	}
)

type (
	GetWatchlistParam struct {
		Venue string `query:"venue"`
	}
)
