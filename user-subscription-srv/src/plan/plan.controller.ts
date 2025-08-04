import { Controller, Get } from '@nestjs/common';
import { ApiTags } from '@nestjs/swagger';
import { Mt5PlanListResponse } from './dto/mt5-plan.dto';
import { PlanService } from './plan.service';

@Controller('/secure')
@ApiTags('Plans')
export class PlanController {
  constructor(private readonly planService: PlanService) {}

  @Get('/plan/mt5')
  async findAllMT5Plan(): Promise<Mt5PlanListResponse> {
    const plans = await this.planService.getAllMT5Plans();
    return {
      plans: plans.map((plan) => {
        return {
          id: plan.id,
          title: plan.title,
          description: plan.description,
          price: `${plan.price}`,
          currency: 'THB',
          month: `${plan.month}`,
        };
      }),
    };
  }
}

@Controller('/public/user-subscription')
@ApiTags('PlansDeprecated')
export class DeprecatedPlanController extends PlanController {}
