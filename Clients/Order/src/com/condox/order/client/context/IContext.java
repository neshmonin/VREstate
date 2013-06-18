package com.condox.order.client.context;


public interface IContext {
	public enum Types {LOGIN, BUILDINGS, SUITES, ORDER_TYPE, SUBMIT}
	public Boolean containsValue(String key);
	public String getValue(String key);
	public void setValue(String key, String value);
	public Types getType();
}