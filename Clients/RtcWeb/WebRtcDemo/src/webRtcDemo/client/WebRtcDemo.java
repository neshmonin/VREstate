package webRtcDemo.client;


import java.util.Date;

import webRtcDemo.client.wrappers.Utils;

import com.condox.rtcHandling.client.ILoggingService;
import com.condox.rtcHandling.client.IMessagingService;
import com.condox.rtcHandling.client.RtcCallHandler;
import com.condox.rtcHandling.client.RtcCallHandlerCallbacks;
import com.condox.rtcHandling.client.RtcCallHandlerErrorType;
import com.google.gwt.core.client.EntryPoint;
import com.google.gwt.core.client.GWT;
import com.google.gwt.core.client.JavaScriptObject;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.event.dom.client.ClickHandler;
import com.google.gwt.media.client.Video;
import com.google.gwt.user.client.rpc.AsyncCallback;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.RootPanel;
import com.google.gwt.user.client.ui.TextBox;

/**
 * Entry point classes define <code>onModuleLoad()</code>.
 */
public class WebRtcDemo implements EntryPoint {
	/**
	 * Create a remote service proxy to talk to the server-side Greeting service.
	 */
	private final GreetingServiceAsync greetingService = GWT
			.create(GreetingService.class);

	
	/**
	 * This is the entry point method.
	 */
	public void onModuleLoad() {
		
		GWT.setUncaughtExceptionHandler(new GWT.UncaughtExceptionHandler() {			
			@Override
			public void onUncaughtException(Throwable e) {				
				Utils.consoleError("Unhandled exception:" + e.toString());					
			}		
		});
		
				
		final TextBox sendMessageTextBox = new TextBox();
		RootPanel.get("chatTextAreaSendContainer").add(sendMessageTextBox);
		
		final TextBox receiveMessageTextBox = new TextBox();
		RootPanel.get("chatTextAreaReceiveContainer").add(receiveMessageTextBox);
		
		final Button sendMessageButton = new Button("Send");
		sendMessageButton.setEnabled(false);
		
				
		RootPanel.get("sendMessageButtonContainer").add(sendMessageButton);
		
		final Button startAudioButton = new Button("Start");
		RootPanel.get("startAudioButtonContainer").add(startAudioButton);
						
		final Button stopAudioButton = new Button("Stop");
		stopAudioButton.setEnabled(false);
		RootPanel.get("stopAudioButtonContainer").add(stopAudioButton);
		
		
		final Video remoteVideo = Video.createIfSupported();
		RootPanel.get("remoteVideoContainer").add(remoteVideo);
						
			
		class RtcCallHandlerCallbacksImpl implements RtcCallHandlerCallbacks {
			
			public RtcCallHandlerCallbacksImpl()
			{}
						
			
			@Override
			public void onChannelClosed() {
				startAudioButton.setEnabled(true);				
				stopAudioButton.setEnabled(false);
				remoteVideo.setSrc(null);
				sendMessageButton.setEnabled(false);
			}
			
			@Override
			public void onChannelStarted(String source) {
				remoteVideo.addSource(source);
				remoteVideo.play();
				sendMessageButton.setEnabled(true);
				stopAudioButton.setEnabled(true);
			}
			
			@Override
			public void onDataReceived(String message) {
				receiveMessageTextBox.setText(message);
			}
						

			@Override
			public void onError(RtcCallHandlerErrorType errorType,
					Throwable caught, JavaScriptObject jso) {				
				
			}
		};
				
		
		final String userName = String.valueOf(new Date().getTime());
		Utils.consoleDebug("Unique username:" + userName);
			
		
		final RtcCallHandler handler =  new RtcCallHandler(new RtcCallHandlerCallbacksImpl(), 
				new MessagingService(userName), 
				new LoggingService());
		
		sendMessageButton.addClickHandler(new ClickHandler() {
			
			@Override
			public void onClick(ClickEvent event) {
				handler.sendData(sendMessageTextBox.getText());						
			}
		});
		
		
		startAudioButton.addClickHandler(new ClickHandler() {
			
			@Override
			public void onClick(ClickEvent event) {
				handler.start();				
				
			}
		});
		
		stopAudioButton.addClickHandler(new ClickHandler() {
			
			@Override
			public void onClick(ClickEvent event) {
				handler.stop();
				
			}
		});

		 
	}
	
	class LoggingService implements ILoggingService
	{

		@Override
		public void error(String message) {
			Utils.consoleError(message);			
		}

		@Override
		public void debug(String message) {
			Utils.consoleDebug(message);
			
		}};
		
	
	class MessagingService implements IMessagingService
	{

		private String userName;

		public MessagingService(String userName) {
			this.userName = userName;
		}
		
		@Override
		public void SendMessage(String message, AsyncCallback<Void> callback) {
			greetingService.Send(userName, new Message(message), callback);				
		}

		@Override
		public void ReceiveMessage(final AsyncCallback<String> callback) {
			greetingService.Receive(userName, new AsyncCallback<Message>() {
				
				@Override
				public void onSuccess(Message result) {
					callback.onSuccess(result.Jso);
					
				}
				
				@Override
				public void onFailure(Throwable caught) {
					callback.onFailure(caught);
					
				}
			}				
		);};
	};
}
