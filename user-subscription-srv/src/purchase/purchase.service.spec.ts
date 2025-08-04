import { Test, TestingModule } from '@nestjs/testing';
import { PurchaseService } from './purchase.service';
import { PlanService } from '../plan/plan.service';
import { PrismaService } from '../prisma.service';
import { UserSubscriptionService } from '../user_subscription/user_subscription.service';
import { PurchaseController } from './purchase.controller';

describe('PurchaseService', () => {
  let service: PurchaseService;

  beforeEach(async () => {
    const module: TestingModule = await Test.createTestingModule({
      controllers: [PurchaseController],
      providers: [
        PurchaseService,
        PlanService,
        PrismaService,
        UserSubscriptionService,
      ],
    }).compile();

    service = module.get<PurchaseService>(PurchaseService);
  });

  it('should be defined', () => {
    expect(service).toBeDefined();
  });
});
