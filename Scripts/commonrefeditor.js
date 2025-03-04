﻿function prepareRefEditor(loadfunction, savefunction) {
	var app = angular.module('mpsapplication', []);
	app.controller('mpscontroller', ["$scope", "$http", function ($scope, $http) {
		$scope.discard = function () {
			window.location.reload();
		};
		$scope.save = function () {
			var prg = StartProgress("Saving data...");
			var refs = [];
			for (var i = 0; i < $scope.refs.length; i++) {
				var ch = $scope.refs[i].changed;
				if (ch) {
					delete $scope.refs[i].changed;
					refs.push($scope.refs[i]);
				}
			}
			$http.post("trservice.asmx/" + savefunction, angular.toJson({ "data": refs }))
				.then(function (response) {
					EndProgress(prg);
					$scope.changed = false;
					$scope.refs.sort(function (a, b) { return a.FORDER - b.FORDER; });
				});
		};

		window[loadfunction]($scope, "refs", $http);
		$scope.$watchCollection('refs', function (newval, oldval) {
			if (!$scope.changed && newval && newval.length > 0) {
				newval.sort(function (a, b) {
					return a.FORDER - b.FORDER;
				});
			}
		});

		$scope.addRef = function (type) {
			var desc = prompt("Please enter new name", "New reference");
			if (desc !== null) {
				var prg = StartProgress("Adding data...");
				$http.post("trservice.asmx/addRef", angular.toJson({ type: type, desc: desc }))
					.then(function () {
						resetReferenceVersion("reload");
						window[loadfunction]($scope, "refs", $http);
						EndProgress(prg);
					});
			}
		};
		$scope.readonly = function () {
			return !IsAdmin() || inProgress();
		};
		$scope.changed = false;
		$scope.itemchanged = function (r) {
			r.changed = true;
			$scope.changed = true;
		};
	}]);
}