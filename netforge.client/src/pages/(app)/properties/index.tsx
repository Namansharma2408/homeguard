import { useMutation, useQueryClient } from '@tanstack/react-query';
import type { ColumnDef } from '@tanstack/react-table';
import { Link, useNavigate } from 'react-router';
import { Plus, Trash2 } from 'lucide-react';
import { toast } from 'sonner';

import { propertiesApi, PROPERTY_PERM, type Property } from '@/lib/api/properties';
import { isApiError } from '@/lib/problem';
import { usePermission } from '@/hooks/use-permission';
import { useDocumentTitle } from '@/hooks/use-document-title';
import { Button } from '@/components/ui/button';
import { PageHeader } from '@/components/data-states';
import { DataGrid, useDataGrid } from '@/components/data-grid';
import { meta } from './meta';

export default function PropertiesPage() {
  useDocumentTitle(meta.title);
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const canCreate = usePermission(PROPERTY_PERM.create);
  const canDelete = usePermission(PROPERTY_PERM.delete);

  const grid = useDataGrid<Property>({
    endpoint: '/properties',
    queryKey: ['properties'],
    defaultSort: { id: 'createdAt', desc: true },
  });

  const remove = useMutation({
    mutationFn: (p: Property) => propertiesApi.delete(p.id),
    onSuccess: () => {
      toast.success('Property deleted');
      queryClient.invalidateQueries({ queryKey: ['properties'] });
    },
    onError: (e) => toast.error(isApiError(e) ? (e.problem.detail ?? e.message) : 'Delete failed'),
  });

  const columns: ColumnDef<Property>[] = [
    {
      accessorKey: 'name',
      header: 'Name',
      cell: ({ row }) => (
        <Link to={`/properties/${row.original.id}`} className="font-medium text-primary hover:underline">
          {row.original.name}
        </Link>
      ),
    },
    { accessorKey: 'propertyType', header: 'Type' },
    { accessorKey: 'city', header: 'City' },
    { accessorKey: 'state', header: 'State' },
    {
      accessorKey: 'healthScore',
      header: 'Health',
      cell: ({ row }) => <HealthBadge score={row.original.healthScore} />,
    },
    {
      id: 'actions',
      header: '',
      enableSorting: false,
      cell: ({ row }) =>
        canDelete ? (
          <Button variant="ghost" size="icon" onClick={() => remove.mutate(row.original)}>
            <Trash2 className="size-4" />
          </Button>
        ) : null,
    },
  ];

  return (
    <div className="grid gap-6">
      <PageHeader
        title="Properties"
        description="Manage your property portfolio and monitor health."
        actions={
          canCreate ? (
            <Button onClick={() => navigate('/properties/new')}>
              <Plus className="me-2 size-4" /> New property
            </Button>
          ) : null
        }
      />
      <DataGrid grid={grid} columns={columns} getRowId={(r) => String(r.id)} />
    </div>
  );
}

function HealthBadge({ score }: { score: number }) {
  const tone = score >= 80 ? 'bg-emerald-500/15 text-emerald-600' : score >= 60 ? 'bg-amber-500/15 text-amber-600' : 'bg-red-500/15 text-red-600';
  return <span className={`inline-flex items-center rounded-full px-2 py-0.5 text-xs font-medium ${tone}`}>{score}</span>;
}

export function Pending() {
  return <p>Loading…</p>;
}
export function Catch() {
  return <p>Something went wrong.</p>;
}