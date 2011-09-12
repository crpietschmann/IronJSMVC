// Copyright (c) 2011 Chris Pietschmann - http://pietschsoft.com
// All rights reserved

//include("includes/date");
include("IndexModel");

(function () {
    return {
        _Layout: 'layout',
        Index: function (id) {
            //this._Layout = 'layout2'; // Set the Layout to use for this Action method
            var model = new Model(id, 'Chris');
            return View(model);
        },
        Index2: function (id) {
            var model = {
                id: id,
                name: 'Fredrik'
            };
            return View(['AlternateIndex', model]);
        },
        Index3: function (id) {
            var model = {
                id: id,
                name: 'Chris' // Date.parse("yesterday").toString() + " -- " + Date.parse("today").toString() // Set name to something from date.js
            };
            return View([model]);
        },
        Index4: function (id) {
            return View();
        },
        Index5: function () {
            return View(['Index4', null]);
        },
        Index6: function (id) {
            return id;
        }
    };
})();