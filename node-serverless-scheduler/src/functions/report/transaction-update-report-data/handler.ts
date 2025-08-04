import { formatJSONResponse } from '@libs/api-gateway';
import { middyfy } from '@libs/lambda';
import { getSequelize } from '@libs/db-utils';
import { initModel, ReportHistory } from 'db/reports/models/ReportHistory';

interface RequestData {
  id: string;
  status: string;
  fileName: string;
}

const update = async (event) => {
  const request = JSON.parse(event.body) as RequestData;
  await updateReportData(request.id, request.status, request.fileName);
  return formatJSONResponse({
    id: request.id,
    status: request.status,
    fileName: request.fileName,
  });
};

const updateReportData = async (
  reportId: string,
  status: string,
  fileName: string
) => {
  console.info(`Update Report Data for ${reportId}`);
  const sequelize = await getSequelize({
    parameterName: 'report',
    dbHost: 'backoffice-db-host',
    dbPassword: 'backoffice-db-password',
    dbUsername: 'backoffice-db-username',
    dbName: 'report_db',
  });

  try {
    initModel(sequelize);
    await ReportHistory.update(
      { status: status, filePath: fileName },
      {
        where: {
          id: reportId,
        },
      }
    );
  } catch (e) {
    console.error('Failed to update report data\n', +JSON.stringify(e));
    throw e;
  }
};

export const main = middyfy(update);
