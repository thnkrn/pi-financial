SHELL := /bin/bash

.PHONY: all build test deps deps-cleancache

GOCMD=go
BUILD_DIR=build
BINARY_DIR=$(BUILD_DIR)/bin
CODE_COVERAGE=code-coverage
DOCKERCMD=docker
DOCKER_IMAGE_NAME=information-srv

${BINARY_DIR}:
	mkdir -p $(BINARY_DIR)

init:
	$(GOCMD) install go.uber.org/mock/mockgen@latest
	$(GOCMD) install github.com/rakyll/gotest@latest


build: ## Compile the code, build Executable File
	$(GOCMD) build -o ./$(BINARY_DIR) -v ./cmd

run: ## Start application
	$(GOCMD) run ./cmd

test: ## Run tests
	gotest ./... -cover -v

test-coverage: ## Run tests and generate coverage file
	gotest ./... -coverprofile=../$(CODE_COVERAGE).out
	$(GOCMD) tool cover -html=../$(CODE_COVERAGE).out

lint: ## Run lint
	golangci-lint run

doc:
	swag init -d cmd,internal/adapters/handler/http --parseDependency --parseInternal && $(GOCMD) run ./cmd/swaggerToOpenApi/swaggerToOpenApi.go

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
		--git-repo-id information-srv/client
	cd client && $(GOCMD) mod tidy && $(GOCMD) fmt ./... && goimports -w .




##### GoMock #####
MOCK_DIR_MAPPINGS = \
	internal/core/ports:../../../mocks/ports_mock \
	
mockgen: ## Generate mocks
	@for mapping in $(MOCK_DIR_MAPPINGS); do \
	    src_dir=$$(echo $$mapping | cut -d ':' -f1); \
	    dest_rel=$$(echo $$mapping | cut -d ':' -f2); \
	    echo "Processing $$src_dir -> $$dest_rel"; \
	    cd $$src_dir; \
	    for file in $$(ls *.go); do \
	        name=$$(basename $$file .go); \
	        mockgen -package mocks -source=$$file -destination=$$dest_rel/$$name"_mock.go"; \
	    done; \
	    cd - > /dev/null; \
	done
	mockgen -package mocks -source=./internal/driver/log/log.go -destination=./mocks/log_mock/log_mock.go;

	