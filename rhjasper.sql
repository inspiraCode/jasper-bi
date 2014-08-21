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
	is_local BOOLEAN
);

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

CREATE OR REPLACE VIEW vw_saldos_vencidos_dias_local AS	 

SElECT dim_clientes.codigo_cliente, SUM(fact_vencido.saldo_vencido) AS total_vencido, 
CASE WHEN dim_grupo_vencimiento.maximo_dias = 0 THEN dim_grupo_vencimiento.minimo_dias || ' Días o más'
ELSE dim_grupo_vencimiento.minimo_dias || '-' || dim_grupo_vencimiento.maximo_dias || ' Días' END AS Grupo
FROM fact_vencido  
INNER JOIN dim_grupo_vencimiento 
ON fact_vencido.id_grupo_vencimiento = dim_grupo_vencimiento.id_grupo_vencimiento
INNER JOIN dim_clientes
ON fact_vencido.id_cliente = dim_clientes.id_cliente
WHERE dim_clientes.es_local = true
GROUP BY dim_clientes.codigo_cliente,Grupo, dim_grupo_vencimiento.id_grupo_vencimiento
ORDER BY dim_grupo_vencimiento.id_grupo_vencimiento;

GRANT ALL ON vw_saldos_vencidos_dias_local to rhjasper;


CREATE OR REPLACE VIEW vw_saldos_por_vencer_dias_local AS 

SElECT dim_clientes.codigo_cliente,
CASE WHEN Total.total_por_vencer = 0 THEN 0
ELSE SUM(saldo_por_vencer) END AS Por_vencer,
CASE WHEN dim_grupo_vencimiento.maximo_dias = 0 THEN dim_grupo_vencimiento.minimo_dias || ' Días o más' 
ELSE dim_grupo_vencimiento.minimo_dias || '-' || dim_grupo_vencimiento.maximo_dias || ' Días' END AS Grupo
FROM fact_por_vencer
INNER JOIN dim_grupo_vencimiento 
ON fact_por_vencer.id_grupo_vencimiento = dim_grupo_vencimiento.id_grupo_vencimiento
INNER JOIN dim_clientes
ON fact_por_vencer.id_cliente = dim_clientes.id_cliente
INNER JOIN (SElECT SUM(saldo_por_vencer) AS total_por_vencer, es_local
FROM fact_por_vencer
INNER JOIN dim_clientes
ON fact_por_vencer.id_cliente = dim_clientes.id_cliente
GROUP BY es_local) AS Total
ON dim_clientes.es_local = Total.es_local
WHERE dim_clientes.es_local = true
GROUP BY dim_clientes.codigo_cliente, Grupo, dim_grupo_vencimiento.id_grupo_vencimiento, Total.total_por_vencer
ORDER BY dim_grupo_vencimiento.id_grupo_vencimiento;

GRANT ALL ON vw_saldos_por_vencer_dias_local to rhjasper;


CREATE OR REPLACE VIEW vw_saldos_locales AS 
 SELECT dim_clientes.codigo_cliente,
    dim_clientes.nombre_cliente,
    ( SELECT vw_saldos_vencidos_dias_local.total_vencido
           FROM vw_saldos_vencidos_dias_local
          WHERE vw_saldos_vencidos_dias_local.grupo = '0-15 Días'::text AND vw_saldos_vencidos_dias_local.codigo_cliente::text = dim_clientes.codigo_cliente::text) AS "0-15 Días",
    ( SELECT vw_saldos_vencidos_dias_local.total_vencido
           FROM vw_saldos_vencidos_dias_local
          WHERE vw_saldos_vencidos_dias_local.grupo = '16-30 Días'::text AND vw_saldos_vencidos_dias_local.codigo_cliente::text = dim_clientes.codigo_cliente::text) AS "16-30 Días",
    ( SELECT vw_saldos_vencidos_dias_local.total_vencido
           FROM vw_saldos_vencidos_dias_local
          WHERE vw_saldos_vencidos_dias_local.grupo = '31-45 Días'::text AND vw_saldos_vencidos_dias_local.codigo_cliente::text = dim_clientes.codigo_cliente::text) AS "31-45 Días",
    ( SELECT vw_saldos_vencidos_dias_local.total_vencido
           FROM vw_saldos_vencidos_dias_local
          WHERE vw_saldos_vencidos_dias_local.grupo = '46 Días o más'::text AND vw_saldos_vencidos_dias_local.codigo_cliente::text = dim_clientes.codigo_cliente::text) AS "46 Días o más",
    ( SELECT vw_saldos_por_vencer_dias_local.por_vencer
           FROM vw_saldos_por_vencer_dias_local
          WHERE vw_saldos_por_vencer_dias_local.grupo = '0-15 Días'::text AND vw_saldos_por_vencer_dias_local.codigo_cliente::text = dim_clientes.codigo_cliente::text) AS "0-15 Días.",
    ( SELECT vw_saldos_por_vencer_dias_local.por_vencer
           FROM vw_saldos_por_vencer_dias_local
          WHERE vw_saldos_por_vencer_dias_local.grupo = '16-30 Días'::text AND vw_saldos_por_vencer_dias_local.codigo_cliente::text = dim_clientes.codigo_cliente::text) AS "16-30 Días.",
    ( SELECT vw_saldos_por_vencer_dias_local.por_vencer
           FROM vw_saldos_por_vencer_dias_local
          WHERE vw_saldos_por_vencer_dias_local.grupo = '31-45 Días'::text AND vw_saldos_por_vencer_dias_local.codigo_cliente::text = dim_clientes.codigo_cliente::text) AS "31-45 Días.",
    ( SELECT vw_saldos_por_vencer_dias_local.por_vencer
           FROM vw_saldos_por_vencer_dias_local
          WHERE vw_saldos_por_vencer_dias_local.grupo = '46 Días o más'::text AND vw_saldos_por_vencer_dias_local.codigo_cliente::text = dim_clientes.codigo_cliente::text) AS "46 Días o más."
   FROM dim_clientes
  ORDER BY dim_clientes.nombre_cliente;

GRANT ALL ON TABLE vw_saldos_locales TO rhjasper;

CREATE OR REPLACE VIEW vw_saldos_vencidos_dias_foraneo AS 
 SELECT dim_clientes.codigo_cliente,
    sum(fact_vencido.saldo_vencido) AS total_vencido,
        CASE
            WHEN dim_grupo_vencimiento.maximo_dias = 0 THEN dim_grupo_vencimiento.minimo_dias || ' Días o más'::text
            ELSE ((dim_grupo_vencimiento.minimo_dias || '-'::text) || dim_grupo_vencimiento.maximo_dias) || ' Días'::text
        END AS grupo
   FROM fact_vencido
   JOIN dim_grupo_vencimiento ON fact_vencido.id_grupo_vencimiento = dim_grupo_vencimiento.id_grupo_vencimiento
   JOIN dim_clientes ON fact_vencido.id_cliente = dim_clientes.id_cliente
  WHERE dim_clientes.es_local = false
  GROUP BY dim_clientes.codigo_cliente,
CASE
    WHEN dim_grupo_vencimiento.maximo_dias = 0 THEN dim_grupo_vencimiento.minimo_dias || ' Días o más'::text
    ELSE ((dim_grupo_vencimiento.minimo_dias || '-'::text) || dim_grupo_vencimiento.maximo_dias) || ' Días'::text
END, dim_grupo_vencimiento.id_grupo_vencimiento
  ORDER BY dim_grupo_vencimiento.id_grupo_vencimiento;

GRANT ALL ON TABLE vw_saldos_vencidos_dias_foraneo TO rhjasper;

CREATE OR REPLACE VIEW vw_saldos_por_vencer_dias_foraneo AS 
 SELECT dim_clientes.codigo_cliente,
        CASE
            WHEN total.total_por_vencer = 0::numeric THEN 0::numeric
            ELSE sum(fact_por_vencer.saldo_por_vencer)
        END AS por_vencer,
        CASE
            WHEN dim_grupo_vencimiento.maximo_dias = 0 THEN dim_grupo_vencimiento.minimo_dias || ' Días o más'::text
            ELSE ((dim_grupo_vencimiento.minimo_dias || '-'::text) || dim_grupo_vencimiento.maximo_dias) || ' Días'::text
        END AS grupo
   FROM fact_por_vencer
   JOIN dim_grupo_vencimiento ON fact_por_vencer.id_grupo_vencimiento = dim_grupo_vencimiento.id_grupo_vencimiento
   JOIN dim_clientes ON fact_por_vencer.id_cliente = dim_clientes.id_cliente
   JOIN ( SELECT sum(fact_por_vencer_1.saldo_por_vencer) AS total_por_vencer,
    dim_clientes_1.es_local
   FROM fact_por_vencer fact_por_vencer_1
   JOIN dim_clientes dim_clientes_1 ON fact_por_vencer_1.id_cliente = dim_clientes_1.id_cliente
  GROUP BY dim_clientes_1.es_local) total ON dim_clientes.es_local = total.es_local
  WHERE dim_clientes.es_local = false
  GROUP BY dim_clientes.codigo_cliente,
CASE
    WHEN dim_grupo_vencimiento.maximo_dias = 0 THEN dim_grupo_vencimiento.minimo_dias || ' Días o más'::text
    ELSE ((dim_grupo_vencimiento.minimo_dias || '-'::text) || dim_grupo_vencimiento.maximo_dias) || ' Días'::text
END, dim_grupo_vencimiento.id_grupo_vencimiento, total.total_por_vencer
  ORDER BY dim_grupo_vencimiento.id_grupo_vencimiento;

GRANT ALL ON TABLE vw_saldos_por_vencer_dias_foraneo TO rhjasper;


CREATE OR REPLACE VIEW vw_saldos_foraneos AS 
 SELECT dim_clientes.codigo_cliente,
    dim_clientes.nombre_cliente,
    ( SELECT vw_saldos_vencidos_dias_foraneo.total_vencido
           FROM vw_saldos_vencidos_dias_foraneo
          WHERE vw_saldos_vencidos_dias_foraneo.grupo = '0-15 Días'::text AND vw_saldos_vencidos_dias_foraneo.codigo_cliente::text = dim_clientes.codigo_cliente::text) AS "0-15 Días",
    ( SELECT vw_saldos_vencidos_dias_foraneo.total_vencido
           FROM vw_saldos_vencidos_dias_foraneo
          WHERE vw_saldos_vencidos_dias_foraneo.grupo = '16-30 Días'::text AND vw_saldos_vencidos_dias_foraneo.codigo_cliente::text = dim_clientes.codigo_cliente::text) AS "16-30 Días",
    ( SELECT vw_saldos_vencidos_dias_foraneo.total_vencido
           FROM vw_saldos_vencidos_dias_foraneo
          WHERE vw_saldos_vencidos_dias_foraneo.grupo = '31-45 Días'::text AND vw_saldos_vencidos_dias_foraneo.codigo_cliente::text = dim_clientes.codigo_cliente::text) AS "31-45 Días",
    ( SELECT vw_saldos_vencidos_dias_foraneo.total_vencido
           FROM vw_saldos_vencidos_dias_foraneo
          WHERE vw_saldos_vencidos_dias_foraneo.grupo = '46 Días o más'::text AND vw_saldos_vencidos_dias_foraneo.codigo_cliente::text = dim_clientes.codigo_cliente::text) AS "46 Días o más",
    ( SELECT vw_saldos_por_vencer_dias_foraneo.por_vencer
           FROM vw_saldos_por_vencer_dias_foraneo
          WHERE vw_saldos_por_vencer_dias_foraneo.grupo = '0-15 Días'::text AND vw_saldos_por_vencer_dias_foraneo.codigo_cliente::text = dim_clientes.codigo_cliente::text) AS "0-15 Días.",
    ( SELECT vw_saldos_por_vencer_dias_foraneo.por_vencer
           FROM vw_saldos_por_vencer_dias_foraneo
          WHERE vw_saldos_por_vencer_dias_foraneo.grupo = '16-30 Días'::text AND vw_saldos_por_vencer_dias_foraneo.codigo_cliente::text = dim_clientes.codigo_cliente::text) AS "16-30 Días.",
    ( SELECT vw_saldos_por_vencer_dias_foraneo.por_vencer
           FROM vw_saldos_por_vencer_dias_foraneo
          WHERE vw_saldos_por_vencer_dias_foraneo.grupo = '31-45 Días'::text AND vw_saldos_por_vencer_dias_foraneo.codigo_cliente::text = dim_clientes.codigo_cliente::text) AS "31-45 Días.",
    ( SELECT vw_saldos_por_vencer_dias_foraneo.por_vencer
           FROM vw_saldos_por_vencer_dias_foraneo
          WHERE vw_saldos_por_vencer_dias_foraneo.grupo = '46 Días o más'::text AND vw_saldos_por_vencer_dias_foraneo.codigo_cliente::text = dim_clientes.codigo_cliente::text) AS "46 Días o más."
   FROM dim_clientes
  ORDER BY dim_clientes.nombre_cliente;

GRANT ALL ON TABLE vw_saldos_foraneos TO rhjasper;