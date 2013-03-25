
onMoreInfo = function () {
	alert('Function onMoreInfo exists, calling it.');
	HideVR();
	ShowViewer();
}

window.onload = function () {
	// Show/Hide VirtualReality and BrandFactory window's
	ShowVR = function () {
		document.getElementById("vrContent").className = "vrVisible";
	}
	
	HideVR = function () {
		document.getElementById("vrContent").className = "vrHidden";
	}
	
	ShowViewer = function () {
		document.getElementById("viewerContent").className = "viewerVisible";
	}
	
	HideViewer = function () {
		document.getElementById("viewerContent").className = "viewerHidden";
	}
	// Reset and timeout VirualReality timer
	TimerReset = function () {
		window.frames['vrFrame'].contentWindow.postMessage('reset', '*');
	}
	
	TimerTimeout = function () {
		window.frames['vrFrame'].contentWindow.postMessage('timeout', '*');
	}
	
	function receiveMessage(event) {
		if (event.data == 'reset') {
			if (typeof(onTimerReset == 'function'))
				onTimerReset();
		}
		if (event.data == 'timeout') {
			if (typeof(onTimerTimeout == 'function'))
				onTimerTimeout();
		}
	}
		
	window.addEventListener("message", receiveMessage, false);
	alert('BrandFactory onload');
}
