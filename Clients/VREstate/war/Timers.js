TimersReset = function() {
	window.postMessage('reset', '*');
	window.frames['vrFrame'].contentWindow.postMessage('reset', '*');
}

TimersTimeout = function() {
	window.postMessage('timeout', '*');
	window.frames['vrFrame'].contentWindow.postMessage('timeout', '*');
	parent.parent.postMessage('timeout', '*');
}

receiveMessage = function(event) {
	// alert(event.data + ' - ' + event.source);
	if (event.data == 'reset') {
		if (typeof (onTimerReset == 'function'))
			onTimerReset();
	}
	if (event.data == 'timeout') {
		if (typeof (onTimerTimeout == 'function'))
			onTimerTimeout();
	}
}