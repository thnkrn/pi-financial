import {
  Model,
  Table,
  Column,
  DataType,
  PrimaryKey,
  HasMany,
  BelongsToMany,
} from 'sequelize-typescript';
import { Marketing } from './marketing';
import { Account } from './account';

@Table({ tableName: 'tblCustomer', timestamps: false })
export class Customer extends Model {
  @PrimaryKey
  @Column({
    type: DataType.STRING,
    get() {
      return this.getDataValue('sCustcode').trim();
    },
  })
  sCustcode: string;

  @BelongsToMany(() => Marketing, () => Account)
  marketings: Marketing[];

  @HasMany(() => Account)
  accounts: Account[];
}
