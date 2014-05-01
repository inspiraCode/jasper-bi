DROP TABLE IF EXISTS dim_clientes CASCADE;
CREATE TABLE dim_clientes
(
	id_cliente	INTEGER NOT NULL PRIMARY KEY,
	codigo_cliente	VARCHAR(11) NOT NULL,
	nombre_cliente VARCHAR(250) NOT NULL,
	es_local	BOOLEAN DEFAULT TRUE
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
	saldo_vencido MONEY NOT NULL,
	dias_vencimiento INTEGER NOT NULL
);

DROP TABLE IF EXISTS fact_por_vencer CASCADE;
CREATE TABLE fact_por_vencer(
	id_cliente INTEGER NOT NULL REFERENCES dim_clientes(id_cliente),
	id_grupo_vencimiento INTEGER NOT NULL REFERENCES dim_grupo_vencimiento(id_grupo_vencimiento),
	saldo_por_vencer MONEY NOT NULL,
	dias_por_vencer INTEGER NOT NULL
);

DROP TABLE IF EXISTS dim_meses CASCADE;
CREATE TABLE dim_meses(
	id_mes SERIAL PRIMARY KEY,
	codigo_mes VARCHAR(50),
	yyyy VARCHAR(4),
	nombre_mes VARCHAR(40)
);

DROP TABLE IF EXISTS fact_ventas_cobrado CASCADE;
CREATE TABLE fact_ventas_cobrado(
	id_mes INTEGER REFERENCES dim_meses(id_mes),
	vendido MONEY,
	cobrado MONEY
);