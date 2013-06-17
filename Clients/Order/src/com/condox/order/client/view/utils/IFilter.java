package com.condox.order.client.view.utils;

public interface IFilter<T> {
	boolean isValid(T value, String filter);
}
