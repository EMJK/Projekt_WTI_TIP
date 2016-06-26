DROP TABLE IF EXISTS registrar_table CASCADE;
DROP TABLE IF EXISTS billing CASCADE;
DROP TABLE IF EXISTS users CASCADE;


CREATE TABLE users (
    id serial,
    username text not null,
    password_hash text not null,
    creation_date timestamp with time zone DEFAULT now() NOT NULL,
    delete_date timestamp with time zone,
    status character(1) DEFAULT 'A'::bpchar NOT NULL
);
ALTER TABLE users ADD PRIMARY KEY (id);
ALTER TABLE public.users ADD CONSTRAINT users_username_uq UNIQUE (username);


CREATE TABLE billing (
    id serial,
    calling_user_id text not null,
    called_user_id text not null,
    source_IP text not null,
    start_billing timestamp with time zone not null,
    stop_billing timestamp with time zone,
    call_id text not null
);
ALTER TABLE billing ADD PRIMARY KEY (id);


CREATE TABLE registrar_table (
    id serial,
    username text not null,
    domain text,
    contact text,
    received text,
    expires timestamp with time zone,
    user_agent text
);
ALTER TABLE registrar_table ADD PRIMARY KEY (id);
