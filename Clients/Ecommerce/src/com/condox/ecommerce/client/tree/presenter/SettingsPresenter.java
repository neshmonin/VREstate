package com.condox.ecommerce.client.tree.presenter;

import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.container.I_Container;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.EcommerceTree.NodeStates;
import com.condox.ecommerce.client.tree.node.SettingsNode;
import com.google.gwt.user.client.ui.Widget;

public class SettingsPresenter implements I_Presenter {

	public static interface I_Display extends I_Contained {
		void setPresenter(SettingsPresenter presenter);

		Widget asWidget();
	}

	private I_Display display = null;
	private SettingsNode node = null;

	public SettingsPresenter(I_Display newDisplay, SettingsNode newNode) {
		display = newDisplay;
		display.setPresenter(this);
		node = newNode;
	}

	@Override
	public void go(I_Container container) {
		container.clear();
		container.add((I_Contained)display);
	}

//	Events
	public void onClose() {
		node.setState(NodeStates.Close);
		node.next();
	}

//	public void onForgotPassword() {
//		String email = display.getUserEmail().trim();
//		String password = display.getUserPassword().trim();
//		node.setData(Field.UserEmail, new Data(email));
//		node.setData(Field.UserPassword, new Data(password));
//		node.setState(NodeStates.ForgotPassword);
//		node.next();
//	}
}
