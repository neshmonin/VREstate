package com.condox.rtcHandling.client;


import com.condox.rtcHandling.client.wrappers.DataChannelCallbacks;
import com.condox.rtcHandling.client.wrappers.GetUserMediaUtils;
import com.condox.rtcHandling.client.wrappers.MediaStream;
import com.condox.rtcHandling.client.wrappers.PeerConnectionCallbacks;
import com.condox.rtcHandling.client.wrappers.PeerConnectionWrapper;
import com.condox.rtcHandling.client.wrappers.RTCSessionDescription;
import com.condox.rtcHandling.client.wrappers.SDPCreateOfferCallback;
import com.condox.rtcHandling.client.wrappers.SDPOfferMediaConstraints;
import com.google.gwt.core.client.JavaScriptObject;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.json.client.JSONString;
import com.google.gwt.user.client.rpc.AsyncCallback;


public class RtcCallHandler {
	
	private final class GotSdpCallback implements SDPCreateOfferCallback {
		@Override
		public void RTCSessionDescriptionCallback(RTCSessionDescription sdp) {
			loggingService.debug("PeerConnectionWrapper RTCSessionDescriptionCallback");
			logJso("sdp", sdp);
			peerConnection.setLocalDescription(sdp);					
			sendOutOfBand(getMessage(sdp));								
		}

		@Override
		public void RTCPeerConnectionErrorCallback(JavaScriptObject error) {				
			loggingService.error("PeerConnectionWrapper RTCPeerConnectionErrorCallback");
			logJso("createAnswer-failure", error);
			callbacks.onError(RtcCallHandlerErrorType.FailureToCreateAnswer, null, error);
		}
	}

	private boolean isDataChannelReady;
	private boolean isMediaChannelReady;
	private boolean isCallInitiator;
	private boolean isCallReceiver;
	private boolean isLocalStreamAdded;
	private boolean isChannelReady;
	private MediaStream localStream; // local stream is needed for video call to receive local video
	private PeerConnectionWrapper peerConnection; 
	private MediaStream remoteStream;	
	
	private IMessagingService messagingService;	
	private boolean useVideo = false;
	private RtcCallHandlerCallbacks callbacks;
	private ILoggingService loggingService;
	
	public RtcCallHandler(RtcCallHandlerCallbacks callbacks, IMessagingService greetingService, ILoggingService loggingService)
	{		
		this.messagingService = greetingService;
		this.callbacks = callbacks;
		this.loggingService = loggingService;
		startReceivingOutOfBand();
	}
			
	public void sendData(String data)
	{
		loggingService.debug("sendData is called");
		peerConnection.sendMessage(data);
	}
	
	public void start()
	{
		loggingService.debug("Start is called");
		isCallReceiver = false;
		isCallInitiator = true;
		getUserMedia();				
	}
	
	public void stop()
	{
		loggingService.debug("Stop is called");
		stopImpl();
		sendMessage("bye");
	}


	private void sendMessage(String messageType) {
		JSONObject jso = new JSONObject();		
		jso.put("type", new JSONString(messageType));								
		sendOutOfBand(getMessage(jso));
	}
	
	
	private void stopImpl()
	{
		if (peerConnection != null)
		{
			peerConnection.close();
			peerConnection = null;
		}
		isLocalStreamAdded = false;
		isCallReceiver = false;
		localStream = null;
		isCallInitiator = false;
		isDataChannelReady = false;
		isMediaChannelReady = false;
	}
		
	
	private void getUserMedia() 
	{
		GetUserMediaUtils.getUserMedia(true, useVideo,
				new GetUserMediaUtils.GetUserMedaCallback() {
					@Override
					public void navigatorUserMediaErrorCallback(
							JavaScriptObject error) 
					{
						loggingService.debug("getUserMedia - failure");															
						logJso("getUserMedia - failure", error);
						callbacks.onError(RtcCallHandlerErrorType.FailureToGetUserMedia, null, error);
					}

					@Override
					public void navigatorUserMediaSuccessCallback(
							MediaStream mediaStream) {
						loggingService.debug("getUserMedia - success");	
						isChannelReady = true;
						localStream = mediaStream;	
						//callbacks.onChannelStarted(mediaStream.createMediaObjectBlobUrl());
						if (!isCallReceiver)
						{
							sendMessage("got user media");
						}
						if (isCallReceiver)
						{
							maybeStart();
						}
					}
					
				});
				
	}
	
	private void sendOutOfBand(String message)
	{			
		loggingService.debug("Sending message:" + message);		
		messagingService.SendMessage(message, new AsyncCallback<Void>() {			
			@Override
			public void onSuccess(Void result) {				
			}
			
			@Override
			public void onFailure(Throwable caught) {
				callbacks.onError(RtcCallHandlerErrorType.FailureToSendMessage, caught, null);
			}
		});
	}
	
	private void startReceivingOutOfBand()
	{
		class ReceiveCallback implements AsyncCallback<String>
		{
			@Override
			public void onFailure(Throwable caught) {
				callbacks.onError(RtcCallHandlerErrorType.FailureToReceiveMessage, caught, null);				
			}

			@Override
			public void onSuccess(String result) {
				
				try
				{
					loggingService.debug("Received message:" + result);
					String type = ((JSONString)getJso(result).get("type")).stringValue();																
					
					if (type.equals("got user media"))
					{
						handleGotUserMedia(result);
					}
					else if (type.equals("offer"))
					{
						handleOffer(result);
					}
					else if (type.equals("answer"))
					{
						handleAnswer(result);
					}
					else if (type.equals("iceCandidate"))
					{
						handleAddIceCandidate(result);
					}
					else if (type.equals("bye"))
					{
						handleRemoteHangup(result);	
					}
					else 
					{
						loggingService.error("Message was not handled properly, Message content:" + result);
					}
				}
				finally
				{
					messagingService.ReceiveMessage(new ReceiveCallback());
				}
			}		
		}
				
		messagingService.ReceiveMessage(new ReceiveCallback());
	}
	
	private JSONObject getJso(String message)
	{
		return (JSONObject) (JSONParser.parseLenient((String) message));
	}
	
	private String getMessage(JSONObject jso)
	{
		return jso.toString();
	}
	
	private String getMessage(JavaScriptObject jso)
	{
		JSONObject jsonObject =  (JSONObject) (JSONParser.parseLenient(stringify(jso)));
		return getMessage(jsonObject);
	}
	
	private void handleGotUserMedia(String result) {
		if (!isCallInitiator)
		{
			isCallReceiver = true;
			getUserMedia();	
		}
		else
		{
			callbacks.onError(RtcCallHandlerErrorType.BadCallerState, null, null);
		}
	}
	
	private void handleRemoteHangup(String result) {
		stopImpl();
		callbacks.onChannelClosed();
	}

	private void handleAddIceCandidate(String result) {
		if (isLocalStreamAdded)
		{			
			JSONObject candidate = (JSONObject) getJso(result);								
			peerConnection.addIceCandidate(candidate.getJavaScriptObject());
		}
	}

	private void handleAnswer(String result) {
		if (isLocalStreamAdded)
		{
			JSONObject sdp = (JSONObject)getJso(result);						
			peerConnection.setRemoteDescription(sdp.getJavaScriptObject());
		}
	}
    
	private void logJso(String description, JavaScriptObject jso)
	{
		loggingService.debug(description + "::" + stringify(jso));
	}
	
	private void handleOffer(String result) {
		
		if (!isCallReceiver && !isLocalStreamAdded)
		{
			maybeStart();
		}
		
		JSONObject sdp = (JSONObject)getJso(result);
		JavaScriptObject jso = sdp.getJavaScriptObject();
				
		peerConnection.setRemoteDescription(jso);				
		peerConnection.createAnswer(new SDPOfferMediaConstraints(true, useVideo), new GotSdpCallback());
		
	}
	
	
	private void maybeStart()
	{			
		if (this.localStream != null && !isLocalStreamAdded && isChannelReady)
		{		
			loggingService.debug("Before constructor");
			peerConnection = new PeerConnectionWrapper(new PeerConnectionCallbacks()
			{
				
				@Override
				public void onremovestream(JavaScriptObject jso) {
					loggingService.debug("PeerConnectionWrapper onremovestream");
					logJso("onremovestream", jso);
					isMediaChannelReady = false;
					callbacks.onChannelClosed();
					stopImpl();
				}
				
				@Override
				public void onopen(JavaScriptObject jso) {
					loggingService.debug("PeerConnectionWrapper onopen");
					logJso("onopen", jso);					
				}
				
				@Override
				public void onicecandidate(JavaScriptObject jso) {
					loggingService.debug("PeerConnectionWrapper onicecandidate");
					logJso("onicecandidate", jso);	
					
					JSONObject json = (JSONObject) (JSONParser.parseLenient(stringify(jso)));
					if (json.get("candidate") != null)
					{
						sendOutOfBand(getMessage(jso));
					}
																	
				}
				
				@Override
				public void onconnecting(JavaScriptObject jso) {					
					loggingService.debug("PeerConnectionWrapper onconnecting");
					logJso("onconnecting", jso);
					
				}
				
				@Override
				public void onaddstream(MediaStream jso) {						
					loggingService.debug("PeerConnectionWrapper onaddstream");
					logJso("onaddstream", jso);	
					isMediaChannelReady = true;	
					//if (isDataChannelReady)
					{
						callbacks.onChannelStarted(jso.createMediaObjectBlobUrl());
					}
				}

				@Override
				public void ondatachannel(JavaScriptObject jso) {					
					loggingService.debug("PeerConnectionWrapper ondatachannel");
					logJso("ondatachannel", jso);										
					
				}
			}, new DataChannelCallbacks() {
				
				@Override
				public void onopen(JavaScriptObject event) {						
					loggingService.debug("DataChannel onopen");
					logJso("onopen", event);
					isDataChannelReady = true;
					if (isMediaChannelReady)
					{
						//callbacks.onChannelStarted(jso.createMediaObjectBlobUrl());
					}
				}
				
				
				@Override
				public void onmessage(String event) {				
					loggingService.debug("DataChannel onmessage");									
					callbacks.onDataReceived(event);					
				}
				
				@Override
				public void onerror(JavaScriptObject event) {					
					loggingService.error("DataChannel onerror");
					logJso("onopen", event);
					stopImpl();					
					callbacks.onError(RtcCallHandlerErrorType.DataChannelFailure, null, event);					
				}
				
				@Override
				public void onclose(JavaScriptObject event) {					
					loggingService.debug("DataChannel onclose");
					logJso("onclose", event);					
					isDataChannelReady = false;
					callbacks.onChannelClosed();
				}
			}, isCallReceiver);
			
			
			loggingService.debug("After constructor");
			
			peerConnection.addStream(localStream);
			isLocalStreamAdded = true;
			if (isCallReceiver)
			{
				doCall();
			}
		};
	}

		
	private static native String stringify(JavaScriptObject jso) /*-{
	  return JSON.stringify(jso);
	}-*/;
		
	
	private void doCall() {		
		SDPOfferMediaConstraints constraints = new SDPOfferMediaConstraints(true, useVideo);		
		peerConnection.createOffer(constraints, new GotSdpCallback());
	}

}
