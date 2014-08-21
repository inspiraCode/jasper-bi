package com.inspiracode.praxma.rhi.web.monitor.db;

import java.io.InputStream;
import java.sql.Connection;
import java.sql.DriverManager;
import java.util.Properties;

public class DbUtil {
	private static Connection connection = null;

	public static Connection getConnection() {
		if (connection != null)
			return connection;

		try {
			Properties prop = new Properties();
			InputStream is = DbUtil.class.getClassLoader().getResourceAsStream(
					"/db.properties");
			prop.load(is);
			String driver = prop.getProperty("driver");
			String url = prop.getProperty("url");
			String user = prop.getProperty("user");
			String password = prop.getProperty("password");
			Class.forName(driver);
			connection = DriverManager.getConnection(url, user, password);
		} catch (Exception e) {
			e.printStackTrace();
		}

		return connection;
	}
}
