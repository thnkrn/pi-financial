import { formatJSONResponse } from '@libs/api-gateway';
import { middyfy } from '@libs/lambda';

import { getmySqlClient } from '@libs/db-utils';
import { getPreSignedUrl } from '@libs/s3-utils';

const getS3PreSignedUrl = async (event) => {
  console.info(
    `Get Pre-Signed URL for Report ${event.pathParameters.reportId}`
  );

  const reportId = event.pathParameters.reportId;
  const bucketName = `backoffice-reports-${process.env.ENVIRONMENT}`;

  const mysql = await getmySqlClient({
    parameterName: 'report',
    dbHost: 'backoffice-db-host',
    dbPassword: 'backoffice-db-password',
    dbUsername: 'backoffice-db-username',
    dbName: 'report_db',
  });

  try {
    const results = await mysql.query<unknown[]>(
      'SELECT * FROM report_history WHERE id=?',
      [reportId]
    );
    if (results.length === 1) {
      const data = results.map((result: any) => ({
        filePath: result.file_path,
      }));

      const url = await getPreSignedUrl(bucketName, data[0].filePath);
      return formatJSONResponse({ url });
    } else {
      return formatJSONResponse({
        error: `Failed to report URL for reportId: ${reportId}`,
      });
    }
  } catch (e) {
    console.error('Failed to get report data\n', +JSON.stringify(e));
    throw e;
  } finally {
    await mysql.end();
  }
};

export const main = middyfy(getS3PreSignedUrl);
