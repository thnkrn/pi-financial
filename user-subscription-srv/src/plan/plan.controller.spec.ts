import { Test, TestingModule } from '@nestjs/testing';
import { PlanController } from './plan.controller';
import { PlanService } from './plan.service';
import { PrismaService } from '../prisma.service';

describe('PlanController', () => {
  let controller: PlanController;

  beforeEach(async () => {
    const module: TestingModule = await Test.createTestingModule({
      controllers: [PlanController],
      providers: [PlanService, PrismaService],
    }).compile();

    controller = module.get<PlanController>(PlanController);
  });

  it('should be defined', () => {
    expect(controller).toBeDefined();
  });
});
