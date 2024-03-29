package com.condox.ecommerce.client.tree.presenter;

import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Actions;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.HasWidgets;
import com.google.gwt.user.client.ui.Widget;

public class UpdateAvatarPresenter implements I_Presenter {

	public static interface I_Display {
		void setPresenter(UpdateAvatarPresenter presenter);

		Widget asWidget();
	}

	private I_Display display = null;
	private EcommerceTree tree = null;


	@Override
	public void go(HasWidgets container) {
		container.clear();
		container.add(display.asWidget());
	}

	public void onApply() {
		tree.next(Actions.Close);
	}

	public void onClose() {
		tree.next(Actions.Close);
	}

	@Override
	public void setView(Composite view) {
		display = (I_Display) view;
		display.setPresenter(this);
	}

	@Override
	public void setTree(EcommerceTree tree) {
		this.tree = tree;
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
