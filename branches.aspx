﻿<%@ Page Title="Branches" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="branches.aspx.cs" Inherits="Branches" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/branches_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/branches_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<table class="table table-hover table-bordered">
			<thead>
				<tr class="info">
					<th>Branch Name</th>
					<th>Date</th>
					<th>Author</th>
					<th>Author Email</th>
				</tr>
			</thead>
			<tbody>
				<tr ng-repeat="b in branches"">
					<td><a href="commits.aspx?branch={{b.NAME}}">{{b.NAME}}</a></td>
					<td><a href="commits.aspx?branch={{b.NAME}}">{{b.DATE}}</a></td>
					<td><a href="commits.aspx?branch={{b.NAME}}">{{b.AUTHOR}}</a></td>
					<td><a href="commits.aspx?branch={{b.NAME}}">{{b.AUTHOREML}}</a></td>
				</tr>
			</tbody>
		</table>
	</div>
</asp:Content>
