package com.condox.rtcHandling.client.wrappers;

import com.google.gwt.core.client.JavaScriptObject;

public interface SDPCreateOfferCallback {
	void RTCSessionDescriptionCallback(RTCSessionDescription sdp);
	void RTCPeerConnectionErrorCallback(JavaScriptObject error);
}
