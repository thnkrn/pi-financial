import { sequelizeConfig } from '@libs/mssql';
import Logger from '@utils/datadog-utils';
import { Op } from 'sequelize';
import { Sequelize } from 'sequelize-typescript';
import { Tus } from './model/tus';

export class DbClientBackoffice {
  private sequelize: Sequelize;
  private readonly host: string;
  private readonly port: string;
  private readonly user: string;
  private readonly password: string;
  private readonly database: string;
  private readonly debug: boolean;
  private readonly timeout: string;

  constructor(
    host: string = process.env.BACKOFFICE_DB_HOST,
    port: string = process.env.BACKOFFICE_DB_PORT,
    user: string = process.env.BACKOFFICE_DB_USER,
    password: string = process.env.BACKOFFICE_DB_PASSWORD,
    database: string = process.env.BACKOFFICE_DB_NAME,
    debug: boolean = process.env.DEBUG === 'true',
    timeout = process.env.BACKOFFICE_DB_REQUEST_TIMEOUT_MS,
  ) {
    this.host = host;
    this.port = port;
    this.user = user;
    this.password = password;
    this.database = database;
    this.debug = debug;
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
          [Tus],
        ),
      );
    }
  }

  public async getMktEmail(userIds: string[]): Promise<Tus[]> {
    const tusRepository = this.sequelize.getRepository(Tus);
    try {
      const results = await tusRepository.findAll({
        attributes: ['userid', 'emailaddress', 'usertname'],
        where: {
          userid: {
            [Op.in]: userIds,
          },
        },
      });

      return results.map((result) => result.dataValues);
    } catch (error) {
      Logger.error(`Error: ${error}`);
      throw new Error('Error fetching get Marketing Email');
    }
  }
}
