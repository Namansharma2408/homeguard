import { useState } from 'react';
import { useNavigate } from 'react-router';
import { toast } from 'sonner';

import { propertiesApi, type CreatePropertyRequest } from '@/lib/api/properties';
import { isApiError } from '@/lib/problem';
import { useDocumentTitle } from '@/hooks/use-document-title';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { PageHeader } from '@/components/data-states';
import { meta } from './meta';

const TYPES = ['House', 'Condo', 'Apartment', 'Townhouse', 'Duplex', 'Studio'];

export default function NewPropertyPage() {
  useDocumentTitle(meta.title);
  const navigate = useNavigate();
  const [form, setForm] = useState<CreatePropertyRequest>({
    name: '',
    propertyType: 'House',
    address: '',
    city: '',
    state: '',
    country: 'USA',
    postalCode: '',
    yearBuilt: null,
    rooms: null,
    squareFeet: null,
  });
  const [saving, setSaving] = useState(false);

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);
    try {
      const created = await propertiesApi.create(form);
      toast.success('Property created');
      navigate(`/properties/${created.id}`);
    } catch (err) {
      toast.error(isApiError(err) ? (err.problem.detail ?? err.message) : 'Create failed');
    } finally {
      setSaving(false);
    }
  };

  const set = (k: keyof CreatePropertyRequest, v: any) => setForm((f) => ({ ...f, [k]: v }));

  return (
    <div className="mx-auto max-w-2xl grid gap-6">
      <PageHeader title="New property" description="Add a property to your portfolio." />
      <form onSubmit={submit} className="grid gap-4">
        <Field label="Name"><Input required value={form.name} onChange={(e) => set('name', e.target.value)} /></Field>
        <Field label="Type">
          <select className="border rounded-md h-9 px-2" value={form.propertyType} onChange={(e) => set('propertyType', e.target.value)}>
            {TYPES.map((t) => <option key={t}>{t}</option>)}
          </select>
        </Field>
        <Field label="Address"><Input required value={form.address} onChange={(e) => set('address', e.target.value)} /></Field>
        <div className="grid grid-cols-2 gap-4">
          <Field label="City"><Input required value={form.city} onChange={(e) => set('city', e.target.value)} /></Field>
          <Field label="State"><Input required value={form.state} onChange={(e) => set('state', e.target.value)} /></Field>
        </div>
        <div className="grid grid-cols-2 gap-4">
          <Field label="Country"><Input required value={form.country} onChange={(e) => set('country', e.target.value)} /></Field>
          <Field label="Postal code"><Input value={form.postalCode ?? ''} onChange={(e) => set('postalCode', e.target.value)} /></Field>
        </div>
        <div className="grid grid-cols-3 gap-4">
          <Field label="Year built"><Input type="number" value={form.yearBuilt ?? ''} onChange={(e) => set('yearBuilt', e.target.value ? Number(e.target.value) : null)} /></Field>
          <Field label="Rooms"><Input type="number" value={form.rooms ?? ''} onChange={(e) => set('rooms', e.target.value ? Number(e.target.value) : null)} /></Field>
          <Field label="Sq ft"><Input type="number" value={form.squareFeet ?? ''} onChange={(e) => set('squareFeet', e.target.value ? Number(e.target.value) : null)} /></Field>
        </div>
        <div className="flex gap-2">
          <Button type="submit" disabled={saving}>{saving ? 'Saving…' : 'Create'}</Button>
          <Button type="button" variant="outline" onClick={() => navigate('/properties')}>Cancel</Button>
        </div>
      </form>
    </div>
  );
}

function Field({ label, children }: { label: string; children: React.ReactNode }) {
  return (
    <div className="grid gap-1.5">
      <Label>{label}</Label>
      {children}
    </div>
  );
}

export function Pending() { return <p>Loading…</p>; }
export function Catch() { return <p>Something went wrong.</p>; }