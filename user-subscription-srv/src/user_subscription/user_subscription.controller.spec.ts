import { Test, TestingModule } from '@nestjs/testing';
import { UserSubscriptionController } from './user_subscription.controller';
import { UserSubscriptionService } from './user_subscription.service';
import { PrismaService } from '../prisma.service';
import { PlanService } from '../plan/plan.service';

describe('UserSubscriptionController', () => {
  let controller: UserSubscriptionController;

  beforeEach(async () => {
    const module: TestingModule = await Test.createTestingModule({
      controllers: [UserSubscriptionController],
      providers: [UserSubscriptionService, PrismaService, PlanService],
    }).compile();

    controller = module.get<UserSubscriptionController>(
      UserSubscriptionController,
    );
  });

  it('should be defined', () => {
    expect(controller).toBeDefined();
  });
});
