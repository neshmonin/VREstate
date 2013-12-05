package webRtcDemo.server;

import java.util.LinkedList;

import webRtcDemo.client.GreetingService;
import webRtcDemo.shared.FieldVerifier;

import com.google.gwt.core.shared.GWT;
import com.google.gwt.user.server.rpc.RemoteServiceServlet;

/**
 * The server side implementation of the RPC service.
 */
@SuppressWarnings("serial")
public class GreetingServiceImpl extends RemoteServiceServlet implements
		GreetingService {

	public GreetingServiceImpl()
	{
		GWT.log("Created GreetingServiceImpl");
	}
	
	public String greetServer(String input) throws IllegalArgumentException {
		// Verify that the input is valid. 
		if (!FieldVerifier.isValidName(input)) {
			// If the input is not valid, throw an IllegalArgumentException back to
			// the client.
			throw new IllegalArgumentException(
					"Name must be at least 4 characters long");
		}

		String serverInfo = getServletContext().getServerInfo();
		String userAgent = getThreadLocalRequest().getHeader("User-Agent");

		// Escape data from the client to avoid cross-site script vulnerabilities.
		input = escapeHtml(input);
		userAgent = escapeHtml(userAgent);

		return "Hello, " + input + "!<br><br>I am running " + serverInfo
				+ ".<br><br>It looks like you are using:<br>" + userAgent;
	}

	/**
	 * Escape an html string. Escaping data received from the client helps to
	 * prevent cross-site script vulnerabilities.
	 * 
	 * @param html the html string to escape
	 * @return the escaped string
	 */
	private String escapeHtml(String html) {
		if (html == null) {
			return null;
		}
		return html.replaceAll("&", "&amp;").replaceAll("<", "&lt;")
				.replaceAll(">", "&gt;");
	}

	private LinkedList<Pair> messages = new LinkedList<Pair>();
	
	static class Pair{
		public String UserName;
		public webRtcDemo.client.Message Message;
	}
	
	private  void Push(String username, webRtcDemo.client.Message message)
	{
		synchronized (messages) {
			Pair pair = new Pair();
			pair.UserName = username;
			pair.Message = message;
			messages.add(pair);
			messages.notifyAll();	
		}
		
	}
	
	private webRtcDemo.client.Message Pop(String username) throws InterruptedException
	{
		while (true)
		{
			synchronized (messages)
			{
				while (messages.isEmpty())
				{
					messages.wait();
				}
				for (Pair pair : messages) {
					if (!pair.UserName.equals(username))
					{
						messages.remove(pair);
						return pair.Message;
					}					
				}
				messages.wait();
			}
		}
	}
	
	@Override
	public void Send(String userName, webRtcDemo.client.Message message) {				
		Push(userName, message);		
	}

	@Override
	public webRtcDemo.client.Message Receive(String userName){			
		try {
			webRtcDemo.client.Message message = Pop(userName);			
			return message;
		} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
			return null;
		}
	}
}
