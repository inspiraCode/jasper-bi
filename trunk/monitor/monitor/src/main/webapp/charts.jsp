<%@ page language="java" contentType="text/html; charset=EUC-KR" pageEncoding="UTF-8"%>
<%@ taglib uri="http://java.sun.com/jstl/core" prefix="c"%>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jstl/fmt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">

<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
<meta http-equiv="refresh" content="195">
<title>Monitor de Ventas</title>

<script type="text/javascript">

var imgId =2;
function changePicture(){
  var divTitulos, divSemanaImg, divSemana, divMesImg,  divMes, divDiariaImg, divDiaria;
  divTitulos = document.getElementById('portada');
  divTitulos.style.display = "none";
  
  divMesImg = document.getElementById('img-month');
  divMesImg.style.display = "none";
  
  divMes = document.getElementById('tab-month');
  divMes.style.display = "none";
  
  divSemanaImg = document.getElementById('img-week');
  divSemanaImg.style.display = "none";
  
  divSemana = document.getElementById('tab-week');
  divSemana.style.display = "none";
  
  divDiariaImg = document.getElementById('img-day');
  divDiariaImg.style.display = "none";
  
  divDiaria = document.getElementById('tab-day');
  divDiaria.style.display = "none";
  
  switch(imgId){
    case 1:
      divTitulos.style.display = "block";
      imgId++;
      setTimeout("changePicture()",5000);
      break;
    case 2:
      divMesImg.style.display = "block";
      imgId++;
      setTimeout("changePicture()",5000);
      break;
    case 3:
      divMes.style.display = "block";
      imgId++;
      setTimeout("changePicture()",5000);
      break;
    case 4:
      divSemanaImg.style.display = "block";
      imgId++;
      setTimeout("changePicture()",5000);
      break;
    case 5:
      divSemana.style.display = "block";
      imgId++;
      setTimeout("changePicture()",5000);
      break;
    case 6:
      divDiariaImg.style.display = "block";
      imgId++;
      setTimeout("changePicture()",5000);
      break;
    case 7:
      divDiaria.style.display = "block";
      imgId = 1;
      setTimeout("changePicture()",5000);
      break;
    default:
      divTitulos.style.display = "block";
      imgId = 2;
      setTimeout("changePicture()",5000);
  }
}

function altRows(id){
	if(document.getElementsByTagName){  
		
		var table = document.getElementById(id);  
		var rows = table.getElementsByTagName("tr"); 
		 
		for(i = 0; i < rows.length; i++){          
			if(i % 2 == 0){
				rows[i].className = "evenrowcolor";
			}else{
				rows[i].className = "oddrowcolor";
			}      
		}
	}
}
window.onload=function(){
	altRows('day-table');
	altRows('week-table');
	altRows('month-table');
	
	setTimeout("changePicture()",5000);
}
function isNumberKey(evt){
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode > 31 && (charCode < 48 || charCode > 57))
        return false;
    return true;
}

</script>

<!-- CSS goes in the document HEAD or added to your external stylesheet -->
<style type="text/css">
table.altrowstable {
	font-family: verdana,arial,sans-serif;
	font-size:48px;
	color:#333333;
	border-width: 1px;
	border-color: #a9c6c9;
	border-collapse: collapse;
	margin: 150px auto;
}
table.altrowstable th {
	border-width: 1px;
	padding: 8px;
	border-style: solid;
	border-color: #a9c6c9;
}
table.altrowstable td {
	border-width: 1px;
	padding: 8px;
	border-style: solid;
	border-color: #a9c6c9;
}
.oddrowcolor{
	background-color:#d4e3e5;
}
.evenrowcolor{
	background-color:#c3dde0;
}

/* DIVS */
h1 {
 font-size:52px;
 font-weight: bold;
}
div {
    position:fixed;
    top: 50%;
    left: 50%;
    width:1280px;
    height:1024px;
    margin-top: -512px; /*set to a negative number 1/2 of your height*/
    margin-left: -640px; /*set to a negative number 1/2 of your width*/
    border: 1px solid #ccc;
    background-color: #f3f3f3;
    display:none;
    text-align:center;
}

</style>
</head>
<body>
	<fmt:setLocale value="es_MX"/>
	<jsp:useBean id="now" class="java.util.Date" />
	<div id="portada" style="display:block;">
		<H1>MONITOR DE CUMPLIMIENTO DE METAS DE VENTA</H1>
		<h2>Empresas de Ramos Hermanos Internacional en AdminPaq</h2><br/><br/><br/>
		<h3>Reporte del Mes de <u><fmt:formatDate pattern="MMMM yyyy" value="${now}" /></u></h3><br/>
		<h3>Actualizado al:<u><c:out value="${cFileDates}" /></u> </h3><br/>
	</div>
	<div id="img-day">
		<img width="1280" src="ChartsController?chart=day" />
	</div>
	<div id="tab-day">
		<H1 style="margin:auto;">RESUMEN DE VENTA DEL DIA</H1>
		<table class="altrowstable" id="day-table">
	        <thead>
	            <tr>
	                <th>Agente</th>
	                <th>Venta</th>
	                <th>Faltante</th>
	            </tr>
	        </thead>
	        <tbody>
	        	<c:if test="${empty daySales}">
	        		<tr><td colspan="3">DESCARGANDO DATOS DE ADMINPAQ</td></tr>
	        	</c:if>
	            <c:forEach items="${daySales}" var="sale">
	            	<tr>
	            		<td><c:out value="${sale.agente}" /></td>
	                    <td><fmt:formatNumber value="${sale.venta}" type="currency" /></td>
	                    <td><fmt:formatNumber value="${sale.faltante}" type="currency" /></td>
	                </tr>
	            </c:forEach>
	        </tbody>
	    </table>
	</div>
	<div id="img-week">
		<img width="1280" src="ChartsController?chart=week" />
	</div>
	<div id="tab-week">
		<H1 style="margin:auto;">RESUMEN DE VENTA DE LA SEMANA</H1>
		<table class="altrowstable" id="week-table">
	        <thead>
	            <tr>
	                <th>Agente</th>
	                <th>Venta</th>
	                <th>Faltante</th>
	            </tr>
	        </thead>
	        <tbody>
	        	<c:if test="${empty weekSales}">
	        		<tr><td colspan="3">DESCARGANDO DATOS DE ADMINPAQ</td></tr>
	        	</c:if>
	            <c:forEach items="${weekSales}" var="sale">
	                <tr><td><c:out value="${sale.agente}" /></td>
	                    <td><fmt:formatNumber value="${sale.venta}" type="currency" /></td>
	                    <td><fmt:formatNumber value="${sale.faltante}" type="currency" /></td>
	                </tr>
	            </c:forEach>
	        </tbody>
	    </table>
	</div>
	<div id="img-month">
		<img width="1280" src="ChartsController?chart=month" />
	</div>
	<div id="tab-month">
		<H1 style="margin:auto;">RESUMEN DE VENTA DEL MES</H1>
		<table class="altrowstable" id="month-table">
	        <thead>
	            <tr>
	                <th>Agente</th>
	                <th>Venta</th>
	                <th>Faltante</th>
	            </tr>
	        </thead>
	        <tbody>
	        	<c:if test="${empty monthSales}">
	        		<tr><td colspan="3">DESCARGANDO DATOS DE ADMINPAQ</td></tr>
	        	</c:if>
	            <c:forEach items="${monthSales}" var="sale">
	                <tr><td><c:out value="${sale.agente}" /></td>
	                    <td><fmt:formatNumber value="${sale.venta}" type="currency" /></td>
	                    <td><fmt:formatNumber value="${sale.faltante}" type="currency" /></td>
	                </tr>
	            </c:forEach>
	        </tbody>
	    </table>
	</div>
</body>
</html>