package com.inspiracode.praxma.rhi.web.saleGoals.dao;

import java.sql.Connection;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.util.ArrayList;
import java.util.List;

import com.inspiracode.praxma.rhi.web.saleGoals.db.DbUtil;
import com.inspiracode.praxma.rhi.web.saleGoals.dto.Enterprise;

public class EnterpriseDao {

	private Connection connection;
	public EnterpriseDao(){
		connection = DbUtil.getConnection();
	}
	
	public List<Enterprise> getAllEnterprises() {
		List<Enterprise> empresas = new ArrayList<Enterprise>();
		try{
			Statement statement = connection.createStatement();
			String sqlString = "SELECT DISTINCT " +
					"empresa, id_empresa " +
					"FROM dim_clientes " +
					"ORDER BY empresa";
			ResultSet rs = statement.executeQuery(sqlString);
			
			while( rs.next() )
			{
				Enterprise empresa = new Enterprise();
				empresa.setId(rs.getInt("id_empresa"));
				empresa.setName(rs.getString("empresa"));
				empresas.add(empresa);
			}
			rs.close();
		} catch (SQLException e){
			e.printStackTrace();
		}
		
		return empresas;
	}
}
