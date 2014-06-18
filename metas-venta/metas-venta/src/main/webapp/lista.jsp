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
.oddrowcolor{
	background-color:#d4e3e5;
}
.evenrowcolor{
	background-color:#c3dde0;
}
</style>

</head>
<body>
	<H1>METAS DE VENTAS POR AGENTE</H1>
	<h2>Empresas de Ramos Hermanos Internacional en AdminPaq</h2><br/>
	<form method="POST" action="SellerController" id="frmEdit" />
    <table class="altrowstable" id="alternatecolor">
        <thead>
            <tr>
                <th>Empresa</th>
                <th>Codigo Agente</th>
                <th>Nombre</th>
                <th>Meta Semanal de Venta</th>
            </tr>
        </thead>
        <tbody>
            <c:forEach items="${sellers}" var="seller">
                <tr><td><c:out value="${seller.company}" /></td>
                    <td><c:out value="${seller.agentCode}" /></td>
                    <td><c:out value="${seller.agentName}" /></td>
                    <td>$<input type="text" value='<c:out value="${seller.weeklyGoal}" />' 
                    name='txtGoal<c:out value="${seller.sellerId}"/>' 
                    id='txtGoal<c:out value="${seller.sellerId}"/>'
                    onkeypress="return isNumberKey(event)"
                    pattern="[0-9]+(\.[0-9]?)?" /></td>
                </tr>
            </c:forEach>
        </tbody>
    </table>
    <br/><br/><input type="submit" value="Actualizar" />
    </form>
</body>
</html>