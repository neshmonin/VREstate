window.onload = function() {
	HOST_TimerExpire = function(expired) {
		// var data = {};
		// data.name = "TimerExpire";
		// data.value = expired;
		// var message = JSON.stringify(data);
		var message = '';
		if (expired)
			message = 'timeout';
		else
			message = 'reset';
		document.getElementById(VRT_FrameId).contentWindow.postMessage(message,
				'*');
	}

	OnMessage = function(event) {
		if (typeof (VRT_TimerExpire) == 'function') {
			if (event.data == 'reset')
				VRT_TimerExpire(false);
			if (event.data == 'timeout')
				VRT_TimerExpire(true);
		}
	}

    function Init () {
        if (window.addEventListener) {  // all browsers except IE before version 9
            window.addEventListener ("message", OnMessage, false);
        }
        else {
            if (window.attachEvent) {   // IE before version 9
                window.attachEvent("onmessage", OnMessage);
            }
        }
    }
    Init();
//	alert('BrandFactory onload');
}
