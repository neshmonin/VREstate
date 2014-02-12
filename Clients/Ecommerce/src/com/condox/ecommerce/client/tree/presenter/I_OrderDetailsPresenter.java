package com.condox.ecommerce.client.tree.presenter;

public interface I_OrderDetailsPresenter<T> {
	void onDeleteItem(T item);
	void onEnableItem(T item, boolean value);
}
