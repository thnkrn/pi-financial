import { Module } from '@nestjs/common';
import { ReportService } from './report.service';
import { PrismaService } from '../prisma.service';

@Module({
  providers: [ReportService, PrismaService],
})
export class ReportModule {}
