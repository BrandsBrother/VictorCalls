
var app = angular.module('AngularAuthApp', ['ngRoute', 'LocalStorageModule', 'angular-loading-bar']);

app.config(function ($routeProvider) {

    $routeProvider.when("/home", {
        controller: "homeController",
        templateUrl: "/app/views/home.html"
    });

    $routeProvider.when("/login", {
        controller: "loginController",
        templateUrl: "/app/views/login.html"
    });

    $routeProvider.when("/signup", {
        controller: "signupController",
        templateUrl: "/app/views/signup.html"
    });

    $routeProvider.when("/orders", {
        controller: "ordersController",
        templateUrl: "/app/views/orders.html"
    });

    $routeProvider.when("/refresh", {
        controller: "refreshController",
        templateUrl: "/app/views/refresh.html"
    });

    $routeProvider.when("/tokens", {
        controller: "tokensManagerController",
        templateUrl: "/app/views/tokens.html"
    });

    $routeProvider.when("/associate", {
        controller: "associateController",
        templateUrl: "/app/views/associate.html"
    });
    $routeProvider.when("/index", {
        controller: "indexController",
        templateUrl: "/app/views/dashboard.html",
        
    });

    $routeProvider.otherwise({ redirectTo: "/login" });

});

var serviceBase = 'http://api.victorcalls.com/';
//var serviceBase = 'http://ngauthenticationapi.azurewebsites.net/';
app.constant('ngAuthSettings', {
    apiServiceBaseUri: serviceBase,
    clientId: 'ngAuthApp'
});

app.config(function ($httpProvider) {
    $httpProvider.interceptors.push('authInterceptorService');
});

app.run(['authService','$location', function (authService,$location) {
    
  
    authService.fillAuthData();
   
    //if (!authService.authentication.isAuth) {
        
    //    window.location = 'http://localhost:32150/Page1.html';
    //}
    ////if ($location.path()!='' &&  !authService.authentication.isAuth)
    ////{
    ////    $location.path('/index');
    ////}
}]);

