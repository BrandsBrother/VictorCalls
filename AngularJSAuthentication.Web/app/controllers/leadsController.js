'use strict';
app.controller('leadsController', ['$scope', 'leadsService','$location','$route','$timeout', function ($scope, leadsService,$location,$route,$timeout) {

    $scope.statusID = "";
    var loadTime = 5000,
        errorCount = 0,
        loadPromise;
    $scope.selectedOptions = [];
    $scope.leads = [];
    $scope.assignedLeads = [];
    $scope.selectedLead = leadsService.getSelectedLead();
    leadsService.getLeads().then(function (results) {
      
        $scope.leads = results.data;

    }, function (error) {
        //alert(error.data.message);

    });

    $scope.leadsCount = {};
    $scope.label = leadsService.getLabel();
    var getData = function () {
        leadsService.getLeadsCount().then(function (results) {
            //alert('gum ra na ghum ra');
            $scope.leadsCount = results.data;
            //alert($location.absUrl());
            errorCount = 0;
            nextLoad();
        }, function (error) {
            //alert(error.data.message);
            nextLoad(++errorCount * 2 * loadTime)
        });
    }

    var cancelNextLoad = function () {
        $timeout.cancel(loadPromise);
    }
    var nextLoad = function (mill) {
        mill = mill || loadTime;
        cancelNextLoad();
        loadPromise = $timeout(getData, mill);
    }
    if ($location.absUrl().includes("dashboard")) {
        getData();
    }
    $scope.$on('$destroy', function () {
        cancelNextLoad();
    });

    $scope.updateLeads = function (lead, selectedOptions) {
        //var itemsStored = lead.items;

        lead.items = selectedOptions;
        for (var counter = 0; counter < lead.items.length; counter++)
        {
            lead.items[counter].statusid = 1;
        }
        leadsService.updateLead(lead, $route).then(function (results) {

            leadsService.getLeads().then(function (results) {

                $scope.leads = results.data;

            }, function (error) {
                //alert(error.data.message);

            });
        });
        
        
        // $scope.leads = null;
       // $scope.leads= RefreshData();
        
        //lead.items = itemsStored;
    };
    var RefreshData = function () {
        var waitfunction = leadsService.refreshLeads();
        waitfunction.then(function (results) {

            return results;

        }, function (error) {
            //alert(error.data.message);

        });
    };
    $scope.showDetails = function (lead) {
        leadsService.setSelectedLead(lead);
        $location.path('/displayLeadItems');
    };
    $scope.showLeads=function(statusid, label)
    {
        leadsService.setStatusID(statusid);
        leadsService.setLabel(label);
        $location.path('/Leads');
    };

       
        
   

}]);