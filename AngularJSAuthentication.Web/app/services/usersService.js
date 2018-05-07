'use strict';
app.factory('usersService', ['$http', 'ngAuthSettings','authService', function ($http, ngAuthSettings,authService) {

    var serviceBase = ngAuthSettings.apiServiceBaseUri;

    var usersServiceFactory = {};

    var _getUsers = function () {
                return $http.get(serviceBase + 'api/Account/Users?userName=' + authService.authentication.userName).then(function (results) {
            return results;
        });
    };
    var _getRoles = function () {
        return $http.get(serviceBase + 'api/Account/Roles').then(function (results) {
            return results;
        });
    };
    var registration = {};
    var _setListUser = function (user) {
        registration = user;
    };
    var _getListUser = function () {
        return registration;
    }
    usersServiceFactory.getListUser = _getListUser;
    usersServiceFactory.setListUser = _setListUser;
    usersServiceFactory.getUsers = _getUsers;
    usersServiceFactory.getRoles = _getRoles;
    return usersServiceFactory;

}]);