package com.condox.order.client.context;


public class WelcomeContext extends Context {

	@Override
	public ContextType getType() {
		return IContext.ContextType.WELCOME;
	}

	@Override
	public String getName() {
		return "Welcome";
	}
	
	

}
