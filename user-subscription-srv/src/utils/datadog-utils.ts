import { Logger as CommonLogger } from '@nestjs/common';
import { WinstonModule } from 'nest-winston';
import { Logform, format, transports } from 'winston';
import TransportStream from 'winston-transport';

const { printf, colorize, combine, timestamp, json } = format;

class Logger {
  private static instance: CommonLogger;

  constructor() {
    if (!Logger.instance) {
      console.log('init logger');
      Logger.instance = Logger.createLogger();
    }
  }

  static getLogger = () => {
    return Logger.instance;
  };

  static createLogger = () => {
    try {
      const consoleFormat: Logform.Format =
        (process.env.NODE_ENV || 'development') === 'development'
          ? combine(
              colorize({ all: true }),
              timestamp({ format: 'YYYY-MM-DD HH:mm:ss' }),
              printf(
                (info) => `[${info.timestamp}] ${info.level} - ${info.message}`,
              ),
            )
          : combine(
              timestamp({ format: 'YYYY-MM-DD HH:mm:ss' }),
              printf((info) =>
                JSON.stringify({ timestamp: `${info.timestamp}`, ...info }),
              ),
            );

      const transportStreams: TransportStream[] = [];
      transportStreams.push(
        new transports.Console({
          format: consoleFormat,
        }),
      );
      const ddEnabled = process.env.DATADOG_ENABLED === 'true';
      const ddApiKey = process.env.DATADOG_API_KEY ?? '';
      if (ddEnabled && ddApiKey) {
        const ddSource = process.env.DATADOG_SOURCE ?? 'usersubscription-api';
        const ddService =
          process.env.DATADOG_SERVICE_NAME ?? 'pi.usersubscription-api';
        const ddtTags = `env:${process.env.NODE_ENV.toLocaleLowerCase()}`;
        const httpTransportOptions: transports.HttpTransportOptions = {
          host: 'http-intake.logs.datadoghq.com',
          path: `/api/v2/logs?ddsource=${ddSource}&service=${ddService}&ddtags=${ddtTags}`,
          ssl: true,
          headers: { 'DD-API-KEY': ddApiKey },
          format: combine(timestamp(), json()),
        };
        transportStreams.push(new transports.Http(httpTransportOptions));
      }

      this.instance = WinstonModule.createLogger({
        transports: transportStreams,
      }) as CommonLogger;
      return Logger.instance;
    } catch (e) {
      console.error('Error in createLogger', e);
    }
  };

  static debug = (message: string, context?: string) => {
    Logger.instance.debug(message, context);
  };
  static log = (message: string, context?: string) => {
    Logger.instance.log(message, context);
  };
  static warn = (message: string, context?: string) => {
    Logger.instance.warn(message, context);
  };
  static error = (message: string, stack?: string, context?: string) => {
    Logger.instance.error(message, stack, context);
  };
}

export default Logger;
