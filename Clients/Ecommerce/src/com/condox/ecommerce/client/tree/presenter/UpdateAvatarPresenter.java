package com.condox.ecommerce.client.tree.presenter;

import com.condox.clientshared.communication.User;
import com.condox.clientshared.communication.User.UserRole;
import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.container.I_Container;
import com.condox.clientshared.tree.Data;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.EcommerceTree.NodeStates;
import com.condox.ecommerce.client.tree.node.UpdateAvatarNode;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.Widget;

public class UpdateAvatarPresenter implements I_Presenter {

	public static interface I_Display extends I_Contained {
		void setPresenter(UpdateAvatarPresenter presenter);

		Widget asWidget();
	}

	private I_Display display = null;
	private UpdateAvatarNode node = null;

	public UpdateAvatarPresenter(I_Display newDisplay, UpdateAvatarNode newNode) {
		display = newDisplay;
		display.setPresenter(this);
		node = newNode;
	}

	@Override
	public void go(I_Container container) {
		container.clear();
		container.add((I_Contained)display);
	}

	public void onApply() {
		node.next(NodeStates.Close);
	}

	public void onClose() {
		node.next(NodeStates.Close);
	}

//	public void onClose() {
//		node.next(NodeStates.Close);
//	}
//
//	public void onCancel() {
//		node.next(NodeStates.Cancel);
//	}
//
//	public void onPrev() {
//		node.next(NodeStates.Prev);
//	}
//
//	public void onFinish() {
//		node.next(NodeStates.Finish);
//	}

}
