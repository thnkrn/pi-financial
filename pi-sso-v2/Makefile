SHELL := /bin/bash

.PHONY: all build test deps deps-cleancache

GOCMD=go
BUILD_DIR=build
BINARY_DIR=$(BUILD_DIR)/bin
CODE_COVERAGE=code-coverage
DOCKERCMD=docker
DOCKER_IMAGE_NAME=pi-sso-v2

${BINARY_DIR}:
	mkdir -p $(BINARY_DIR)

build: ## Compile the code, build Executable File
	$(GOCMD) build -o ./$(BINARY_DIR) -v ./cmd

run: ## Start application
	$(GOCMD) run ./cmd

run-watch: ## Start application with watch mode
	reflex -r '(\.go$$|go\.mod)' -s $(GOCMD) run ./cmd

test: ## Run tests
	$(GOCMD) test ./... -cover -v

test-coverage: ## Run tests and generate coverage file
	$(GOCMD) test ./... -coverprofile=../$(CODE_COVERAGE).out
	$(GOCMD) tool cover -html=../$(CODE_COVERAGE).out

doc:
	swag init -d cmd,internal/handler --parseDependency --parseInternal

deps: ## Install dependencies
	$(GOCMD) install github.com/google/wire/cmd/wire@latest
	$(GOCMD) install github.com/swaggo/swag/cmd/swag@latest
	# go get $(go list -f '{{if not (or .Main .Indirect)}}{{.Path}}{{end}}' -m all)
	$(GOCMD) get -u -t -d -v ./...
	$(GOCMD) mod tidy
	$(GOCMD) mod vendor

deps-cleancache: ## Clear cache in Go module
	$(GOCMD) clean -modcache

wire: ## Generate wire_gen.go
	cd di && wire

mockery: ## Generate mock package for testing
	cd pkg/usecase && mockery --all --output=../mocks/usecase --case underscore
	cd ../../
	cd pkg/repository && mockery --all --output=../mocks/repository --case underscore

docker-build: ## Build docker image with default setting and platform
	$(DOCKERCMD) build -t $(DOCKER_IMAGE_NAME) . --no-cache

docker-run: ## Run docker image
	$(DOCKERCMD) run --rm -it -p 8080:8080 $(DOCKER_IMAGE_NAME)

docker-compose-run: ## Run docker image with postgres database in the contianer
	$(DOCKERCMD) compose up --build

help: ## Display this help screen
	@grep -h -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFIsLE_LIST) | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}'

gen-client:
	rm -f -R ${PWD}/client
	docker run --rm -v "${PWD}:/local" openapitools/openapi-generator-cli generate \
		-g go \
		-i /local/docs/swagger.yaml \
		-o /local/client \
		-c /local/docs/client-generator-config.json \
		--git-user-id pi-financial \
		--git-repo-id pi-sso-v2/client
	cd client && $(GOCMD) mod tidy && $(GOCMD) fmt ./... && goimports -w .

migrate:
	atlas migrate diff --env local

migrate-apply:
	atlas migrate apply --env local
