import { Column, DataType, Model, Table } from 'sequelize-typescript';

@Table({ tableName: 'tus', timestamps: false })
export class Tus extends Model {
  @Column(DataType.NUMBER)
  get userid(): number {
    return this.getDataValue('userid');
  }

  @Column(DataType.STRING)
  get emailaddress(): string {
    return this.getDataValue('emailaddress');
  }

  @Column(DataType.STRING)
  get usertname(): string {
    return this.getDataValue('usertname');
  }
}
