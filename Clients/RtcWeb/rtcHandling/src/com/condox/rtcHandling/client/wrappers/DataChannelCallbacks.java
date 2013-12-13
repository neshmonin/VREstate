package com.condox.rtcHandling.client.wrappers;

import com.google.gwt.core.client.JavaScriptObject;

public interface DataChannelCallbacks {
	public void onopen(JavaScriptObject event);
	public void onclose(JavaScriptObject event);
	public void onerror(JavaScriptObject event);
	public void onmessage(String event);
	
}
