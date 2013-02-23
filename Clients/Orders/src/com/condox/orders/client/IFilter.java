package com.condox.orders.client;

public interface IFilter<T> {
	boolean isValid(T value, String filter);
}
