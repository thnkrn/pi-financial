import { Test, TestingModule } from '@nestjs/testing';
import { UserSubscriptionService } from './user_subscription.service';
import { PrismaService } from '../prisma.service';
import { PlanService } from '../plan/plan.service';

describe('UserSubscriptionService', () => {
  let service: UserSubscriptionService;

  beforeEach(async () => {
    const module: TestingModule = await Test.createTestingModule({
      providers: [UserSubscriptionService, PrismaService, PlanService],
    }).compile();

    service = module.get<UserSubscriptionService>(UserSubscriptionService);
  });

  it('should be defined', () => {
    expect(service).toBeDefined();
  });
});
