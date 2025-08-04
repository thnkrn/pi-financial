import { DataTypes, Model, Optional } from 'sequelize';
import mySqlDbConnection from '@cgs-bank/database-services/mysql-db-connection';

import { ScbBaseRecord } from '@cgs-bank/models/scb/scb-base-record';
import { SCBRegistrationResponse } from '@cgs-bank/external-api-services/scb-api/response/scb-registration-response';
import { RegistrationRequest } from '@cgs-bank/models/common/registration';
import { decrypt, encrypt } from '@cgs-bank/utils/aes-coder';

interface SCBDDRegisterAttribute
  extends ScbBaseRecord,
    RegistrationRequest,
    SCBRegistrationResponse {}

export type SCBDDRegisterInput = Optional<SCBDDRegisterAttribute, 'id'>;

export class MySqlSCBDDRegister
  extends Model<SCBDDRegisterAttribute, SCBDDRegisterInput>
  implements SCBDDRegisterAttribute
{
  public readonly id: number;

  // RegistrationRequest
  citizenId: string;
  redirectUrl: string;
  registrationRefCode: string;
  remarks: string;

  // SCBBaseResponseAttributed
  merchantId: string;
  subAccountId: string;

  // SCBResultStatus
  code: string;
  description: string;
  details: string;

  // SCB Registration Response
  webURL: string;

  public readonly createdAt!: Date;
  public readonly updatedAt!: Date;
}

MySqlSCBDDRegister.init(
  {
    id: {
      type: DataTypes.INTEGER.UNSIGNED,
      autoIncrement: true,
      primaryKey: true,
    },
    citizenId: { type: DataTypes.STRING },
    redirectUrl: { type: DataTypes.STRING },
    registrationRefCode: { type: DataTypes.STRING },
    remarks: { type: DataTypes.STRING },

    // SCBBaseResponseAttributed
    merchantId: { type: DataTypes.STRING },
    subAccountId: { type: DataTypes.STRING },

    // SCBResultStatus
    code: { type: DataTypes.STRING },
    description: { type: DataTypes.STRING },
    details: { type: DataTypes.STRING },

    // SCB Registration Response
    webURL: {
      field: 'web_url',
      type: DataTypes.STRING,
    },
  },
  {
    tableName: 'scb_dd_register',
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
      afterFind: (result: MySqlSCBDDRegister | MySqlSCBDDRegister[] | null) => {
        if (Array.isArray(result)) {
          result.forEach((obj) => {
            if (obj.dataValues.citizenId)
              obj.dataValues.citizenId = decrypt(obj.dataValues.citizenId);
          });
        } else if (result.dataValues.citizenId) {
          result.dataValues.citizenId = decrypt(result.dataValues.citizenId);
        }
      },
    },
  }
);
