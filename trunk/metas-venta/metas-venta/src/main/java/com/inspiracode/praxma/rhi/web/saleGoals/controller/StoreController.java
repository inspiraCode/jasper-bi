package com.inspiracode.praxma.rhi.web.saleGoals.controller;

import java.io.IOException;

import javax.servlet.RequestDispatcher;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import com.inspiracode.praxma.rhi.web.saleGoals.dao.EnterpriseDao;
import com.inspiracode.praxma.rhi.web.saleGoals.dao.StoreDao;
import com.inspiracode.praxma.rhi.web.saleGoals.dto.Store;

public class StoreController extends HttpServlet {
	private static final long serialVersionUID = 32083172472037439L;

	private static String LIST_STORES = "/bodegas.jsp";
	private StoreDao storeDao;
	private EnterpriseDao enterpriseDao;

	public StoreController() {
		super();
		storeDao = new StoreDao();
		enterpriseDao = new EnterpriseDao();
	}
	
	@Override
	protected void doGet(HttpServletRequest request,
			HttpServletResponse response) throws ServletException, IOException {
		
		String sRemove = request.getParameter("remove");
		if(sRemove != null)
		{
			int iRemove = Integer.parseInt(sRemove);
			
			Store store = new Store();
			store.setStoreId(iRemove);
			
			storeDao.deleteStore(store);
		}
		
		ForwardView(request, response, "");
	}

	@Override
	protected void doPost(HttpServletRequest request,
			HttpServletResponse response) throws ServletException, IOException {

		Store store = new Store();
		
		String sEnterpriseId = request.getParameter("selEmpresa");
		int iEnterpriseId = Integer.parseInt(sEnterpriseId);
		if(iEnterpriseId == 0)
		{
			ForwardView(request, response, "Seleccione Empresa");
			return;
		}
		
		String sStoreName = request.getParameter("txtStoreName");
		
		if("".equals(sStoreName.trim()))
		{
			ForwardView(request, response, "Capture Nombre");
			return;
		}
		
		store.setEnterpriseId(iEnterpriseId);
		store.setStoreName(sStoreName);
		storeDao.AddStore(store);
		
		ForwardView(request, response, "");
	}
	
	private void ForwardView(HttpServletRequest request, HttpServletResponse response, String errorDescription) throws ServletException, IOException{
		RequestDispatcher view = request.getRequestDispatcher(LIST_STORES);
		request.setAttribute("stores", storeDao.getAllStores());
		request.setAttribute("enterprises", enterpriseDao.getAllEnterprises());
		
		request.setAttribute("errorDescription", errorDescription);
		
		view.forward(request, response);
	}

}
