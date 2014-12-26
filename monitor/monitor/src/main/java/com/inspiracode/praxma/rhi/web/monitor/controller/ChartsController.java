package com.inspiracode.praxma.rhi.web.monitor.controller;

import java.io.ByteArrayInputStream;
import java.io.File;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.nio.file.Files;
import java.nio.file.StandardCopyOption;
import java.util.Properties;
import java.util.UUID;

import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import com.inspiracode.praxma.rhi.web.monitor.ZipFileHandler;
import com.inspiracode.praxma.rhi.web.monitor.db.DbUtil;
import com.inspiracode.praxma.rhi.web.monitor.dto.Monitor;

public class ChartsController extends HttpServlet {
	private static final long serialVersionUID = 3747050811360653171L;

	private String baseDir, dailyZip, weeklyZip, monthlyZip;
	
	@Override
	public void doGet(HttpServletRequest req, HttpServletResponse resp)
			throws IOException {
		
		resp.setContentType("image/png");
		
		String zipFilePath = "";
		String tempFilePath = "";
		String entryPath = "";
		
		loadConfig();
		String requestedChart = req.getParameter("chart");
		ZipFileHandler zfh = null;
		
		Monitor theMonitor = (Monitor) req.getSession().getAttribute("oMonitor");
		
		if("day".equalsIgnoreCase(requestedChart)){
			zipFilePath = baseDir + "/" + dailyZip + "_" + theMonitor.getMonitorName().replace(" ", "_") + ".html.zip";
			tempFilePath = baseDir + "/temp_" + dailyZip + "_" + theMonitor.getMonitorName().replace(" ", "_") + ".html.zip";
			entryPath = dailyZip + "_" + theMonitor.getMonitorName().replace(" ", "_") + ".html_files/img_0_0_0";
			
		}
		
		if("week".equalsIgnoreCase(requestedChart)){
			zipFilePath = baseDir + "/" + weeklyZip + "_" + theMonitor.getMonitorName().replace(" ", "_") + ".html.zip";
			tempFilePath = baseDir + "/temp_" + weeklyZip + "_" + theMonitor.getMonitorName().replace(" ", "_") + ".html.zip";
			entryPath = weeklyZip + "_" + theMonitor.getMonitorName().replace(" ", "_") + ".html_files/img_0_0_0";
		}
		
		if("month".equalsIgnoreCase(requestedChart)){
			zipFilePath = baseDir + "/" + monthlyZip + "_" + theMonitor.getMonitorName().replace(" ", "_") + ".html.zip";
			tempFilePath = baseDir + "/temp_" + monthlyZip + "_" + theMonitor.getMonitorName().replace(" ", "_") + ".html.zip";
			entryPath = monthlyZip + "_" + theMonitor.getMonitorName().replace(" ", "_") + ".html_files/img_0_0_0";
		}
		tempFilePath+= UUID.randomUUID().toString();
		
		File fSource = new File(zipFilePath);
		File fDestination = new File(tempFilePath);
		Files.copy(fSource.toPath(), fDestination.toPath(), StandardCopyOption.REPLACE_EXISTING);
		
		zfh = new ZipFileHandler();
		
	    InputStream in = new ByteArrayInputStream(zfh.getEntry(tempFilePath, entryPath));
	    
	    OutputStream out = resp.getOutputStream();

		// Copy the contents of the file to the output stream
		byte[] buf = new byte[1024];
		int count = 0;
		while ((count = in.read(buf)) >= 0) {
			out.write(buf, 0, count);
		}
		out.close();
		in.close();
		Files.delete(fDestination.toPath());
	}
	
	private void loadConfig(){
		try {
			Properties prop = new Properties();
			InputStream is = DbUtil.class.getClassLoader().getResourceAsStream(
					"/images.properties");
			prop.load(is);
			
			baseDir = prop.getProperty("base_dir");
			dailyZip = prop.getProperty("daily_zip");
			weeklyZip = prop.getProperty("weekly_zip");
			monthlyZip = prop.getProperty("monthly_zip");
		} catch (Exception e) {
			e.printStackTrace();
		}
	}

}
