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

import com.inspiracode.praxma.rhi.web.monitor.dao.SalesDao;
import com.inspiracode.praxma.rhi.web.monitor.db.DbUtil;
import com.inspiracode.praxma.rhi.web.monitor.dto.SaleSummary;

public class SalesController extends HttpServlet {
	private static final long serialVersionUID = 7549089572991914962L;
	private static String CHART_CARROUSEL = "/charts.jsp";
	private SalesDao salesDao;

	private List<SaleSummary> daySales = new ArrayList<SaleSummary>();
	private List<SaleSummary> weekSales = new ArrayList<SaleSummary>();
	private List<SaleSummary> monthSales = new ArrayList<SaleSummary>();

	public SalesController() {
		super();
		salesDao = new SalesDao();
	}

	@Override
	protected void doGet(HttpServletRequest request,
			HttpServletResponse response) throws ServletException, IOException {

		if (!salesDao.getDailySales().isEmpty()) {
			daySales = salesDao.getDailySales();
		}
		
		if (!salesDao.getWeeklySales().isEmpty()) {
			weekSales = salesDao.getWeeklySales();
		}
		
		if (!salesDao.getMonthlySales().isEmpty()) {
			monthSales = salesDao.getMonthlySales();
		}

		request.setAttribute("daySales", daySales);
		request.setAttribute("weekSales", weekSales);
		request.setAttribute("monthSales", monthSales);
		
		request.setAttribute("cFileDates", getLastUpdated());
		
		RequestDispatcher view = request.getRequestDispatcher(CHART_CARROUSEL);
		view.forward(request, response);
	}
	
	private String getLastUpdated() throws IOException
	{
		Properties prop = new Properties();
		InputStream is = DbUtil.class.getClassLoader().getResourceAsStream(
				"/images.properties");
		prop.load(is);
		
		Path p = Paths.get(prop.getProperty("base_dir") + "/" + prop.getProperty("monthly_zip") + ".html.zip");
	    BasicFileAttributes view
	       = Files.getFileAttributeView(p, BasicFileAttributeView.class)
	              .readAttributes();
	    
	    FileTime created = view.lastModifiedTime();
	    Date dtCreated = new Date(created.toMillis());
	    
	    String result = (new SimpleDateFormat("HH:mm")).format(dtCreated);
	    return result;
	}
}
