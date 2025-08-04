import { BankServiceRecord } from '@cgs-bank/models/bank-service-record';
import { RegistrationRequest } from '@cgs-bank/models/common/registration';
import { DataTypes, Model, Optional } from 'sequelize';
import mySqlDbConnection from '@cgs-bank/database-services/mysql-db-connection';
import { encrypt, decrypt } from '@cgs-bank/utils/aes-coder';

interface KBANKDDRegisterAttributed
  extends BankServiceRecord,
    RegistrationRequest {
  regId: string;
  returnCode: string;
  returnMessage: string;
  returnStatus: string;
}

export type KBANKDDRegisterInput = Optional<KBANKDDRegisterAttributed, 'id'>;

export class MySqlKBANKDDRegister
  extends Model<KBANKDDRegisterAttributed, KBANKDDRegisterInput>
  implements KBANKDDRegisterAttributed
{
  readonly id: number;

  //RegistrationRequest
  citizenId: string;
  redirectUrl: string;
  registrationRefCode: string;

  // KBANKRegistrationResponse
  regId: string;
  returnCode: string;
  returnMessage: string;
  returnStatus: string;

  readonly createdAt: Date;
  readonly updatedAt: Date;
}

MySqlKBANKDDRegister.init(
  {
    id: {
      type: DataTypes.INTEGER.UNSIGNED,
      autoIncrement: true,
      primaryKey: true,
    },
    citizenId: {
      type: DataTypes.STRING,
    },
    redirectUrl: { type: DataTypes.STRING },
    registrationRefCode: { type: DataTypes.STRING },

    // KBANKRegistrationResponse
    regId: { type: DataTypes.STRING },
    returnCode: { type: DataTypes.STRING },
    returnMessage: { type: DataTypes.STRING },
    returnStatus: { type: DataTypes.STRING },
  },
  {
    tableName: 'kbank_dd_register',
    timestamps: true,
    underscored: true,
    sequelize: mySqlDbConnection,
    hooks: {
      beforeCreate(attributes, _) {
        attributes.dataValues.citizenId = encrypt(
          attributes.dataValues.citizenId
        );
      },
      beforeUpdate: (attributes, _) => {
        attributes.dataValues.citizenId = encrypt(
          attributes.dataValues.citizenId
        );
      },
      afterFind: (
        result: MySqlKBANKDDRegister | MySqlKBANKDDRegister[] | null
      ) => {
        if (Array.isArray(result)) {
          for (const obj of result) {
            if (obj.dataValues.citizenId)
              obj.dataValues.citizenId = decrypt(obj.dataValues.citizenId);
          }
        } else if (result.dataValues.citizenId) {
          result.dataValues.citizenId = decrypt(result.dataValues.citizenId);
        }
      },
    },
  }
);
