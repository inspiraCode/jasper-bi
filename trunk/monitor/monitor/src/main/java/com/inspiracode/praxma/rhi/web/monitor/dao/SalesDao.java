package com.inspiracode.praxma.rhi.web.monitor.dao;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.List;

import com.inspiracode.praxma.rhi.web.monitor.db.DbUtil;
import com.inspiracode.praxma.rhi.web.monitor.dto.SaleSummary;

public class SalesDao {
	private Connection connection;

	public SalesDao() {
		connection = DbUtil.getConnection();
	}

	public List<SaleSummary> getDailySales(int monitor_id) {
		List<SaleSummary> sales = new ArrayList<SaleSummary>();

		try {
			String sqlString = "SELECT agent_name, SUM(sold_today)/1000 AS venta, "
					+ "((weekly_goal/6) - SUM(sold_today))/1000 AS faltante "
					+ "FROM dim_sellers "
					+ "INNER JOIN fact_sales "
					+ "ON fact_sales.seller_id = dim_sellers.seller_id "
					+ "WHERE weekly_goal>0 AND is_local=true AND dim_sellers.monitor_id = ?"
					+ "GROUP BY agent_name, weekly_goal "
					+ "ORDER BY agent_name;";
			
			PreparedStatement ps = connection.prepareStatement(sqlString);
			ps.setInt(1, monitor_id);
			
			ResultSet rs = ps.executeQuery();

			while (rs.next()) {
				SaleSummary sale = new SaleSummary();
				sale.setAgente(rs.getString("agent_name"));
				sale.setVenta(rs.getDouble("venta"));
				sale.setFaltante(rs.getDouble("faltante"));

				sales.add(sale);

			}
		} catch (SQLException e) {
			e.printStackTrace();
		}

		return sales;
	}

	public List<SaleSummary> getWeeklySales(int monitor_id) {
		List<SaleSummary> sales = new ArrayList<SaleSummary>();

		try {
			String sqlString = "SELECT agent_name, SUM(sold_week)/1000 AS venta, "
					+ "(weekly_goal - SUM(sold_week))/1000 AS faltante "
					+ "FROM dim_sellers "
					+ "INNER JOIN fact_sales "
					+ "ON fact_sales.seller_id = dim_sellers.seller_id "
					+ "WHERE weekly_goal>0 AND is_local=true AND dim_sellers.monitor_id = ?"
					+ "GROUP BY agent_name, weekly_goal "
					+ "ORDER BY agent_name;";
			PreparedStatement ps = connection.prepareStatement(sqlString);
			ps.setInt(1, monitor_id);
			
			ResultSet rs = ps.executeQuery();

			while (rs.next()) {
				SaleSummary sale = new SaleSummary();
				sale.setAgente(rs.getString("agent_name"));
				sale.setVenta(rs.getDouble("venta"));
				sale.setFaltante(rs.getDouble("faltante"));

				sales.add(sale);

			}
		} catch (SQLException e) {
			e.printStackTrace();
		}

		return sales;
	}

	public List<SaleSummary> getMonthlySales(int monitor_id) {
		List<SaleSummary> sales = new ArrayList<SaleSummary>();

		try {
			String sqlString = "SELECT agent_name, SUM(sold_month)/1000 AS venta, "
					+ "((weekly_goal * 4) - SUM(sold_month))/1000 AS faltante "
					+ "FROM dim_sellers "
					+ "INNER JOIN fact_sales "
					+ "ON fact_sales.seller_id = dim_sellers.seller_id "
					+ "WHERE weekly_goal>0 AND is_local=true AND dim_sellers.monitor_id = ?"
					+ "GROUP BY agent_name, weekly_goal "
					+ "ORDER BY agent_name;";
			PreparedStatement ps = connection.prepareStatement(sqlString);
			ps.setInt(1, monitor_id);
			
			ResultSet rs = ps.executeQuery();

			while (rs.next()) {
				SaleSummary sale = new SaleSummary();
				sale.setAgente(rs.getString("agent_name"));
				sale.setVenta(rs.getDouble("venta"));
				sale.setFaltante(rs.getDouble("faltante"));

				sales.add(sale);

			}
		} catch (SQLException e) {
			e.printStackTrace();
		}

		return sales;
	}

}
