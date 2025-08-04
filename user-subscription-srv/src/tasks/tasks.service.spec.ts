import { Test, TestingModule } from '@nestjs/testing';
import { TasksService } from './tasks.service';
import { PrismaService } from '../prisma.service';
import { UserSubscriptionService } from '../user_subscription/user_subscription.service';
import { ReportService } from '../report/report.service';
import { PlanService } from '../plan/plan.service';
import { TaxService } from '../tax/tax.service';

describe('TasksService', () => {
  let service: TasksService;

  beforeEach(async () => {
    const module: TestingModule = await Test.createTestingModule({
      providers: [
        TasksService,
        PrismaService,
        UserSubscriptionService,
        ReportService,
        PlanService,
        TaxService,
      ],
    }).compile();

    service = module.get<TasksService>(TasksService);
  });

  it('should be defined', () => {
    expect(service).toBeDefined();
  });
});
