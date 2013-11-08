package com.condox.vrestate.client.my2;

import com.condox.clientshared.container.I_Container;
import com.condox.clientshared.tree.Data;

public class DefaultPresenter extends Node implements I_Presenter {
	
	public DefaultPresenter(I_Node parent) {
		super(parent);
	}
	
	@Override
	public void go(I_Container container) {
		setData(DataFields.USER_LOGIN.name(), new Data("web"));
		setData(DataFields.USER_PASSWORD.name(), new Data("web"));
	}

}
