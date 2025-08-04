import type { ValidatedEventAPIGatewayProxyEvent } from '@libs/api-gateway';
import { formatJSONResponse } from '@libs/api-gateway';
import schema from './schema';
import { middyfy } from '@libs/lambda';
import { getSequelize } from '@libs/db-utils';
import { ReportStatus } from '../../../constants/report';
import { initModel, ReportHistory } from 'db/reports/models/ReportHistory';

const insert: ValidatedEventAPIGatewayProxyEvent<typeof schema> = async (
  event
) => {
  console.log(JSON.stringify(event));
  await insertReportData(
    event.body.id,
    event.body.reportName,
    event.body.userName,
    event.body.dateFrom,
    event.body.dateTo,
    event.body.status ?? ReportStatus.Processing,
    event.body.fileName ?? null
  );
  return formatJSONResponse({
    id: event.body.id,
    dateFrom: event.body.dateFrom,
    dateTo: event.body.dateTo,
  });
};

const insertReportData = async (
  id: string,
  reportName: string,
  userName: string,
  dateFrom: string,
  dateTo: string,
  status?: string | null,
  fileName?: string | null
) => {
  console.info(`Insert Report Data for ${id}`);

  const sequelize = await getSequelize({
    parameterName: 'report',
    dbHost: 'backoffice-db-host',
    dbPassword: 'backoffice-db-password',
    dbUsername: 'backoffice-db-username',
    dbName: 'report_db',
  });
  try {
    const data = {
      id: id,
      reportName: reportName,
      userName: userName,
      dateFrom: new Date(dateFrom),
      dateTo: new Date(dateTo),
      status: status,
      filePath: fileName,
      createdAt: new Date(),
      updatedAt: new Date(),
    };
    initModel(sequelize);
    await ReportHistory.create(data);
  } catch (e) {
    console.error('Failed to insert report data\n', +JSON.stringify(e));
    throw e;
  }
};

export const main = middyfy(insert);
