/*
// IronJSMVC - https://github.com/crpietschmann/IronJSMVC
// Copyright (c) 2011 Chris Pietschmann (http://pietschsoft.com)
// This work is licensed under a Creative Commons Attribution 3.0 United States License, unless explicitly stated otherwise within the posted content.
// http://creativecommons.org/licenses/by/3.0/us/
*/

var Model = (function () {
    var model = function (id, name) {
        this.id = id;
        this.name = name;
    };
    model.prototype = {
        getId: function () {
            return this.id;
        },
        setId: function (v) {
            this.id = v;
            return this;
        },
        getName: function () {
            return this.name;
        },
        setName: function (v) {
            this.name = v;
            return this;
        }
    };
    return model;
})();