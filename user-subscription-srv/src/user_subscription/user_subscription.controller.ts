import { getProductFromAccountCode } from '@libs/smartsales/db-smartsales-client';
import { ProductType } from '@libs/smartsales/model/product-type';
import { Controller, Get, Headers, Logger, Post, Query } from '@nestjs/common';
import { ApiTags } from '@nestjs/swagger';
import {
  getCustomerCodeFromUserId,
  getInternalUser,
} from '@utils/custcode-helper';
import { isDevelopment } from '@utils/env-helper';
import { GuestAccountError, UndefineUserId } from '../exception';
import { ActiveSubscriptionResponse } from './dto/active-subscription.dto';
import { CheckStatusResponse } from './dto/check-status-response.dto';
import { PayableStatusResponse } from './dto/payable-status-response.dto';
import { UserSubscriptionService } from './user_subscription.service';

@Controller('/secure/user-subscription')
@ApiTags('UserSubscriptions')
export class UserSubscriptionController {
  private readonly logger = new Logger(UserSubscriptionController.name);

  constructor(
    private readonly userSubscriptionService: UserSubscriptionService,
  ) {}

  @Post('/status')
  async checkStatus(
    @Headers('user-id') userId: string,
  ): Promise<CheckStatusResponse> {
    try {
      if (!userId) {
        this.logger.warn(`UndefineUserId => ${userId}`);
        throw new UndefineUserId();
      }
      let customerCode: string;
      let hasThaiEquityAccount: boolean;
      let customerCodes: string[];
      if (isDevelopment()) {
        customerCode = userId;
        hasThaiEquityAccount = true;
        customerCodes = [userId];
      } else {
        const user = await getInternalUser(userId);
        const customerCodeList = user.data.customerCodes;
        customerCodes = customerCodeList.map((it) => it.code);
        if (customerCodeList.length > 1) {
          const list = customerCodeList.map((it) => it.code);
          this.logger.log(`Multiple Custcode => ${list}, [userId:${userId}]`);

          const preferCustomerCode =
            await this.userSubscriptionService.findPreferCustomerCodeFromList(
              list,
            );
          customerCode = preferCustomerCode ?? user.data.customerCodes[0].code;
          this.logger.log(
            `customerCode => ${customerCode}, [userId:${userId}, preferCustomerCode:${preferCustomerCode}]`,
          );
        } else if (customerCodeList.length > 0) {
          customerCode = user.data.customerCodes[0].code;
        } else {
          this.logger.warn(`userId ${userId} is Guest account`);
          throw new GuestAccountError();
        }
        hasThaiEquityAccount = customerCodeList
          .flatMap((it) =>
            it.tradingAccounts.map((sub) => getProductFromAccountCode(sub)),
          )
          .includes(ProductType.EQUITY);
      }

      this.logger.log(`findCurrentMT5Status ${customerCode}`);
      const mt5 = await this.userSubscriptionService.findCurrentMT5Status(
        customerCode,
      );

      const result = {
        hasThaiEquityAccount,
        mt5,
        activeCustomerCode: customerCode,
        customerCodes,
      };
      this.logger.log(`Return response => ${JSON.stringify(result)}`);

      return result;
    } catch (error) {
      this.logger.warn(`checkStatus => ${error}, [userId:${userId}]`);
      throw error;
    }
  }

  @Post('/active')
  async findOne(
    @Headers('user-id') userId: string,
  ): Promise<ActiveSubscriptionResponse> {
    if (!userId) {
      this.logger.warn(`UndefineUserId => ${userId}`);
      throw new UndefineUserId();
    }
    const customerCode = await getCustomerCodeFromUserId(userId);
    const result = await this.userSubscriptionService.findActiveSubscriptions(
      customerCode,
    );

    return {
      isActive: !!result,
      activeDate: result?.activeDate ?? null,
      expiredDate: result?.expiredDate ?? null,
    };
  }

  @Get('/payableStatus')
  async getPayableStatus(
    @Query('custcodes') custcodes: string,
  ): Promise<PayableStatusResponse> {
    if (!custcodes || custcodes.length === 0) {
      this.logger.warn(`Undefined custcodes => ${custcodes}`);
      throw new UndefineUserId();
    }
    try {
      const custcodeList = custcodes.split(',');
      const status = await this.userSubscriptionService.getPayableStatus(
        custcodeList,
      );
      this.logger.log(
        `getPayableStatus => ${JSON.stringify(
          status,
        )}, [custcodes:${custcodes}]`,
      );
      return status;
    } catch (error) {
      this.logger.warn(
        `getPayableStatus => ${error}, [custcodes:${custcodes}]`,
      );
      return { payable: true, lastEndDate: new Date() };
    }
  }
}
