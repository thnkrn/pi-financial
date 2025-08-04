import {
  Model,
  Table,
  Column,
  DataType,
  PrimaryKey,
  HasMany,
  DefaultScope,
} from 'sequelize-typescript';
import { Op } from 'sequelize';
import { Marketing } from './marketing';

@DefaultScope(() => ({
  where: {
    sTName: { [Op.notLike]: '*%' },
    sTeamid: { [Op.notIn]: ['0000000000'] },
  },
}))
@Table({ tableName: 'tblTeam', timestamps: false })
export class Team extends Model {
  @PrimaryKey
  @Column(DataType.STRING)
  sTeamid: string;

  @Column(DataType.STRING)
  sTName: string;

  @Column(DataType.STRING)
  sTmDepartment: string;

  @Column(DataType.STRING)
  sTmGroup: string;

  @HasMany(() => Marketing)
  marketings: Marketing[];
}
