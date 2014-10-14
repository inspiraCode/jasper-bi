package com.inspiracode.praxma.rhi.web.saleGoals.dto;

public class Store {
	private int storeId;
	private String storeName;
	private int enterpriseId;
	private String enterpriseName;

	public int getStoreId() {
		return storeId;
	}

	public void setStoreId(int value) {
		storeId = value;
	}

	public String getStoreName() {
		return storeName;
	}

	public void setStoreName(String value) {
		storeName = value;
	}

	public int getEnterpriseId() {
		return enterpriseId;
	}

	public void setEnterpriseId(int value) {
		enterpriseId = value;
	}

	public String getEnterpriseName() { return enterpriseName; }
	public void setEnterpriseName(String value){ enterpriseName = value; }
	
	@Override
	public String toString() {
		return String.format("storeId:{0};storeName:{1};enterpriseId:{2}",
				storeId, storeName, enterpriseId);
	}
}
