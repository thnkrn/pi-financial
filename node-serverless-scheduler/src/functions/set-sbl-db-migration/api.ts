import got from 'got';

export type InternalTradingAccountRequest = {
    customercode: string;
};

export type InternalTradingAccountResponse = {
    data: InternalTradingAccount[]
};

export type InternalTradingAccount = {
    id: string
    tradingAccountNo: string
    customerCode: string
    accountTypeCode: string
};

export async function getInternalTradingAccount(
    serviceHost: string,
    requestData: InternalTradingAccountRequest
) {
    const url = new URL('/internal/v2/trading-accounts', serviceHost);
    url.searchParams.append('customercode', requestData.customercode);
    url.searchParams.append('withBankAccounts', 'false');
    url.searchParams.append('withExternalAccounts', 'false');
    return got
        .get(url)
        .json<InternalTradingAccountResponse>()
        .catch((e) => {
            console.error(`Failed to get internal trading account information. Exception: ${e}`);
            throw e;
        });
};
