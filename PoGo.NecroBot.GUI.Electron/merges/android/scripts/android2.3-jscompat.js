// Polyfill pour la prise en charge de Function.prototype.bind() sur Android 2.3
(function () {
    if (!Function.prototype.bind) {
        Function.prototype.bind = function (thisValue) {
            if (typeof this !== "function") {
                throw new TypeError(this + " cannot be bound as it is not a function");
            }

            // bind() permet également d'ajouter des arguments au début de l'appel
            var preArgs = Array.prototype.slice.call(arguments, 1);

            // Fonction réelle à laquelle lier la valeur et les arguments "this"
            var functionToBind = this;
            var noOpFunction = function () { };

            // Argument "this" à utiliser
            var thisArg = this instanceof noOpFunction && thisValue ? this : thisValue;

            // Fonction liée résultante
            var boundFunction = function () {
                return functionToBind.apply(thisArg, preArgs.concat(Array.prototype.slice.call(arguments)));
            };

            noOpFunction.prototype = this.prototype;
            boundFunction.prototype = new noOpFunction();

            return boundFunction;
        };
    }
}());
