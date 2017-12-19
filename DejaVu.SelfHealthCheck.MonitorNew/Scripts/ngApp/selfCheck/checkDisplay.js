(function () {

    function checkDisplay() {
        return{
            restrict: 'E',
            replace: true,
            templateUrl: 'Scripts/ngApp/selfCheck/templates/checkDisplay.html',
            scope:{ 
                component: '='
            },
            controller: function () {

            }
        }

    }
    angular.module('App').directive('checkDisplay', checkDisplay);
})();