export class Mt5PlanResponse {
  id: number;
  title: string;
  description: string;
  price: string;
  currency: string;
  month: string;
}

export class Mt5PlanListResponse {
  plans: Mt5PlanResponse[];
}
