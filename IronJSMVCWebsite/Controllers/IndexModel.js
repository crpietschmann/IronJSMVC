/* Copyright (c) 2011 Chris Pietschmann - http://pietschsoft.com
   All rights reserved
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