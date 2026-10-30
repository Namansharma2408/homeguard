import { useMutation, useQueryClient } from '@tanstack/react-query';
import type { ColumnDef } from '@tanstack/react-table';
import { toast } from 'sonner';

import { riskApi, type RiskEvent } from '@/lib/api/risk';
import { isApiError } from '@/lib/problem';
import { usePermission } from '@/hooks/use-permission';
import { useDocumentTitle } from '@/hooks/use-document-title';
import { Button } from '@/components/ui/button';
import { PageHeader } from '@/components/data-states';
import { DataGrid, useDataGrid } from '@/components/data-grid';
import { meta } from './meta';
import { RISK_PERM } from '@/lib/api/risk';

export default function RiskPage() {
  useDocumentTitle(meta.title);
  const qc = useQueryClient();
  const canManage = usePermission(RISK_PERM.manage);

  const grid = useDataGrid<RiskEvent>({
    endpoint: '/risk/events',
    queryKey: ['risk-events'],
    defaultSort: { id: 'createdAt', desc: true },
  });

  const genAll = useMutation({
    mutationFn: () => riskApi.generateSample(),
    onSuccess: () => {
      toast.success('Sample readings generated');
      qc.invalidateQueries({ queryKey: ['risk-events'] });
      qc.invalidateQueries({ queryKey: ['properties'] });
    },
    onError: (e) => toast.error(isApiError(e) ? (e.problem.detail ?? e.message) : 'Failed'),
  });

  const columns: ColumnDef<RiskEvent>[] = [
    {
      accessorKey: 'severity',
      header: 'Severity',
      cell: ({ row }) => <SeverityBadge severity={row.original.severity} />,
    },
    { accessorKey: 'category', header: 'Category' },
    { accessorKey: 'description', header: 'Description' },
    { accessorKey: 'observedValue', header: 'Observed' },
    { accessorKey: 'status', header: 'Status' },
    {
      accessorKey: 'createdAt',
      header: 'Created',
      cell: ({ row }) => new Date(row.original.createdAt).toLocaleString(),
    },
  ];

  return (
    <div className="grid gap-6">
      <PageHeader
        title="Risk Monitoring"
        description="Simulated telemetry and deterministic risk assessment."
        actions={
          canManage ? (
            <Button onClick={() => genAll.mutate()} disabled={genAll.isPending}>
              {genAll.isPending ? 'Generating…' : 'Generate sample readings'}
            </Button>
          ) : null
        }
      />
      <DataGrid grid={grid} columns={columns} getRowId={(r) => String(r.id)} />
    </div>
  );
}

function SeverityBadge({ severity }: { severity: string }) {
  const tone = severity === 'Critical' ? 'bg-red-500/15 text-red-600' : severity === 'High' ? 'bg-orange-500/15 text-orange-600' : severity === 'Medium' ? 'bg-amber-500/15 text-amber-600' : 'bg-emerald-500/15 text-emerald-600';
  return <span className={`inline-flex items-center rounded-full px-2 py-0.5 text-xs font-medium ${tone}`}>{severity}</span>;
}

export function Pending() { return <p>Loading…</p>; }
export function Catch() { return <p>Something went wrong.</p>; }