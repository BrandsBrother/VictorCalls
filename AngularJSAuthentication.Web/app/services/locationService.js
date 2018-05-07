'use strict';
app.factory('locationService', ['$http', 'ngAuthSettings','authService', function ($http, ngAuthSettings,authService) {

    var serviceBase = ngAuthSettings.apiServiceBaseUri;

    var locationServiceFactory = {};
   
    var _getLocations = function () {
        var config = {
            params: {
                userName: authService.authentication.userName

            }
        };
        return $http.get(serviceBase + 'api/Leads/Locations', config).then(function (results) {
           
            return results;
        });
    };

    locationServiceFactory.getLocations = _getLocations;

    return locationServiceFactory;

}]);