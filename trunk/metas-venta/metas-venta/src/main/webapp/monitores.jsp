<%@ page language="java" contentType="text/html; charset=EUC-KR" pageEncoding="EUC-KR"%>
<%@ taglib uri="http://java.sun.com/jstl/core" prefix="c"%>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">

<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=EUC-KR">
<title>Metas de venta</title>

<script type="text/javascript">
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
	altRows('alternatecolor');
}
</script>

<!-- CSS goes in the document HEAD or added to your external stylesheet -->
<style type="text/css">
table.altrowstable {
	font-family: verdana,arial,sans-serif;
	font-size:11px;
	color:#333333;
	border-width: 1px;
	border-color: #a9c6c9;
	border-collapse: collapse;
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
table a {
	text-decoration:none;
	color:white;
	font-size:16px;
	font-family: Tahoma, Geneva, sans-serif;
	font-weight: bold;
}
.oddrowcolor{
	background-color:#d4e3e5;
}
.evenrowcolor{
	background-color:#c3dde0;
}
a {
text-decoration:none;
color:white;
font-size:16px;
font-family: Tahoma, Geneva, sans-serif;
}
#header {
border-bottom: thin solid black;
}
#menu {
background-color:#006;
border-bottom: thin solid black;
padding-left:25px;
color:white;
}
#input-form {
border: thin solid #a9c6c9;
background-color: #cedde0;
width: 450px;
}
#errorDescription {
background-color:red;
width:450px;
color: white;
display: 
<c:choose>
<c:when test="${errorDescription eq ''}">none;</c:when>
<c:otherwise>block;</c:otherwise>
</c:choose>
}
</style>

</head>
<body>
	<div id="header">
	<H1>MONITORES</H1>
	<h2>Empresas de Ramos Hermanos Internacional en AdminPaq</h2>
	<div id="menu">
		<a href="SellerController">Metas</a>&nbsp;|&nbsp;<a href="StoreController">Bodegas</a>
		&nbsp;|&nbsp;<a href="MonitorController">Monitores</a>
	</div>
	</div>
	<br/>
	<div id="input-form">
	<form method="POST" action="MonitorController" id="frmEdit">
		<br/>
		Monitor: <input name="txtMonitorName" type="text" size=30 /> <input type="submit" value="Agregar" />
	</form>
	</div>
	<div id="errorDescription">
	<c:out value="${errorDescription}" />
	</div>
	<br/>
    <table class="altrowstable" id="alternatecolor">
        <thead>
            <tr>
                <th>Monitor</th>
                <th></th>
            </tr>
        </thead>
        <tbody>
            <c:forEach items="${monitors}" var="monitor">
                <tr>
                    <td><c:out value="${monitor.monitorName}" /></td>
                    <td><a href="MonitorController?remove=<c:out value="${monitor.monitorId}"/>">Borrar</a></td>
                </tr>
            </c:forEach>
        </tbody>
    </table>
</body>
</html>