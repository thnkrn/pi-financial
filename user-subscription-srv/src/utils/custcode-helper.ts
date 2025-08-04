import { rethrow } from '@nestjs/core/helpers/rethrow';
import axios, { AxiosError } from 'axios';
import { CustcodeNotFound } from '../exception';
import Logger from './datadog-utils';
import { isDevelopment } from './env-helper';

interface ResponseWrapper<T> {
  data: T;
}
export interface UserResponse {
  customerCodes: CustomerCode[];
}

export interface CustomerCode {
  code: string;
  tradingAccounts: string[];
}

function validateNumberOfCustcode(count: number) {
  if (count === 0) {
    throw new CustcodeNotFound();
  }
  // else if (count > 1) {
  //   throw new MultipleCustcode();
  // }
}

export async function getInternalUser(userId: string) {
  try {
    const path = `${process.env.USER_API_HOST}/internal/v2/user/${userId}`;
    Logger.log(`[getInternalUser] path: ${path}`);
    const result = await axios.get(path);
    const { id, devices, customerCodes, tradingAccounts } = result.data.data;
    Logger.log(
      `[getInternalUser] result.data: ${JSON.stringify({
        id,
        devices,
        customerCodes,
        tradingAccounts,
      })}`,
    );
    return result.data as ResponseWrapper<UserResponse>;
  } catch (error) {
    if (error instanceof AxiosError) {
      Logger.error(`CustcodeNotFound: ${userId}: ${error.message}`);
      throw new CustcodeNotFound();
    } else {
      Logger.error(`getInternalUser: ${userId}: ${error}`);
      rethrow(error);
    }
  }
}

export async function getCustomerCodeFromUserId(
  userId: string,
): Promise<string> {
  if (isDevelopment()) {
    return userId;
  }
  try {
    const result = await getInternalUser(userId);
    const custcodeCount = result.data.customerCodes.length ?? 0;
    validateNumberOfCustcode(custcodeCount);
    return result.data?.customerCodes[0].code;
  } catch (error) {
    if (error instanceof AxiosError) {
      Logger.error(`CustcodeNotFound: ${userId}: ${error.message}`);
      throw new CustcodeNotFound();
    } else {
      Logger.error(`getCustomerCodeFromUserId: ${userId}: ${error}`);
      rethrow(error);
    }
  }
}
