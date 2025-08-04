# syntax=docker.io/docker/dockerfile:1.7-labs
FROM golang:1.23.1-alpine AS build_base

WORKDIR /temp

COPY go.mod go.sum ./

RUN go mod download && go mod verify

COPY --parents **/*.go .

# Build the Go app
RUN CGO_ENABLED=0 GOOS=linux GOARCH=amd64 go build -o /temp/bin/server -v ./cmd

# Start fresh from a smaller image
FROM gcr.io/distroless/static-debian12:nonroot

COPY --from=build_base /temp/bin/server .

EXPOSE 8080

CMD ["./server"]