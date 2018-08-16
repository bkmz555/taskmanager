﻿function enterTT() {
	var ttid = prompt("Please enter TT ID", getParameterByName("ttid"));
	if (ttid != null) {
		//window.location = replaceUrlParam(location.href, "ttid", ttid);
		return true;
	}
	return false;
}
$(function () {
	//fix after resizable columns support:
	$("table thead tr th").css("overflow", "visible");

	$(".refmenu").click(function (event) {
		var el = event.target.parentElement.querySelector(".refselector");
		setTimeout(function () { el.focus(); }, 100);
	});
	$(".refselector").keyup(function (event) {
		if (event.keyCode === 27) { //esc
			event.target.parentElement.parentElement.classList.remove("open");
			return;
		}
		var txt = event.target.value.toLowerCase();
		var items = event.target.parentElement.querySelectorAll("label");
		if (txt === "") {
			items.forEach(function (i) {
				i.style.display = "";
			});
		}
		else {
			items.forEach(function (i) {
				if (i.innerText.toLowerCase().includes(txt)) {
					i.style.display = "";
				} else {
					i.style.display = "none";
				}
			});
		}
	});

	var app = angular.module('mpsapplication', []);
	app.filter('getUserById', getUserById);
	app.filter('getCompById', getCompById);
	app.filter('getSeveById', getSeveById);
	app.filter('getDispoById', getDispoById);
	app.filter('getDispoColorById', getDispoColorById);

	app.controller('mpscontroller', ["$scope", "$http", function ($scope, $http) {
		var f = getParameterByName("filter");
		if (f) {
			localStorage.DefectsFilter = f;
		}


		window.addEventListener("popstate", function (event) {
			localStorage.DefectsFilter = JSON.stringify(Object.assign({}, event.state));
			$scope.loadData();
		});

		$scope.onGo = function (keyEvent) {
			if (keyEvent.which === 13) {
				$scope.applyfilter();
				keyEvent.preventDefault();
			}
		};

		//references section
		getDispos($scope, "dispos", $http);
		getUsers($scope, "users", $http);
		getTypes($scope, "types", $http);
		getPriorities($scope, "priorities", $http);
		getSevers($scope, "severs", $http);
		getProducts($scope, "products", $http);
		getComps($scope, "comps", $http);

		$scope.loadData = function () {
			$scope.defectsselected = false;
			var taskprg = StartProgress("Loading tasks...");
			$scope.changed = false;
			$scope.DefectsFilter = {};
			if (localStorage.DefectsFilter) {
				$scope.DefectsFilter = JSON.parse(localStorage.DefectsFilter);
			}
			if (!("dispositions" in $scope.DefectsFilter)) {
				$scope.DefectsFilter.dispositions = [];
			}
			if (!("components" in $scope.DefectsFilter)) {
				$scope.DefectsFilter.components = [];
			}
			if (!("severities" in $scope.DefectsFilter)) {
				$scope.DefectsFilter.severities = [];
			}
			if (!("createdUsers" in $scope.DefectsFilter)) {
				$scope.DefectsFilter.createdUsers = [];
			}
			if (!("users" in $scope.DefectsFilter)) {
				$scope.DefectsFilter.users = [];
			}
			if (!("text" in $scope.DefectsFilter)) {
				$scope.DefectsFilter.text = "";
			}
			$http.post("trservice.asmx/gettasks", JSON.stringify({ "f": $scope.DefectsFilter }))
				.then(function (response) {
					$scope.defects = response.data.d;
					for (var i = 0; i < $scope.defects.length; i++) {
						$scope.defects.checked = false;
					}
					EndProgress(taskprg);;
				});
		};

		$scope.loadData();

		$scope.apply = {};
		$scope.apply.disposition = { "use": false, "value": -1 };
		$scope.apply.component = { "use": false, "value": -1 };
		$scope.apply.severity = { "use": false, "value": -1 };
		$scope.apply.user = { "use": false, "value": -1 };

		$scope.checkall = function () {
			if ($scope.defects.length < 1) {
				return;
			}
			var check = !$scope.defects[0].checked;
			$scope.defects.forEach(function (d) {
				d.checked = check;
			});
			$scope.defectsselected = check;
		};
		$scope.applyfilter = function () {
			localStorage.DefectsFilter = JSON.stringify($scope.DefectsFilter);
			var o = Object.assign({}, $scope.DefectsFilter);
			window.history.pushState(o, "filter:" + localStorage.DefectsFilter, replaceUrlParam(location.href, "filter", localStorage.DefectsFilter));
			$scope.loadData();
		};
		$scope.discardfilter = function () {
			window.location.reload();
		};
		$scope.referenceFiltered = function (id, refname) {
			return $scope.DefectsFilter[refname].findIndex(function (x) { return x == id; }) > -1;
		};
		$scope.styleFiltered = function (refname) {
			if ($scope.DefectsFilter[refname].length > 0) {
				return { "background-color": "yellow" };
			}
			return {};
		};

		$scope.changeDefects = function () {
			var updated = [];
			$scope.defects.forEach(function (d) {
				if (d.checked) {
					var copy = Object.assign({}, d);
					if ($scope.apply.disposition.use && $scope.apply.disposition.value > 0) {
						copy.DISPO = $scope.apply.disposition.value;
					}
					if ($scope.apply.component.use && $scope.apply.component.value > 0) {
						copy.COMP = $scope.apply.component.value;
					}
					if ($scope.apply.severity.use && $scope.apply.severity.value > 0) {
						copy.SEVE = $scope.apply.severity.value;
					}
					if ($scope.apply.user.use && $scope.apply.user.value > 0) {
						copy.AUSER = $scope.apply.user.value;
					}
					delete copy["checked"];
					updated.push(copy);
				}
			});
			if (confirm("Are you sure you want to change " + updated.length + " defects ?")) {
				var updatingprg = StartProgress("Updating tasks...");
				$http.post("trservice.asmx/settaskBase", JSON.stringify({ "defects": updated }))
					.then(function (response) {
						$scope.loadData();
						EndProgress(updatingprg);
					});
			} else {
				// Do nothing!
			}
		};

		$scope.$watch("defects", function (newVal, oldVal) {
			if (newVal && oldVal && $scope.defects.length > 0) {
				var newcheck = false;
				$scope.defects.forEach(function (d) {
					if (d.checked) {
						newcheck = true;
					}
				});
				if ($scope.defectsselected != newcheck) {
					$scope.defectsselected = newcheck;
				}
			}
		}, true);

		$scope.resetReferenceFilter = function (refname, obj) {
			$(obj.target).parent().find("input").prop("checked", false)
			$scope.changed = true;
			$scope.DefectsFilter[refname] = [];
		};
		$scope.changeReferenceFilter = function (id, refname) {
			$scope.changed = true;
			var index = $scope.DefectsFilter[refname].findIndex(function (x) { return x == id; });
			if (index > -1) {
				$scope.DefectsFilter[refname].splice(index, 1);
			} else {
				$scope.DefectsFilter[refname].push(id);
			}
		};
	}]);
})