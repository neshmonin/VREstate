package com.condox.order.client.wizard.presenter;

import com.condox.order.client.I_Presenter;
import com.condox.order.client.wizard.model.ProductModel;
import com.google.gwt.user.client.ui.HasWidgets;
import com.google.gwt.user.client.ui.Widget;

public class ProductPresenter implements I_Presenter {

	public static interface I_Display {
		void setPresenter(ProductPresenter presenter);
		boolean getListing();
		boolean getPrivate();
		boolean getShared();
		boolean getLayout();
		void setListing(boolean value);
		void setPrivate(boolean value);
		void setShared(boolean value);
		void setLayout(boolean value);
		Widget asWidget();
	}
	
	private I_Display display = null;
	private ProductModel model = null;
	
	public ProductPresenter(I_Display display, ProductModel model) {
		this.display = display;
		this.display.setPresenter(this);
		this.model = model;
	}

	@Override
	public void go(HasWidgets container) {
		display.setListing(model.getListing());
		display.setPrivate(model.getListingPrivate());
		display.setShared(model.getListingShared());
		display.setLayout(model.getLayout());
		container.clear();
		container.add(this.display.asWidget());
	}
	
	public void onPrev() {
		model.prev();
	}
	
	public void onNext() {
		model.setListing(display.getListing());
		model.setListingPrivate(display.getPrivate());
		model.setListingShared(display.getShared());
		model.setLayout(display.getLayout());
		model.next();
	}
}
