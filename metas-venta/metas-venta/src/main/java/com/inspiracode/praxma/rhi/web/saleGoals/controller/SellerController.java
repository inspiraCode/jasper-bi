package com.inspiracode.praxma.rhi.web.saleGoals.controller;

import java.io.IOException;
import java.util.List;

import javax.servlet.RequestDispatcher;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import com.inspiracode.praxma.rhi.web.saleGoals.dao.SellerDao;
import com.inspiracode.praxma.rhi.web.saleGoals.dto.Seller;

public class SellerController extends HttpServlet {
	private static final long serialVersionUID = -1612444270330131996L;
	private static String LIST_GOALS = "/lista.jsp";
	private SellerDao dao;
	
	
	public SellerController() {
		super();
		dao = new SellerDao();
	}
	
	@Override
	protected void doGet(HttpServletRequest request,  HttpServletResponse response) throws ServletException, IOException{
		request.setAttribute("sellers", dao.getAllSellers());
		RequestDispatcher view = request.getRequestDispatcher(LIST_GOALS);
		view.forward(request, response);
	}
	
	@Override
	protected void doPost(HttpServletRequest request, HttpServletResponse response) throws ServletException, IOException {
		
		List<Seller> sellers = dao.getAllSellers();
		
		for(Seller seller : sellers) {
			String goal = request.getParameter("txtGoal" + seller.getSellerId());
			
			double dGoal = Double.parseDouble(goal);
			if(dGoal == seller.getWeeklyGoal()) continue;
			
			seller.setWeeklyGoal(dGoal);
			dao.updateSellerGoal(seller);
			
			String log = String.format("GOAL updated for seller:{0} to {1}", seller.getAgentName(), dGoal);
			System.out.println(log);
		}
		
		RequestDispatcher view = request.getRequestDispatcher(LIST_GOALS);
		request.setAttribute("sellers", dao.getAllSellers());
		view.forward(request, response);
	}
	
}