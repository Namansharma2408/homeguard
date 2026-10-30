import { api } from './client';
import type { PagedResult } from './paged';

export const RISK_PERM = {
  read: 'risk.read',
  manage: 'risk.manage',
  resolve: 'risk.resolve',
} as const;

export interface Sensor {
  id: number;
  propertyId: number;
  name: string;
  metricType: string;
  createdAt: string;
}

export interface TelemetryReading {
  id: number;
  sensorId: number;
  propertyId: number;
  metricType: string;
  value: number;
  recordedAt: string;
}

export interface RiskEvent {
  id: number;
  propertyId: number;
  category: string;
  severity: string;
  description: string;
  observedValue: string;
  recommendedAction: string;
  status: string;
  createdAt: string;
}

export const riskApi = {
  sensors: (propertyId?: number) =>
    api.get<Sensor[]>(`/risk/sensors${propertyId ? `?propertyId=${propertyId}` : ''}`),
  registerSensor: (body: { propertyId: number; name: string; metricType: string }) =>
    api.post<Sensor>('/risk/sensors', body),
  readings: (propertyId?: number) =>
    api.get<TelemetryReading[]>(`/risk/readings${propertyId ? `?propertyId=${propertyId}` : ''}`),
  ingest: (body: { sensorId: number; propertyId: number; metricType: string; value: number }) =>
    api.post<TelemetryReading>('/risk/telemetry', body),
  events: (query = '') => api.get<PagedResult<RiskEvent>>(`/risk/events/?${query}`),
  updateStatus: (id: number, status: string, resolutionNotes?: string) =>
    api.put<RiskEvent>(`/risk/events/${id}/status`, { status, resolutionNotes }),
  generateSample: (propertyId?: number) =>
    api.post<{ generated: boolean }>('/risk/sample/generate', propertyId ? { propertyId } : {}),
};