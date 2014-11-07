package com.inspiracode.praxma.rhi.web.saleGoals.controller;

import java.io.IOException;

import javax.servlet.RequestDispatcher;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import com.inspiracode.praxma.rhi.web.saleGoals.dao.MonitorDao;
import com.inspiracode.praxma.rhi.web.saleGoals.dto.Monitor;

public class MonitorController extends HttpServlet {
	private static final long serialVersionUID = 32083172472037439L;

	private static String LIST_MONITORS = "/monitores.jsp";
	private MonitorDao monitorDao;

	public MonitorController() {
		super();
		monitorDao = new MonitorDao();
	}
	
	@Override
	protected void doGet(HttpServletRequest request,
			HttpServletResponse response) throws ServletException, IOException {
		
		String sRemove = request.getParameter("remove");
		if(sRemove != null)
		{
			int iRemove = Integer.parseInt(sRemove);
			
			Monitor monitor = new Monitor();
			monitor.setMonitorId(iRemove);
			
			monitorDao.deleteMonitor(monitor);
		}
		
		ForwardView(request, response, "");
	}

	@Override
	protected void doPost(HttpServletRequest request,
			HttpServletResponse response) throws ServletException, IOException {

		Monitor monitor = new Monitor();
		
		String sMonitorName = request.getParameter("txtMonitorName");
		
		if("".equals(sMonitorName.trim()))
		{
			ForwardView(request, response, "Capture Nombre");
			return;
		}
		
		monitor.setMonitorName(sMonitorName);
		monitorDao.AddMonitor(monitor);
		
		ForwardView(request, response, "");
	}
	
	private void ForwardView(HttpServletRequest request, HttpServletResponse response, String errorDescription) throws ServletException, IOException{
		RequestDispatcher view = request.getRequestDispatcher(LIST_MONITORS);
		request.setAttribute("monitors", monitorDao.getAllMonitors());
		
		request.setAttribute("errorDescription", errorDescription);
		
		view.forward(request, response);
	}

}
