begin;

-- A) app_role enum: keeps role values consistent
do $$
begin
  if not exists (select 1 from pg_type where typname = 'app_role') then
create type public.app_role as enum ('admin', 'author', 'subscriber');
end if;
end $$;

-- B) user_roles table: maps auth.users -> app role
create table if not exists public.user_roles (
                                                 user_id uuid primary key references auth.users(id) on delete cascade,
    role public.app_role not null default 'subscriber',
    created_at timestamptz not null default now()
    );

-- C) Protect it with RLS:
-- users can read only their own row, and only service_role can write roles.
alter table public.user_roles enable row level security;

drop policy if exists "user_roles_read_own" on public.user_roles;
create policy "user_roles_read_own"
on public.user_roles
for select
                    to authenticated
                    using (user_id = auth.uid());

drop policy if exists "user_roles_write_service_only" on public.user_roles;
create policy "user_roles_write_service_only"
on public.user_roles
for all
to service_role
using (true)
with check (true);

commit;
