package com.condox.ecommerce.client.tree.presenter;

import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.event.dom.client.ClickHandler;
import com.google.gwt.event.dom.client.HasClickHandlers;
import com.google.gwt.event.shared.HandlerManager;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.DialogBox;
import com.google.gwt.user.client.ui.HasWidgets;
import com.google.gwt.user.client.ui.Widget;

public class WarningPresenter implements I_Presenter {

	public static interface I_Display {
		HasClickHandlers getOK();
		void setPresenter(WarningPresenter presenter);
		void setMessage(String value);
		Widget asWidget();
	}

	private I_Display display = null;
	private HandlerManager eventBus;
	private EcommerceTree tree = null;
	private DialogBox popup = new DialogBox();
	
	public WarningPresenter(I_Display display, HandlerManager eventBus) {
		this.display = display;
		this.eventBus = eventBus;
	}

	@Override
	public void go(HasWidgets container) {
		bind();
//		container.clear();
//		container.add(display.asWidget());
		popup.setGlassEnabled(true);
		popup.setWidget(display.asWidget());
		popup.center();
	}
	
	void bind() {
		display.getOK().addClickHandler(new ClickHandler() {
			
			@Override
			public void onClick(ClickEvent event) {
//				popup.hide();
				Window.Location.reload();
			}
		});
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

	public void setMessage(String value) {
		display.setMessage(value);
	}

}
