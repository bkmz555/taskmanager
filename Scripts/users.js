﻿$(function () {
	var app = angular.module('mpsapplication', []);
	app.filter('passwordFilter', function () {
		return function (input) {
			var split = input.split('');
			var result = "";
			for (var i = 0; i < split.length; i++) {
				result += "*";
			}
			return result;
		};
	});
	app.controller('mpscontroller', ["$scope", "$http", function ($scope, $http) {
		$scope.readonly = !IsAdmin();
		$scope.discard = function () {
			window.location.reload();
		};
		$scope.changeImg = function (id) {
			var file = $('<input type="file" name="filefor" style="display: none;" />');
			file.on('input', function (e) {
				var r = new FileReader();
				r.attfilename = this.files[0].name;
				r.onloadend = function (e) {
					for (var i = 0; i < $scope.users.length; i++) {
						if ($scope.users[i].ID === id) {
							$scope.users[i].IMGTRANSFER = e.target.result;
							$scope.users[i].changed = true;
							$scope.changed = true;
							$scope.$apply();
							break;
						}
					}
				};
				r.readAsDataURL(this.files[0]);
			});
			file.trigger('click');
		};
		$scope.save = function () {
			var prg = StartProgress("Saving data...");
			var users = [];
			for (var i = 0; i < $scope.users.length; i++) {
				var u = $scope.users[i];
				var ch = u.changed;
				if (ch) {
					delete u.changed;
					var uexport = Object.assign({}, u);
					uexport.BIRTHDAY = DateToString(uexport.BIRTHDAY);
					users.push(uexport);
				}
			}
			$http.post("trservice.asmx/setusers", angular.toJson({ "users": users }))
				.then(function (response) {
					EndProgress(prg);
					$scope.changed = false;
				});
		};

		var taskprg = StartProgress("Loading data...");
		$scope.filter = true;
		$scope.users = [];
		$http.post("trservice.asmx/getMPSUsers", JSON.stringify({ "active": false }))
			.then(function (result) {
				$scope.users = result.data.d;
				$scope.users.sort(function (x, y) {
					return (x.RETIRED === y.RETIRED) ? (x.PERSON_NAME.localeCompare(y.PERSON_NAME)) : (x.RETIRED ? 1 : -1);
				});
				$scope.users.forEach(function (u) {
					if (u.BIRTHDAY !== "") {
						u.BIRTHDAY = StringToDate(u.BIRTHDAY);
					}
				});
			});
		$scope.newUser = function () {
			$http.post("UsersService.asmx/newUser", JSON.stringify({}))
				.then(function (result) {
					var u = result.data.d;
					if (u) {
						if (u.BIRTHDAY !== "") {
							u.BIRTHDAY = StringToDate(u.BIRTHDAY);
						}
						$scope.users.unshift(u);
					}
				});
		};
		$scope.changed = false;
		$scope.itemchanged = function (r) {
			r.changed = true;
			$scope.changed = true;
		};
		EndProgress(taskprg);
	}]);
})