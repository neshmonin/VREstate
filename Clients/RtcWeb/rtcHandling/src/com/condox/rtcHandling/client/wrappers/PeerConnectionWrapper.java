package com.condox.rtcHandling.client.wrappers;

import com.google.gwt.core.client.JavaScriptObject;

public class PeerConnectionWrapper {
	JavaScriptObject pc;
	JavaScriptObject dataChannel;
	
	public PeerConnectionWrapper(PeerConnectionCallbacks callbacks, 
			DataChannelCallbacks dataChannelCallbacks,
			boolean initiator) {
		init( callbacks, dataChannelCallbacks, initiator);
	}

	public native void init(PeerConnectionCallbacks callbacks,
			DataChannelCallbacks dataChannelCallbacks,
			boolean initiator) /*-{
			
		var theInstance = this;		
		var pc_config; 	

		var pc_constraints = {
		  'optional': [
		    {'DtlsSrtpKeyAgreement': true},
		    {'RtpDataChannels': true}
		  ]};
		  
  		if (navigator.mozGetUserMedia)
  		{
  			RTCPeerConnection = mozRTCPeerConnection;
  			pc_config = {'iceServers':[{'url':'stun:23.21.150.121'}]};
  		}
  		else if (navigator.webkitGetUserMedia)
  		{
  			RTCPeerConnection = webkitRTCPeerConnection;
  			pc_config = {'iceServers': [{'url': 'stun:stun.l.google.com:19302'}]};
  		}
  
		theInstance.@com.condox.rtcHandling.client.wrappers.PeerConnectionWrapper::pc = new RTCPeerConnection(
				pc_config, pc_constraints);

		var _onicecandidate = function(e) {
			console.log(JSON.stringify(e));
			callbacks.@com.condox.rtcHandling.client.wrappers.PeerConnectionCallbacks::onicecandidate(Lcom/google/gwt/core/client/JavaScriptObject;)(e);
		}
		var _onconnecting = function(e) {
			callbacks.@com.condox.rtcHandling.client.wrappers.PeerConnectionCallbacks::onconnecting(Lcom/google/gwt/core/client/JavaScriptObject;)(e);
		}
		var _onopen = function(e) {
			callbacks.@com.condox.rtcHandling.client.wrappers.PeerConnectionCallbacks::onopen(Lcom/google/gwt/core/client/JavaScriptObject;)(e);
		}
		var _onaddstream = function(e) {					
			callbacks.@com.condox.rtcHandling.client.wrappers.PeerConnectionCallbacks::onaddstream(Lcom/condox/rtcHandling/client/wrappers/MediaStream;)(e.stream);
		}
		var _onremovestream = function(e) {
			callbacks.@com.condox.rtcHandling.client.wrappers.PeerConnectionCallbacks::onremovestream(Lcom/google/gwt/core/client/JavaScriptObject;)(e);
		}
		
		var _ondatachannel = function(e) {
			var receiveChannel = e.channel;
			
			var _dataChannelOnMessage = function(e) {
				console.log("Message received");
				dataChannelCallbacks.@com.condox.rtcHandling.client.wrappers.DataChannelCallbacks::onmessage(Ljava/lang/String;)(e.data);
			}
			
			receiveChannel.onmessage = _dataChannelOnMessage; 
			callbacks.@com.condox.rtcHandling.client.wrappers.PeerConnectionCallbacks::ondatachannel(Lcom/google/gwt/core/client/JavaScriptObject;)(e);
		}

		theInstance.@com.condox.rtcHandling.client.wrappers.PeerConnectionWrapper::pc.onicecandidate = _onicecandidate;
		theInstance.@com.condox.rtcHandling.client.wrappers.PeerConnectionWrapper::pc.onconnecting = _onconnecting;
		theInstance.@com.condox.rtcHandling.client.wrappers.PeerConnectionWrapper::pc.onopen = _onopen;
		theInstance.@com.condox.rtcHandling.client.wrappers.PeerConnectionWrapper::pc.onaddstream = _onaddstream;
		theInstance.@com.condox.rtcHandling.client.wrappers.PeerConnectionWrapper::pc.onremovestream = _onremovestream;
		theInstance.@com.condox.rtcHandling.client.wrappers.PeerConnectionWrapper::pc.ondatachannel  = _ondatachannel;
		
		theInstance.@com.condox.rtcHandling.client.wrappers.PeerConnectionWrapper::dataChannel = theInstance.@com.condox.rtcHandling.client.wrappers.PeerConnectionWrapper::pc.createDataChannel('RTCDataChannel', { reliable: false}  );	    
	    
    	var _dataChannelOnOpen = function(e) {
			dataChannelCallbacks.@com.condox.rtcHandling.client.wrappers.DataChannelCallbacks::onopen(Lcom/google/gwt/core/client/JavaScriptObject;)(e);
		}
		var _dataChannelOnClose = function(e) {
			dataChannelCallbacks.@com.condox.rtcHandling.client.wrappers.DataChannelCallbacks::onclose(Lcom/google/gwt/core/client/JavaScriptObject;)(e);
		}
		var _dataChannelOnError = function(e) {
			dataChannelCallbacks.@com.condox.rtcHandling.client.wrappers.DataChannelCallbacks::onerror(Lcom/google/gwt/core/client/JavaScriptObject;)(e);
		}
		var _dataChannelOnMessage = function(e) {
			console.log("Message received1");
			dataChannelCallbacks.@com.condox.rtcHandling.client.wrappers.DataChannelCallbacks::onmessage(Ljava/lang/String;)(e);
		}
		theInstance.@com.condox.rtcHandling.client.wrappers.PeerConnectionWrapper::dataChannel.onopen = _dataChannelOnOpen;
		theInstance.@com.condox.rtcHandling.client.wrappers.PeerConnectionWrapper::dataChannel.onclose  = _dataChannelOnClose;
		theInstance.@com.condox.rtcHandling.client.wrappers.PeerConnectionWrapper::dataChannel.onerror  = _dataChannelOnError;
		theInstance.@com.condox.rtcHandling.client.wrappers.PeerConnectionWrapper::dataChannel.onmessage  = _dataChannelOnMessage;

	}-*/;

	public native void sendMessage(String message) /*-{
		var theInstance = this;
		theInstance.@com.condox.rtcHandling.client.wrappers.PeerConnectionWrapper::dataChannel.send(message);
	}-*/;
	
	
	public native void createAnswer(SDPOfferMediaConstraints mediaConstraints,
			SDPCreateOfferCallback callback) /*-{
		var theInstance = this;

		var offerCallback = function(event) {
			callback.@com.condox.rtcHandling.client.wrappers.SDPCreateOfferCallback::RTCSessionDescriptionCallback(Lcom/condox/rtcHandling/client/wrappers/RTCSessionDescription;)(event);
		}
		var errorCallback = function(event) {
			callback.@com.condox.rtcHandling.client.wrappers.SDPCreateOfferCallback::RTCPeerConnectionErrorCallback(Lcom/google/gwt/core/client/JavaScriptObject;)(event);
		}

		var mc = mediaConstraints.@com.condox.rtcHandling.client.wrappers.SDPOfferMediaConstraints::asJavaScriptObject(*)();

		theInstance.@com.condox.rtcHandling.client.wrappers.PeerConnectionWrapper::pc
				.createAnswer(offerCallback, errorCallback, mc);
	}-*/;

	public native void createOffer(SDPOfferMediaConstraints mediaConstraints,
			SDPCreateOfferCallback callback) /*-{
		var theInstance = this;

		var offerCallback = function(event) {			
			callback.@com.condox.rtcHandling.client.wrappers.SDPCreateOfferCallback::RTCSessionDescriptionCallback(Lcom/condox/rtcHandling/client/wrappers/RTCSessionDescription;)(event);
		}
		var errorCallback = function(event) {
			callback.@com.condox.rtcHandling.client.wrappers.SDPCreateOfferCallback::RTCPeerConnectionErrorCallback(Lcom/google/gwt/core/client/JavaScriptObject;)(event);
		}

		var mc = mediaConstraints.@com.condox.rtcHandling.client.wrappers.SDPOfferMediaConstraints::asJavaScriptObject(*)();

		var constraints = {'optional': [], 'mandatory': {'MozDontOfferDataChannel': true}};
		
		if (navigator.mozGetUserMedia)
  		{
  			RTCPeerConnection = mozRTCPeerConnection;
  		}
  		else if (navigator.webkitGetUserMedia)
  		{
  			RTCPeerConnection = webkitRTCPeerConnection;
  			for (var prop in constraints.mandatory) 
  			{
      			if (prop.indexOf('Moz') !== -1) 
      			{
        			delete constraints.mandatory[prop];
      			}
     		}
  		}
  		        
        var merged = constraints;
        for (var name in mc.mandatory) {
		    merged.mandatory[name] = mc.mandatory[name];
		  }
		  merged.optional.concat(mc.optional);

		theInstance.@com.condox.rtcHandling.client.wrappers.PeerConnectionWrapper::pc
				.createOffer(offerCallback, errorCallback, mc);
	}-*/;

	
	
	
	public native JavaScriptObject getLocalDescription()/*-{
		var theInstance = this;
		return theInstance.@com.condox.rtcHandling.client.wrappers.PeerConnectionWrapper::pc.localDescription;
	}-*/;

	public native JavaScriptObject getRemoteDescription()/*-{
		var theInstance = this;
		return theInstance.@com.condox.rtcHandling.client.wrappers.PeerConnectionWrapper::pc.remoteDescription;
	}-*/;

	public native void setLocalDescription(JavaScriptObject jso)/*-{
		var theInstance = this;
		theInstance.@com.condox.rtcHandling.client.wrappers.PeerConnectionWrapper::pc
				.setLocalDescription(jso);
	}-*/;

	public native void addStream(JavaScriptObject jso)/*-{
		var theInstance = this;
		console.debug(jso);		
		theInstance.@com.condox.rtcHandling.client.wrappers.PeerConnectionWrapper::pc
				.addStream(jso);
	}-*/;

	public native void setRemoteDescription(JavaScriptObject jso)/*-{
		var theInstance = this;
		if (navigator.mozGetUserMedia)
  		{
  			RTCSessionDescription = mozRTCSessionDescription;
  		}
		theInstance.@com.condox.rtcHandling.client.wrappers.PeerConnectionWrapper::pc
				.setRemoteDescription(new RTCSessionDescription(jso));
	}-*/;

	public native void close()/*-{
		var theInstance = this;		
		theInstance.@com.condox.rtcHandling.client.wrappers.PeerConnectionWrapper::pc.close();		
		
	}-*/;

	public native void dumpPCToConsole() /*-{
		console.debug(this.@com.condox.rtcHandling.client.wrappers.PeerConnectionWrapper::pc);
	}-*/;

	public native void addIceCandidate(JavaScriptObject jso) /*-{
		var theInstance = this;
		
		if (navigator.mozGetUserMedia)
  		{  			
  			theInstance.@com.condox.rtcHandling.client.wrappers.PeerConnectionWrapper::pc
				.addIceCandidate(new mozRTCIceCandidate(jso));
  		}
  		else
  		{
  			theInstance.@com.condox.rtcHandling.client.wrappers.PeerConnectionWrapper::pc
				.addIceCandidate(new webkitRTCIceCandidate (jso));
  		}
  		  		
		
	}-*/;

}
