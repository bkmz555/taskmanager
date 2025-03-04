﻿<%@ Page Title="Builder" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="builder.aspx.cs" Inherits="builder" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/builder_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/builder_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="row">
			<div class="col-md-3">
				<div class="alert alert-info mt-2" style="text-align: center" ng-hide="lockid == null">
					<a data-toggle="tooltip" title="Click to see full plan for the person" target="_blank" href="editplan.aspx?userid={{lockid}}">
						<img class="rounded-circle" ng-src="{{'getUserImg.ashx?sz=60&id=' + lockid}}" alt="Smile" height="60" width="60" />
						<div>
							<strong>Locked by: {{lockid | getUserById:this}}</strong>
						</div>
					</a>
					<i class="fas fa-unlock-alt"></i>
				</div>
			</div>
			<div class="col-md-6">
				<div class="alert alert-success">
					<strong>Attention!</strong> Do not use/click/see this page if you are not aware what it does!
				</div>
				<button ng-click="start()" ng-disabled="readonly || progress()" type="button" class="btn btn-block btn-success">Get git.<img ng-show="progress()" height="20" width="20" src="images/process.gif" /></button>
				<pre ng-show="gitStatus.length > 0"><code ng-bind-html="gitStatus | rawHtml"></code></pre>
				<button ng-show="gitStatus.length > 0" ng-click="release()" ng-disabled="readonly || progress()" type="button" class="btn btn-block btn-success">Increment Version.<img ng-show="progress()" height="20" width="20" src="images/process.gif" /></button>
				<pre ng-show="psStatus.length > 0"><code ng-bind-html="psStatus | rawHtml"></code></pre>
				<button ng-show="psStatus.length > 0" ng-click="push2Master()" ng-disabled="readonly || progress()" type="button" class="btn btn-block btn-success">Push To mater.<img ng-show="progress()" height="20" width="20" src="images/process.gif" /></button>
				<div ng-show="pushStatus.length > 0" class="well well-sm" ng-bind-html="pushStatus | rawHtml"></div>
			</div>
			<div class="col-md-3">
				<div class="toast" data-autohide="false">
					<div class="toast-header">
						<strong class="mr-auto text-primary">Scheduled Builds</strong>
					</div>
					<div class="toast-body">
						<div class="custom-control custom-switch">
							<input type="checkbox" class="custom-control-input" id="sched" ng-model="scheduledBuild.ENABLED">
							<label class="custom-control-label" for="sched">Scheduled Build Enabled</label>
						</div>
						<input type="time" id="stime" name="scheduletime" ng-model="scheduledBuild.TIME" class="form-control" ng-disabled="!scheduledBuild.ENABLED">
						<span>Recur on:</span>
						<ul class="list-group">
							<li class="list-group-item list-group-item-light" ng-repeat="d in scheduledBuild.DAYS">
								<div class="custom-control custom-switch">
									<input type="checkbox" class="custom-control-input" id="day{{d}}" ng-model="d.USE" ng-disabled="!scheduledBuild.ENABLED">
									<label class="custom-control-label" for="day{{d}}">{{d.DAYNAME}}</label>
								</div>
							</li>
						</ul>
						<button type="button" class="btn btn-outline-secondary btn-sm" ng-click="applySchedule()">Apply Settings</button>
					</div>
				</div>
			</div>
		</div>
	</div>
</asp:Content>
