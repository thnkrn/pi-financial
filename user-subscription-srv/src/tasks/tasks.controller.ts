import { InternalServerError } from '@aws-sdk/client-dynamodb';
import {
  BadRequestException,
  Body,
  ConflictException,
  Controller,
  Get,
  Post,
} from '@nestjs/common';
import { ApiTags } from '@nestjs/swagger';
import dayjs from 'dayjs';
import {
  ManualUpdateCustCodeRequest,
  TasksProcessRequest,
  TasksProcessRequestWithMktId,
} from './dto/tasks-process-request.dto';
import { IncentiveService } from './incentive.service';
import { TasksService } from './tasks.service';

@Controller('/internal/tasks')
@ApiTags('Tasks')
export class TasksController {
  constructor(
    private readonly tasksService: TasksService,
    private readonly incentiveService: IncentiveService,
  ) {}

  @Post('/oracle-daily-report')
  async oracleDailyReportTrigger(
    @Body() data: TasksProcessRequest,
  ): Promise<void> {
    this.tasksService.manualOracleDailyReport(data.fromDate, data.toDate);
    return;
  }

  @Post('/tax-monthly-report')
  async taxMonthlyReportTrigger(
    @Body() data: TasksProcessRequest,
  ): Promise<void> {
    this.tasksService.manualMonthlyTaxReport(data.fromDate, data.mailTo);
  }

  @Post('/oracle-monthly-report')
  async oracleMonthlyReportTrigger(
    @Body() data: TasksProcessRequest,
  ): Promise<string> {
    this.tasksService.manualOracleMonthlyReport(data.fromDate, data.mailTo);
    return;
  }

  @Post('/tks-daily-report')
  async tksDailyReportTrigger(
    @Body() data: TasksProcessRequest,
  ): Promise<string> {
    this.tasksService.manualTKSDailyReport(data.fromDate, data.toDate);
    return '';
  }

  @Post('/update-subscriptions')
  async updateSubscriptions(
    @Body() data: TasksProcessRequest,
  ): Promise<string> {
    this.tasksService.updateSubscriptions(data.fromDate, data.toDate);
    return '';
  }

  @Post('/manual-update-subscriptions')
  async manualUpdateCustCode(
    @Body() data: ManualUpdateCustCodeRequest,
  ): Promise<string | boolean> {
    if (!data.currentCustCode) {
      throw new BadRequestException('currentCustCode is required');
    } else if (!data.newCustCode) {
      throw new BadRequestException('newCustCode is required');
    } else if (data.currentCustCode === data.newCustCode) {
      throw new BadRequestException('CustCode must be different');
    } else if (!dayjs(data.effectiveDate, 'YYYYMMDD').isValid()) {
      throw new BadRequestException(
        'effectiveDate is not valid (format: YYYYMMDD)',
      );
    }

    let warning: string | undefined;
    try {
      warning = await this.tasksService.manualUpdateCustCode(
        data.currentCustCode,
        data.newCustCode,
        data.effectiveDate,
        data.force,
      );
      if (warning) {
        throw new ConflictException(warning);
      }

      return true;
    } catch (e) {
      if (e instanceof ConflictException) throw e;
      throw new InternalServerError(e);
    }
  }

  // @Post('/remind-user-subscription')
  // async findNearlyExpiredUsersInternal(
  //   @Body() data: TasksProcessRequest,
  // ): Promise<void> {
  //   await this.tasksService.scheduleProcessRemindUser(data.fromDate);
  // }

  @Post('/remind-user-subscription-schedule')
  async remindUserSubscription(
    @Body() data: TasksProcessRequestWithMktId,
  ): Promise<boolean> {
    this.tasksService.remindExpiryUser(data.toDate, data.mktId);
    return true;
  }

  @Post('/sync-mt5-user-from-freeview')
  async syncMt5UserFromFreeView(
    @Body() data: TasksProcessRequest,
  ): Promise<boolean> {
    const success = await this.tasksService.manualSyncMt5UserFromFreeView(
      data.fromDate,
    );
    if (success) return true;
    throw new Error('Failed to Sync MT5 user from Freeview');
  }

  /**
   * @deprecated The api is temporally.
   */
  @Get('/temp/backup-incentive')
  async backupIncentive(): Promise<boolean> {
    this.incentiveService.backupIncentive();
    return true;
  }
}
