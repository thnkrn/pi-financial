import { Module } from '@nestjs/common';
import { PurchaseService } from './purchase.service';
import { PurchaseController } from './purchase.controller';
import { PlanService } from '../plan/plan.service';
import { PrismaService } from '../prisma.service';
import { UserSubscriptionService } from '../user_subscription/user_subscription.service';

@Module({
  controllers: [PurchaseController],
  providers: [
    PurchaseService,
    PlanService,
    PrismaService,
    UserSubscriptionService,
  ],
})
export class PurchaseModule {}
