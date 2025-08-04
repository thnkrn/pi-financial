import { Test, TestingModule } from '@nestjs/testing';
import { PlanService } from './plan.service';
import { PrismaService } from '../prisma.service';

describe('PlanService', () => {
  let service: PlanService;

  beforeEach(async () => {
    const module: TestingModule = await Test.createTestingModule({
      providers: [PlanService, PrismaService],
    }).compile();

    service = module.get<PlanService>(PlanService);
  });

  it('should be defined', () => {
    expect(service).toBeDefined();
  });
});
