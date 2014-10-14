package com.inspiracode.praxma.rhi.web.saleGoals.dto;

public class Seller {
	private int sellerId;
	private int adminPaqId;
	private String agentCode;
	private String agentName;
	private String phone;
	private double weeklyGoal;
	private String company;
	private int companyId;
	private int bodega;
	
	public int getBodega(){return bodega;}
	public void setBodega(int value){bodega=value;}
	
	public int getSellerId() { return sellerId; }
	public void setSellerId(int value){ sellerId = value; }
	
	public int getAdminPaqId(){ return adminPaqId; }
	public void setAdminPaqId(int value){ adminPaqId = value; }
	
	public String getAgentCode(){ return agentCode; }
	public void setAgentCode(String value){ agentCode = value; }
	
	public String getAgentName() { return agentName; }
	public void setAgentName(String value){ agentName = value; }
	
	public String getPhone() { return phone; }
	public void setPhone(String value){ phone = value; }
	
	public double getWeeklyGoal(){ return weeklyGoal; }
	public void setWeeklyGoal(double value){ weeklyGoal = value; }
	
	public String getCompany(){ return company; }
	public void setCompany( String value ){ company = value; }
	
	public int getCompanyId(){ return companyId; }
	public void setCompanyId( int value ){ companyId = value; }
	
	@Override
	public String toString(){
		return String.format("sellerId:{0};adminPaqId:{1};agentCode:{2};agentName:{3};email:{4};weeklyGoal:{5};" +
		"company:{6};companyId:{7}", sellerId, adminPaqId, agentCode, phone, weeklyGoal, company, companyId);
	}
	
}
