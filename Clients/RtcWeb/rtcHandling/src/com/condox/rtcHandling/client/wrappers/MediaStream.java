package com.condox.rtcHandling.client.wrappers;

import com.google.gwt.core.client.JavaScriptObject;

public class MediaStream extends JavaScriptObject {
	protected MediaStream() {
	}

	public final native String createMediaObjectBlobUrl() /*-{
																			var theInstance=this;
																			return URL.createObjectURL(this);
																			}-*/;

}
