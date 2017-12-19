(function () {
    function componentService($rootScope,$interval) {
        var healthMessage = null;

        var _initialize = function () {
            console.log("Initializing hub");
            healthMessage = $.connection.healthMessageHub;
            //var _connection = healthMessage.
            $.connection.hub.start().done(function () {
                console.log("server connected");
                $rootScope.$broadcast("hubConnected");
            });
         
            healthMessage.client.test = function ()
            {
                console.log(" hello test function "); 

            };
            healthMessage.client.sendHealthStatus = function (message,aggregatedStatus) {
                console.log("Received a message");
                console.log(healthmessage);
                console.log("Overall Status " + aggregatedStatus);
                
                $rootScope.$broadcast("checks", message, aggregatedStatus);
            }

            //load all apps and their replicas
            healthMessage.client.loadAllApllications = function (app, replicaIPs, replicaStatus) {
                var replicaComponents = [];
                var appJson = JSON.parse(app);
                if (replicaIPs != null) {
                    
                    var IPs = JSON.parse(replicaIPs);

                    for (i = 0; i < IPs.length; i++) {
                      var  replica = { replicaStatus: replicaStatus[i], replicaIP: IPs[i] };
                        replicaComponents.push(replica);

                    }
                    appJson["replicas"] = replicaComponents;

                }
              
                $rootScope.$broadcast("allApplications", appJson);

                                               
            }

           

             healthMessage.client.reportAppStatus = function (appID, aggStatus) {
                console.log('App Status' + 'AppId:' + appID + 'STATUS' + aggStatus);
                $rootScope.$broadcast("reports", appID);
               // updateApplicationDiv(appID, aggStatus, null, false);
            };
        }

        var _sendRequest = function () {
            //Invoking greetAll method defined in hub
            healthMessage.server.loadAllApplications();
            console.log("calling LoadApp");
        }

        var _checkValues = function () {
            //console.log("Requesting for Health Status");
            //healthMessage.server.getHealthStatusOfApps();
           // console.log("Requesting for Health Status2");

        }

       
        return {
            initialize: _initialize,
            healthMessage: _sendRequest,
            values : _checkValues
        }
    }
    angular.module('App').factory('componentService', componentService);
})()
