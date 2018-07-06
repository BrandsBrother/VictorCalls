'use strict';
app.factory('leadsService', ['$http', 'ngAuthSettings','authService', function ($http, ngAuthSettings,authService) {

    var serviceBase = ngAuthSettings.apiServiceBaseUri;

    var leadServiceFactory = {};
    var selectedLead = {};

    var _getLeadsCount = function () {
        //alert(authService.authentication.userName);
        return $http.get(serviceBase + 'api/Leads/LeadStatusCounts', { params: {userName:authService.authentication.userName}}).then(function (results) {
            return results;
        });
    };
   
    var _getLeads = function () {
        var config = {
            params: {
                userName: authService.authentication.userName,
                statusid: statusID
            }

        };
        if (statusID != 16) {
            return $http.get(serviceBase + 'api/Leads/Company', config).success(function (results) {
                return results;
            });
        }
        else {
            return $http.get(serviceBase + 'api/Leads/Company/RawLeads', config).success(function (results) {
                return results;
            });
        }
    };
    var _refreshLeads = function () {
        var config = {
            params: {
                userName: authService.authentication.userName,
                statusid: statusID
            }
            
        };
        if (statusID != 16) {
            return $http.get(serviceBase + 'api/Leads/Company', config).success(function (results) {
              return results;
            });
        }
        else {
            return $http.get(serviceBase + 'api/Leads/Company/RawLeads', config).success(function (results) {
               return results;
            });
        }
    };

    var _leads = function (updateLead,$route) {
        var completed = false;
        var config = { headers: { "Content-type": "application/JSON" } };
       return $http.put(serviceBase + 'api/Leads/7', updateLead).then(function (response) {
            completed = true;
           // $route.reload();
            return response;
            
        });
     

    };
    var _setLead = function (lead)
    {
        selectedLead = lead;
    };
    var _getLead = function () {
        return selectedLead;
    };
    var statusID;
    var _setStatusID = function (statusid) {
        statusID = statusid;
    };
    var label;
    var _setLabel = function (Label) {
        label = Label;
    };
    var _getLabel = function () {
        return label;
    };
    leadServiceFactory.refreshLeads = _refreshLeads;
    leadServiceFactory.setLabel = _setLabel;
    leadServiceFactory.getLabel = _getLabel;
    leadServiceFactory.setStatusID = _setStatusID;
    leadServiceFactory.getSelectedLead = _getLead;
    leadServiceFactory.setSelectedLead = _setLead;
    leadServiceFactory.getLeadsCount = _getLeadsCount;
    leadServiceFactory.getLeads = _getLeads;
    leadServiceFactory.updateLead = _leads;

    return leadServiceFactory;

}]);