package com.condox.ecommerce.client.tree.presenter;

import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.HasWidgets;
import com.google.gwt.user.client.ui.Widget;

public class ErrorPresenter implements I_Presenter {

	public static interface I_Display {
		void setPresenter(ErrorPresenter presenter);

		void setData(String data);
		
		Widget asWidget();
	}

	private I_Display display = null;
	private EcommerceTree tree = null;
	
	public ErrorPresenter(I_Display display) {
		this.display = display;
	}

	@Override
	public void go(HasWidgets container) {
		container.clear();
		container.add(display.asWidget());
	}
	
	@Override
	public void setView(Composite view) {
//		display = (I_Display) view;
		display.setPresenter(this);
	}

	@Override
	public void setTree(EcommerceTree tree) {
		this.tree = tree;
	}

}
