package com.inspiracode.praxma.rhi.web.saleGoals.dao;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.util.ArrayList;
import java.util.List;

import com.inspiracode.praxma.rhi.web.saleGoals.db.DbUtil;
import com.inspiracode.praxma.rhi.web.saleGoals.dto.Seller;


public class SellerDao {
	private Connection connection;
	public SellerDao(){
		connection = DbUtil.getConnection();
	}
	
	public void updateSellerGoal(Seller seller) {
		try{
			String updateString = "UPDATE dim_sellers " +
			"SET weekly_goal = ?, " +
			"store_id = ?, " +
			"monitor_id = ? " + 
			"WHERE seller_id = ?";
			
			PreparedStatement ps = connection.prepareStatement(updateString);
			
			ps.setDouble(1, seller.getWeeklyGoal());
			ps.setInt(2, seller.getBodega());
			ps.setInt(3, seller.getMonitor());
			ps.setInt(4, seller.getSellerId());
			
			ps.executeUpdate();
		}catch(Exception e){
			e.printStackTrace();
		}
	}
	
	public List<Seller> getAllSellers() {
		List<Seller> sellers = new ArrayList<Seller>();
		try{
			Statement statement = connection.createStatement();
			String sqlString = "SELECT " +
					"seller_id, ap_id, agent_code, agent_name, weekly_goal, empresa, id_empresa, store_id, monitor_id " +
					"FROM dim_sellers " +
					"ORDER BY seller_id";
			ResultSet rs = statement.executeQuery(sqlString);
			
			while( rs.next() )
			{
				Seller seller = new Seller();
				seller.setSellerId(rs.getInt("seller_id"));
				seller.setAdminPaqId(rs.getInt("ap_id"));
				seller.setAgentCode(rs.getString("agent_code"));
				seller.setAgentName(rs.getString("agent_name"));
				seller.setWeeklyGoal(rs.getDouble("weekly_goal"));
				seller.setCompany(rs.getString("empresa"));
				seller.setCompanyId(rs.getInt("id_empresa"));
				seller.setBodega(rs.getInt("store_id"));
				seller.setMonitor(rs.getInt("monitor_id"));
				
				sellers.add(seller);
			}
			rs.close();
		} catch (SQLException e){
			e.printStackTrace();
		}
		
		return sellers;
	}
	
	public Seller getSellerById(int sellerId){
		Seller seller = null;
		
		try{
			String sqlString = "SELECT " +
					"ap_id, agent_code, agent_name, phone, weekly_goal, empresa, id_empresa, store_id, monitor_id " +
					"FROM dim_sellers " +
					"WHERE seller_id = ?";
			PreparedStatement ps = connection.prepareStatement(sqlString);
			ps.setInt(1, sellerId);
			
			ResultSet rs = ps.executeQuery();
			
			if(rs.next()){
				seller = new Seller();
				seller.setSellerId(sellerId);
				seller.setAdminPaqId(rs.getInt("ap_id"));
				seller.setAgentCode(rs.getString("agent_code"));
				seller.setAgentName(rs.getString("agent_name"));
				seller.setPhone(rs.getString("phone"));
				seller.setWeeklyGoal(rs.getDouble("weekly_goal"));
				seller.setCompany(rs.getString("empresa"));
				seller.setCompanyId(rs.getInt("id_empresa"));
				seller.setBodega(rs.getInt("store_id"));
				seller.setMonitor(rs.getInt("monitor_id"));
			}
			
		}catch(SQLException e){
			e.printStackTrace();
		}
		
		return seller;
	}
	
	
}
