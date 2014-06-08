CREATE USER rhjasper WITH PASSWORD 'rhjasper' SUPERUSER CREATEDB NOCREATEROLE;

CREATE DATABASE rhjasper
  WITH OWNER = rhjasper
       ENCODING = 'LATIN9'
       TABLESPACE = pg_default
       LC_COLLATE = 'C'
       LC_CTYPE = 'C'
       CONNECTION LIMIT = -1
       TEMPLATE = template0;
