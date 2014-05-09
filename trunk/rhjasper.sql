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
	saldo_vencido NUMERIC NOT NULL DEFAULT 0
);

DROP TABLE IF EXISTS fact_por_vencer CASCADE;
CREATE TABLE fact_por_vencer(
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