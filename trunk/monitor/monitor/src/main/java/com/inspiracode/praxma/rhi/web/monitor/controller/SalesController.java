package com.inspiracode.praxma.rhi.web.monitor.controller;

import java.io.IOException;
import java.io.InputStream;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.nio.file.attribute.BasicFileAttributeView;
import java.nio.file.attribute.BasicFileAttributes;
import java.nio.file.attribute.FileTime;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.Properties;

import javax.servlet.RequestDispatcher;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import com.inspiracode.praxma.rhi.web.monitor.dao.MonitorDao;
import com.inspiracode.praxma.rhi.web.monitor.dao.SalesDao;
import com.inspiracode.praxma.rhi.web.monitor.db.DbUtil;
import com.inspiracode.praxma.rhi.web.monitor.dto.Monitor;
import com.inspiracode.praxma.rhi.web.monitor.dto.SaleSummary;

public class SalesController extends HttpServlet {
	private static final long serialVersionUID = 7549089572991914962L;
	private static String CHART_CARROUSEL = "/charts.jsp";
	private static String ERROR_PAGE = "/error.jsp";
	private SalesDao salesDao;
	private MonitorDao monitorDao;

	private List<SaleSummary> daySales = new ArrayList<SaleSummary>();
	private List<SaleSummary> weekSales = new ArrayList<SaleSummary>();
	private List<SaleSummary> monthSales = new ArrayList<SaleSummary>();

	public SalesController() {
		super();
		salesDao = new SalesDao();
		monitorDao = new MonitorDao();
	}

	@Override
	protected void doGet(HttpServletRequest request,
			HttpServletResponse response) throws ServletException, IOException {
		
		String sMonitor = request.getParameter("monitor");
		Monitor oMonitor = null;
		if(sMonitor == null || sMonitor.trim() == ""){
			request.setAttribute("sErrorMessage", "Error: Monitor no especificado.");
			RequestDispatcher view = request.getRequestDispatcher(ERROR_PAGE);
			view.forward(request, response);
			return;
		}
		
		
		sMonitor = sMonitor.replace("%20", " ");
		
		List<Monitor> allMonitors = monitorDao.getAllMonitors();
		for(Monitor monitor : allMonitors){
			if(monitor.getMonitorName().equals(sMonitor)){
				oMonitor = monitor;
				break;
			}
		}
		
		if(oMonitor == null){
			request.setAttribute("sErrorMessage", "Error: Monitor no existente.");
			RequestDispatcher view = request.getRequestDispatcher(ERROR_PAGE);
			view.forward(request, response);
			return;
		}
		
		List<SaleSummary> validateDaySales =salesDao.getDailySales(oMonitor.getMonitorId());
		List<SaleSummary> validateWeekSales =salesDao.getWeeklySales(oMonitor.getMonitorId());
		List<SaleSummary> validateMonthSales =salesDao.getMonthlySales(oMonitor.getMonitorId());
		
		if (!validateDaySales.isEmpty()) {
			daySales = validateDaySales;
		}
		
		if (!validateWeekSales.isEmpty()) {
			weekSales =validateWeekSales;
		}
		
		if (!validateMonthSales.isEmpty()) {
			monthSales = validateMonthSales;
		}

		request.setAttribute("daySales", daySales);
		request.setAttribute("weekSales", weekSales);
		request.setAttribute("monthSales", monthSales);
		
		request.getSession().setAttribute("oMonitor", oMonitor);
		
		request.setAttribute("cFileDates", getLastUpdated(oMonitor));
		
		RequestDispatcher view = request.getRequestDispatcher(CHART_CARROUSEL);
		view.forward(request, response);
	}
	
	private String getLastUpdated(Monitor monitor) throws IOException
	{
		Properties prop = new Properties();
		InputStream is = DbUtil.class.getClassLoader().getResourceAsStream(
				"/images.properties");
		prop.load(is);
		
		Path p = Paths.get(prop.getProperty("base_dir") + "/" + prop.getProperty("monthly_zip") + "_" + monitor.getMonitorName().replace(" ", "_") + ".html.zip");
	    BasicFileAttributes view
	       = Files.getFileAttributeView(p, BasicFileAttributeView.class)
	              .readAttributes();
	    
	    FileTime created = view.lastModifiedTime();
	    Date dtCreated = new Date(created.toMillis());
	    
	    String result = (new SimpleDateFormat("HH:mm")).format(dtCreated);
	    return result;
	}
}
