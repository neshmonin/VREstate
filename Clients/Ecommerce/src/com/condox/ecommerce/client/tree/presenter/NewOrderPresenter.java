package com.condox.ecommerce.client.tree.presenter;

import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.container.I_Container;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.EcommerceTree.Actions;
import com.condox.ecommerce.client.tree.node.NewOrderNode;
import com.google.gwt.user.client.ui.Widget;

public class NewOrderPresenter implements I_Presenter {

	public static interface I_Display extends I_Contained {
		void setPresenter(NewOrderPresenter presenter);

		Widget asWidget();
	}

	private I_Display display = null;
	private NewOrderNode node = null;

	public NewOrderPresenter(I_Display newDisplay, NewOrderNode newNode) {
		display = newDisplay;
		display.setPresenter(this);
		node = newNode;
	}

	@Override
	public void go(I_Container container) {
//		container.clear();
//		container.add((I_Contained)display);
		node.next(Actions.Next);
	}

//	Events
	public void onCancel() {
		node.setState(Actions.Cancel);
		node.next();
	}

	public void onNext() {
		node.setState(Actions.Next);
		node.next();
	}

}
