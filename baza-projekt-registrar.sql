DROP TABLE registrar_table;
DROP TABLE billing;
DROP TABLE users;


CREATE TABLE users (
    id serial,
    username text not null,
    password_hash text not null,
    creation_date timestamp with time zone DEFAULT now() NOT NULL,
    delete_date timestamp with time zone,
    status character(1) DEFAULT 'A'::bpchar NOT NULL
);
ALTER TABLE users ADD PRIMARY KEY (id);
ALTER TABLE users COMMENT ON COLUMN


CREATE TABLE billing (
    id serial,
    calling_user_id int not null,
    called_user_id int not null,
    source_IP text not null,
    destination_IP text not null,
    start_billing timestamp with time zone not null,
    stop_billing timestamp with time zone
);
ALTER TABLE billing ADD PRIMARY KEY (id);
ALTER TABLE billing ADD CONSTRAINT calling_user_id_fk FOREIGN KEY (id) REFERENCES users ON DELETE CASCADE;
ALTER TABLE billing ADD CONSTRAINT called_user_id_fk FOREIGN KEY (id) REFERENCES users ON DELETE CASCADE;


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
