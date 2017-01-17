document.addEventListener("DOMNodeInserted", function(event) {
	if (!!window && !(!!window.$)) {
		window.$ = window.jQuery = require('./jquery-3.1.0.min.js');
	}
});
