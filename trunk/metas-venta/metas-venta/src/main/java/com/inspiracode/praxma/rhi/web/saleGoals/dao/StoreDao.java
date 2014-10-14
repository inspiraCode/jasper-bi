package com.inspiracode.praxma.rhi.web.saleGoals.dao;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.Statement;
import java.util.ArrayList;
import java.util.List;

import com.inspiracode.praxma.rhi.web.saleGoals.db.DbUtil;
import com.inspiracode.praxma.rhi.web.saleGoals.dto.Store;

public class StoreDao {

	private Connection connection;

	public StoreDao() {
		connection = DbUtil.getConnection();
	}

	public void AddStore(Store store) {
		try {
			String updateString = "INSERT INTO dim_store (store_name, id_empresa) "
					+ "VALUES(?, ?)";

			PreparedStatement ps = connection.prepareStatement(updateString);
			ps.setString(1, store.getStoreName());
			ps.setInt(2, store.getEnterpriseId());
			ps.executeUpdate();
		} catch (Exception e) {
			e.printStackTrace();
		}
	}

	public void deleteStore(Store store) {
		try {
			String updateString = "UPDATE dim_sellers SET store_id = 0 "
					+ "WHERE store_id = ?";

			PreparedStatement ps = connection.prepareStatement(updateString);
			ps.setInt(1, store.getStoreId());
			ps.executeUpdate();

			String deleteString = "DELETE FROM dim_store WHERE store_id = ?";
			PreparedStatement dps = connection.prepareStatement(deleteString);
			dps.setInt(1, store.getStoreId());
			dps.executeUpdate();
		} catch (Exception e) {
			e.printStackTrace();
		}
	}

	public List<Store> getAllStores() {
		List<Store> stores = new ArrayList<Store>();
		try {
			Statement statement = connection.createStatement();
			String sqlString = "SELECT DISTINCT store_id, dim_store.id_empresa, store_name, dim_clientes.empresa " +
			"FROM dim_store " +
			"INNER JOIN dim_clientes ON dim_store.id_empresa = dim_clientes.id_empresa " +
			"ORDER BY dim_store.id_empresa, store_name";
			
			ResultSet rs = statement.executeQuery(sqlString);

			while (rs.next()) {
				Store store = new Store();
				store.setStoreId(rs.getInt("store_id"));
				store.setEnterpriseId(rs.getInt("id_empresa"));
				store.setStoreName(rs.getString("store_name"));
				store.setEnterpriseName(rs.getString("empresa"));

				stores.add(store);
			}
			rs.close();
		} catch (Exception e) {
			e.printStackTrace();
		}
		return stores;
	}

}
