import { getProductFromAccountCode } from '@libs/smartsales/db-smartsales-client';
import { ProductType } from '@libs/smartsales/model/product-type';
import {
  BelongsTo,
  Column,
  DataType,
  ForeignKey,
  Model,
  PrimaryKey,
  Table,
} from 'sequelize-typescript';
import { Customer } from './customer';
import { Marketing } from './marketing';

@Table({ tableName: 'tblCustDetail', timestamps: false })
export class Account extends Model {
  @PrimaryKey
  @Column({
    type: DataType.STRING,
    get() {
      return this.getDataValue('sAccount').trim();
    },
  })
  sAccount: string;

  @ForeignKey(() => Customer)
  @Column({
    type: DataType.STRING,
    get() {
      return this.getDataValue('sCustCode').trim();
    },
  })
  sCustCode: string;

  @BelongsTo(() => Customer)
  customer: Customer;

  @ForeignKey(() => Marketing)
  @Column(DataType.STRING)
  sMktID: string;

  @BelongsTo(() => Marketing)
  marketing: Marketing;

  @Column({
    type: DataType.VIRTUAL,
    get() {
      return getProductFromAccountCode(this.getDataValue('sAccount'));
    },
  })
  product: ProductType;
}
