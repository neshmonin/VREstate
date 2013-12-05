package com.condox.ecommerce.client;

public interface IFilter<T> {
	boolean isValid(T value, String filter);
}
