import { api } from './client';
import type { PagedResult } from './paged';

export const INCIDENT_PERM = {
  read: 'incidents.read',
  create: 'incidents.create',
  update: 'incidents.update',
  delete: 'incidents.delete',
} as const;

export interface Incident {
  id: number;
  propertyId: number;
  title: string;
  description: string | null;
  category: string;
  priority: string;
  status: string;
  reportedDate: string;
  resolutionNotes: string | null;
  createdAt: string;
}

export interface CreateIncidentRequest {
  propertyId: number;
  title: string;
  description?: string | null;
  category: string;
  priority: string;
  status?: string;
}

export const incidentsApi = {
  list: (query = '') => api.get<PagedResult<Incident>>(`/incidents/?${query}`),
  get: (id: number) => api.get<Incident>(`/incidents/${id}`),
  create: (body: CreateIncidentRequest) => api.post<Incident>('/incidents/', body),
  updateStatus: (id: number, status: string, resolutionNotes?: string) =>
    api.put<Incident>(`/incidents/${id}/status`, { status, resolutionNotes }),
  delete: (id: number) => api.del<void>(`/incidents/${id}`),
};