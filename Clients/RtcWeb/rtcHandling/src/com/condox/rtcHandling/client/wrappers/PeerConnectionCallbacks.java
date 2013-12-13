package com.condox.rtcHandling.client.wrappers;

import com.google.gwt.core.client.JavaScriptObject;

public interface PeerConnectionCallbacks {
	public void onicecandidate(JavaScriptObject jso);
	public void onconnecting(JavaScriptObject jso);
	public void onopen(JavaScriptObject jso);
	public void onaddstream(MediaStream stream);
	public void onremovestream(JavaScriptObject jso);
	public void ondatachannel(JavaScriptObject jso);
}
