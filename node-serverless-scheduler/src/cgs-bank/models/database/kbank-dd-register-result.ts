import { BankServiceRecord } from '@cgs-bank/models/bank-service-record';
import { KbankRegistrationCallback } from '@cgs-bank/external-api-services/kbank-api/kbank-registration-callback';
import { DataTypes, Model, Optional } from 'sequelize';
import mySqlDbConnection from '@cgs-bank/database-services/mysql-db-connection';
import { decrypt, encrypt } from '@cgs-bank/utils/aes-coder';

interface KBANKDDRegisterResultAttributed
  extends BankServiceRecord,
    KbankRegistrationCallback {}

export type KBANKDDRegisterResultInput = Optional<
  KBANKDDRegisterResultAttributed,
  'id'
>;

export class MySqlKBANKDDRegisterResult
  extends Model<KBANKDDRegisterResultAttributed, KBANKDDRegisterResultInput>
  implements KBANKDDRegisterResultAttributed
{
  readonly id: number;

  // KbankRegistrationCallback
  accountNo: string;
  espaId: string;
  externalReference: string;
  idMatching: string;
  payerShortName: string;
  returnCode: string;
  returnMessage: string;
  returnStatus: string;
  timestamp: string;
  userEmailMatching: string;
  userMobileMatching: string;

  readonly createdAt: Date;
  readonly updatedAt: Date;
}

MySqlKBANKDDRegisterResult.init(
  {
    id: {
      type: DataTypes.INTEGER.UNSIGNED,
      autoIncrement: true,
      primaryKey: true,
    },
    accountNo: { type: DataTypes.STRING },
    espaId: { type: DataTypes.STRING },
    externalReference: { type: DataTypes.STRING },
    idMatching: { type: DataTypes.STRING },
    payerShortName: { type: DataTypes.STRING },
    returnCode: { type: DataTypes.STRING },
    returnMessage: { type: DataTypes.STRING },
    returnStatus: { type: DataTypes.STRING },
    timestamp: { type: DataTypes.STRING },
    userEmailMatching: { type: DataTypes.STRING },
    userMobileMatching: { type: DataTypes.STRING },
  },
  {
    tableName: 'kbank_dd_register_result',
    timestamps: true,
    underscored: true,
    sequelize: mySqlDbConnection,
    hooks: {
      beforeCreate(attributes, _) {
        attributes.dataValues.accountNo = encrypt(
          attributes.dataValues.accountNo
        );
      },
      beforeUpdate: (attributes, _) => {
        attributes.dataValues.accountNo = encrypt(
          attributes.dataValues.accountNo
        );
      },
      afterFind: (
        result: MySqlKBANKDDRegisterResult | MySqlKBANKDDRegisterResult[] | null
      ) => {
        if (Array.isArray(result)) {
          for (const obj of result) {
            if (obj.dataValues.accountNo)
              obj.dataValues.accountNo = decrypt(obj.dataValues.accountNo);
          }
        } else if (result.dataValues.accountNo) {
          result.dataValues.accountNo = decrypt(result.dataValues.accountNo);
        }
      },
    },
  }
);
