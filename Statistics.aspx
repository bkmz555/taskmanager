﻿<%@ Page Title="Statistics" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="statistics.aspx.cs" Inherits="Statistics" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/statistics_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/statistics_js")%>
	<script src="http://mps.resnet.com/cdn/angular/angular.min.js"></script>
	<script src="http://mps.resnet.com/cdn/chart/Chart.bundle.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="row">
			<div class="col-sm-6">
				<div class="panel panel-primary">
					<div class="panel-heading">Total Hours</div>
					<div class="panel-body">
						<canvas id="hourspermonth" width="400" height="400"></canvas>
					</div>
				</div>
			</div>
			<div class="col-sm-6">
				<div class="panel panel-primary">
					<div class="panel-heading">Total hours by person</div>
					<div class="panel-body">
						<canvas id="hourspermonthP" width="400" height="400"></canvas>
					</div>
				</div>
			</div>
		</div>
		<div class="row">
			<div class="col-sm-6">
				<div class="panel panel-success">
					<div class="panel-heading">Count</div>
					<div class="panel-body">
						<canvas id="countpermonth" width="400" height="400"></canvas>
					</div>
				</div>
			</div>
			<div class="col-sm-6">
				<div class="panel panel-success">
					<div class="panel-heading">Count by person</div>
					<div class="panel-body">
						<canvas id="countpermonthP" width="400" height="400"></canvas>
					</div>
				</div>
			</div>
		</div>
	</div>
</asp:Content>
