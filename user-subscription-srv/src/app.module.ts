import { MiddlewareConsumer, Module, NestModule } from '@nestjs/common';
import { ConfigModule } from '@nestjs/config';
import dayjs from 'dayjs';
import timezone from 'dayjs/plugin/timezone';
import utc from 'dayjs/plugin/utc';
import { AppController } from './app.controller';
import { AppService } from './app.service';
import { LoggerMiddleware } from './middlewares/logger.middlewares';
import { PlanModule } from './plan/plan.module';
import { PurchaseModule } from './purchase/purchase.module';
import { ReportModule } from './report/report.module';
import { TasksModule } from './tasks/tasks.module';
import { TaxModule } from './tax/tax.module';
import { UserSubscriptionModule } from './user_subscription/user_subscription.module';

dayjs.extend(utc);
dayjs.extend(timezone);

process.env.NODE_ENV = process.env.NODE_ENV || 'development';
process.env.APP_ENV = process.env.APP_ENV || process.env.NODE_ENV;
const env = process.env.APP_ENV.toLowerCase();

console.log(`Running in .env.${env}`);

@Module({
  imports: [
    ConfigModule.forRoot({
      envFilePath: [`.env.${env}`],
      isGlobal: true,
    }),
    PlanModule,
    PurchaseModule,
    UserSubscriptionModule,
    ReportModule,
    TasksModule,
    TaxModule,
    // ...(env !== 'development' ? [ApmModule.register()] : []),
  ],
  controllers: [AppController],
  providers: [AppService],
})
export class AppModule implements NestModule {
  configure(consumer: MiddlewareConsumer) {
    consumer.apply(LoggerMiddleware).forRoutes('*');
  }
}
