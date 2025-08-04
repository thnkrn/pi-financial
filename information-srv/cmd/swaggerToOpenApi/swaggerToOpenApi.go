package main

import (
	"fmt"
	"gopkg.in/yaml.v2"
	"log"
	"os"
	"strings"
)

// Swagger2 represents a Swagger 2.0 specification
type Swagger2 struct {
	Swagger     string                 `yaml:"swagger"`
	Info        map[string]interface{} `yaml:"info"`
	Host        string                 `yaml:"host"`
	BasePath    string                 `yaml:"basePath"`
	Schemes     []string               `yaml:"schemes"`
	Consumes    []string               `yaml:"consumes"`
	Produces    []string               `yaml:"produces"`
	Paths       map[string]interface{} `yaml:"paths"`
	Definitions map[string]interface{} `yaml:"definitions"`
	Parameters  map[string]interface{} `yaml:"parameters"`
	Responses   map[string]interface{} `yaml:"responses"`
	Tags        []interface{}          `yaml:"tags"`
}

// OpenAPI3 represents an OpenAPI 3.0 specification
type OpenAPI3 struct {
	OpenAPI    string                 `yaml:"openapi"`
	Info       map[string]interface{} `yaml:"info"`
	Servers    []Server               `yaml:"servers,omitempty"`
	Paths      map[string]interface{} `yaml:"paths"`
	Components Components             `yaml:"components,omitempty"`
	Tags       []interface{}          `yaml:"tags,omitempty"`
}

type Server struct {
	URL         string `yaml:"url"`
	Description string `yaml:"description,omitempty"`
}

type Components struct {
	Schemas    map[string]interface{} `yaml:"schemas,omitempty"`
	Parameters map[string]interface{} `yaml:"parameters,omitempty"`
	Responses  map[string]interface{} `yaml:"responses,omitempty"`
}

func main() {
	inputFile := "docs/swagger.yaml"
	outputFile := strings.Replace(inputFile, ".yaml", ".yaml", 1)

	// Read Swagger 2.0 file
	data, err := os.ReadFile(inputFile)
	if err != nil {
		log.Fatalf("Error reading file: %v", err)
	}

	var swagger2 Swagger2
	err = yaml.Unmarshal(data, &swagger2)
	if err != nil {
		log.Fatalf("Error parsing YAML: %v", err)
	}

	// Convert to OpenAPI 3.0
	openapi3 := convertToOpenAPI3(swagger2)

	// Write OpenAPI 3.0 file
	output, err := yaml.Marshal(openapi3)
	if err != nil {
		log.Fatalf("Error marshaling to YAML: %v", err)
	}

	// Remove input file
	err = os.Remove(inputFile)
	if err != nil {
		log.Fatalf("Error removing input file: %v", err)
	}
	err = os.WriteFile(outputFile, output, 0644)
	if err != nil {
		log.Fatalf("Error writing file: %v", err)
	}

	fmt.Printf("Successfully converted %s to %s\n", inputFile, outputFile)
}

func convertToOpenAPI3(swagger2 Swagger2) OpenAPI3 {
	openapi3 := OpenAPI3{
		OpenAPI: "3.0.0",
		Info:    swagger2.Info,
		Paths:   convertPaths(swagger2.Paths, swagger2.Consumes),
		Tags:    swagger2.Tags,
	}

	// Convert servers from host, basePath, and schemes
	if swagger2.Host != "" || swagger2.BasePath != "" || len(swagger2.Schemes) > 0 {
		openapi3.Servers = convertServers(swagger2.Host, swagger2.BasePath, swagger2.Schemes)
	}

	// Convert components
	components := Components{}
	if swagger2.Definitions != nil {
		components.Schemas = convertDefinitionsToSchemas(swagger2.Definitions)
	}
	if swagger2.Parameters != nil {
		components.Parameters = convertParametersToOpenAPI3(swagger2.Parameters)
	}
	if swagger2.Responses != nil {
		convertedResponses := convertValue(swagger2.Responses)
		if responsesMap, ok := convertedResponses.(map[string]interface{}); ok {
			components.Responses = responsesMap
		} else {
			log.Fatalf("Error converting responses: unexpected type")
		}
	}

	if len(components.Schemas) > 0 || len(components.Parameters) > 0 || len(components.Responses) > 0 {
		openapi3.Components = components
	}

	return openapi3
}

func convertDefinitionsToSchemas(definitions map[string]interface{}) map[string]interface{} {
	schemas := make(map[string]interface{})

	for name, definition := range definitions {
		convertedDef := convertSchemaReferences(definition)

		// Special handling for ResponseSuccess to remove "data" property
		if name == "result.ResponseSuccess" {
			if defMap, ok := convertedDef.(map[string]interface{}); ok {
				if properties, exists := defMap["properties"]; exists {
					if propsMap, ok := properties.(map[string]interface{}); ok {
						// Remove the "data" property
						delete(propsMap, "data")
						defMap["properties"] = propsMap
						convertedDef = defMap
					}
				}
			}
		}

		schemas[name] = convertedDef
	}

	return schemas
}

func convertParametersToOpenAPI3(parameters map[string]interface{}) map[string]interface{} {
	convertedParams := make(map[string]interface{})

	for name, param := range parameters {
		if paramMap, ok := param.(map[interface{}]interface{}); ok {
			convertedParam := make(map[string]interface{})
			var schemaFields map[string]interface{}

			for k, v := range paramMap {
				if keyStr, ok := k.(string); ok {
					switch keyStr {
					case "type", "format", "items", "enum", "minimum", "maximum", "minLength", "maxLength", "pattern":
						// These fields should be moved under "schema" in OpenAPI 3.0
						if schemaFields == nil {
							schemaFields = make(map[string]interface{})
						}
						schemaFields[keyStr] = convertValue(v)
					default:
						convertedParam[keyStr] = convertValue(v)
					}
				}
			}

			// Add schema object if we have schema fields
			if schemaFields != nil {
				convertedParam["schema"] = schemaFields
			}

			convertedParams[name] = convertedParam
		} else {
			convertedParams[name] = convertValue(param)
		}
	}

	return convertedParams
}

func convertSchemaReferences(value interface{}) interface{} {
	switch v := value.(type) {
	case map[interface{}]interface{}:
		converted := make(map[string]interface{})
		for k, val := range v {
			if keyStr, ok := k.(string); ok {
				converted[keyStr] = convertSchemaReferences(val)
			}
		}
		return converted
	case map[string]interface{}:
		converted := make(map[string]interface{})
		for k, val := range v {
			converted[k] = convertSchemaReferences(val)
		}
		return converted
	case []interface{}:
		converted := make([]interface{}, len(v))
		for i, val := range v {
			converted[i] = convertSchemaReferences(val)
		}
		return converted
	case string:
		// Replace all Swagger 2.0 definition references with OpenAPI 3.0 component references
		if strings.Contains(v, "#/definitions/") {
			return strings.ReplaceAll(v, "#/definitions/", "#/components/schemas/")
		}
		return v
	default:
		return v
	}
}

func convertServers(host, basePath string, schemes []string) []Server {
	var servers []Server

	if len(schemes) == 0 {
		schemes = []string{"https"}
	}

	for _, scheme := range schemes {
		server := Server{}
		if host != "" {
			server.URL = fmt.Sprintf("%s://%s", scheme, host)
			if basePath != "" && basePath != "/" {
				server.URL += basePath
			}
		} else if basePath != "" {
			server.URL = basePath
		}

		if server.URL != "" {
			servers = append(servers, server)
		}
	}

	return servers
}

func convertPaths(paths map[string]interface{}, globalConsumes []string) map[string]interface{} {
	convertedPaths := make(map[string]interface{})

	for path, pathItem := range paths {
		if pathItemMap, ok := pathItem.(map[interface{}]interface{}); ok {
			convertedPathItem := make(map[string]interface{})

			for method, operation := range pathItemMap {
				if methodStr, ok := method.(string); ok {
					if operationMap, ok := operation.(map[interface{}]interface{}); ok {
						convertedOperation := convertOperation(operationMap, globalConsumes)
						convertedPathItem[methodStr] = convertedOperation
					}
				}
			}

			convertedPaths[path] = convertedPathItem
		}
	}

	return convertedPaths
}

func convertOperation(operation map[interface{}]interface{}, globalConsumes []string) map[string]interface{} {
	converted := make(map[string]interface{})
	var bodyParam map[string]interface{}
	var nonBodyParams []interface{}
	var operationConsumes []string

	// Extract consumes for this operation
	if consumes, exists := operation["consumes"]; exists {
		if consumesSlice, ok := consumes.([]interface{}); ok {
			for _, consume := range consumesSlice {
				if consumeStr, ok := consume.(string); ok {
					operationConsumes = append(operationConsumes, consumeStr)
				}
			}
		}
	}

	// If no operation-level consumes, use global
	if len(operationConsumes) == 0 {
		operationConsumes = globalConsumes
	}

	for key, value := range operation {
		if keyStr, ok := key.(string); ok {
			switch keyStr {
			case "consumes", "produces":
				// These are handled in requestBody and responses in OpenAPI 3.0
				continue
			case "parameters":
				bodyParam, nonBodyParams = extractBodyParameter(value)
				if len(nonBodyParams) > 0 {
					converted[keyStr] = convertParametersWithSchema(nonBodyParams)
				}
			case "responses":
				converted[keyStr] = convertResponses(value)
			default:
				converted[keyStr] = convertValue(value)
			}
		}
	}

	// Add requestBody if we found a body parameter
	if bodyParam != nil {
		converted["requestBody"] = createRequestBody(bodyParam, operationConsumes)
	}

	return converted
}

func convertParametersWithSchema(params []interface{}) []interface{} {
	convertedParams := make([]interface{}, len(params))

	for i, param := range params {
		if paramMap, ok := param.(map[string]interface{}); ok {
			convertedParam := make(map[string]interface{})
			var schemaFields map[string]interface{}

			for k, v := range paramMap {
				switch k {
				case "type", "format", "items", "enum", "minimum", "maximum", "minLength", "maxLength", "pattern":
					// These fields should be moved under "schema" in OpenAPI 3.0
					if schemaFields == nil {
						schemaFields = make(map[string]interface{})
					}
					schemaFields[k] = v
				default:
					convertedParam[k] = v
				}
			}

			// Add schema object if we have schema fields
			if schemaFields != nil {
				convertedParam["schema"] = schemaFields
			}

			convertedParams[i] = convertedParam
		} else {
			convertedParams[i] = param
		}
	}

	return convertedParams
}

func extractBodyParameter(params interface{}) (map[string]interface{}, []interface{}) {
	var bodyParam map[string]interface{}
	var nonBodyParams []interface{}

	if paramsList, ok := params.([]interface{}); ok {
		for _, param := range paramsList {
			if paramMap, ok := param.(map[interface{}]interface{}); ok {
				// Convert map[interface{}]interface{} to map[string]interface{}
				convertedParam := make(map[string]interface{})
				for k, v := range paramMap {
					if keyStr, ok := k.(string); ok {
						convertedParam[keyStr] = convertValue(v)
					}
				}

				if in, exists := convertedParam["in"]; exists && in == "body" {
					bodyParam = convertedParam
				} else {
					nonBodyParams = append(nonBodyParams, convertedParam)
				}
			}
		}
	}

	return bodyParam, nonBodyParams
}

func createRequestBody(bodyParam map[string]interface{}, consumes []string) map[string]interface{} {
	requestBody := make(map[string]interface{})

	// Add description if available
	if description, exists := bodyParam["description"]; exists {
		requestBody["description"] = description
	}

	// Add required if available
	if required, exists := bodyParam["required"]; exists {
		requestBody["required"] = required
	}

	// Create content based on consumes
	content := make(map[string]interface{})

	if len(consumes) == 0 {
		consumes = []string{"application/json"}
	}

	for _, mediaType := range consumes {
		mediaTypeContent := make(map[string]interface{})
		if schema, exists := bodyParam["schema"]; exists {
			mediaTypeContent["schema"] = schema
		}
		content[mediaType] = mediaTypeContent
	}

	requestBody["content"] = content

	return requestBody
}

func convertResponses(responses interface{}) interface{} {
	switch v := responses.(type) {
	case map[interface{}]interface{}:
		converted := make(map[string]interface{})
		for k, val := range v {
			if keyStr, ok := k.(string); ok {
				if responseMap, ok := val.(map[interface{}]interface{}); ok {
					convertedResponse := make(map[string]interface{})
					var schema interface{}

					for respKey, respVal := range responseMap {
						if respKeyStr, ok := respKey.(string); ok {
							if respKeyStr == "schema" {
								schema = convertValue(respVal)
							} else {
								convertedResponse[respKeyStr] = convertValue(respVal)
							}
						}
					}

					// Add content with application/json if schema exists
					if schema != nil {
						content := map[string]interface{}{
							"application/json": map[string]interface{}{
								"schema": schema,
							},
						}
						convertedResponse["content"] = content
					}

					converted[keyStr] = convertedResponse
				} else {
					converted[keyStr] = convertValue(val)
				}
			}
		}
		return converted
	default:
		return convertValue(responses)
	}
}

func convertValue(value interface{}) interface{} {
	switch v := value.(type) {
	case map[interface{}]interface{}:
		converted := make(map[string]interface{})
		for k, val := range v {
			if keyStr, ok := k.(string); ok {
				converted[keyStr] = convertValue(val)
			}
		}
		return converted
	case []interface{}:
		converted := make([]interface{}, len(v))
		for i, val := range v {
			converted[i] = convertValue(val)
		}
		return converted
	case string:
		// Replace all Swagger 2.0 definition references with OpenAPI 3.0 component references
		if strings.Contains(v, "#/definitions/") {
			return strings.ReplaceAll(v, "#/definitions/", "#/components/schemas/")
		}
		return v
	default:
		return v
	}
}
