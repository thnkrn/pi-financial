import * as sql from 'mssql';

export type QueryResult = {
  customerCode: string;
  lapsedDate: Date;
};

// eslint-disable-next-line @typescript-eslint/ban-types
export type QueryParam = {};
export type QueryByLapsedDateParam = QueryParam & {
  lapsedDateFrom: string;
  lapsedDateUntil: string;
};
export type QueryByLapsedDateParamOrRenewedDate = QueryByLapsedDateParam & {
  renewedDateFrom: string;
  renewedDateUntil: string;
};

export enum Query {
  getSuitByLapsedDate = `
      SELECT DISTINCT dExpiredate AS "lapsedDate", sCustcode AS "customerCode"
      FROM dbo.tblTcustscore
      WHERE (@lapsedDateFrom <= dExpiredate AND dExpiredate <= @lapsedDateUntil)
  `,
  getKycByLapsedDate = `
      SELECT DISTINCT NextReviewDate AS "lapsedDate", Custcode AS "customerCode"
      FROM dbo.Tkyc
      WHERE (@lapsedDateFrom <= NextReviewDate AND NextReviewDate <= @lapsedDateUntil)
  `,
  getSuitByLapsedDateOrRenewedDate = `
      SELECT DISTINCT dExpiredate AS "lapsedDate", sCustcode AS "customerCode"
      FROM dbo.tblTcustscore
      WHERE (@lapsedDateFrom <= dExpiredate AND dExpiredate <= @lapsedDateUntil)
         OR (@renewedDateFrom <= dCompletedate AND dCompletedate <= @renewedDateUntil)
  `,
  getKycByLapsedDateOrRenewedDate = `
      SELECT DISTINCT NextReviewDate AS "lapsedDate", Custcode AS "customerCode"
      FROM dbo.Tkyc
      WHERE (@lapsedDateFrom <= NextReviewDate AND NextReviewDate <= @lapsedDateUntil)
         OR (@renewedDateFrom <= ReviewDate AND ReviewDate <= @renewedDateUntil)
  `,
}

export async function queryFromDb<
  QP extends QueryParam,
  QR extends QueryResult = QueryResult
>(
  contextName: string,
  sqlConfig: sql.config,
  query: Query,
  params?: QP
): Promise<QueryResult[]> {
  let pool: sql.ConnectionPool;
  try {
    console.debug(`[${contextName}] Connecting to db`);
    pool = await sql.connect(sqlConfig);
    let request = new sql.Request(pool);

    if (params) {
      console.debug(
        `[${contextName}] Using parameters: ${JSON.stringify(params)}`
      );
      request = Object.entries(params).reduce(
        (req, param) => req.input(...param),
        request
      );
    }

    console.debug(`[${contextName}] Querying from db`);
    const queryResult = await request.query<QR>(query);

    console.info(
      `[${contextName}] Query finished with count: ${queryResult.recordset.length}`
    );

    return queryResult.recordset;
  } catch (e) {
    console.error(`[${contextName}] Failed to query from db: ${e}`);
    throw e;
  } finally {
    void pool?.close();
  }
}
