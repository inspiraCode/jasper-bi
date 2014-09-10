package com.inspiracode.praxma.rhi.web.monitor.dao;

import java.sql.Connection;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.util.ArrayList;
import java.util.List;

import com.inspiracode.praxma.rhi.web.monitor.db.DbUtil;
import com.inspiracode.praxma.rhi.web.monitor.dto.SaleSummary;

public class SalesDao {
	private Connection connection;

	public SalesDao() {
		connection = DbUtil.getConnection();
	}

	public List<SaleSummary> getDailySales() {
		List<SaleSummary> sales = new ArrayList<SaleSummary>();

		try {
			Statement statement = connection.createStatement();
			String sqlString = "SELECT agent_name, SUM(sold_today)/1000 AS venta, "
					+ "((weekly_goal/6) - SUM(sold_today))/1000 AS faltante "
					+ "FROM dim_sellers "
					+ "INNER JOIN fact_sales "
					+ "ON fact_sales.seller_id = dim_sellers.seller_id "
					+ "WHERE weekly_goal>0 AND is_local=true "
					+ "GROUP BY agent_name, weekly_goal "
					+ "ORDER BY agent_name;";
			ResultSet rs = statement.executeQuery(sqlString);

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

	public List<SaleSummary> getWeeklySales() {
		List<SaleSummary> sales = new ArrayList<SaleSummary>();

		try {
			Statement statement = connection.createStatement();
			String sqlString = "SELECT agent_name, SUM(sold_week)/1000 AS venta, "
					+ "(weekly_goal - SUM(sold_week))/1000 AS faltante "
					+ "FROM dim_sellers "
					+ "INNER JOIN fact_sales "
					+ "ON fact_sales.seller_id = dim_sellers.seller_id "
					+ "WHERE weekly_goal>0 AND is_local=true "
					+ "GROUP BY agent_name, weekly_goal "
					+ "ORDER BY agent_name;";
			ResultSet rs = statement.executeQuery(sqlString);

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

	public List<SaleSummary> getMonthlySales() {
		List<SaleSummary> sales = new ArrayList<SaleSummary>();

		try {
			Statement statement = connection.createStatement();
			String sqlString = "SELECT agent_name, SUM(sold_month)/1000 AS venta, "
					+ "((weekly_goal * 4) - SUM(sold_month))/1000 AS faltante "
					+ "FROM dim_sellers "
					+ "INNER JOIN fact_sales "
					+ "ON fact_sales.seller_id = dim_sellers.seller_id "
					+ "WHERE weekly_goal>0 AND is_local=true "
					+ "GROUP BY agent_name, weekly_goal "
					+ "ORDER BY agent_name;";
			ResultSet rs = statement.executeQuery(sqlString);

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
