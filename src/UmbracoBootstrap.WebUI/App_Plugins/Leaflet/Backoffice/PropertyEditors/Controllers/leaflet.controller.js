angular.module("umbraco").controller("Leaflet.MapController", ["$scope", "$element", "assetsService", "$timeout", function ($scope, $element, assetsService, $timeout) {
    var defaults = {
        minZoom: 0,
        maxZoom: 18,
        center: [51.505, -0.09],
        imagePath: "/App_Plugins/Leaflet/Assets/Images/",
        urlTemplate: '//{s}.tile.openstreetmap.org/{z}/{x}/{y}.png',
        attribution: '',
        height: 300,
        width: "auto",
        zoom: 13,
    };

    $scope.ready = false;

    if (!angular.isObject($scope.model.value)) {
        $scope.model.value = {
            latLng: {},
            center: {},
            zoom: 0,
        };
    }

    // Remove null values
    for (prop in $scope.model.config)
        if ($scope.model.config[prop] === null)
            delete $scope.model.config[prop];

    var config = angular.extend(defaults, $scope.model.config);

    // Set zoom level from last save
    if ($scope.model.value.zoom >= 0) {
        config.zoom = $scope.model.value.zoom;
    }

    // Set center from saved latlng
    if ($scope.model.value.latLng !== null) {
        if ($scope.model.value.latLng.lat && $scope.model.value.latLng.lng) {
            config.center = $scope.model.value.latLng;
        }
    }
    $scope.style = {
        height: config.height > 0 ? config.height : config.height,
        width: config.width > 0 ? config.width : "auto"
    };

    //$scope.zoomEditor = {
    //    alias: 'Umbraco.Slider',
    //    label: 'Text',
    //    description: 'Load some stuff here',
    //    view: '/umbraco/Views/propertyeditors/slider/slider.html',
    //    config: {
    //        orientation: "horizontal",
    //        initVal1: 0,
    //        initVal2: 18,
    //        minVal: 0,
    //        maxVal: 18,
    //        step: 1,
    //        handle: "round",
    //        enableRange: "1",
    //        tooltipFormat: '{0}:{1}'
    //    },
    //    value: $scope.zoomValue
    //};

    function createMapEditor() {
        $scope.ready = true;
        // create map
        console.log(config);
        var map = L.map($element.find('.leaflet-map')[0], {
            center: config.center,
            zoom: config.zoom,
            minZoom: config.minZoom,
            maxZoom: config.maxZoom,
        });

        L.Icon.Default.imagePath = config.imagePath;

        map.on('zoomend', function () {
            $scope.model.value.zoom = map.getZoom();

            var bounds = map.getBounds();
            $scope.model.value.bounds = {
                southWest: bounds.getSouthWest(),
                northEast: bounds.getNorthEast(),
                northWest: bounds.getNorthWest(),
                southEast: bounds.getSouthEast()
            }
            var center = map.getCenter();
            $scope.model.value.center = {
                lat: center.lat,
                lng: center.lng,
            };
        });

        // Assign tiles layer
        var tileLayer = new L.TileLayer(config.urlTemplate, {
            attribution: config.attribution
        }).addTo(map);

        // create draggable marker
        var marker = createMarker(config.center, map);

        var geosearch = new L.Control.GeoSearch({
            provider: new L.GeoSearch.Provider.Esri(),
            showMarker: true,
            showPopup: true
        }).addTo(map);

        geosearch._positionMarker = marker;
        map.on('geosearch_showlocation', function (event) {
            console.log("Found: ", event);
            $scope.model.value.latLng = {
                lat: event.Location.Y,
                lng: event.Location.X
            };
        });

        $timeout(function () {
            map.invalidateSize();
        }, 300);
    }

    function createMarker(latlng, map) {
        var marker = L.marker(latlng, {
            draggable: true
        }).addTo(map);
        marker.on('dragend', function () {
            var latLng = marker.getLatLng();
            $scope.model.value.latLng = {
                lat: latLng.lat,
                lng: latLng.lng,
            };
        });
        return marker;
    }

    $scope.$watch('model.value', function (newValue, oldValue) {
        console.log(newValue);
    }, true);

    assetsService.load([
            "/App_Plugins/Leaflet/Assets/leaflet.js",
            "/App_Plugins/Leaflet/Assets/plugins/l.geosearch/js/l.control.geosearch.js",
            "/App_Plugins/Leaflet/Assets/plugins/l.geosearch/js/l.geosearch.provider.esri.js",
        ])
        .then(function () {
            //this function will execute when all dependencies have loaded
            createMapEditor();
        });

    assetsService.loadCss("/App_Plugins/Leaflet/Assets/leaflet.css");
    assetsService.loadCss("/App_Plugins/Leaflet/Assets/Plugins/l.geosearch/css/l.geosearch.css");
}]);

