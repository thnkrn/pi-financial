import { DataTypes, Model, Optional } from 'sequelize';
import mySqlDbConnection from '@cgs-bank/database-services/mysql-db-connection';
import { BankServiceRecord } from '@cgs-bank/models/bank-service-record';
import { SCBRegistrationInformation } from '@cgs-bank/external-api-services/scb-api/response/scb-dd-registration-information';
import { RegistrationCallbackResponse } from '@cgs-bank/models/common/registration';

interface SCBDDRegisterResultAttribute
  extends BankServiceRecord,
    SCBRegistrationInformation,
    RegistrationCallbackResponse {}

export type SCBDDRegisterResultInput = Optional<
  SCBDDRegisterResultAttribute,
  'id'
>;

export class MySqlSCBDDRegisterResult
  extends Model<SCBDDRegisterResultAttribute, SCBDDRegisterResultInput>
  implements SCBDDRegisterResultAttribute
{
  public readonly id: number;

  // SCBRegistrationInformation
  accountNo: string;
  backURL: string;
  errorCode: string;
  ref1: string;
  ref2: string;
  regRef: string;
  statusCode: string;
  statusDesc: string;

  // RegistrationCallbackResponse
  registrationRefCode: string;

  public readonly createdAt!: Date;
  public readonly updatedAt!: Date;
}

MySqlSCBDDRegisterResult.init(
  {
    id: {
      type: DataTypes.INTEGER.UNSIGNED,
      autoIncrement: true,
      primaryKey: true,
    },

    // SCBRegistrationInformation
    accountNo: { type: DataTypes.STRING },
    backURL: {
      field: 'back_url',
      type: DataTypes.STRING,
    },

    errorCode: { type: DataTypes.STRING },
    ref1: { type: DataTypes.STRING },
    ref2: { type: DataTypes.STRING },
    regRef: { type: DataTypes.STRING },
    statusCode: { type: DataTypes.STRING },
    statusDesc: { type: DataTypes.STRING },

    // RegistrationCallbackResponse
    registrationRefCode: { type: DataTypes.STRING },
  },
  {
    tableName: 'scb_dd_register_result',
    timestamps: true,
    underscored: true,
    sequelize: mySqlDbConnection,
  }
);
