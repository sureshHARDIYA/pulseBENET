alter table public.categories enable row level security;

-- 1) READ: public
drop policy if exists "categories_read_public" on public.categories;
create policy "categories_read_public"
on public.categories
for select
to anon, authenticated
using (true);

-- 2) INSERT: admin OR author
drop policy if exists "categories_insert_admin_or_author" on public.categories;
create policy "categories_insert_admin_or_author"
on public.categories
for insert
to authenticated
with check (
  is_admin() OR jwt_role() = 'author'
);
-- 3) UPDATE: admin OR (author owns)
drop policy if exists "categories_update_admin_or_author_owner" on public.categories;
create policy "categories_update_admin_or_author_owner"
on public.categories
for update
to authenticated
using (
  is_admin() OR (jwt_role() = 'author' AND "CreatedBy" = auth.uid())
)
with check (
  is_admin() OR (jwt_role() = 'author' AND "CreatedBy" = auth.uid())
);

-- 4) DELETE: admin OR (author owns)
drop policy if exists "categories_delete_admin_or_author_owner" on public.categories;
create policy "categories_delete_admin_or_author_owner"
on public.categories
for delete
to authenticated
using (
  is_admin() OR (jwt_role() = 'author' AND "CreatedBy" = auth.uid())
);
