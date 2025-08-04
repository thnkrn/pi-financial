import { Injectable, Logger, NestMiddleware } from '@nestjs/common';
import { NextFunction, Request, Response } from 'express';

@Injectable()
export class LoggerMiddleware implements NestMiddleware {
  private logger = new Logger('HTTP');
  private pathRegex = /^.*\.(jpg|jpeg|png|webp|ico|txt)$/;

  private isValidPath = (path: string) => {
    return (
      !['/', '/public'].includes(path) &&
      !['/_next/'].every((prefix) => path.startsWith(prefix)) &&
      !this.pathRegex.test(path)
    );
  };

  use(request: Request, response: Response, next: NextFunction): void {
    const requestPath = request.originalUrl.split('?')[0];
    if (this.isValidPath(requestPath)) {
      this.logger.log(
        `[${request.method}:${requestPath}] [${request.header(
          'user-id',
        )}] ${JSON.stringify(request.query)} ${JSON.stringify(request.body)}`,
      );

      response.on('finish', () => {
        const isValid = this.isValidPath(requestPath);
        if (isValid) {
          this.logger.log(`[RES:${requestPath}] ${response.statusCode}`);

          if (response.statusCode >= 500) {
            const json = response.json();
            this.logger.error(
              `Error: [${request.method}]${requestPath} => ${json.statusCode} ${json.statusMessage}`,
            );
          } else if (response.statusCode >= 400) {
            const json = response.json();
            this.logger.warn(
              `Warn: [${request.method}]${requestPath} => ${json.statusCode} ${json.statusMessage}`,
            );
          }
        }
      });
    }
    next();
  }
}
