import { Injectable } from '@nestjs/common';

@Injectable()
export class AppService {
  getHello(): string {
    return `[${process.env.APP_NAME ?? 'user-subscription-srv'}] Hello World!`;
  }
}
