package com.condox.order.client.presenter;

import com.condox.order.client.view.IViewContainer;


public abstract interface IPresenter {
	public abstract void go(IViewContainer container);
	public void stop();
}