package com.condox.ecommerce.client.tree.presenter;

import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.container.I_Container;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.EcommerceTree.NodeStates;
import com.condox.ecommerce.client.tree.node.HelloAgentNode;
import com.google.gwt.user.client.ui.Widget;

public class HelloAgentPresenter implements I_Presenter/*, I_HelloAgent*/ {

	public static interface I_Display extends I_Contained {
		void setPresenter(HelloAgentPresenter presenter);

		Widget asWidget();
	}

	private I_Display display = null;
	private HelloAgentNode node = null;

	public HelloAgentPresenter(I_Display newDisplay, HelloAgentNode newNode) {
		display = newDisplay;
		display.setPresenter(this);
		node = newNode;
	}

	@Override
	public void go(I_Container container) {
		container.clear();
		container.add((I_Contained)display);
	}

	public void onLogout() {
		node.setState(NodeStates.Logout);
		node.next();
	}

	// Events
//	public void onHelloAgent() {
//		node.setState(NodeStates.Guest);
//		node.next();
//	}

}
