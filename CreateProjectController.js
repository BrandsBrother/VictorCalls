'use strict';
app.Controller('ProjectController', ['$scope','Projectservice'], function ($scope,Projectservice) {

    $scope.project=[];

    Projectservice.getProject().then(function (response) {
       $scope.project= response.project;
    }, function (error) {

    });

});
