package com.condox.ecommerce.client.tree.presenter;

import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.container.I_Container;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.EcommerceTree.NodeStates;
import com.condox.ecommerce.client.tree.node.UsingMLSNode;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.Widget;

public class UsingMLSPresenter implements I_Presenter {

	public static interface I_Display extends I_Contained {
		void setPresenter(UsingMLSPresenter presenter);

		boolean isUsingMLS();
		
		Widget asWidget();
	}

	private I_Display display = null;
	private UsingMLSNode node = null;

	public UsingMLSPresenter(I_Display newDisplay, UsingMLSNode newNode) {
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
	public void onCancel() {
		node.setState(NodeStates.Cancel);
		node.next();
	}

	public void onNext() {
		if (display.isUsingMLS())
			node.setState(NodeStates.UsingMLS);
		else
			node.setState(NodeStates.NotUsingMLS);
		node.next();
	}

}
