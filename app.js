
//Conversation opened. 1 unread message.

//Skip to content
//Using Gmail with screen readers

//More 
//28 of 20,596
 
//app.js 
//Inbox
//x 

//Gaurav Bhardwaj <vedagya19@gmail.com>
//5:24 PM (19 hours ago)
//to me 

var app = angular.module('AngularAuthApp', ['ngRoute', 'LocalStorageModule', 'angular-loading-bar']);

app.config(function ($routeProvider) {

    $routeProvider.when("/home", {
        controller: "leadsController",
        templateUrl: "/app/views/dashboard.html"
    });
    $routeProvider.when("/AddUser", {
        controller: "signupController",
        templateUrl: "/app/views/signup.html"
    });

    $routeProvider.when("/updateuser", {
        controller: "userController",
        templateUrl: "/app/views/updateuser.html"
    });

    $routeProvider.when("/login", {
        controller: "loginController",
        templateUrl: "/app/views/login.html"
    });

   

    $routeProvider.when("/users", {
        controller: "userController",
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
    $routeProvider.when("/dashboard", {
        controller: "leadsController",
        templateUrl: "/app/views/dashboard.html",

    });
<<<<<<< HEAD

    $routeProvider.when("/CreateProject", {
        controller: "CreateProjectController",
        templateUrl:"/app/views/CreateProject.html",
    });
=======
    $routeProvider.when("/Leads", {
        controller: "leadsController",
        templateUrl: "/app/views/Leads.html",

    });
    $routeProvider.when("/displayLeadItems", {
        controller: "leadsController",
        templateUrl: "/app/views/leadItems.html",

    });
    $routeProvider.when("/locations", {
        controller: "locationController",
        templateUrl: "/app/views/Location.html",

    });
    
>>>>>>> origin/master

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

	
//Click here to Reply or Forward
//1.89 GB (12%) of 15 GB used
//Manage
//Terms - Privacy
//Last account activity: 0 minutes ago
//Details

