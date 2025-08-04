package pkg

type Response struct {
	Title  string      `json:"title"`
	Data   interface{} `json:"data"`
	Detail string      `json:"detail"`
	Status int         `json:"status"`
}
