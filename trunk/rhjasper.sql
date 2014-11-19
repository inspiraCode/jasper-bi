DROP TABLE IF EXISTS fact_documents CASCADE;
CREATE TABLE fact_documents
(
	sale			BOOLEAN DEFAULT FALSE,
	credit			BOOLEAN DEFAULT FALSE,
	payment			BOOLEAN DEFAULT FALSE,
	amount			NUMERIC NOT NULL DEFAULT 0,
	seller			INTEGER NOT NULL DEFAULT 0,
	doco_date		DATE,
	due_date		DATE,
	company_cd		INTEGER,
	enterprise_id	INTEGER
);

DROP TABLE IF EXISTS dim_clientes CASCADE;
CREATE TABLE dim_clientes
(
	id_cliente	SERIAL PRIMARY KEY,
	codigo_cliente	VARCHAR(11) NOT NULL,
	nombre_cliente VARCHAR(250) NOT NULL,
	es_local	BOOLEAN DEFAULT TRUE,
	empresa varchar(150),
	id_empresa INTEGER NOT NULL
);

DROP TABLE IF EXISTS dim_grupo_vencimiento CASCADE;
CREATE TABLE dim_grupo_vencimiento(
	id_grupo_vencimiento SERIAL PRIMARY KEY,
	minimo_dias INTEGER,
	maximo_dias INTEGER,
	a_vencidos BOOLEAN DEFAULT TRUE,
	a_por_vencer BOOLEAN DEFAULT TRUE
);

DROP TABLE IF EXISTS fact_vencido CASCADE;
CREATE TABLE fact_vencido
(
	id_cliente INTEGER NOT NULL REFERENCES dim_clientes(id_cliente),
	id_grupo_vencimiento INTEGER NOT NULL REFERENCES dim_grupo_vencimiento(id_grupo_vencimiento),
	saldo_vencido NUMERIC NOT NULL DEFAULT 0
);

DROP TABLE IF EXISTS fact_vencido_usd CASCADE;
CREATE TABLE fact_vencido_usd
(
	id_cliente INTEGER NOT NULL REFERENCES dim_clientes(id_cliente),
	id_grupo_vencimiento INTEGER NOT NULL REFERENCES dim_grupo_vencimiento(id_grupo_vencimiento),
	saldo_vencido NUMERIC NOT NULL DEFAULT 0
);

DROP TABLE IF EXISTS fact_por_vencer CASCADE;
CREATE TABLE fact_por_vencer(
	id_cliente INTEGER NOT NULL REFERENCES dim_clientes(id_cliente),
	id_grupo_vencimiento INTEGER NOT NULL REFERENCES dim_grupo_vencimiento(id_grupo_vencimiento),
	saldo_por_vencer NUMERIC NOT NULL DEFAULT 0
);

DROP TABLE IF EXISTS fact_por_vencer_usd CASCADE;
CREATE TABLE fact_por_vencer_usd(
	id_cliente INTEGER NOT NULL REFERENCES dim_clientes(id_cliente),
	id_grupo_vencimiento INTEGER NOT NULL REFERENCES dim_grupo_vencimiento(id_grupo_vencimiento),
	saldo_por_vencer NUMERIC NOT NULL DEFAULT 0
);

DROP TABLE IF EXISTS dim_meses CASCADE;
CREATE TABLE dim_meses(
	id_mes SERIAL PRIMARY KEY,
	codigo_mes VARCHAR(50),
	yyyy VARCHAR(4),
	nombre_mes VARCHAR(40),
	indice_mes INTEGER
);

DROP TABLE IF EXISTS fact_collection CASCADE;
CREATE TABLE fact_collection (
	id_mes INTEGER REFERENCES dim_meses(id_mes) ON DELETE CASCADE,
	vendido NUMERIC DEFAULT 0,
	cobrado NUMERIC DEFAULT 0,
	incobrable NUMERIC DEFAULT 0
);

DROP TABLE IF EXISTS dim_store CASCADE;
CREATE TABLE dim_store(
	store_id SERIAL PRIMARY KEY,
	id_empresa INTEGER,
	store_name VARCHAR(150)
);

DROP TABLE IF EXISTS dim_monitor CASCADE;
CREATE TABLE dim_monitor(
	monitor_id SERIAL PRIMARY KEY,
	monitor_name VARCHAR(150)
);



DROP TABLE IF EXISTS dim_sellers CASCADE;
CREATE TABLE dim_sellers(
	seller_id SERIAL PRIMARY KEY,
	sms BOOLEAN DEFAULT false,
	ap_id INTEGER NOT NULL,
	agent_code VARCHAR(10),
	agent_name VARCHAR(150),
	phone VARCHAR(10),
	cellphone character varying (150),
	weekly_goal NUMERIC DEFAULT 0,
	empresa VARCHAR(150),
	id_empresa INTEGER,
	is_local BOOLEAN,
	store_id INTEGER
);

ALTER TABLE dim_sellers
   ADD COLUMN monitor_id integer;

DROP TABLE IF EXISTS dim_directors CASCADE;
CREATE TABLE dim_directors
(
  director_id serial NOT NULL,
  director_name character varying(150),
  email character varying(150),
  cellphone character varying (150),
  sms BOOLEAN DEFAULT false,
  id_empresa integer,
  CONSTRAINT dim_directors_pkey PRIMARY KEY (director_id)
  );

DROP TABLE IF EXISTS fact_sales CASCADE;
CREATE TABLE fact_sales(
	seller_id INTEGER NOT NULL REFERENCES dim_sellers(seller_id),
	sold_today NUMERIC DEFAULT 0,
	sold_week NUMERIC DEFAULT 0,
	sold_month NUMERIC DEFAULT 0
);