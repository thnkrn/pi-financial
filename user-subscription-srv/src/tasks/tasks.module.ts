import { Module } from '@nestjs/common';
import { TasksService } from './tasks.service';
import { TasksController } from './tasks.controller';
import { PrismaService } from '../prisma.service';
import { UserSubscriptionService } from '../user_subscription/user_subscription.service';
import { UserSubscriptionModule } from '../user_subscription/user_subscription.module';
import { ReportModule } from '../report/report.module';
import { ReportService } from '../report/report.service';
import { PlanModule } from '../plan/plan.module';
import { PlanService } from '../plan/plan.service';
import { TaxService } from '../tax/tax.service';
import { IncentiveService } from './incentive.service';

@Module({
  imports: [UserSubscriptionModule, ReportModule, PlanModule],
  controllers: [TasksController],
  providers: [
    TasksService,
    PrismaService,
    UserSubscriptionService,
    ReportService,
    PlanService,
    TaxService,
    IncentiveService,
  ],
})
export class TasksModule {}
