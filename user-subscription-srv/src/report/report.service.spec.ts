import { Test, TestingModule } from '@nestjs/testing';
import { ReportService } from './report.service';
import { PrismaService } from '../prisma.service';

describe('ReportService', () => {
  let service: ReportService;

  beforeEach(async () => {
    const module: TestingModule = await Test.createTestingModule({
      providers: [ReportService, PrismaService],
    }).compile();

    service = module.get<ReportService>(ReportService);
  });

  it('should be defined', () => {
    expect(service).toBeDefined();
  });
});
