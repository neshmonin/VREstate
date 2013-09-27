package com.condox.order.client.wizard.presenter;

import com.condox.order.client.I_Presenter;
import com.condox.order.client.wizard.model.MLSModel;
import com.google.gwt.user.client.ui.HasWidgets;
import com.google.gwt.user.client.ui.Widget;

public class MLSPresenter implements I_Presenter {

	public static interface I_Display {
		void setPresenter(MLSPresenter presenter);
		String getMLS();
		void setMLS(String value);
		Widget asWidget();
	}
	
	private I_Display display = null;
	private MLSModel model = null;
	
	public MLSPresenter(I_Display display, MLSModel model) {
		this.display = display;
		this.display.setPresenter(this);
		this.model = model;
	}

	@Override
	public void go(HasWidgets container) {
		/*display.setListing(model.getListing());
		display.setPrivate(model.getListingPrivate());
		display.setShared(model.getListingShared());
		display.setLayout(model.getLayout());*/
		display.setMLS(model.getMLS());
		container.clear();
		container.add(this.display.asWidget());
	}
	
	public void onPrev() {
		model.prev();
	}
	
	public void onNext() {
		/*model.setListing(display.getListing());
		model.setListingPrivate(display.getPrivate());
		model.setListingShared(display.getShared());
		model.setLayout(display.getLayout());*/
		model.setMLS(display.getMLS());
		model.next();
	}
}
