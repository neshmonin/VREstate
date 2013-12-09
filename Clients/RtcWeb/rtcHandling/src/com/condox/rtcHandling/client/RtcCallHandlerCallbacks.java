package com.condox.rtcHandling.client;

import com.google.gwt.core.client.JavaScriptObject;

public interface RtcCallHandlerCallbacks {	
	public void onChannelClosed();
	public void onChannelStarted(String source);
	public void onDataReceived(String message);
	public void onError(RtcCallHandlerErrorType errorType, Throwable caught, JavaScriptObject jso);
}
