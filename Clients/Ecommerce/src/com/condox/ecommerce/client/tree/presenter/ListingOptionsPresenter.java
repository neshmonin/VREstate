package com.condox.ecommerce.client.tree.presenter;

import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.container.I_Container;
import com.condox.clientshared.tree.Data;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.EcommerceTree.State;
import com.condox.ecommerce.client.tree.model.ListingOptionsModel;
import com.google.gwt.user.client.ui.Widget;

public class ListingOptionsPresenter implements I_Presenter {

	public static interface I_Display extends I_Contained {
		void setPresenter(ListingOptionsPresenter presenter);
		void setMLS(String value);
		void setVirtualTourURL(String value);
		void setMoreInfoURL(String value);
		String getMls();
		String getVirtualTourURL();
		String getMoreInfoURL();
		Widget asWidget();
	}

	private I_Display display = null;
	private ListingOptionsModel model = null;

	public ListingOptionsPresenter(I_Display display, ListingOptionsModel model) {
		this.display = display;
		this.display.setPresenter(this);
		this.model = model;
	}

	@Override
	public void go(I_Container container) {
		container.clear();
		container.add((I_Contained)display);
		updateData();
	}

	public void onEnter() {
	}

	public void onPrev() {
		model.prev();
		
	}

	public void onNext() {
		EcommerceTree.set(Field.MLS, new Data(display.getMls()));

		EcommerceTree.set(Field.VirtualTourURL, new Data(display.getVirtualTourURL()));
		EcommerceTree.set(Field.MoreInfoURL, new Data(display.getMoreInfoURL()));
		EcommerceTree.transitState(State.OptionsReady);

		model.next();
	}
	
	private void updateData() {
		display.setMLS(EcommerceTree.get(Field.MLS).asString());
		display.setVirtualTourURL(EcommerceTree.get(Field.VirtualTourURL).asString());
		display.setMoreInfoURL(EcommerceTree.get(Field.MoreInfoURL).asString());
	}
}
