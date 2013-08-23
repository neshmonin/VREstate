package com.condox.order.client.wizard.presenter;

import com.condox.clientshared.communication.GET;
import com.condox.order.client.Globals;
import com.condox.order.client.I_Presenter;
import com.condox.order.client.wizard.model.ListingOptionsModel;
import com.condox.order.client.wizard.model.LoginModel;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.http.client.URL;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.HasWidgets;
import com.google.gwt.user.client.ui.Widget;

public class ListingOptionsPresenter implements I_Presenter {

	public static interface I_Display {
		void setPresenter(ListingOptionsPresenter presenter);

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
	public void go(HasWidgets container) {
		container.clear();
		container.add(this.display.asWidget());
	}

	public void onEnter() {
	}

	public void onPrev() {
		model.prev();
		
	}

	public void onNext() {
		model.next();
	}
}
