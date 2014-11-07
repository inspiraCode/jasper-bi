package com.inspiracode.praxma.rhi.web.saleGoals.dao;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.Statement;
import java.util.ArrayList;
import java.util.List;

import com.inspiracode.praxma.rhi.web.saleGoals.db.DbUtil;
import com.inspiracode.praxma.rhi.web.saleGoals.dto.Monitor;

public class MonitorDao {

	private Connection connection;

	public MonitorDao() {
		connection = DbUtil.getConnection();
	}

	public void AddMonitor(Monitor monitor) {
		try {
			String updateString = "INSERT INTO dim_monitor (monitor_name) "
					+ "VALUES(?)";

			PreparedStatement ps = connection.prepareStatement(updateString);
			ps.setString(1, monitor.getMonitorName());
			ps.executeUpdate();
		} catch (Exception e) {
			e.printStackTrace();
		}
	}

	public void deleteMonitor(Monitor monitor) {
		try {
			String updateString = "UPDATE dim_sellers SET monitor_id = 0 "
					+ "WHERE monitor_id = ?";

			PreparedStatement ps = connection.prepareStatement(updateString);
			ps.setInt(1, monitor.getMonitorId());
			ps.executeUpdate();

			String deleteString = "DELETE FROM dim_monitor WHERE monitor_id = ?";
			PreparedStatement dps = connection.prepareStatement(deleteString);
			dps.setInt(1, monitor.getMonitorId());
			dps.executeUpdate();
		} catch (Exception e) {
			e.printStackTrace();
		}
	}

	public List<Monitor> getAllMonitors() {
		List<Monitor> monitors = new ArrayList<Monitor>();
		try {
			Statement statement = connection.createStatement();
			String sqlString = "SELECT DISTINCT monitor_id, monitor_name " +
			"FROM dim_monitor " +
			"ORDER BY monitor_name";
			
			ResultSet rs = statement.executeQuery(sqlString);

			while (rs.next()) {
				Monitor monitor = new Monitor();
				monitor.setMonitorId(rs.getInt("monitor_id"));
				monitor.setMonitorName(rs.getString("monitor_name"));

				monitors.add(monitor);
			}
			rs.close();
		} catch (Exception e) {
			e.printStackTrace();
		}
		return monitors;
	}

}
