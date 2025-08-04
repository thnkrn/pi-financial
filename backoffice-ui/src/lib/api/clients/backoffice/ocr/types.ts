import { PiBackofficeServiceApplicationModelsOcrThirdPartyApiResponse } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceApplicationModelsOcrThirdPartyApiResponse'

export interface IGetOcrResultsRequest {
  files: string[]
  documentType: string
  output: string
  password: string | null
}

export interface IGetOcrResultsResponse extends PiBackofficeServiceApplicationModelsOcrThirdPartyApiResponse {}
