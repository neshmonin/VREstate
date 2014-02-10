package com.condox.ecommerce.client.tree.presenter;

import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Actions;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.HasWidgets;
import com.google.gwt.user.client.ui.Widget;

public class NewOrderPresenter implements I_Presenter {

	public static interface I_Display {
		void setPresenter(NewOrderPresenter presenter);

		Widget asWidget();
	}

	private I_Display display = null;
	private EcommerceTree tree = null;

	@Override
	public void go(HasWidgets container) {
//		container.clear();
//		container.add((I_Contained)display);
		tree.next(Actions.Next);
	}

//	Events
	public void onCancel() {
		tree.next(Actions.Cancel);
	}

	public void onNext() {
		tree.next(Actions.Next);
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

}
