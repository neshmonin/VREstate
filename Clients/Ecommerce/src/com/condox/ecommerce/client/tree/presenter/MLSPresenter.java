package com.condox.ecommerce.client.tree.presenter;

import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.Data;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.EcommerceTree.State;
import com.condox.ecommerce.client.tree.I_Contained;
import com.condox.ecommerce.client.tree.I_Container;
import com.condox.ecommerce.client.tree.model.MLSModel;
import com.google.gwt.user.client.ui.Widget;

public class MLSPresenter implements I_Presenter {

	public static interface I_Display extends I_Contained {
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
	public void go(I_Container container) {
		display.setMLS("");
		container.clear();
		container.add((I_Contained)display);
	}
	
	public void onPrev() {
		model.prev();
	}
	
	public void onNext() {
		String mls = display.getMLS();
		EcommerceTree.set(Field.MLS, new Data(mls));
		if (mls.isEmpty())
			EcommerceTree.transitState(State.Address);
		else
			EcommerceTree.transitState(State.MLS);

		model.next();
	}
}
