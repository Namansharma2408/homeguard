import { api } from './client';
import type { PagedResult } from './paged';

export const PROPERTY_PERM = {
  read: 'properties.read',
  create: 'properties.create',
  update: 'properties.update',
  delete: 'properties.delete',
} as const;

export interface Property {
  id: number;
  name: string;
  propertyType: string;
  address: string;
  city: string;
  state: string;
  country: string;
  postalCode: string | null;
  yearBuilt: number | null;
  rooms: number | null;
  squareFeet: number | null;
  healthScore: number;
  createdAt: string;
}

export interface CreatePropertyRequest {
  name: string;
  propertyType: string;
  address: string;
  city: string;
  state: string;
  country: string;
  postalCode?: string | null;
  yearBuilt?: number | null;
  rooms?: number | null;
  squareFeet?: number | null;
}

export type UpdatePropertyRequest = CreatePropertyRequest;

export const propertiesApi = {
  list: (query = '') => api.get<PagedResult<Property>>(`/properties/?${query}`),
  get: (id: number) => api.get<Property>(`/properties/${id}`),
  create: (body: CreatePropertyRequest) => api.post<Property>('/properties/', body),
  update: (id: number, body: UpdatePropertyRequest) => api.put<Property>(`/properties/${id}`, body),
  delete: (id: number) => api.del<void>(`/properties/${id}`),
};