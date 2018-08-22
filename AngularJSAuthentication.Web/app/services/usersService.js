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
    var _getProjects = function () {
        var config = {
            params: {
                userName: authService.authentication.userName

            }
        };
        return $http.get(serviceBase + 'api/Account/Projects',config).then(function (results) {
            return results;
        });
    };
    var _deleteUser = function (userID) {
        return $http.delete(serviceBase + 'api/Account/Users?userID=' + userID).then(function (results) {
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
    usersServiceFactory.deleteUser = _deleteUser;
    usersServiceFactory.getProjects = _getProjects;
    return usersServiceFactory;

}]);