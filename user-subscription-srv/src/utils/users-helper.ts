import { query } from '@libs/mssql';
import { PiChkfrontname, UserInfo } from '@types';
import Logger from '@utils/datadog-utils';
import { IRecordSet } from 'mssql';

const parseUser = <T>(rows: IRecordSet<any>) => {
  return rows.map((row) => {
    return {
      custcode: row.custcode?.trim() ?? undefined,
      account: row.account?.trim() ?? undefined,
      effdate: row.effdate ?? undefined,
      enddate: row.enddate ?? undefined,
      isActive: row.isActive === 1,
    } as T;
  });
};

const parsePayableUser = <T>(rows: IRecordSet<any>) => {
  return rows.map((row) => {
    return {
      custcode: row.custcode?.trim() ?? undefined,
      account: row.account?.trim() ?? undefined,
      effdate: row.effdate ?? undefined,
      enddate: row.enddate ?? undefined,
      frontname: row.frontname ?? undefined,
      itradeflag: row.itradeflag ?? undefined,
      custacct: row.custacct ?? undefined,
    } as T;
  });
};

export const getNewUsersFromSBA = async (date: string) => {
  try {
    const sqlQuery = `
    SELECT DISTINCT custcode, effdate
    FROM dbo.portal_tpa
    WHERE itradeflag = '1'
    AND custacct <> 'F'
    AND frontname = 'Q'
    AND enddate = '9999-12-31'
    AND effdate >= '${date}'`;

    // Get user from Freeview
    const _freeviewUsers = await query(sqlQuery);
    return parseUser<UserInfo>(_freeviewUsers.recordset);
  } catch (error) {
    Logger.error(`Error connecting to BackOfficeN: ${error}`);
    throw error;
  }
};

export const getAccountsFromSBA = async (
  custcode: string,
  active: boolean | null,
) => {
  const sqlQuery = `
SELECT custcode, account, enddate, CASE WHEN enddate = '9999-12-31' THEN 1 ELSE 0 END AS isActive
FROM (
    SELECT custcode, trim(account) AS account, MAX(enddate) AS enddate
	FROM dbo.portal_tpa
	WHERE custcode = '${custcode}'
	  --AND itradeflag = '1'
	  AND canvieworder = '1'
	  AND frontname = 'Q'
	  AND grouptype = 'G'
	  -- custacct is a AccountCode's suffix by H=8
	  AND custacct in ('1','6','H')
	GROUP BY custcode, trim(account)
) tmp `;
  const users = await query(sqlQuery);
  return parseUser<UserInfo>(users.recordset).filter(
    (i) => active === null || i.isActive === active,
  );
};

/**
 * Retrieves inactive accounts from SBA for a given customer code.
 * @param custcode The customer code
 * @returns An array of UserInfo representing the inactive accounts.
 */
export const getInactiveAccountsFromSBA = async (custcode: string) => {
  try {
    return await getAccountsFromSBA(custcode, false);
  } catch (error) {
    Logger.error(`Error connecting to BackOfficeN: ${error}`);
    throw error;
  }
};

/**
 * Retrieves inactive accounts from SBA for a given customer code.
 * @param custcode The customer code
 * @returns An array of UserInfo representing the inactive accounts.
 */
export const getActiveAccountsFromSBA = async (custcode: string) => {
  try {
    return await getAccountsFromSBA(custcode, true);
  } catch (error) {
    Logger.error(`Error connecting to BackOfficeN: ${error}`);
    throw error;
  }
};

export const getPiChkfrontnameSBA = async (custcode: string) => {
  const sqlQuery = `EXEC dbo.pi_chkfrontname '${custcode}'`;
  const users = await query(sqlQuery);
  return parsePayableUser<PiChkfrontname>(users.recordset);
};

/**
 * Retrieves payable accounts from SBA for a given customer code.
 * @param custcode The customer code
 * @returns An array of PiChkfrontname representing the accounts.
 */
export const getPayableAccountsFromSBA = async (custcode: string) => {
  try {
    const items = await getPiChkfrontnameSBA(custcode);
    return items.filter(
      (i) =>
        i.frontname === 'Q' &&
        i.itradeflag === '1' &&
        ['1', '6', 'H'].includes(i.custacct),
    );
  } catch (error) {
    Logger.error(`Error connecting to BackOffice: ${error}`);
    throw error;
  }
};

export const getUniqCustcodes = (freeviewUsers: UserInfo[]) => {
  const codes = freeviewUsers.reduce((items: Set<string>, row: UserInfo) => {
    items.add(row.custcode);
    return items;
  }, new Set());
  return [...codes];
};
