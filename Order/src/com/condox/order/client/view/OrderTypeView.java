package com.condox.order.client.view;

import com.condox.order.client.context.Tree;
import com.condox.order.client.view.factory.IView;
import com.google.gwt.core.client.GWT;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.RadioButton;
import com.google.gwt.user.client.ui.Widget;

public class OrderTypeView extends Composite implements IView {

	private static OrderTypeViewUiBinder uiBinder = GWT
			.create(OrderTypeViewUiBinder.class);
	@UiField RadioButton rbPrivate;
	@UiField RadioButton rbPublic;
	@UiField Button button;
	@UiField Button btnNext;

	interface OrderTypeViewUiBinder extends UiBinder<Widget, OrderTypeView> {
	}

	public OrderTypeView() {
		initWidget(uiBinder.createAndBindUi(this));
	}

	private Tree tree;
	
	public OrderTypeView(Tree tree) {
		this();
		this.tree = tree;
		
		String type = tree.getValue("orderType");
		if ("private".equals(type))
			rbPrivate.setValue(true);
		else
			rbPublic.setValue(true);
	}

	@UiHandler("rbPrivate")
	void onRbPrivateClick(ClickEvent event) {
		if (rbPrivate.getValue())
			tree.setValue("orderType", "private");
		else
			tree.setValue("orderType", "public");
//		Log.write("orderType:" + tree.getValue("orderType"));
	}
	@UiHandler("rbPublic")
	void onRbPublicClick(ClickEvent event) {
		if (rbPrivate.getValue())
			tree.setValue("orderType", "private");
		else
			tree.setValue("orderType", "public");
//		Log.write("orderType:" + tree.getValue("orderType"));
	}
	@UiHandler("button")
	void onButtonClick(ClickEvent event) {
		tree.prev();
	}
	@UiHandler("btnNext")
	void onBtnNextClick(ClickEvent event) {
		String building = "Building #" + tree.getValue("selectedBuilding");
		String suite = "Suite #" + tree.getValue("selectedSuite");
		String type = "Order " + tree.getValue("orderType");
		String message = "Sending {" + building + ", " + suite + ", " + type + "}...";
		Window.alert(message);
	}
}
