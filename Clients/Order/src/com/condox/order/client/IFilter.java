package com.condox.order.client;

public interface IFilter<T> {
	boolean isValid(T value, String filter);
}
