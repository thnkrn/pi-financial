import got from 'got';

const timeoutConfig = { request: Number(process.env.API_TIMEOUT || 100000) };

export type InternalEmployeesResponse = {
  data: {
    id: string;
    nameTh: string;
    teamId: string;
    email: string;
  }[];
};

export async function getEmployeeByIds(
  employeeIds: string[],
  employeeServiceHost: string
) {
  const url = new URL('/internal/employees', employeeServiceHost);
  console.log(`Path: {employee}/internal/employees`);
  const commaSeparatedCodes = employeeIds.join(',');
  url.searchParams.append('ids', commaSeparatedCodes);

  return got
    .get(url, { timeout: timeoutConfig })
    .json<InternalEmployeesResponse>()
    .catch((e) => {
      console.error(
        `Failed to get employee from employeeIds. employeeIds: ${employeeIds}. Exception: ${e}`
      );
      throw e;
    });
}
