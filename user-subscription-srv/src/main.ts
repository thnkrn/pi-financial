import { INestApplication } from '@nestjs/common';
import { NestFactory } from '@nestjs/core';
import {
  DocumentBuilder,
  SwaggerDocumentOptions,
  SwaggerModule,
} from '@nestjs/swagger';
import Logger from '@utils/datadog-utils';
import { EnvKeys } from '@utils/env-helper';
import { initDdTrace } from '@utils/tracing';

import Fs from 'fs';
import * as Yaml from 'js-yaml';
import { AppModule } from './app.module';

async function getApp(): Promise<INestApplication> {
  const logger = Logger.createLogger();

  // Start DataDog-SDK before nestjs factory create
  // setupInstrumentation();
  initDdTrace(logger);

  const app = await NestFactory.create(AppModule, {
    cors: true,
    bufferLogs: true,
    autoFlushLogs: true,
  });

  if (process.env[EnvKeys.SWAGGER_ENABLED]) {
    const config = new DocumentBuilder()
      .setTitle('User Subscription Service')
      // .setDescription('The API description')
      .setVersion('1.0')
      .build();
    const swaggerOptions: SwaggerDocumentOptions = {
      operationIdFactory: (controllerKey, methodKey) => {
        // remove 'Controller' suffix and camelcase controller key
        const prefix = controllerKey
          .replace(/Controller$/, '')
          .replace(/\s(.)/g, function ($1) {
            return $1.toUpperCase();
          })
          .replace(/\s/g, '')
          .replace(/^(.)/, function ($1) {
            return $1.toLowerCase();
          });
        return `${prefix}_${methodKey}`;
      },
    };
    const document = SwaggerModule.createDocument(app, config, swaggerOptions);
    SwaggerModule.setup('swagger', app, document);

    if (process.env[EnvKeys.SWAGGER_WRITE_FILE]) {
      try {
        Fs.mkdirSync('./spec');
      } catch (e) {
        if (e.code !== 'EEXIST') {
          throw e;
        }
      }
      Fs.writeFileSync(
        './spec/swagger.yml',
        Yaml.dump(document, { noRefs: true }),
      );
    }
  }
  app.useLogger(logger);

  return app;
}

async function bootstrap() {
  const app = await getApp();
  await app.listen(3000);
}

////////////////////////////////////////////////////////////////////////////////

if (process.argv.includes('swagger-spec')) {
  process.env[EnvKeys.SWAGGER_ENABLED] = '1';
  process.env[EnvKeys.SWAGGER_WRITE_FILE] = '1';

  getApp().then((app) => app.close());
} else {
  void bootstrap();
}
