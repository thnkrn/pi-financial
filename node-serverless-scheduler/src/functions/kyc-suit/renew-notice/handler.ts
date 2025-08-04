import {
  Config,
  configToSqlConfig,
  getConfig,
} from '@functions/kyc-suit/renew-notice/config';
import {
  NotificationTemplateId,
  NotificationType,
} from '@functions/kyc-suit/renew-notice/constants';
import {
  createNotification,
  deleteNotification,
} from '@functions/kyc-suit/renew-notice/api';
import {
  Query,
  QueryByLapsedDateParam,
  QueryByLapsedDateParamOrRenewedDate,
  queryFromDb,
} from '@functions/kyc-suit/renew-notice/db';
import { dateToStartEndDate } from '@libs/date-utils';
import { UserBulkResponse, resolveCustomerIds } from '@libs/user-api';

type BaseNotificationRequestItem = {
  customerCode: string;
};

type CreateNotificationRequestItem = BaseNotificationRequestItem & {
  lapsedDate: Date;
};

type DeleteNotificationRequestItem = BaseNotificationRequestItem;

type NotificationExecutionResponseItem = {
  customerCode: string;
  error?: Error;
};

////////////////////////////// EXPORTS ///////////////////////////////

// noinspection JSUnusedGlobalSymbols
export async function createAdvanceNotices(): Promise<
  NotificationExecutionResponseItem[]
> {
  return new HandlerWrapper().createAdvanceNotices();
}

// noinspection JSUnusedGlobalSymbols
export async function removeAdvanceNotices(): Promise<
  NotificationExecutionResponseItem[]
> {
  return new HandlerWrapper().removeAdvanceNotices();
}

// noinspection JSUnusedGlobalSymbols
export async function createExpiredNotice(): Promise<
  NotificationExecutionResponseItem[]
> {
  return new HandlerWrapper().createExpiredNotices();
}

class HandlerWrapper {
  ///////////////////////////// CONSTRUCTOR //////////////////////////////
  private readonly configPromise: Promise<Config>;

  constructor() {
    this.configPromise = getConfig();
  }

  /////////////////////////////// HELPERS ////////////////////////////////
  private _getQueryByLapsedDateParam(
    baseDate: Date,
    offset = 0
  ): QueryByLapsedDateParam {
    if (offset) {
      baseDate.setDate(baseDate.getDate() + offset);
    }
    const startEndDate = dateToStartEndDate(baseDate);
    return {
      lapsedDateFrom: startEndDate.startDate.toISOString(),
      lapsedDateUntil: startEndDate.endDate.toISOString(),
    };
  }

  private _getQueryByLapsedDateParamOrRenewedDate(
    baseLapsedDate: Date,
    baseRenewedDate: Date,
    { lapsedOffset = 0, renewedOffset = 0 } = {}
  ): QueryByLapsedDateParamOrRenewedDate {
    const temp = this._getQueryByLapsedDateParam(
      baseRenewedDate,
      renewedOffset
    );
    return {
      renewedDateFrom: temp.lapsedDateFrom,
      renewedDateUntil: temp.lapsedDateUntil,
      ...this._getQueryByLapsedDateParam(baseLapsedDate, lapsedOffset),
    };
  }

  ////////////////////////////// HANDLERS ////////////////////////////////

  async createAdvanceNotices(): Promise<NotificationExecutionResponseItem[]> {
    const config = await this.configPromise;
    const handler = new Handler(config);

    return Promise.all([
      handler.queryAndCreate(
        NotificationType.KYC,
        Query.getKycByLapsedDate,
        this._getQueryByLapsedDateParam(new Date(), +config.notifyPeriodKyc),
        NotificationTemplateId.upcoming[NotificationType.KYC]
      ),
      handler.queryAndCreate(
        NotificationType.SUIT,
        Query.getSuitByLapsedDate,
        this._getQueryByLapsedDateParam(new Date(), +config.notifyPeriodSuit),
        NotificationTemplateId.upcoming[NotificationType.SUIT]
      ),
    ]).then((results) => results.flatMap((x) => x));
  }

  async removeAdvanceNotices(): Promise<NotificationExecutionResponseItem[]> {
    const config = await this.configPromise;
    const handler = new Handler(config);

    const queryParams = this._getQueryByLapsedDateParamOrRenewedDate(
      new Date(),
      new Date(),
      {
        renewedOffset: -1,
      }
    );

    return Promise.all([
      handler.queryAndDelete(
        NotificationType.KYC,
        Query.getKycByLapsedDateOrRenewedDate,
        queryParams
      ),
      handler.queryAndDelete(
        NotificationType.SUIT,
        Query.getSuitByLapsedDateOrRenewedDate,
        queryParams
      ),
    ]).then((results) => results.flatMap((x) => x));
  }

  async createExpiredNotices(): Promise<NotificationExecutionResponseItem[]> {
    const config = await this.configPromise;
    const handler = new Handler(config);

    const queryParams = this._getQueryByLapsedDateParam(new Date());

    return Promise.all([
      handler.queryAndCreate(
        NotificationType.KYC,
        Query.getKycByLapsedDate,
        queryParams,
        NotificationTemplateId.upcoming[NotificationType.KYC]
      ),
      handler.queryAndCreate(
        NotificationType.SUIT,
        Query.getSuitByLapsedDate,
        queryParams,
        NotificationTemplateId.upcoming[NotificationType.SUIT]
      ),
    ]).then((results) => results.flatMap((x) => x));
  }
}

class Handler {
  private readonly config: Config;

  constructor(config: Config) {
    this.config = config;
  }

  async queryAndCreate(
    type: NotificationType,
    query: Query,
    queryParams: QueryByLapsedDateParam,
    templateId: string | number
  ): Promise<NotificationExecutionResponseItem[]> {
    const dbRes = await queryFromDb<QueryByLapsedDateParam>(
      type,
      configToSqlConfig(this.config),
      query,
      queryParams
    );

    return this.executeCreateApi(
      type,
      templateId,
      dbRes.map((item) => ({
        lapsedDate: item.lapsedDate,
        customerCode: item.customerCode,
      }))
    );
  }

  async queryAndDelete(
    type: NotificationType,
    query: Query,
    queryParams: QueryByLapsedDateParamOrRenewedDate
  ): Promise<NotificationExecutionResponseItem[]> {
    const dbRes = await queryFromDb<QueryByLapsedDateParamOrRenewedDate>(
      type,
      configToSqlConfig(this.config),
      query,
      queryParams
    );

    return this.executeDeleteApi(
      type,
      dbRes.map((item) => ({
        customerCode: item.customerCode,
      }))
    );
  }

  /////////////////////////// API INTERFACES /////////////////////////////

  private executeCreateApi(
    type: NotificationType,
    templateId: string | number,
    requestItems: CreateNotificationRequestItem[]
  ): Promise<NotificationExecutionResponseItem[]> {
    let action: (
      customerId: string,
      requestItem: CreateNotificationRequestItem
    ) => void | Promise<void>;
    switch (templateId) {
      case NotificationTemplateId.upcoming[NotificationType.KYC]:
      case NotificationTemplateId.upcoming[NotificationType.SUIT]:
        action = (customerId, requestItem) =>
          createNotification({
            type,
            cmsTemplateId: templateId,
            userId: customerId,
            shouldStoreDb: true,
            isPush: true,
            payloadBody: [
              requestItem.lapsedDate.toISOString().substring(0, 10),
            ],
          });
        break;
      default:
        action = (customerId) =>
          createNotification({
            type,
            cmsTemplateId: templateId,
            userId: customerId,
            shouldStoreDb: true,
            isPush: true,
          });
    }

    return this._executeApi(type, 'Create', action, requestItems);
  }

  private executeDeleteApi(
    type: NotificationType,
    requestItems: DeleteNotificationRequestItem[]
  ): Promise<NotificationExecutionResponseItem[]> {
    return this._executeApi(
      type,
      'Delete',
      (customerId) => deleteNotification({ type, userId: customerId }),
      requestItems
    );
  }

  private readonly GET_USERID_CHUNK_SIZE = 50;

  private async _executeApi<PR extends BaseNotificationRequestItem>(
    type: NotificationType,
    actionName: string,
    action: (customerId: string, requestItem: PR) => void | Promise<void>,
    requestItems: PR[]
  ): Promise<NotificationExecutionResponseItem[]> {
    const getUserIdPromises: Promise<UserBulkResponse>[] = [];
    for (let i = 0; i < requestItems.length; i += this.GET_USERID_CHUNK_SIZE) {
      const custCodes = requestItems
        .slice(i, i + this.GET_USERID_CHUNK_SIZE)
        .map((item) => item.customerCode);

      console.debug(
        `[${type}] Resolving customerId for custCodes ${custCodes}`
      );

      const promise = resolveCustomerIds(
        custCodes,
        this.config.userServiceHost
      );
      promise.then(() =>
        console.debug(
          `[${type}] Resolved customerId for custCodes ${custCodes}`
        )
      );
      getUserIdPromises.push(promise);
    }

    const customerIds = await Promise.all(getUserIdPromises).then((results) =>
      results.reduce((result, item) => ({ ...result, ...item }), {})
    );

    const results: NotificationExecutionResponseItem[] = await Promise.all(
      requestItems.map(async (item) => {
        try {
          const customerId = customerIds[item.customerCode];

          if (!customerId) {
            throw new Error('customerId not found');
          }

          console.debug(
            `[${type}] Executing "${actionName}" notification for custCode ${item.customerCode}`
          );
          await action(customerId, item);
          console.debug(
            `[${type}] Executed "${actionName}" notification for custCode ${item.customerCode}`
          );

          return {
            customerCode: item.customerCode,
          };
        } catch (e) {
          console.debug(
            `[${type}] Error executing "${actionName}" notification for custCode ${item.customerCode}, ${e}`
          );
          return {
            customerCode: item.customerCode,
            error: e,
          };
        }
      })
    );

    const errorCount = results.filter((x) => x.error).length;

    console.info(
      `[${type}] Executed "${actionName}" requests with count: ${requestItems.length}, error count: ${errorCount}`
    );
    return results;
  }
}
