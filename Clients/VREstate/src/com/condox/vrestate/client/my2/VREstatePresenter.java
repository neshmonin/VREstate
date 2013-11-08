package com.condox.vrestate.client.my2;

import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.container.I_Container;

public class VREstatePresenter extends Node implements I_Presenter {

	public interface I_Display extends I_Contained {
		void setPresenter(VREstatePresenter presenter);
		void setUserInfo(String info);
	}

	private I_Display display = null;

	public VREstatePresenter(I_Node parent, I_Display display) {
		super(parent);
		display.setPresenter(this);
		this.display = display;
	}
	
	@Override
	public void go(I_Container container) {
		container.add(display);
		String login = getData(DataFields.USER_LOGIN.name()).asString();
		String password = getData(DataFields.USER_PASSWORD.name()).asString();
		String info = "User login: " + login + "; user password: " + password;
		display.setUserInfo(info);
	}

}
