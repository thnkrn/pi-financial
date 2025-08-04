import { Module } from '@nestjs/common';
import { TaxService } from './tax.service';
import { PrismaService } from '../prisma.service';

@Module({
  providers: [TaxService, PrismaService],
})
export class TaxModule {}
