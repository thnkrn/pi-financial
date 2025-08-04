import { ApiProperty } from '@nestjs/swagger';
import { PurchaseRequestStatus } from '@prisma/client';

export class PaymentStatusResponse {
  @ApiProperty({ enum: Object.values(PurchaseRequestStatus) })
  status: PurchaseRequestStatus;
}
