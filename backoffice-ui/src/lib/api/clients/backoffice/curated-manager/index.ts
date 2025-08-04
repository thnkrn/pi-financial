
import { backofficeAxiosInstance } from '@/lib/api'
import { CuratedFilterGroup, CuratedFiltersResponse, IGetCuratedFiltersRequest } from '@/types/curated-manager/filter'
import { CuratedListResponse } from '@/types/curated-manager/list'
import { CuratedMembersResponse } from '@/types/curated-manager/member'

export const getCuratedList = async (): Promise<CuratedListResponse> => {
  const instance = await backofficeAxiosInstance()

  const { data } = await instance.get<CuratedListResponse>('/curated-manager/list')

  return data
}

export const uploadCuratedManualList = async (formData: FormData, dataSource: string): Promise<CuratedListResponse> => {
  const instance = await backofficeAxiosInstance()
  
  const { data } = await instance.post<CuratedListResponse>(
    `/curated-manager/list?dataSource=${dataSource}`,
    formData,
    {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    }
  )

  return data
}

export const updateCuratedListById = async (id: string, payload: Record<string, string>, dataSource: string): Promise<void> => {
  const instance = await backofficeAxiosInstance()

  await instance.patch(`/curated-manager/list/${id}?dataSource=${dataSource}`, payload)
}

export const deleteCuratedListById = async (id: string, dataSource: string): Promise<void> => {
  const instance = await backofficeAxiosInstance()

  await instance.delete(`/curated-manager/list/${id}?dataSource=${dataSource}`)
}

export const getCuratedFilterGroups = async (request: IGetCuratedFiltersRequest): Promise<CuratedFilterGroup[]> => {
  const instance = await backofficeAxiosInstance()
  const { groupName } = request

  const { data } = await instance.get<CuratedFiltersResponse>('/curated-manager/filters', {
    params: {
      groupName,
    },
  })

  return data.data
}

export const uploadCuratedFilters = async (formData: FormData, dataSource: string): Promise<CuratedFilterGroup[]> => {
  const instance = await backofficeAxiosInstance()

  const { data } = await instance.post<CuratedFiltersResponse>(
    `/curated-manager/filters?dataSource=${dataSource}`,
    formData,
    {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    }
  )

  return data.data
}

export const getCuratedMembersByCuratedId = async (curatedListId: number): Promise<CuratedMembersResponse> => {
  const instance = await backofficeAxiosInstance()

  const { data } = await instance.get<CuratedMembersResponse>(`/curated-manager/members/${curatedListId}`)

  return data
}