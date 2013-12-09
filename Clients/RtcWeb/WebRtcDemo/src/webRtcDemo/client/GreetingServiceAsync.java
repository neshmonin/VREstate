package webRtcDemo.client;

import com.google.gwt.user.client.rpc.AsyncCallback;

/**
 * The async counterpart of <code>GreetingService</code>.
 */
public interface GreetingServiceAsync {
	void Send(String userName, Message message, AsyncCallback<Void> callback);

	void Receive(String userName, AsyncCallback<Message> callback);
	
}
