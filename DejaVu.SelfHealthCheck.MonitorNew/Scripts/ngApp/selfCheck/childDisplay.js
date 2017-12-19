
(function () {
    function childDisplay() {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: 'Scripts/ngApp/selfCheck/templates/childDisplay.html',
            scope: {
                component: '='
            },
            controller: function () {

            }
        }
    }
angular.module('App').directive('childDisplay',childDisplay);
})()