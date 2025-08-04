'use strict';

import { trace } from '@opentelemetry/api';

// Not functionally required but gives some insight what happens behind the scenes
// import { trace, diag, DiagConsoleLogger, DiagLogLevel } from '@opentelemetry/api';
// diag.setLogger(new DiagConsoleLogger(), DiagLogLevel.INFO);
import { Logger as CommonLogger } from '@nestjs/common';
import { registerInstrumentations } from '@opentelemetry/instrumentation';
import { ExpressInstrumentation } from '@opentelemetry/instrumentation-express';
import { HttpInstrumentation } from '@opentelemetry/instrumentation-http';
import { WinstonInstrumentation } from '@opentelemetry/instrumentation-winston';
import { Resource } from '@opentelemetry/resources';
import {
  ConsoleSpanExporter,
  SimpleSpanProcessor,
} from '@opentelemetry/sdk-trace-base';
import { NodeTracerProvider } from '@opentelemetry/sdk-trace-node';
import { SemanticResourceAttributes } from '@opentelemetry/semantic-conventions';
import ddTrae, { TracerOptions } from 'dd-trace';
/**
 * Method to setup instrumentation, can be parametarized or refactored to make it config based
 *
 * @return {Tracer} returns the tracer created for instrumentation
 */
export const setupInstrumentation = () => {
  const serviceName =
    process.env.DATADOG_SERVICE_NAME || 'pi.usersubscription.api';
  const enableConsoleSpan = process.env.ENABLE_CONSOLE_SPAN_EXPORTER === 'true';

  const provider = new NodeTracerProvider({
    resource: new Resource({
      [SemanticResourceAttributes.SERVICE_NAME]: serviceName,
    }),
  });
  registerInstrumentations({
    tracerProvider: provider,
    instrumentations: [
      // Express instrumentation expects HTTP layer to be instrumented
      new HttpInstrumentation(),
      new ExpressInstrumentation(),
      // winston instrumentation for logger
      new WinstonInstrumentation({
        // Optional hook to insert additional context to log metadata.
        // Called after trace context is injected to metadata.
        logHook: (span, record) => {
          record['resource.service.name'] = serviceName;
        },
      }),
    ],
  });

  // check if want to print spans to console using environment variable
  if (enableConsoleSpan) {
    provider.addSpanProcessor(
      new SimpleSpanProcessor(new ConsoleSpanExporter()),
    );
  }

  // provider.addSpanProcessor(new SimpleSpanProcessor(new OTLPTraceExporter({ url: 'http://127.0.0.1:8126/v0.4/traces' })));

  // Initialize the OpenTelemetry APIs to use the NodeTracerProvider bindings
  provider.register();

  return trace.getTracer(serviceName);
};

/**
 * Initializes the Datadog tracing for the whole application.
 */
export const initDdTrace = (logger: CommonLogger) => {
  const nodeEnv = process.env.NODE_ENV || 'development';
  const serviceName =
    process.env.DATADOG_SERVICE_NAME || 'pi.usersubscription-api';
  const ddAgentHost = process.env.DATADOG_AGENT_HOST || '127.0.0.1';
  const ddAgentPort = +process.env.DATADOG_AGENT_PORT || 8126;

  logger.log(`Initializing Datadog tracing, ${ddAgentHost}:${ddAgentPort}`);
  const options: TracerOptions = {
    hostname: ddAgentHost,
    port: ddAgentPort,
    service: serviceName,
    env: nodeEnv,
    logInjection: true,
    logger: logger as any,
    dogstatsd: {
      hostname: ddAgentHost,
      port: 8125, // dogstatsd default port
    },
  };
  ddTrae.init({ ...options });
};
