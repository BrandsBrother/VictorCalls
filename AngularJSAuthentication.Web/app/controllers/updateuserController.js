'use strict';
app.controller('updateuserController', ['$scope', '$location', '$timeout', 'authService', function ($scope, $location, $timeout, authService) {

    $scope.savedSuccessfully = false;
    $scope.message = "";

    $scope.user = {
        //userName: "",
        //password: "",
        //confirmPassword: "",
        //companyId: "",
        //accessFailedCount: "",
        //email: "",
        //emailConfirmed: "",
        //id: "",
        //lockOutEndDateUtc: "",
        //lockOutEnabledDateUtc: "",
        //phoneNumber: "",
        //phoneNumberConfirmed: "",
        //securityStamp: "",
        //twoFactorEnabled: "",
        //firstName: "",
        //lastName: "",
        //role: {
        //    id: "",
        //    name: ""
        //}

    };

    $scope.updateuser = function () {

        authService.updateUser($scope.user).then(function (response) {

            $scope.savedSuccessfully = true;
            // $scope.message = "User has been registered successfully, you will be redicted to login page in 2 seconds.";
            startTimer();

        },
         function (response) {
             var errors = [];
             for (var key in response.data.modelState) {
                 for (var i = 0; i < response.data.modelState[key].length; i++) {
                     errors.push(response.data.modelState[key][i]);
                 }
             }
             $scope.message = "Failed to Update user due to:" + errors.join(' ');
         });
    };

    var startTimer = function () {
        var timer = $timeout(function () {
            $timeout.cancel(timer);
            // $location.path('/login');
        }, 2000);
    }

}]);