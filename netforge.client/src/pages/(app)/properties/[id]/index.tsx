import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useParams } from 'react-router';
import { toast } from 'sonner';

import { propertiesApi } from '@/lib/api/properties';
import { riskApi, type RiskEvent } from '@/lib/api/risk';
import { incidentsApi, type Incident } from '@/lib/api/incidents';
import { isApiError } from '@/lib/problem';
import { useDocumentTitle } from '@/hooks/use-document-title';
import { Button } from '@/components/ui/button';
import { PageHeader, ErrorState, EmptyState } from '@/components/data-states';
import { AlertTriangle, Siren } from 'lucide-react';
import { meta } from './meta';

export default function PropertyDetailPage() {
  const { id } = useParams();
  const pid = Number(id);
  const qc = useQueryClient();
  useDocumentTitle(meta.title);

  const property = useQuery({ queryKey: ['property', pid], queryFn: () => propertiesApi.get(pid) });
  const events = useQuery({ queryKey: ['risk-events', pid], queryFn: () => riskApi.events(`propertyId=eq:${pid}&pageSize=20`) });
  const incidents = useQuery({ queryKey: ['incidents', pid], queryFn: () => incidentsApi.list(`propertyId=eq:${pid}&pageSize=20`) });

  const genSample = useMutation({
    mutationFn: () => riskApi.generateSample(pid),
    onSuccess: () => {
      toast.success('Sample readings generated');
      qc.invalidateQueries({ queryKey: ['property', pid] });
      qc.invalidateQueries({ queryKey: ['risk-events', pid] });
    },
    onError: (e) => toast.error(isApiError(e) ? (e.problem.detail ?? e.message) : 'Failed'),
  });

  if (property.isLoading) return <p>Loading…</p>;
  if (property.isError || !property.data) return <ErrorState onRetry={() => property.refetch()} />;
  const p = property.data;

  return (
    <div className="grid gap-6">
      <PageHeader
        title={p.name}
        description={`${p.propertyType} · ${p.address}, ${p.city}, ${p.state}`}
        actions={<Button onClick={() => genSample.mutate()} disabled={genSample.isPending}>Generate sample readings</Button>}
      />

      <div className="grid gap-4 sm:grid-cols-4">
        <StatCard label="Health score" value={String(p.healthScore)} />
        <StatCard label="Rooms" value={p.rooms != null ? String(p.rooms) : '—'} />
        <StatCard label="Sq ft" value={p.squareFeet != null ? String(p.squareFeet) : '—'} />
        <StatCard label="Year built" value={p.yearBuilt != null ? String(p.yearBuilt) : '—'} />
      </div>

      <section>
        <h2 className="text-lg font-semibold mb-3">Risk events</h2>
        {events.data?.items.length ? (
          <ul className="grid gap-2">
            {events.data.items.map((e: RiskEvent) => (
              <li key={e.id} className="rounded-lg border p-3">
                <div className="flex items-center gap-2">
                  <SeverityBadge severity={e.severity} />
                  <span className="font-medium">{e.category}</span>
                  <span className="text-xs text-muted-foreground ms-auto">{new Date(e.createdAt).toLocaleString()}</span>
                </div>
                <p className="text-sm mt-1">{e.description}</p>
                <p className="text-xs text-muted-foreground mt-1">Observed: {e.observedValue} · {e.recommendedAction}</p>
              </li>
            ))}
          </ul>
        ) : (
          <EmptyState icon={AlertTriangle} title="No risk events" description="Generate sample readings to evaluate risk." />
        )}
      </section>

      <section>
        <h2 className="text-lg font-semibold mb-3">Incidents</h2>
        {incidents.data?.items.length ? (
          <ul className="grid gap-2">
            {incidents.data.items.map((inc: Incident) => (
              <li key={inc.id} className="rounded-lg border p-3 flex items-center gap-3">
                <StatusBadge status={inc.status} />
                <div>
                  <div className="font-medium">{inc.title}</div>
                  <div className="text-xs text-muted-foreground">{inc.category} · {inc.priority}</div>
                </div>
              </li>
            ))}
          </ul>
        ) : (
          <EmptyState icon={Siren} title="No incidents" description="No incidents reported for this property." />
        )}
      </section>
    </div>
  );
}

function StatCard({ label, value }: { label: string; value: string }) {
  return (
    <div className="rounded-xl border bg-card p-4">
      <div className="text-xs text-muted-foreground">{label}</div>
      <div className="text-2xl font-semibold mt-1">{value}</div>
    </div>
  );
}

function SeverityBadge({ severity }: { severity: string }) {
  const tone = severity === 'Critical' ? 'bg-red-500/15 text-red-600' : severity === 'High' ? 'bg-orange-500/15 text-orange-600' : severity === 'Medium' ? 'bg-amber-500/15 text-amber-600' : 'bg-emerald-500/15 text-emerald-600';
  return <span className={`inline-flex items-center rounded-full px-2 py-0.5 text-xs font-medium ${tone}`}>{severity}</span>;
}

function StatusBadge({ status }: { status: string }) {
  return <span className="inline-flex items-center rounded-full bg-muted px-2 py-0.5 text-xs font-medium">{status}</span>;
}

export function Pending() { return <p>Loading…</p>; }
export function Catch() { return <p>Something went wrong.</p>; }