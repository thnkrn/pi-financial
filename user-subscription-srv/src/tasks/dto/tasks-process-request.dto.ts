export class TasksProcessRequest {
  fromDate?: string;
  toDate?: string;
  mailTo?: string;
}

export class TasksProcessRequestWithMktId extends TasksProcessRequest {
  mktId?: string;
}

export class ManualUpdateCustCodeRequest {
  currentCustCode: string;
  newCustCode: string;
  effectiveDate: string;
  force?: boolean = false;
}
