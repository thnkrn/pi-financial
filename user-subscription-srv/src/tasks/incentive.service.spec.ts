import { Test, TestingModule } from '@nestjs/testing';
import { PrismaService } from '../prisma.service';
import { UserSubscriptionService } from '../user_subscription/user_subscription.service';
import { ReportService } from '../report/report.service';
import { PlanService } from '../plan/plan.service';
import { TaxService } from '../tax/tax.service';
import { IncentiveService } from './incentive.service';
import { TasksService } from './tasks.service';

describe('IncentiveService', () => {
  let service: IncentiveService;

  beforeEach(async () => {
    const module: TestingModule = await Test.createTestingModule({
      providers: [
        TasksService,
        PrismaService,
        UserSubscriptionService,
        ReportService,
        PlanService,
        TaxService,
        IncentiveService,
      ],
    }).compile();

    service = module.get<IncentiveService>(IncentiveService);
  });

  it('should be defined', () => {
    expect(service).toBeDefined();
  });
});
