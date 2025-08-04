import {
  Model,
  Table,
  Column,
  DataType,
  PrimaryKey,
  BelongsTo,
  ForeignKey,
} from 'sequelize-typescript';
import { Team } from './team';

@Table({ tableName: 'tblTus', timestamps: false })
export class Marketing extends Model {
  @PrimaryKey
  @Column(DataType.STRING)
  sUserid: string;

  @Column(DataType.STRING)
  sUserTname: string;

  @ForeignKey(() => Team)
  @Column(DataType.STRING)
  sTeamid: string;

  @BelongsTo(() => Team)
  team: Team;
}
