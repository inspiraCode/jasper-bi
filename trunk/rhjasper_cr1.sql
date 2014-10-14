DROP TABLE IF EXISTS dim_store CASCADE;
CREATE TABLE dim_store(
	store_id SERIAL PRIMARY KEY,
	id_empresa INTEGER,
	store_name VARCHAR(150)
);

ALTER TABLE dim_sellers
	ADD store_id INTEGER DEFAULT 0;

INSERT INTO dim_store(id_empresa, store_name) VALUES (11, 'Local');
INSERT INTO dim_store(id_empresa, store_name) VALUES (11, 'Foraneo');
INSERT INTO dim_store(id_empresa, store_name) VALUES (26, 'Degollado');
INSERT INTO dim_store(id_empresa, store_name) VALUES (26, 'B 105');