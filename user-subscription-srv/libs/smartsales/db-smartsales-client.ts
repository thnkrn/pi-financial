import { sequelizeConfig } from '@libs/mssql';
import { Account } from '@libs/smartsales/model/account';
import { Customer } from '@libs/smartsales/model/customer';
import { Marketing } from '@libs/smartsales/model/marketing';
import { ProductType } from '@libs/smartsales/model/product-type';
import { Team } from '@libs/smartsales/model/team';
import { Op, QueryTypes } from 'sequelize';
import { Sequelize } from 'sequelize-typescript';
import { CustomerWithMarketingDetail } from 'src/tasks/model/customer-with-marketing-detail';

export class DbClientSmartSales {
  private sequelize: Sequelize;
  private readonly host: string;
  private readonly port: string;
  private readonly user: string;
  private readonly password: string;
  private readonly database: string;
  private readonly debug: boolean;
  private readonly timeout: string;

  constructor(
    host: string = process.env.SMARTSALES_DB_HOST,
    port: string = process.env.SMARTSALES_DB_PORT,
    user: string = process.env.SMARTSALES_DB_USER,
    password: string = process.env.SMARTSALES_DB_PASSWORD,
    database: string = process.env.SMARTSALES_DB_NAME,
    timeout = process.env.SMARTDSALES_DB_REQUEST_TIMEOUT_MS,
  ) {
    this.host = host;
    this.port = port;
    this.user = user;
    this.password = password;
    this.database = database;
    this.timeout = timeout;
  }
  public async connect() {
    if (!this.sequelize) {
      this.sequelize = new Sequelize(
        await sequelizeConfig(
          this.host,
          this.port,
          this.user,
          this.password,
          this.database,
          this.timeout,
          [Team, Marketing, Customer, Account],
        ),
      );
    }
  }

  public async getCustomerDetail(customerCode: string): Promise<{
    title: string;
    name: string;
    lastname: string;
    email: string;
    address: string[];
    zipCode: string;
    cardId: string;
  }> {
    const customerRepository = this.sequelize.getRepository(Customer);
    const customer = await customerRepository.findOne({
      where: {
        sCustcode: { [Op.eq]: customerCode },
      },
      attributes: [
        'sCustcode',
        'sTTitle',
        'sTName',
        'sTSurname',
        'sFirstAddr1',
        'sFirstAddr2',
        'sFirstAddr3',
        'sFirstZipCode',
        'sCardId',
        'sEmailBackOffice',
      ],
    });

    return {
      title: customer.getDataValue('sTTitle'),
      name: customer.getDataValue('sTName'),
      lastname: customer.getDataValue('sTSurname'),
      email: customer.getDataValue('sEmailBackOffice'),
      address: [
        customer.getDataValue('sFirstAddr1'),
        customer.getDataValue('sFirstAddr2'),
        customer.getDataValue('sFirstAddr3'),
      ],
      zipCode: customer.getDataValue('sFirstZipCode'),
      cardId: customer.getDataValue('sCardId'),
    };
  }

  /**
   * Retrieves the details of customers with IC from Smartsales DB.
   * @param customerCodes An array of customer codes to retrieve details for.
   * @returns An array of `CustomerWithMarketingDetail`.
   */
  public async getCustomersDetail(
    customerCodes: string[],
  ): Promise<CustomerWithMarketingDetail[]> {
    const query = `
      SELECT tm.sMktID AS MktId, tm.sTName AS MktName, tm.sTeamID AS TeamID, tm.sEName AS EName,
      trim(tc.sCustcode) AS Custcode, tc.sTTitle + ' ' + tc.sTName + ' ' + tc.sTSurname AS CustomerName,
      tc.sFirstTelNo1 AS TelNo, tc.sEmailBackOffice AS Email
      FROM tblCustomer tc
      INNER JOIN tblMkt tm ON tm.sMktID = tc.sMktID
      WHERE trim(tc.sCustcode) IN (:customerCodes)
    `;

    const replacements = { customerCodes };
    const customers = await this.sequelize.query(query, {
      replacements,
      type: QueryTypes.SELECT,
    });

    const mappedCustomers: CustomerWithMarketingDetail[] = customers.map(
      (customer: any) => ({
        mktId: customer.MktId,
        mktName: customer.MktName,
        teamID: customer.TeamID,
        eName: customer.EName,
        custcode: customer.Custcode,
        customerName: customer.CustomerName,
        telNo: customer.TelNo,
        email: customer.Email,
      }),
    );
    return mappedCustomers;
  }

  public async getMarketingDetail(custcode: string): Promise<{
    marketingId: string;
    marketingName: string;
    marketingTeamName: string;
  }> {
    const accountRepository = this.sequelize.getRepository(Account);
    const marketingRepository = this.sequelize.getRepository(Marketing);
    const teamRepository = this.sequelize.getRepository(Team);

    const account = await accountRepository.findOne({
      where: {
        sCustCode: { [Op.eq]: custcode },
      },
      attributes: ['sMktID'],
    });
    const mktId = account.getDataValue('sMktID');
    const marketing = await marketingRepository.findOne({
      where: {
        sUserid: { [Op.eq]: mktId },
      },
      include: [
        {
          model: teamRepository,
          as: 'team',
        },
      ],
    });

    return {
      marketingId: marketing?.getDataValue('sUserid') ?? 'ID',
      marketingName: marketing?.getDataValue('sUserTname') ?? 'Marketing Name',
      marketingTeamName:
        marketing?.getDataValue('team')?.getDataValue('sTName') ?? 'Team Name',
    };
  }

  async getAccounts(customerCode: string) {
    const accountRepository = this.sequelize.getRepository(Account);
    const accounts = await accountRepository.findAll({
      where: {
        sCustCode: { [Op.eq]: customerCode },
        sAcctStatus: { [Op.not]: 'C' },
      },
      attributes: ['sAccount'],
    });
    return accounts.map((item) => item.getDataValue('sAccount') as string);
  }
}

export function getProductFromAccountCode(accountCode: string): ProductType {
  const type = accountCode.slice(accountCode.length - 1, accountCode.length);
  switch (type) {
    case '1':
    case '6':
    case '8':
      return ProductType.EQUITY;
    case '0':
      return ProductType.TFEX;
    case '2':
      return ProductType.GLOBAL_EQUITY;
    default:
      return ProductType.UNKNOWN;
  }
}
