SHELL := /bin/bash

.PHONY: all build test deps deps-cleancache

GOCMD=go
BUILD_DIR=build
BINARY_DIR=$(BUILD_DIR)/bin
CODE_COVERAGE=code-coverage
DOCKERCMD=docker
DOCKER_IMAGE_NAME=go-boilerplate

${BINARY_DIR}:
	mkdir -p $(BINARY_DIR)

init:
	brew install pre-commit
	brew install golangci-lint
	brew upgrade golangci-lint

	@echo "== pre-commit setup =="
	pre-commit install

	@echo "== install reflex1 =="
	$(GOCMD) install github.com/google/wire/cmd/wire@latest
	$(GOCMD) install github.com/swaggo/swag/cmd/swag@latest
	$(GOCMD) install github.com/cespare/reflex@latest
	$(GOCMD) install github.com/go-delve/delve/cmd/dlv@latest
	$(GOCMD) install go.uber.org/mock/mockgen@latest

# Run `pre-commit clean` if golangci-lint aren't using the latest config
precommit.rehooks:
	pre-commit autoupdate
	pre-commit install --install-hooks
	pre-commit install --hook-type commit-msg

ci.lint:
	@echo "== 🙆 ci.linter =="
	golangci-lint run -v ./... --fix

build: ## Compile the code, build Executable File
	$(GOCMD) build -o ./$(BINARY_DIR) -v ./cmd

run: ## Start application
	$(GOCMD) run ./cmd

test: ## Run tests
	$(GOCMD) test ./... -cover -v

test-coverage: ## Run tests and generate coverage file
	$(GOCMD) test ./... -coverprofile=../$(CODE_COVERAGE).out
	$(GOCMD) tool cover -html=../$(CODE_COVERAGE).out

deps: ## Install dependencies
	$(GOCMD) mod tidy && \
	$(GOCMD) mod download && \
	$(GOCMD) mod verify && \
	$(GOCMD) mod vendor && \
	$(GOCMD) fmt ./... \

deps-cleancache: ## Clear cache in Go module
	$(GOCMD) clean -modcache

mockery: ## Generate mock package for testing
	cd internal/core/port && mockery --all

docker-build: ## Build docker image with default setting and platform
	$(DOCKERCMD) build -t $(DOCKER_IMAGE_NAME) . --no-cache

docker-run: ## Run docker image
	$(DOCKERCMD) run --rm -it -p 8080:8080 $(DOCKER_IMAGE_NAME)

docker-compose-run: ## Run docker image with postgres database in the contianer
	$(DOCKERCMD) compose up --build

help: ## Display this help screen
	@grep -h -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFIsLE_LIST) | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}'

migrate-new:
	atlas migrate new --env local

migrate-hash:
	atlas migrate hash --env local

migrate-diff:
	atlas migrate diff --env local

migrate-apply:
	atlas migrate apply --env local

gen-swagger:
	docker run --rm -v "$(PWD):/code" ghcr.io/swaggo/swag:latest
	swag init -g cmd/main.go --parseDependency --parseInternal
	swag fmt

wire: ## Generate wire_gen.go
	cd di && wire

test:
	@echo "== 🦸‍️ ci.tester =="
	go test -v ./... -tags dynamic -coverprofile=coverage.out -covermode=atomic
