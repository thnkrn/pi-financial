export interface UserInfo {
  custcode?: string;
  account?: string;
  effdate?: Date;
  enddate?: Date;
  isActive?: boolean;
}

export interface PiChkfrontname {
  custacct: string;
  itradeflag: string;
  frontname: string;
  custcode: string;
  account: string;
  effdate: Date;
  enddate: Date;
}
