package com.condox.order.client.view;

import com.condox.order.client.context.Tree;
import com.condox.order.client.view.factory.IView;
import com.google.gwt.core.client.GWT;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.Widget;

public class WelcomeView extends Composite implements IView {

	private static WelcomeViewUiBinder uiBinder = GWT
			.create(WelcomeViewUiBinder.class);

	interface WelcomeViewUiBinder extends UiBinder<Widget, WelcomeView> {
	}

	public WelcomeView(Tree tree) {
		this();
//		initWidget(uiBinder.createAndBindUi(this));
	}
	
	public WelcomeView() {
		initWidget(uiBinder.createAndBindUi(this));
	}
}
