"use strict";

import Globalize from "globalize";
import cldrData from "cldr-data";

(function ($, Globalize)
{
    const twoLetterIsoLangugeCode = document.querySelector('html').getAttribute('lang');

    Gloalize.load( require( "cldr-data" ).entireSupplemental() );
    Globalize.load( require( "cldr-data" ).entireMainFor( twoLetterIsoLangugeCode ) );
    Globalize.locale(twoLetterIsoLangugeCode);

}(Globalize));