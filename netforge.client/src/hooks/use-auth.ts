import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';

import { authApi, type AuthUser } from '@/lib/api/auth';
import { isApiError } from '@/lib/problem';

export const authKeys = { me: ['auth', 'me'] as const };

async function fetchMe(): Promise<AuthUser | null> {
  try {
    return await authApi.me();
  } catch (error) {
    // 401 just means "not signed in" — a null user, not an error to surface.
    if (isApiError(error) && error.status === 401) return null;
    throw error;
  }
}

export function useCurrentUser() {
  return useQuery({ queryKey: authKeys.me, queryFn: fetchMe, staleTime: 60_000, retry: false });
}

/** Convenience view over the current-user query for guards and the app chrome. */
export function useAuth() {
  const { data, isLoading } = useCurrentUser();
  return { user: data ?? null, isAuthenticated: !!data, isLoading };
}

/** Seed the cached identity after a profile write so the UI updates without a round-trip. */
export function useSetCurrentUser() {
  const queryClient = useQueryClient();
  return (user: AuthUser) => queryClient.setQueryData(authKeys.me, user);
}

/**
 * Switch the client to a different session: drop everything the previous one cached, then seed the
 * new identity (`null` when signing out).
 *
 * The query cache lives outside the component tree, so it survives a sign-out — and the next person
 * to sign in on the same tab reads the previous user's server state until a hard reload. The
 * dashboard is where that shows first and worst: it seeds its board from the cached default layout
 * on the very first render, so an admin inherited the limited widget set of whoever signed in
 * before them. Every other cached list leaks the same way, which is also a privacy problem on a
 * shared machine.
 *
 * Queries only, not `queryClient.clear()`: the sign-in mutation is still running its own onSuccess
 * when this is called, and the mutation cache holds nothing cross-user worth that risk.
 *
 * The realtime hub goes with it. It is the same hazard in a socket: it authenticates once and then
 * keeps delivering to whoever opened it, so it has to end where the session does — including on the
 * path where the session died on its own and the next sign-in is the first thing we hear about it.
 */
export function useStartSession() {
  const queryClient = useQueryClient();
  return (user: AuthUser | null) => {
    queryClient.removeQueries();
    queryClient.setQueryData(authKeys.me, user);
  };
}

export function useLogout() {
  const startSession = useStartSession();
  return useMutation({
    mutationFn: authApi.logout,
    onSettled: () => startSession(null),
  });
}
