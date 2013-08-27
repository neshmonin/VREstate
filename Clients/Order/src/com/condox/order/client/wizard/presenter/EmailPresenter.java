package com.condox.order.client.wizard.presenter;

import com.condox.order.client.I_Presenter;
import com.condox.order.client.wizard.model.EmailModel;
import com.google.gwt.user.client.ui.HasWidgets;
import com.google.gwt.user.client.ui.Widget;

public class EmailPresenter implements I_Presenter {

	public static interface I_Display {
		void setPresenter(EmailPresenter presenter);

		Widget asWidget();
	}

	private I_Display display = null;
	private EmailModel model = null;

	public EmailPresenter(I_Display display, EmailModel model) {
		this.display = display;
		this.display.setPresenter(this);
		this.model = model;
	}

	@Override
	public void go(HasWidgets container) {
		container.clear();
		container.add(this.display.asWidget());
	}

	public void onPrev() {
		model.prev();
	}

	public void onNext() {
		model.next();
	}
}
