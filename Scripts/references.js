﻿var _admin = undefined;
function IsAdmin() {
	if (typeof _admin === "undefined") {
		_admin = document.getElementById("isadmin").value;
	}
	return _admin === "True";
}
function userID() {
	return parseInt(document.getElementById("userid").value);
}
function ttUserID() {
	return parseInt(document.getElementById("ttuserid").value);
}
function loadReference($scope, member, $http, localmember, functionname, params, func) {
	params = params || {};
	var m = localmember + "_storageversion";
	var curr = document.getElementById("referenceid").value;
	if (localStorage[m] != curr) {
		localStorage.removeItem(localmember);
		localStorage[m] = curr;
	}

	if (localStorage[localmember]) {
		$scope[member] = JSON.parse(localStorage[localmember]);
		if (func) func();
	} else {
		if (!("loaders" in $scope)) {
			$scope["loaders"] = 0;
		}
		var prgtypes = StartProgress("Loading " + localmember + "...");
		$scope["loaders"]++;
		$http.post("trservice.asmx/" + functionname, JSON.stringify(params))
			.then(function (result) {
				$scope[member] = result.data.d;
				localStorage[localmember] = JSON.stringify($scope[member]);
				EndProgress(prgtypes); $scope["loaders"]--;
				if (func) func();
			});
	}
}
var defDispCol = { "background-color": "white" };
function getDispoColorById() {
	return function (id, $scope) {
		if ($scope.dispos.length < 1) {
			return defDispCol;
		}
		var res = $scope.dispos.filter(function (x) { return x.ID == id; });
		if (res.length < 1) {
			return defDispCol;
		}
		return { "background-color": res[0].COLOR };
	};
}
function getDispoById() {
	return function (id, $scope) {
		if ($scope.dispos.length < 1) {
			return "";
		}
		var res = $scope.dispos.filter(function (x) { return x.ID == id; });
		if (res.length < 1) {
			return "";
		}
		return res[0].DESCR;
	};
}
function getCompById() {
	return function (id, $scope) {
		if ($scope.comps.length < 1) {
			return "";
		}
		var res = $scope.comps.filter(function (x) { return x.ID == id; });
		if (res.length < 1) {
			return "";
		}
		return res[0].DESCR;
	};
}
function getSeveById() {
	return function (id, $scope) {
		var r = $scope.severs.filter(function (x) { return x.ID == id; });
		if (r.length > 0) {
			return r[0].DESCR;
		}
		return "";
	};
}
function getUserById() {
	return function (id, $scope) {
		if (!$scope.users) {
			return "";
		}
		var r = $scope.users.filter(function (x) { return x.ID == id; });
		if (r.length > 0) {
			return r[0].FULLNAME;
		}
		return "";
	};
}
function getUserTRIDById() {
	return function (id, $scope) {
		if (!$scope.users) {
			return "";
		}
		var r = $scope.users.filter(function (x) { return x.ID == id; });
		if (r.length > 0) {
			return r[0].TRID;
		}
		return "";
	};
}

function getTypes($scope, member, $http) {
	loadReference($scope, member, $http, "types", "gettasktypes");
}
function getDispos($scope, member, $http) {
	loadReference($scope, member, $http, "dispos", "gettaskdispos");
}
function getUsers($scope, member, $http) {
	loadReference($scope, member, $http, "users", "gettaskusers");
}
function getMPSUsers($scope, member, $http, func) {
	$scope[member] = [];
	loadReference($scope, member, $http, "MPSUsers", "getMPSUsers", { "active": true }, func);
}
function getPriorities($scope, member, $http) {
	loadReference($scope, member, $http, "priorities", "gettaskpriorities");
}
function getSevers($scope, member, $http) {
	loadReference($scope, member, $http, "severs", "gettasksevers");
}
function getProducts($scope, member, $http) {
	loadReference($scope, member, $http, "products", "gettaskproducts");
}
function getComps($scope, member, $http) {
	loadReference($scope, member, $http, "comps", "gettaskcomps");
}

function createTasksFilter(filter)
{
	if (!("dispositions" in filter)) {
		filter.dispositions = [];
	}
	if (!("components" in filter)) {
		filter.components = [];
	}
	if (!("severities" in filter)) {
		filter.severities = [];
	}
	if (!("createdUsers" in filter)) {
		filter.createdUsers = [];
	}
	if (!("users" in filter)) {
		filter.users = [];
	}
	if (!("text" in filter)) {
		filter.text = "";
	}
	if (!("startDateEnter" in filter)) {
		filter.startDateEnter = "";
	} else {
		if (filter.startDateEnter !== "") {
			filter.startDateEnter = StringToDate(filter.startDateEnter);
		}
	}
	if (!("endDateEnter" in filter)) {
		filter.endDateEnter = "";
	} else {
		if (filter.endDateEnter !== "") {
			filter.endDateEnter = StringToDate(filter.endDateEnter);
		}
	}
	if (!("startDateCreated" in filter)) {
		filter.startDateCreated = "";
	} else {
		if (filter.startDateCreated !== "") {
			filter.startDateCreated = StringToDate(filter.startDateCreated);
		}
	}
	if (!("endDateCreated" in filter)) {
		filter.endDateCreated = "";
	} else {
		if (filter.endDateCreated !== "") {
			filter.endDateCreated = StringToDate(filter.endDateCreated);
		}
	}
	if (!("startEstim" in filter)) {
		filter.startEstim = "";
	}
	if (!("endEstim" in filter)) {
		filter.endEstim = "";
	}

}
function enterTT() {
	var ttid = parseInt(prompt("Please enter TT ID", getParameterByName("ttid")));
	if (!isNaN(ttid)) {
		window.open("showtask.aspx?ttid=" + ttid, '_blank');
	}
}
function reActivateTooltips() {
	setTimeout(function () { $('[data-toggle="tooltip"]').tooltip(); }, 2000);//when data loaded - activate tooltip.
}
$(function () {
	$(document).bind("keydown", function (e) {
		if (e.keyCode === 188 && event.ctrlKey) {
			enterTT();
		}
	});
	reActivateTooltips();
});