import { Injectable, Logger } from '@nestjs/common';
import { PrismaService } from '../prisma.service';

@Injectable()
export class PlanService {
  private readonly logger = new Logger(PlanService.name);
  constructor(private readonly prismaService: PrismaService) {}

  async getAllMT5Plans() {
    try {
      const data = this.prismaService.plan.findMany();
      return data;
    } catch (e) {
      this.logger.log(e);
      throw 'Cannot get mt5 plans';
    }
  }

  async getPlan(id: number) {
    try {
      const result = await this.prismaService.plan.findFirst({
        where: {
          id: {
            equals: id,
          },
        },
      });

      if (!result) {
        throw `Plan ${id} not found`;
      }
      return result;
    } catch (e) {
      this.logger.log(e);
      throw 'Cannot get Plan';
    }
  }
}
