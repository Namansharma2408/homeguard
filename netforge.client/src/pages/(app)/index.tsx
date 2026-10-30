import { useQuery } from '@tanstack/react-query';
import { Building2, AlertTriangle, Siren, Activity, HeartPulse } from 'lucide-react';

import { propertiesApi } from '@/lib/api/properties';
import { riskApi } from '@/lib/api/risk';
import { incidentsApi } from '@/lib/api/incidents';
import { useAuth } from '@/hooks/use-auth';
import { useDocumentTitle } from '@/hooks/use-document-title';

export default function DashboardPage() {
  useDocumentTitle('HomeGuard AI');
  const { user } = useAuth();
  const firstName = user?.displayName?.split(' ')[0];

  const properties = useQuery({ queryKey: ['properties-all'], queryFn: () => propertiesApi.list('pageSize=100') });
  const events = useQuery({ queryKey: ['risk-events-all'], queryFn: () => riskApi.events('pageSize=100') });
  const incidents = useQuery({ queryKey: ['incidents-all'], queryFn: () => incidentsApi.list('pageSize=100') });

  const props = properties.data?.items ?? [];
  const evs = events.data?.items ?? [];
  const incs = incidents.data?.items ?? [];

  const total = props.length;
  const healthy = props.filter((p) => p.healthScore >= 80).length;
  const elevated = props.filter((p) => p.healthScore >= 60 && p.healthScore < 80).length;
  const highRisk = props.filter((p) => p.healthScore < 60).length;
  const openIncidents = incs.filter((i) => i.status !== 'Resolved' && i.status !== 'Closed').length;
  const avgHealth = total ? Math.round(props.reduce((a, p) => a + p.healthScore, 0) / total) : 0;

  // category counts
  const byCategory: Record<string, number> = {};
  evs.forEach((e) => { byCategory[e.category] = (byCategory[e.category] ?? 0) + 1; });

  const recentEvents = [...evs].sort((a, b) => +new Date(b.createdAt) - +new Date(a.createdAt)).slice(0, 6);
  const recentIncidents = [...incs].sort((a, b) => +new Date(b.reportedDate) - +new Date(a.reportedDate)).slice(0, 5);

  return (
    <div className="grid gap-6">
      <div>
        <h1 className="text-2xl font-semibold tracking-tight">
          {firstName ? `Welcome back, ${firstName}` : 'HomeGuard AI Dashboard'}
        </h1>
        <p className="text-muted-foreground mt-1">Property intelligence and risk overview.</p>
      </div>

      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-6">
        <Kpi label="Total properties" value={total} icon={Building2} />
        <Kpi label="Healthy" value={healthy} icon={HeartPulse} tone="text-emerald-600" />
        <Kpi label="Elevated risk" value={elevated} icon={AlertTriangle} tone="text-amber-600" />
        <Kpi label="High risk" value={highRisk} icon={AlertTriangle} tone="text-red-600" />
        <Kpi label="Open incidents" value={openIncidents} icon={Siren} tone="text-orange-600" />
        <Kpi label="Avg health score" value={avgHealth} icon={Activity} />
      </div>

      <div className="grid gap-6 lg:grid-cols-2">
        <div className="rounded-xl border bg-card p-5">
          <h2 className="font-semibold mb-3">Risk events by category</h2>
          {Object.keys(byCategory).length === 0 ? (
            <p className="text-sm text-muted-foreground">No risk events yet.</p>
          ) : (
            <ul className="grid gap-2">
              {Object.entries(byCategory).map(([cat, count]) => (
                <li key={cat} className="flex items-center justify-between text-sm">
                  <span>{cat}</span>
                  <span className="font-medium">{count}</span>
                </li>
              ))}
            </ul>
          )}
        </div>

        <div className="rounded-xl border bg-card p-5">
          <h2 className="font-semibold mb-3">Property health distribution</h2>
          <ul className="grid gap-2 text-sm">
            <li className="flex justify-between"><span>Healthy (≥80)</span><span className="font-medium">{healthy}</span></li>
            <li className="flex justify-between"><span>Elevated (60–79)</span><span className="font-medium">{elevated}</span></li>
            <li className="flex justify-between"><span>High risk (&lt;60)</span><span className="font-medium">{highRisk}</span></li>
          </ul>
        </div>
      </div>

      <div className="grid gap-6 lg:grid-cols-2">
        <div className="rounded-xl border bg-card p-5">
          <h2 className="font-semibold mb-3">Recent risk events</h2>
          {recentEvents.length === 0 ? (
            <p className="text-sm text-muted-foreground">No risk events yet.</p>
          ) : (
            <ul className="grid gap-2">
              {recentEvents.map((e) => (
                <li key={e.id} className="text-sm flex items-center gap-2">
                  <SeverityBadge severity={e.severity} />
                  <span className="truncate">{e.category} · {e.description}</span>
                </li>
              ))}
            </ul>
          )}
        </div>

        <div className="rounded-xl border bg-card p-5">
          <h2 className="font-semibold mb-3">Recent incidents</h2>
          {recentIncidents.length === 0 ? (
            <p className="text-sm text-muted-foreground">No incidents yet.</p>
          ) : (
            <ul className="grid gap-2">
              {recentIncidents.map((i) => (
                <li key={i.id} className="text-sm flex items-center gap-2">
                  <span className="rounded-full bg-muted px-2 py-0.5 text-xs">{i.status}</span>
                  <span className="truncate">{i.title}</span>
                </li>
              ))}
            </ul>
          )}
        </div>
      </div>

      <div className="rounded-xl border bg-card p-5">
        <h2 className="font-semibold mb-3">Property portfolio overview</h2>
        {props.length === 0 ? (
          <p className="text-sm text-muted-foreground">No properties yet.</p>
        ) : (
          <ul className="grid gap-2 sm:grid-cols-2">
            {props.map((p) => (
              <li key={p.id} className="flex items-center justify-between text-sm rounded-lg border px-3 py-2">
                <span className="truncate">{p.name} <span className="text-muted-foreground">· {p.city}</span></span>
                <span className="font-medium">{p.healthScore}</span>
              </li>
            ))}
          </ul>
        )}
      </div>
    </div>
  );
}

function Kpi({ label, value, icon: Icon, tone }: { label: string; value: number; icon: React.ComponentType<{ className?: string }>; tone?: string }) {
  return (
    <div className="rounded-xl border bg-card p-4">
      <div className="flex items-center justify-between">
        <div className="text-xs text-muted-foreground">{label}</div>
        <Icon className={`size-4 ${tone ?? 'text-muted-foreground'}`} />
      </div>
      <div className="text-2xl font-semibold mt-1">{value}</div>
    </div>
  );
}

function SeverityBadge({ severity }: { severity: string }) {
  const tone = severity === 'Critical' ? 'bg-red-500/15 text-red-600' : severity === 'High' ? 'bg-orange-500/15 text-orange-600' : severity === 'Medium' ? 'bg-amber-500/15 text-amber-600' : 'bg-emerald-500/15 text-emerald-600';
  return <span className={`inline-flex items-center rounded-full px-2 py-0.5 text-xs font-medium ${tone}`}>{severity}</span>;
}

export function Pending() { return <p>Loading…</p>; }
export function Catch() { return <p>Something went wrong.</p>; }