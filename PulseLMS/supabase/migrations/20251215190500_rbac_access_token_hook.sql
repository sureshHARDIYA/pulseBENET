begin;

-- This function runs when a user signs in / refreshes token.
-- It injects our DB role into the JWT as claim "user_role".
create or replace function public.custom_access_token_hook(event jsonb)
returns jsonb
language plpgsql
stable
as $$

declare
  v_user_id_text text;
  v_user_id uuid;
  v_role public.app_role;
  claims jsonb;
begin
  -- Try both locations (Supabase payload differs depending on flow)
  v_user_id_text :=
    coalesce(
      event->>'user_id',
      event->'user'->>'id',
      event->'auth_event'->>'actor_id'
    );

  raise log 'hook payload user id text=%', v_user_id_text;

  v_user_id := v_user_id_text::uuid;

  select role into v_role
  from public.user_roles
  where user_id = v_user_id;

  raise log 'hook resolved user_id=% role=%', v_user_id, v_role;

  if v_role is null then
    v_role := 'subscriber';
  end if;

  claims := coalesce(event->'claims', '{}'::jsonb);
  claims := jsonb_set(claims, '{user_role}', to_jsonb(v_role::text), true);

  event := jsonb_set(event, '{claims}', claims, true);
  return event;
end;
$$;

commit;
