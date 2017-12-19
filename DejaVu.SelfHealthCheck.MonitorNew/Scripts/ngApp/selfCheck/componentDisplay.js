(function () {

    console.log("calling directives");
    function componentDisplay() {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: 'Scripts/ngApp/selfCheck/templates/componentDisplay.html',
            scope:{
                component: "="
            },
            controller: function ($scope) {
                function showProperties() {
                    console.log('name : ' + $scope.controller.AppName);
                    console.log('dependent components: ' + $scope.controller.ChildrenComponents);

                }
            }
        }
    };
    angular.module('App').directive('componentDisplay', componentDisplay);
})()