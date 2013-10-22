package com.condox.order.client.wizard.model;

import com.google.gwt.user.client.ui.DialogBox;
import com.google.gwt.user.client.ui.VerticalPanel;
import com.google.gwt.user.client.ui.HorizontalPanel;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.HasHorizontalAlignment;
import com.google.gwt.user.client.ui.Image;
import com.google.gwt.user.client.ui.HTML;
import com.google.gwt.event.dom.client.ClickHandler;
import com.google.gwt.event.dom.client.ClickEvent;

public class ModalMessage extends DialogBox {
	public ModalMessage(String msg, String iconURL) {
		
		VerticalPanel verticalPanel = new VerticalPanel();
		verticalPanel.setSpacing(10);
		setWidget(verticalPanel);
		verticalPanel.setWidth("350px");
		
		HorizontalPanel horizontalPanel = new HorizontalPanel();
		horizontalPanel.setSpacing(10);
		verticalPanel.add(horizontalPanel);
		
		Image image = new Image(iconURL);
		horizontalPanel.add(image);
		image.setSize("100px", "100px");
		
		HTML htmlNewHtml = new HTML(msg, true);
		htmlNewHtml.setHorizontalAlignment(HasHorizontalAlignment.ALIGN_LEFT);
		horizontalPanel.add(htmlNewHtml);
		
		Button btnNewButton = new Button("OK");
		btnNewButton.addClickHandler(new ClickHandler() {
			public void onClick(ClickEvent event) {
				hide();
			}
		});
		verticalPanel.add(btnNewButton);
		btnNewButton.setWidth("100px");
		verticalPanel.setCellHorizontalAlignment(btnNewButton, HasHorizontalAlignment.ALIGN_CENTER);
		//***************
		setGlassEnabled(true);
		setModal(true);
	}
}
