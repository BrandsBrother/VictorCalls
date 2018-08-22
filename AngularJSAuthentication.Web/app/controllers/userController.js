'use strict';
app.controller('userController', ['$scope', 'usersService', '$timeout', '$location', 'authService', function ($scope, usersService, $timeout, $location, authService) {

    $scope.users = [];
    $scope.roles = [];
    $scope.Projects = [];
    usersService.getProjects().then(function (results) {

        $scope.Projects = results.data;

    }, function (error) {
        //alert(error.data.message);
    });
    usersService.getUsers().then(function (results) {

        $scope.users = results.data;

    }, function (error) {
        //alert(error.data.message);
        });

    usersService.getRoles().then(function (results) {

        $scope.roles = results.data;

    }, function (error) {
        //alert(error.data.message);
    });
    $scope.savedSuccessfully = false;
    $scope.message = "";
    // $scope.confirmEmail = "";
    // $scope.confirmPhoneNumber = "";

    $scope.registration = usersService.getListUser();

    $scope.selectedRole = $scope.registration.role;
    $scope.selectedProject = $scope.registration.project;
    $scope.updateuser = function (_registration) {
        _registration.userName = _registration.phoneNumber;
        // _registration.phoneNumberConfirmed = _registration.phoneNumber == $scope.confirmPhoneNumber ? true :false;


        usersService.setListUser(_registration);
        $location.path("/updateuser");

        //authService.updateuser($scope.registration).then(function (response) {

        //    $scope.savedSuccessfully = true;
        //    //$scope.message = "User has been registered successfully, you will be redicted to login page in 2 seconds.";
        //    //startTimer();

        //},
        // function (response) {
        //     var errors = [];
        //     for (var key in response.data.modelState) {
        //         for (var i = 0; i < response.data.modelState[key].length; i++) {
        //             errors.push(response.data.modelState[key][i]);
        //         }
        //     }
        //     $scope.message = "Failed to register user due to:" + errors.join(' ');
        // });
    };

    $scope.createUser = function () {
        $location.path("/AddUser");
    };
    $scope.deleteUser = function (user)
    {
        if (confirm('Are you sure you want to delete user ?')) {
            if (confirm('Really?')) {
                usersService.deleteUser(user.id);
                usersService.getUsers().then(function (results) {

                    $scope.users = results.data;

                }, function (error) {
                    //alert(error.data.message);
                });
            }
        }
        
    };
    var text = "";
    $scope.updateUser = function (registration) {
        $scope.message = "";
        registration.userName = registration.phoneNumber;
        $scope.registration.role = $scope.selectedRole;
        $scope.registration.project = $scope.selectedProject;
        authService.updateUser(registration).then(function (response) {

            $scope.savedSuccessfully = true;
            $scope.message = "";
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
             //$scope.message = "Failed to Update user due to:" + errors.join(' ');
         });

    };

}]);