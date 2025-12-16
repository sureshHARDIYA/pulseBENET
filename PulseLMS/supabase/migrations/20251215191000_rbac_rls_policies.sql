begin;

-- Extract role from JWT.
-- Supabase makes JWT claims available through request.jwt.claims setting.
create or replace function public.jwt_role()
returns text
language sql
stable
as $$
select coalesce(
               nullif(current_setting('request.jwt.claims', true), '')::jsonb ->> 'user_role',
    'subscriber'
  );
$$;

create or replace function public.is_admin()
returns boolean language sql stable
as $$ select public.jwt_role() = 'admin'; $$;

create or replace function public.is_author()
returns boolean language sql stable
as $$ select public.jwt_role() = 'author'; $$;

-- Ownership check: is the authenticated user the row owner?
create or replace function public.is_owner(owner_id uuid)
returns boolean
language sql
stable
as $$ select auth.uid() = owner_id; $$;

commit;
