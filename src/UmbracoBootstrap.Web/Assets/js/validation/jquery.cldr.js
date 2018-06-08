(function ($, Globalize) {
    var cultureIdentifier = $("html").prop('lang');

    $.when(
        $.get("/node_modules/cldr-data/main/" + cultureIdentifier + "/ca-gregorian.json"),
        $.get("/node_modules/cldr-data/main/" + cultureIdentifier + "/numbers.json"),
        $.get("/node_modules/cldr-data/supplemental/likelySubtags.json"),
        $.get("/node_modules/cldr-data/supplemental/numberingSystems.json"),
        $.get("/node_modules/cldr-data/supplemental/timeData.json"),
        $.get("/node_modules/cldr-data/supplemental/weekData.json")
    ).then(function () {

        // Normalize $.get results, we only need the JSON, not the request statuses.
        return [].slice.apply(arguments, [0]).map(function (result) {
            return result[0];
        });

    }).then(Globalize.load).then(function () {
        Globalize.locale(cultureIdentifier);
        // Your code goes here.
    });
}(jQuery, Globalize));