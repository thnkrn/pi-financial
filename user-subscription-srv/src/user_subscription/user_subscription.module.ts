import { Module } from '@nestjs/common';
import { UserSubscriptionService } from './user_subscription.service';
import { UserSubscriptionController } from './user_subscription.controller';
import { PrismaService } from '../prisma.service';
import { PlanService } from '../plan/plan.service';

@Module({
  controllers: [UserSubscriptionController],
  providers: [UserSubscriptionService, PrismaService, PlanService],
})
export class UserSubscriptionModule {}
