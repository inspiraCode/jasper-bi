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
	maximo_dias INTEGER
);

DROP TABLE IF EXISTS fact_vencido CASCADE;
CREATE TABLE fact_vencido
(
	id_cliente INTEGER NOT NULL REFERENCES dim_clientes(id_cliente),
	id_grupo_vencimiento INTEGER NOT NULL REFERENCES dim_grupo_vencimiento(id_grupo_vencimiento),
	saldo_vencido NUMERIC NOT NULL DEFAULT 0,
	dias_vencimiento INTEGER
);

DROP TABLE IF EXISTS fact_por_vencer CASCADE;
CREATE TABLE fact_por_vencer(
	id_cliente INTEGER NOT NULL REFERENCES dim_clientes(id_cliente),
	id_grupo_vencimiento INTEGER NOT NULL REFERENCES dim_grupo_vencimiento(id_grupo_vencimiento),
	saldo_por_vencer NUMERIC NOT NULL DEFAULT 0,
	dias_por_vencer INTEGER
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

DROP TABLE IF EXISTS dim_sellers CASCADE;
CREATE TABLE dim_sellers(
	seller_id SERIAL PRIMARY KEY,
	ap_id INTEGER NOT NULL,
	agent_code VARCHAR(10),
	agent_name VARCHAR(150),
	email VARCHAR(150),
	weekly_goal NUMERIC DEFAULT 0,
	empresa VARCHAR(150),
	id_empresa INTEGER
);

DROP TABLE IF EXISTS fact_sales CASCADE;
CREATE TABLE fact_sales(
	seller_id INTEGER NOT NULL REFERENCES dim_sellers(seller_id),
	sold_today NUMERIC DEFAULT 0,
	sold_week NUMERIC DEFAULT 0,
	sold_month NUMERIC DEFAULT 0
);