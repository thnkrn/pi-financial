import { validate as validateJsonSchema } from 'jsonschema';

const validateJson = function validate(model, body) {
  const result = validateJsonSchema(body, model);
  if (result.errors.length > 0) {
    throw new Error(
      `Request body validation failed: ${result.errors
        .map((e) => e.message)
        .join(', ')}`
    );
  }
};

export default validateJson;
