import { useState } from 'react';
import { Trash2 } from 'lucide-react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import type { ColumnDef } from '@tanstack/react-table';
import { toast } from 'sonner';

import { incidentsApi, type Incident, type CreateIncidentRequest } from '@/lib/api/incidents';
import { propertiesApi } from '@/lib/api/properties';
import { isApiError } from '@/lib/problem';
import { usePermission } from '@/hooks/use-permission';
import { useDocumentTitle } from '@/hooks/use-document-title';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { PageHeader } from '@/components/data-states';
import { DataGrid, useDataGrid } from '@/components/data-grid';
import { meta } from './meta';
import { INCIDENT_PERM } from '@/lib/api/incidents';

const STATUSES = ['Reported', 'Investigating', 'InProgress', 'Resolved', 'Closed'];

export default function IncidentsPage() {
  useDocumentTitle(meta.title);
  const qc = useQueryClient();
  const canCreate = usePermission(INCIDENT_PERM.create);
  const canUpdate = usePermission(INCIDENT_PERM.update);
  const canDelete = usePermission(INCIDENT_PERM.delete);
  const properties = useQuery({ queryKey: ['properties-all'], queryFn: () => propertiesApi.list('pageSize=100') });

  const grid = useDataGrid<Incident>({
    endpoint: '/incidents',
    queryKey: ['incidents'],
    defaultSort: { id: 'reportedDate', desc: true },
  });

  const [showForm, setShowForm] = useState(false);
  const [form, setForm] = useState<CreateIncidentRequest>({ propertyId: 0, title: '', category: 'General', priority: 'Medium' });

  const create = useMutation({
    mutationFn: () => incidentsApi.create(form),
    onSuccess: () => {
      toast.success('Incident created');
      qc.invalidateQueries({ queryKey: ['incidents'] });
      setShowForm(false);
      setForm({ propertyId: 0, title: '', category: 'General', priority: 'Medium' });
    },
    onError: (e) => toast.error(isApiError(e) ? (e.problem.detail ?? e.message) : 'Create failed'),
  });

  const updateStatus = useMutation({
    mutationFn: ({ id, status }: { id: number; status: string }) => incidentsApi.updateStatus(id, status),
    onSuccess: () => { toast.success('Status updated'); qc.invalidateQueries({ queryKey: ['incidents'] }); },
    onError: (e) => toast.error(isApiError(e) ? (e.problem.detail ?? e.message) : 'Update failed'),
  });

  const remove = useMutation({
    mutationFn: (id: number) => incidentsApi.delete(id),
    onSuccess: () => { toast.success('Incident deleted'); qc.invalidateQueries({ queryKey: ['incidents'] }); },
    onError: (e) => toast.error(isApiError(e) ? (e.problem.detail ?? e.message) : 'Delete failed'),
  });

  const columns: ColumnDef<Incident>[] = [
    { accessorKey: 'title', header: 'Title' },
    { accessorKey: 'category', header: 'Category' },
    { accessorKey: 'priority', header: 'Priority' },
    {
      accessorKey: 'status',
      header: 'Status',
      cell: ({ row }) =>
        canUpdate ? (
          <select
            className="border rounded-md h-8 px-2 text-sm"
            value={row.original.status}
            onChange={(e) => updateStatus.mutate({ id: row.original.id, status: e.target.value })}
          >
            {STATUSES.map((s) => <option key={s}>{s}</option>)}
          </select>
        ) : (
          <span>{row.original.status}</span>
        ),
    },
    {
      accessorKey: 'reportedDate',
      header: 'Reported',
      cell: ({ row }) => new Date(row.original.reportedDate).toLocaleDateString(),
    },
    {
      id: 'actions',
      header: '',
      enableSorting: false,
      cell: ({ row }) =>
        canDelete ? (
          <Button variant="ghost" size="icon" onClick={() => remove.mutate(row.original.id)}>
            <Trash2 className="size-4" />
          </Button>
        ) : null,
    },
  ];

  return (
    <div className="grid gap-6">
      <PageHeader
        title="Incidents"
        description="Track and resolve property incidents."
        actions={
          canCreate ? (
            <Button onClick={() => setShowForm((v) => !v)}>{showForm ? 'Cancel' : 'New incident'}</Button>
          ) : null
        }
      />

      {showForm && (
        <form
          onSubmit={(e) => { e.preventDefault(); create.mutate(); }}
          className="grid gap-4 rounded-xl border bg-card p-4"
        >
          <div className="grid gap-1.5">
            <Label>Property</Label>
            <select
              className="border rounded-md h-9 px-2"
              value={form.propertyId}
              onChange={(e) => setForm((f) => ({ ...f, propertyId: Number(e.target.value) }))}
              required
            >
              <option value={0} disabled>Select property</option>
              {properties.data?.items.map((p) => <option key={p.id} value={p.id}>{p.name}</option>)}
            </select>
          </div>
          <div className="grid gap-1.5">
            <Label>Title</Label>
            <Input required value={form.title} onChange={(e) => setForm((f) => ({ ...f, title: e.target.value }))} />
          </div>
          <div className="grid grid-cols-2 gap-4">
            <div className="grid gap-1.5">
              <Label>Category</Label>
              <Input value={form.category} onChange={(e) => setForm((f) => ({ ...f, category: e.target.value }))} />
            </div>
            <div className="grid gap-1.5">
              <Label>Priority</Label>
              <select className="border rounded-md h-9 px-2" value={form.priority} onChange={(e) => setForm((f) => ({ ...f, priority: e.target.value }))}>
                {['Low', 'Medium', 'High', 'Critical'].map((p) => <option key={p}>{p}</option>)}
              </select>
            </div>
          </div>
          <div className="grid gap-1.5">
            <Label>Description</Label>
            <Input value={form.description ?? ''} onChange={(e) => setForm((f) => ({ ...f, description: e.target.value }))} />
          </div>
          <Button type="submit" disabled={create.isPending}>{create.isPending ? 'Saving…' : 'Create incident'}</Button>
        </form>
      )}

      <DataGrid grid={grid} columns={columns} getRowId={(r) => String(r.id)} />
    </div>
  );
}



export function Pending() { return <p>Loading…</p>; }
export function Catch() { return <p>Something went wrong.</p>; }