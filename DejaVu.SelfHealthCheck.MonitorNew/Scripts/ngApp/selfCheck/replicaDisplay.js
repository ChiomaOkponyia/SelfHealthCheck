(function () {

    console.log("calling directives");
    function replicaDisplay() {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: 'Scripts/ngApp/selfCheck/templates/replicaDisplay.html',
            scope: {
                replica: "="
            },
            controller: function () {
                //function showProperties() {
                //    console.log('IP : ' + $scope.controller.replicaIP);
                //    console.log('Status: ' + $scope.controller.replicaStatus);

                //}
            }
        }
    }
    angular.module('App').directive('replicaDisplay', replicaDisplay);
})()