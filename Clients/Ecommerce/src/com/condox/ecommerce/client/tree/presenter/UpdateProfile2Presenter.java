package com.condox.ecommerce.client.tree.presenter;

import com.condox.clientshared.communication.User;
import com.condox.clientshared.communication.User.UserRole;
import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.container.I_Container;
import com.condox.clientshared.tree.Data;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.EcommerceTree.NodeStates;
import com.condox.ecommerce.client.tree.node.UpdateProfile2Node;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.Widget;

public class UpdateProfile2Presenter implements I_Presenter {

	public static interface I_Display extends I_Contained {
		void setPresenter(UpdateProfile2Presenter presenter);

		Widget asWidget();
	}

	private I_Display display = null;
	private UpdateProfile2Node node = null;

	public UpdateProfile2Presenter(I_Display newDisplay, UpdateProfile2Node newNode) {
		display = newDisplay;
		display.setPresenter(this);
		node = newNode;
	}

	@Override
	public void go(I_Container container) {
		container.clear();
		container.add((I_Contained)display);
	}

	public void onClose() {
		node.next(NodeStates.Close);
	}

	public void onCancel() {
		node.next(NodeStates.Cancel);
	}

	public void onPrev() {
		node.next(NodeStates.Prev);
	}

	public void onFinish() {
		node.next(NodeStates.Finish);
	}

	// Events
//	public void onForgotPassword() {
//		String email = display.getUserEmail().trim();
//		String password = display.getUserPassword().trim();
//		node.setData(Field.UserEmail, new Data(email));
//		node.setData(Field.UserPassword, new Data(password));
//		node.setState(NodeStates.ForgotPassword);
//		node.next();
//	}

}
