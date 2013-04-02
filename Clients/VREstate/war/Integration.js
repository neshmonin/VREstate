window.onload = function() {

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
}
