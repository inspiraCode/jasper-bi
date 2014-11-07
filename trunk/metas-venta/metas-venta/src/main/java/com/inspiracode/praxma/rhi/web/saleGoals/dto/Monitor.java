package com.inspiracode.praxma.rhi.web.saleGoals.dto;

public class Monitor {
	private int monitorId;
	private String monitorName;


	public int getMonitorId() {
		return monitorId;
	}

	public void setMonitorId(int monitorId) {
		this.monitorId = monitorId;
	}

	public String getMonitorName() {
		return monitorName;
	}

	public void setMonitorName(String monitorName) {
		this.monitorName = monitorName;
	}
	

	
	@Override
	public String toString() {
		return String.format("monitorId:{0};monitorName:{1};",
				monitorId, monitorName);
	}
}
