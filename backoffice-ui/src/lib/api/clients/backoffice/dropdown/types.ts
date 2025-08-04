import { PiBackofficeServiceAPIModelsNameAliasResponse } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceAPIModelsNameAliasResponse'
import { PiBackofficeServiceAPIModelsResponseCodeResponse } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceAPIModelsResponseCodeResponse'
import { PiBackofficeServiceApplicationModelsReportTypeNameAliasResponse } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceApplicationModelsReportTypeNameAliasResponse'
import { PiBackofficeServiceDomainAggregateModelsTransactionAggregateBank } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceDomainAggregateModelsTransactionAggregateBank'

export interface IGetDropdownRequest {
  field: string
  url: string
}

export interface IGetDropdownResponse {
  field: string
  data:
    | PiBackofficeServiceAPIModelsResponseCodeResponse[]
    | PiBackofficeServiceApplicationModelsReportTypeNameAliasResponse[]
    | PiBackofficeServiceDomainAggregateModelsTransactionAggregateBank[]
    | PiBackofficeServiceAPIModelsNameAliasResponse[]
}
