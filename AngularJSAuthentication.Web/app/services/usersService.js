'use strict';
app.factory('usersService', ['$http', 'ngAuthSettings','authService', function ($http, ngAuthSettings,authService) {

    var serviceBase = ngAuthSettings.apiServiceBaseUri;

    var usersServiceFactory = {};

    var _getUsers = function () {
        alert('yes i m called');
        return $http.get(serviceBase + 'api/Account/Users?userName=' + authService.authentication.userName).then(function (results) {
            return results;
        });
    };

    usersServiceFactory.getUsers = _getUsers;

    return usersServiceFactory;

}]);