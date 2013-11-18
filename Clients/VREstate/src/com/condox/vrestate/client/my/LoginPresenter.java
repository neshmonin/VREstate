package com.condox.vrestate.client.my;

import com.condox.clientshared.communication.User;
import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.container.I_Container;
import com.condox.vrestate.client.VREstate;
import com.google.gwt.user.client.ui.Widget;

public class LoginPresenter extends VREstateTreeNode {
	
	public static String simpleName = "Login";
	private I_Display display = null;
	
	public interface I_Display extends I_Contained {
		void setPresenter(LoginPresenter presenter);
		Widget asWidget();
	}
	
	public LoginPresenter(I_Display display) {
		display.setPresenter(this);
		this.display = display;
	}

	@Override
	public void go(I_Container container) {
//		this.container = container;
//		this.container.clear();
//		this.container.add(display);
		User.Login(VREstate.instance);
	}

	public void onGuestLogin() {
		container.clear();
		VREstate.instance.LoginUser();
	}
	
	
}
