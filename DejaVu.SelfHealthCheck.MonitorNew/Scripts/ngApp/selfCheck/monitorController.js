(function () {

    function monitorController($scope, $interval, $rootScope, $filter, componentService) {
        componentService.initialize();
        var checkList = null;
        $scope.components = [];
       
        //var selfCheckHub = backendHubProxy("HealthMessageHub");

        //selfCheckHub.on("paymentReady", function (data) {

        //});

        //selfCheckHub.invoke("sendPayment", function (data) {

        //});
        
        $scope.year = new Date().getUTCFullYear();

        $scope.allComponents = function () {
            componentService.healthMessage();
        }

        $scope.$on("hubConnected", function (e) {
            console.log("hubConnected event received");
            componentService.healthMessage();
            // componentService.values();
        });

  
        $scope.$on("checks", function (e, message, aggregatedStatus) {
           debug;
            console.log("check  " + message.AppID);
            console.log($scope.components);
            

            var theComponent = $filter('filter')($scope.components, { AppID: message.AppID });
            console.log("at zero " + theComponent[0].AppID);
            var index = $scope.components.indexOf(theComponent[0]);
            console.log('Index ' + index);
            //console.log("the Component " + $scope.components[index].AppID);
            console.log('Current Status : ' + $scope.components[index].TheStatus + " ---------- Changed Status " + aggregatedStatus)
            console.log($scope.components[index].replicas);
            $scope.components[index].TheStatus = aggregatedStatus;

            if (message.IPAddress != null) {
                if ($scope.components[index].replicas != null) {
                    var theReplica = $filter('filter')($scope.components[index].replicas, { replicaIP: message.IPAddress });

                    if (theReplica == null) {
                        var replica = { replicaStatus: message.OverallStatus, replicaIP: message.IPAddress };
                        $scope.components[index].replicas.push(replica);
                    }
                    else {
                        var replicaIndex = $scope.components[index].replicas.indexOf(theReplica);
                        $scope.components[index].replicas[replicaIndex].replicaStatus = message.OverallStatus;
                    }
                } else {
                    var replicaComponents = [];
                    var replica = { replicaStatus: message.OverallStatus, replicaIP: message.IPAddress };
                    replicaComponents.push(replica);
                    $scope.components[index]["replicas"] = replicaComponents;
                }
            }
            
            $scope.$apply();
        });

        $scope.$on("reports", function (e, appId) {
            console.log("getting reports");
            console.log("reports: " + appId);
        });


        $scope.$on("allApplications", function (e, app) {
            console.log("application data received");
            if (app.replicas != null) {
                console.log(app.replicas);
            }

          
            $scope.components.push(app);

            $scope.$apply();
        });
        
        $scope.selected = null;

        $scope.select = function (component) {
            if ($scope.selected === component) {
                $scope.selected = null;
                $scope.componentName = null;
                $scope.children = component.null;
                $scope.replicaComponents = null
                $scope.checks = null;
            }
            else {
                console.log("component Clicked");
                console.log(component.replicas);
                $scope.selected = component;
                $scope.componentName = component.AppName;
                $scope.children = component.ChildrenComponents;
                $scope.replicaComponents = component.replicas;
                console.log("replicas added");
                console.log(components.replicas);
                $scope.checks = getCheckForApp(checkList, component.AppID);
            }

            function getCheckForApp(checkList, appId) {
                var valueCheck = [];
                console.log('App Id being passed')
                console.log(appId);
                console.log("initialised replicas");
                console.log( $scope.replicaComponents);

                var values = _.values(checkList);
                valueitem = _.filter(values, function (item) {
                    console.log(item.id)
                    return item.id == appId;
                });

                console.log(valueitem);
                return valueitem
                // console.log(checkList);

            }



        };
        $scope.isActive = function (component) {
            return $scope.selected === component;
        }

    };
    angular.module('App').controller('monitorController', monitorController);
})();