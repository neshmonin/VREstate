package com.condox.order.client.context;


public interface IContext {
	public static enum ContextType {
		WELCOME, BUILDINGS, SUITES, ORDER_TYPE
	}

	void setValue(String key, String value);

	String getValue(String key);

	Object getData();

	String getString();

	Boolean isValid();
	
	ContextType getType();

	// TODO for debug inly
	String getName();
}
