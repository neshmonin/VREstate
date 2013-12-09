package com.condox.rtcHandling.client.wrappers;

import com.google.gwt.core.client.JavaScriptObject;

public class GetUserMediaUtils {
	public static native void getUserMedia(boolean audio, boolean video,
			GetUserMedaCallback callback) /*-{
		var cb = function(stream) {						
			callback.@com.condox.rtcHandling.client.wrappers.GetUserMediaUtils.GetUserMedaCallback::navigatorUserMediaSuccessCallback(Lcom/condox/rtcHandling/client/wrappers/MediaStream;)(stream);
		}

		var ecb = function(error) {			
			callback.@com.condox.rtcHandling.client.wrappers.GetUserMediaUtils.GetUserMedaCallback::navigatorUserMediaErrorCallback(Lcom/google/gwt/core/client/JavaScriptObject;)(error);
		}
		try {
			
			navigator.mozGetUserMedia({
				audio : audio,
				video : video
			}, cb, ecb);
		} catch (err) {
			ecb(err);
		}
	}-*/;

	public interface GetUserMedaCallback {
				
		
		public void navigatorUserMediaSuccessCallback(MediaStream localStream);

		public void navigatorUserMediaErrorCallback(JavaScriptObject error);
	}

}
