import type { ValidatedEventAPIGatewayProxyEvent } from '@libs/api-gateway';
import { formatJSONResponse } from '@libs/api-gateway';
import { getSequelize } from '@libs/db-utils';
import { middyfy } from '@libs/lambda';
import { initModel, ReportHistory } from 'db/reports/models/ReportHistory';
import { ReportStatus } from '../../../constants/report';
import schema from './schema';

const insert: ValidatedEventAPIGatewayProxyEvent<typeof schema> = async (
  event
) => {
  console.info(JSON.stringify(event));
  await insertReportHistory(
    event.body.id,
    event.body.fileName,
    event.body.userName,
    event.body.dateFrom,
    event.body.dateTo,
    event.body.status ?? ReportStatus.Processing,
    event.body.fileKey ?? null
  );
  return formatJSONResponse({
    id: event.body.id,
    dateFrom: event.body.dateFrom,
    dateTo: event.body.dateTo,
  });
};

const insertReportHistory = async (
  id: string,
  fileName: string,
  userName: string,
  dateFrom: string,
  dateTo: string,
  status?: string | null,
  fileKey?: string | null
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
      reportName: fileName,
      userName: userName,
      dateFrom: new Date(dateFrom),
      dateTo: new Date(dateTo),
      status: status,
      filePath: fileKey ?? null,
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
