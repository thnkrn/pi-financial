import { utils, WorkBook, WorkSheet, write } from 'xlsx';

const createReport = (
  header: string[],
  data: object[],
  wscols: object[] = null,
  merges: object[] = null,
): Buffer => {
  const workbook: WorkBook = utils.book_new();
  const worksheet: WorkSheet = utils.json_to_sheet(data, {
    skipHeader: false,
  });
  if (wscols !== null) {
    worksheet['!cols'] = wscols;
  }
  if (merges !== null) {
    // eslint-disable-next-line @typescript-eslint/ban-ts-comment
    // @ts-ignore
    worksheet['!merges'] = merges;
  }
  utils.sheet_add_aoa(worksheet, [header]);
  utils.book_append_sheet(workbook, worksheet);
  return write(workbook, { bookType: 'xlsx', type: 'buffer' });
};

export const createTaxInvoiceMonthlyReport = (data: object[]): Buffer => {
  const topHeader = [
    '',
    'Tax Invoice',
    '',
    '',
    '',
    'Customer Address',
    '',
    '',
    '',
    '',
  ];
  const secondHeaders = [
    'No.',
    'Date',
    'Tax Invoice No.',
    'Customer Name',
    'Customer ID',
    'Headquarter',
    'Branch',
    'Amount_ex_VAT',
    'VAT',
    'Total Amount_in_VAT',
  ];

  return createReport(topHeader, [secondHeaders, ...data], null, [
    { s: { r: 0, c: 2 }, e: { r: 0, c: 3 } },
    { s: { r: 0, c: 6 }, e: { r: 0, c: 7 } },
  ]);
};

export const createDailyOracleReport = (
  header: string[],
  data: object[],
  summary: object[],
): Buffer => {
  const rowCount = data.length + 1;
  return createReport(header, data.concat(summary), null, [
    { s: { r: rowCount, c: 0 }, e: { r: rowCount, c: 2 } },
  ]);
};
