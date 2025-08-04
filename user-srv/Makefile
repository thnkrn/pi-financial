GOCMD=go

# Run local and copy swagger json then paste in /codegen/swagger.json
gen-client:
	rm -f -R ${PWD}/client
	docker run --rm -v "${PWD}:/local" openapitools/openapi-generator-cli generate \
		-g go \
		-i /local/codegen/swagger.json \
		-o /local/go-client \
		-c /local/codegen/go-client-generator-config.json \
		--git-user-id pi-financial \
		--git-repo-id user-srv/go-client
	cd go-client && $(GOCMD) mod tidy && $(GOCMD) fmt ./... && goimports -w .
