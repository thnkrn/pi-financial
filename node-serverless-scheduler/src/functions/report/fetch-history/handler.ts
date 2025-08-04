import type { ValidatedEventAPIGatewayProxyEvent } from '@libs/api-gateway';
import { formatJSONResponse } from '@libs/api-gateway';
import { middyfy } from '@libs/lambda';
import { getSequelize } from '@libs/db-utils';
import schema from './schema';
import { initModel, ReportHistory } from 'db/reports/models/ReportHistory';
import { Op } from 'sequelize';

const fetchHistory: ValidatedEventAPIGatewayProxyEvent<typeof schema> = async (
  event
) => {
  console.info('event.body', JSON.stringify(event.body));
  const { data, total } = await fetchReportData(
    event.body.page,
    event.body.pageSize,
    event.body.reportTypes,
    event.body.dateFrom ? new Date(event.body.dateFrom) : null,
    event.body.dateTo ? new Date(event.body.dateTo) : null
  );
  return formatJSONResponse({
    data,
    total,
  });
};

const fetchReportData = async (
  page: number,
  pageSize: number,
  reportTypes: Array<string> | null,
  dateFrom: Date | null,
  dateTo: Date | null
) => {
  console.info(`Fetching Report Data`);

  const sequelize = await getSequelize({
    parameterName: 'report',
    dbHost: 'backoffice-db-ro-host',
    dbPassword: 'backoffice-db-password',
    dbUsername: 'backoffice-db-username',
    dbName: 'report_db',
  });
  try {
    let filters = {};

    if (reportTypes) {
      filters = { reportName: reportTypes, ...filters };
    }

    if (dateFrom && dateTo) {
      /** Convert TimeZone */
      // If date range is from 2023-11-07 to 2023-11-08 BKK TimeZone (UTC+ 7)
      // It queries the DB from 2023-11-06T17:00:00 UTC To 2023-11-08T17:00:00 UTC

      // If date range is from 2023-11-09 to 2023-11-09 BKK TimeZone (UTC+ 7)
      // It queries the DB from 2023-11-08T17:00:00 UTC To 2023-11-09T17:00:00 UTC
      const filterDateFrom = new Date(dateFrom.getTime() - 7 * 60 * 60 * 1000);
      const filterDateTo = new Date(dateTo.getTime() + 17 * 60 * 60 * 1000);
      filters = {
        updatedAt: {
          [Op.and]: {
            [Op.gte]: filterDateFrom,
            [Op.lte]: filterDateTo,
          },
        },
        ...filters,
      };
    }

    initModel(sequelize);
    const total = await ReportHistory.count({ where: filters });
    const data = await ReportHistory.findAll({
      offset: (page - 1) * pageSize,
      limit: pageSize,
      order: [['updatedAt', 'DESC']],
      where: filters,
    });

    return { data, total: total };
  } catch (e) {
    console.error('Failed to fetch report data\n', +JSON.stringify(e));
    throw e;
  }
};

export const main = middyfy(fetchHistory);
