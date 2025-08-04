import { Test, TestingModule } from '@nestjs/testing';
import { TasksController } from './tasks.controller';
import { TasksService } from './tasks.service';
import { PrismaService } from '../prisma.service';
import { UserSubscriptionService } from '../user_subscription/user_subscription.service';
import { ReportService } from '../report/report.service';
import { PlanService } from '../plan/plan.service';
import { TaxService } from '../tax/tax.service';
import { IncentiveService } from './incentive.service';

describe('TasksController', () => {
  let controller: TasksController;

  beforeEach(async () => {
    const module: TestingModule = await Test.createTestingModule({
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
    }).compile();

    controller = module.get<TasksController>(TasksController);
  });

  it('should be defined', () => {
    expect(controller).toBeDefined();
  });
});
