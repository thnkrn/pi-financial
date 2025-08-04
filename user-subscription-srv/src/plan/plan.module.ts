import { Module } from '@nestjs/common';
import { PlanService } from './plan.service';
import { DeprecatedPlanController, PlanController } from './plan.controller';
import { PrismaService } from '../prisma.service';

@Module({
  controllers: [DeprecatedPlanController, PlanController],
  providers: [PlanService, PrismaService],
})
export class PlanModule {}
