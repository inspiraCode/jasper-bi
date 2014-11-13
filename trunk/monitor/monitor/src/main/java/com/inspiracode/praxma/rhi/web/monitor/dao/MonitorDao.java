package com.inspiracode.praxma.rhi.web.monitor.dao;

import java.sql.Connection;
import java.sql.ResultSet;
import java.sql.Statement;
import java.util.ArrayList;
import java.util.List;

import com.inspiracode.praxma.rhi.web.monitor.db.DbUtil;
import com.inspiracode.praxma.rhi.web.monitor.dto.Monitor;


public class MonitorDao {

	private Connection connection;

	public MonitorDao() {
		connection = DbUtil.getConnection();
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
