SHELL := /bin/bash

.PHONY: all build test deps deps-cleancache

GOCMD=go
BUILD_DIR=build
BINARY_DIR=$(BUILD_DIR)/bin
CODE_COVERAGE=code-coverage
DOCKERCMD=docker
DOCKER_IMAGE_NAME=user-srv-v2

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

test:
	@echo "== 🦸‍️ ci.tester =="
	go test -v ./... -tags dynamic -coverprofile=coverage.out -covermode=atomic

run: ## Start application with watch mode
	reflex -r '(\.go$$|go\.mod)' -s $(GOCMD) run ./cmd

doc:
	swag init -d cmd,internal/handler --parseDependency --parseInternal && $(GOCMD) run ./cmd/swaggerToOpenApi/main.go

wire: ## Generate wire_gen.go
	cd di && wire

gen-client:
	rm -f -R ${PWD}/client
	docker run --rm -v "${PWD}:/local" openapitools/openapi-generator-cli generate \
		-g go \
		-i /local/docs/swagger.yaml \
		-o /local/client \
		-c /local/docs/client-generator-config.json \
		--git-user-id pi-financial \
		--git-repo-id user-srv-v2/client
	cd client && $(GOCMD) mod tidy && $(GOCMD) fmt ./... && goimports -w .

migrate:
	atlas migrate diff --env local

migrate-apply:
	atlas migrate apply --env local
