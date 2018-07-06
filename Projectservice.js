'use strict';
app.factory('projectservice', ['$http', 'ngAuthSettings', 'authService', function ($http, ngAuthSetting, authService) {

    var serviceBase = ngAuthSetting.apiServiceBaseuri;
    var projectserviveFactory = {};

    var _getProject = function () {
        var config = {
            params: {
                projectId: 1
            }
        }

    };

   return $http.get(serviceBase + 'api/project/Id', config).then(function (response) {
        return response;
    });
}]);