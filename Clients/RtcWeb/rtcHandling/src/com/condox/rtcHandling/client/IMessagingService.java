package com.condox.rtcHandling.client;

import com.google.gwt.user.client.rpc.AsyncCallback;

public interface IMessagingService {
	public void SendMessage(String message, AsyncCallback<Void> callback);
	public void ReceiveMessage(AsyncCallback<String> callback);
}
