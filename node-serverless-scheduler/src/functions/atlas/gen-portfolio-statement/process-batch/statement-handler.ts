import { initModel as initModelPortfolioJobHistory } from 'db/portfolio-summary/models/PortfolioJobHistory';

import { convertFilesToAttachments, sendEmailToSES } from '@libs/email-utils';
import { InternalEmployeesResponse } from '@libs/employee-api';
import {
  InternalCustomerByIdentificationNo,
  UserInfo,
} from '@libs/user-v2-api';
import { PortfolioJobHistory } from 'db/portfolio-summary/models/PortfolioJobHistory';
import { Op, Sequelize } from 'sequelize';
import { CustomerData } from '../get-customers/handler';
import {
  companyLogoAttachment,
  getPDFPassword,
  mergePdf,
  modifyAndEncryptPdf,
} from '../utils/process-batch-utils';
import {
  buildGroupedMarketingProductTypes,
  getCustomerTradingInfo,
  getEmployeeInfos,
  getFormattedDate,
  getLastDateToQuery,
  getUserIdByCustomerCode,
  loadSequelize,
  splitByEmail,
} from './handler-helper';
import { GroupedMarketingProductType } from './internal-model';

export async function statementHandler(
  event,
  context,
  buildPdfBuffer: (
    customerInfo: InternalCustomerByIdentificationNo,
    marketingInfos: InternalEmployeesResponse,
    groupedMarketings: GroupedMarketingProductType[],
    marketingId: string,
    lastDateToQuery: string,
    sequelize: Sequelize,
    isHttp: boolean,
    isMarketingEmail: boolean,
    dateFrom: Date | null,
    dateTo: Date | null,
    custcode: string,
    userId: string
  ) => Promise<Buffer>,
  htmlEmailContent: string,
  filePrefix: string
) {
  console.log('Function name: ', context.functionName);
  const isHttp = event.body ? true : false;

  let customerBatch: CustomerData[] = [];
  if (isHttp) {
    let userId = '';
    let userInfos: UserInfo;
    if (event.body.userId) {
      userId = event.body.userId;
      if (
        event.path?.includes('atlasGenGlobalEquityStatementProcessBatch') &&
        !event.body?.custcode
      ) {
        throw new Error(
          'No custcode provided in atlasGenGlobalEquityStatementProcessBatch'
        );
      }
    } else if (event.body.custcode) {
      console.log('[API](1) - getCustomerInfoByCustcode [UserV2]');
      try {
        const userInfosResponse = await getUserIdByCustomerCode(
          event.body.custcode
        );
        const matchUserInfos = userInfosResponse.find((info) =>
          info.custCodes.some((code) => code === event.body.custcode)
        );
        userId = matchUserInfos.id;
        userInfos = matchUserInfos;
      } catch (error) {
        console.log(error);
      }
    } else {
      throw new Error('No identificationHash or custcode provided');
    }

    customerBatch = [
      {
        ids: [],
        userId: userId,
        userInfos: userInfos,
        marketingId: event.body.marketingId,
        sendDate: event.body.sendDate,
        dateFrom: event.body.dateFrom,
        dateTo: event.body.dateTo,
        custcode: event.body.custcode,
      },
    ];
  } else {
    customerBatch = event;
  }

  let sequelize: Sequelize = null;
  // re-use the sequelize instance across invocations to improve performance
  if (!sequelize) {
    console.log('Load Sequelize');
    sequelize = await loadSequelize();
  } else {
    console.log('Restart Sequelize');
    // restart connection pool to ensure connections are not re-used across invocations
    sequelize.connectionManager.initPools();
    // restore `getConnection()` if it has been overwritten by `close()`
    if ('getConnection' in sequelize.connectionManager) {
      console.log('delete getConnection');
      delete sequelize.connectionManager.getConnection;
    }
  }

  try {
    initModelPortfolioJobHistory(sequelize);

    const encodedPdfs = [];

    await Promise.all(
      customerBatch.map(async (customer) => {
        const dateFrom = customer.dateFrom;
        const dateTo = customer.dateTo;
        const custcode = customer.custcode;
        console.log('[API](2) - getCustomerInfoByUserId [UserV2]');
        const customerIdentification = await getCustomerTradingInfo(
          customer.userInfos,
          customer.marketingId
        );

        const isMarketingEmail = customer.marketingId != '';
        let customerGroupedByEmail = splitByEmail(customerIdentification);
        if (isHttp || isMarketingEmail) {
          customerGroupedByEmail = [
            {
              customerEmail: null, //not used
              dateOfBirth: null, //not used
              customerInfo: customerIdentification,
            },
          ];
        }

        await Promise.all(
          customerGroupedByEmail.map(async (group) => {
            const groupedMarketings = buildGroupedMarketingProductTypes(
              group.customerInfo
            );

            console.log(JSON.stringify(groupedMarketings));

            const lastDateToQuery = getLastDateToQuery(
              customer.sendDate ?? new Date(),
              isMarketingEmail || isHttp
            );

            console.log('[API](3) - getEmployeeInfos [Employee]');
            const marketingInfos = await getEmployeeInfos(
              groupedMarketings.map((marketing) => marketing.marketingId)
            );

            let pdfBuffer: Buffer = await buildPdfBuffer(
              group.customerInfo,
              marketingInfos,
              groupedMarketings,
              customer.marketingId,
              lastDateToQuery,
              sequelize,
              isHttp,
              isMarketingEmail,
              dateFrom,
              dateTo,
              custcode,
              customer.userId
            );

            if (pdfBuffer != null) {
              if (group.dateOfBirth) {
                const encryptedPdf = await modifyAndEncryptPdf(
                  pdfBuffer,
                  getPDFPassword(group.dateOfBirth)
                );
                pdfBuffer = Buffer.from(encryptedPdf);
              } else {
                pdfBuffer = Buffer.from(pdfBuffer);
              }

              encodedPdfs.push({
                userId: customer.userId,
                encodedPdf: pdfBuffer,
              });

              const filename = `${filePrefix}_${customer.sendDate}.pdf`;
              const attachments = convertFilesToAttachments(
                [filename],
                [pdfBuffer]
              );
              attachments.push(companyLogoAttachment);

              const isSendingEmail: boolean =
                (group.customerEmail && process.env.DEBUG_LOCAL !== 'true') ||
                event.body.emailToSend;
              if (isSendingEmail) {
                let emailToSend = isMarketingEmail
                  ? marketingInfos.data.find(
                      (info) => info.id == customer.marketingId
                    )?.email
                  : group.customerEmail;

                if (event.body.emailToSend)
                  emailToSend = event.body.emailToSend;

                await sendEmailToSES(
                  'pi-tech-byd@pi.financial', //TODO use emailToSend
                  `[Test] Pi Securities Monthly Statement (${customer.sendDate})`,
                  attachments,
                  'no-reply@transaction.pi.financial',
                  htmlEmailContent
                );
                console.log(
                  'sending email to ' +
                    customer.custcode +
                    ' with ' +
                    emailToSend +
                    ' on ' +
                    customer.ids
                );
              }
            }
          })
        );

        if (!isHttp && process.env.DEBUG_LOCAL !== 'true') {
          await PortfolioJobHistory.update(
            { status: 'success' },
            {
              where: {
                id: {
                  [Op.in]: customer.ids,
                },
              },
            }
          );
        }
      })
    );

    if (isHttp) {
      const result = Buffer.from(
        await mergePdf(encodedPdfs.map((pdf) => pdf.encodedPdf))
      ).toString('base64');

      const custcode = event.body.custcode;
      console.log('Returning PDF Response');
      return {
        statusCode: 200,
        headers: {
          'Content-Type': 'application/pdf',
          'Content-Disposition': `attachment; filename=${filePrefix}_${getFormattedDate()}${
            custcode ? `_${event.body.custcode}` : '' //only available via http get
          }.pdf`,
        },
        body: result,
        isBase64Encoded: true,
      };
    } else {
      return {
        statusCode: 200,
        body: JSON.stringify({ message: 'Batch processed successfully' }),
      };
    }
  } catch (error) {
    console.error(error);
    return {
      statusCode: 400,
      body: JSON.stringify({ exception: error }),
    };
  } finally {
    if (sequelize) {
      if (sequelize.connectionManager) {
        const readConnection = await sequelize.connectionManager.getConnection({
          type: 'read',
        });
        const writeConnection = await sequelize.connectionManager.getConnection(
          {
            type: 'write',
          }
        );

        await sequelize.connectionManager.releaseConnection(readConnection);
        await sequelize.connectionManager.releaseConnection(writeConnection);
        await sequelize.connectionManager.destroyConnection(readConnection);
        await sequelize.connectionManager.destroyConnection(writeConnection);
        await sequelize.connectionManager.close();
        await sequelize.close();
      }
    }
    console.log('Closed Sequelize Connection to ensure all connections closed');
  }
}
