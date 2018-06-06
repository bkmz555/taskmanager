﻿function enterTT() {
	var ttid = prompt("Please enter TT ID", getParameterByName("ttid"));
	if (ttid != null) {
		window.location = replaceUrlParam(location.href, "ttid", ttid);
		return true;
	}
	return false;
}
$(function () {
	var ttid = getParameterByName("ttid");
	if (ttid == "") {
		while (enterTT() == false) {
		}
	}

	$(document).bind('keydown', function (e) {
		if (e.keyCode == 188 && event.ctrlKey) {
			enterTT();
		}
	});

	var app = angular.module('mpsapplication', []);
	app.filter('getUserById', function () {
		return function (id, $scope) {
			for (i = 0; i < $scope.users.length; i++) {
				if ($scope.users[i].ID == id) {
					return $scope.users[i].FIRSTNAME + " " + $scope.users[i].LASTNAME;
				}
			}
			return "";
		};
	});
	app.filter('getUserImgById', function () {
		return function (id, $scope) {
			for (i = 0; i < $scope.users.length; i++) {
				if ($scope.users[i].ID == id) {
					return $scope.getPersonImg($scope.users[i].EMAIL);
				}
			}
			return "";
		};
	});

	app.controller('mpscontroller', function ($scope, $http, $interval, $window) {

		$window.onbeforeunload = function () {
			$http.post("trservice.asmx/unlocktask", angular.toJson({ "ttid": ttid, "lockid": $scope.currentlock }));
		};

		$scope.currentlock = guid();
		$scope.globallock = "";
		$scope.locktask = function () {
			$http.post("trservice.asmx/locktask", angular.toJson({ "ttid": ttid, "lockid": $scope.currentlock }))
				.then(function (response) {
					$scope.globallock = response.data.d.globallock;
					$scope.lockedby = response.data.d.lockedby;
				});
		}

		$scope.getDispoColor = function () {
			if ($scope.defect && $scope.dispos) {
				var col = $scope.dispos.filter(x => x.ID == $scope.defect.DISPO)[0].COLOR;
				return "background-color: " + col;
			}
			return "";
		}

		$scope.locktask();
		$interval(function () {
			$scope.locktask();
		}, 20000);

		$scope.getPersonImg = function (email) {
			if ($scope.lockedby) {
				return "images/personal/" + email + ".jpg";
			}
			return "";
		}

		$scope.addFile = function () {
			var file = $('<input type="file" name="filefor" style="display: none;" />');
			file.on('input', function (e) {
				var att = { "ID": -1, "FILENAME": this.files[0].name, "ARCHIVE": "", "DATE": "", "SIZE": this.files[0].size, "newfile": this.files[0] };
				$scope.attachs.push(att);
				$scope.changed = true;
				$scope.$apply();
			});
			file.trigger('click');
		}
		$scope.discardDefect = function () {
			window.location.reload();
		}
		$scope.canChangeDefect = function () {
			return ($scope.defect != null) && ($scope.loaders == 0) && ($scope.currentlock == $scope.globallock);
		}
		$scope.loaders = 0;
		$scope.saveDefect = function () {
			//updating object to convert date
			var copy = Object.assign({}, $scope.defect);
			copy.DATE = DateToString(copy.DATE);
			if (!copy.ORDER) {
				copy.ORDER = -1;
			}

			var prgsaving = StartProgress("Saving task..."); $scope.loaders++;

			for (var a = 0; a < $scope.attachs.length; a++) {
				if ($scope.attachs[a].newfile) {
					var filename = $scope.attachs[a].newfile.name;
					var r = new FileReader();
					r.onloadend = function (e) {
						var data = e.target.result;
						var fileupload = StartProgress("Uploading file " + filename + "..."); $scope.loaders++;
						$http.post("trservice.asmx/newfileupload", angular.toJson({ "ttid": ttid, "filename": filename, "data": data }))
							.then(function (response) {
								EndProgress(fileupload); $scope.loaders--;
							});
					}
					r.readAsDataURL($scope.attachs[a].newfile);
				}
			}

			$http.post("trservice.asmx/settask", angular.toJson({ "d": copy }), )
				.then(function (response) {
					EndProgress(prgsaving); $scope.loaders--;
					$scope.changed = false;
					$http.post("trservice.asmx/gettaskhistory", JSON.stringify({ "ttid": ttid }))
						.then(function (result) {
							$scope.history = result.data.d;
						});
					$http.post("trservice.asmx/gettaskevents", JSON.stringify({ "ttid": ttid }))
						.then(function (result) {
							$scope.events = result.data.d;
						});
				});
		}

		//references secion:
		$scope.users = getUsers();
		if (!$scope.users) {
			var prgusers = StartProgress("Loading users..."); $scope.loaders++;
			$scope.users = [];
			$http.post("trservice.asmx/gettaskusers", JSON.stringify({}))
				.then(function (result) {
					$scope.users = result.data.d;
					setUsers($scope.users);
					EndProgress(prgusers); $scope.loaders--;
				});
		}

		$scope.dispos = getDispos();
		if (!$scope.dispos) {
			var prgdispos = StartProgress("Loading dispositions..."); $scope.loaders++;
			$scope.dispos = [];
			$http.post("trservice.asmx/gettaskdispos", JSON.stringify({}))
				.then(function (result) {
					$scope.dispos = result.data.d;
					setDispos($scope.dispos);
					EndProgress(prgdispos); $scope.loaders--;
				});
		}

		$scope.types = getTypes();
		if (!$scope.types) {
			var prgtypes = StartProgress("Loading types..."); $scope.loaders++;
			$scope.types = [];
			$http.post("trservice.asmx/gettasktypes", JSON.stringify({}))
				.then(function (result) {
					$scope.types = result.data.d;
					setTypes($scope.types);
					EndProgress(prgtypes); $scope.loaders--;
				});
		}

		$scope.severs = getSevers();
		if (!$scope.severs) {
			var prgseve = StartProgress("Loading severities..."); $scope.loaders++;
			$scope.severs = [];
			$http.post("trservice.asmx/gettasksevers", JSON.stringify({}))
				.then(function (result) {
					$scope.severs = result.data.d;
					setSevers($scope.severs);
					EndProgress(prgseve); $scope.loaders--;
				});
		}

		$scope.products = getProducts();
		if (!$scope.products) {
			var prgproducts = StartProgress("Loading products..."); $scope.loaders++;
			$scope.products = [];
			$http.post("trservice.asmx/gettaskproducts", JSON.stringify({}))
				.then(function (result) {
					$scope.products = result.data.d;
					setProducts($scope.products);
					EndProgress(prgproducts); $scope.loaders--;
				});
		}

		$scope.priorities = getPriorities();
		if (!$scope.priorities) {
			var prgprio = StartProgress("Loading priorities..."); $scope.loaders++;
			$scope.priorities = [];
			$http.post("trservice.asmx/gettaskpriorities", JSON.stringify({}))
				.then(function (result) {
					$scope.priorities = result.data.d;
					setPriorities($scope.priorities);
					EndProgress(prgprio); $scope.loaders--;
				});
		}

		$scope.comps = getComps();
		if (!$scope.comps) {
			var prgcompo = StartProgress("Loading components..."); $scope.loaders++;
			$scope.comps = [];
			$http.post("trservice.asmx/gettaskcomps", JSON.stringify({}))
				.then(function (result) {
					$scope.comps = result.data.d;
					setComps($scope.comps);
					EndProgress(prgcompo); $scope.loaders--;
				});
		}

		//data section
		var taskprg = StartProgress("Loading task..."); $scope.loaders++;
		$http.post("trservice.asmx/gettask", JSON.stringify({ "ttid": ttid }))
			.then(function (response) {
				$scope.defect = response.data.d;
				if ($scope.defect) {
					$scope.defect.DATE = StringToDate($scope.defect.DATE);
					if ($scope.defect.ORDER == -1) {
						$scope.defect.ORDER = null;
					}
				}
				document.title = "Task: #" + ttid;
				EndProgress(taskprg); $scope.loaders--;
			});

		var prghistory = StartProgress("Loading history..."); $scope.loaders++;
		$scope.history = [];
		$http.post("trservice.asmx/gettaskhistory", JSON.stringify({ "ttid": ttid }))
			.then(function (result) {
				$scope.history = result.data.d;
				EndProgress(prghistory); $scope.loaders--;
			});

		var prgevents = StartProgress("Loading events..."); $scope.loaders++;
		$scope.events = [];
		$http.post("trservice.asmx/gettaskevents", JSON.stringify({ "ttid": ttid }))
			.then(function (result) {
				$scope.events = result.data.d;
				EndProgress(prgevents); $scope.loaders--;
			});

		var prgattach = StartProgress("Loading attachments..."); $scope.loaders++;
		$scope.attachs = [];
		$http.post("trservice.asmx/getattachsbytask", JSON.stringify({ "ttid": ttid }))
			.then(function (result) {
				$scope.attachs = result.data.d;
				EndProgress(prgattach); $scope.loaders--;
			});

		$scope.changed = false;
		$scope.$watchCollection('defect', function (newval, oldval) {
			if (newval && oldval) {
				$scope.changed = true;
			}
		});
	});
})