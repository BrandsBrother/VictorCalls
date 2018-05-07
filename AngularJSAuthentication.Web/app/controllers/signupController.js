'use strict';
app.controller('signupController', ['$scope', '$location', '$timeout', 'authService','usersService', function ($scope, $location, $timeout, authService,usersService) {

    $scope.savedSuccessfully = false;
    $scope.message = "";
    $scope.selectedRole = {};
    $scope.registration = {};

    $scope.registration.companyId = 1;
    $scope.roles = [];

    var text = "";
    usersService.getRoles().then(function (results) {

        $scope.roles = results.data;

    }, function (error) {
        //alert(error.data.message);
    });
    $scope.signUp = function () {
        text = "";
        $scope.registration.userName = $scope.registration.phoneNumber;
        if ($scope.registration.password != $scope.registration.confirmPassword) {
            text = "Password and confirm password do not match";
        }
        else if (isNaN($scope.registration.phoneNumber) && $scope.registration.phoneNumber.length >= 13 && $scope.registration.length < 10)
        {
            text = text + "Phone number is not valid.";
        }
        else {
            $scope.registration.role = $scope.selectedRole;
            authService.saveRegistration($scope.registration).then(function (response) {

                $scope.savedSuccessfully = true;
                //$scope.message = "User has been registered successfully, you will be redicted to login page in 2 seconds.";
                //startTimer();
                $location.path("/users");

            },
             function (response) {
                 var errors = [];
                 for (var key in response.data.modelState) {
                     for (var i = 0; i < response.data.modelState[key].length; i++) {
                         errors.push(response.data.modelState[key][i]);
                     }
                 }
                 $scope.message = "Failed to register user due to:" + errors.join(' ');
             });
        }
        if (text != "")
        {
            $scope.message = text;
        }
    };

    var startTimer = function () {
        var timer = $timeout(function () {
            $timeout.cancel(timer);
            $location.path('/login');
        }, 2000);
    }

}]);