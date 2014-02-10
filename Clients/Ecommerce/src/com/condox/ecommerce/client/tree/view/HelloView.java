package com.condox.ecommerce.client.tree.view;

import java.util.List;

import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.container.I_Container;
import com.condox.ecommerce.client.tree.presenter.HelloPresenter.I_Display;
import com.condox.ecommerce.client.tree.presenter.I_HelloPresenter;
import com.google.gwt.cell.client.AbstractCell;
import com.google.gwt.core.client.GWT;
import com.google.gwt.event.dom.client.HasClickHandlers;
import com.google.gwt.safehtml.shared.SafeHtmlBuilder;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.user.cellview.client.CellList;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.HTMLPanel;
import com.google.gwt.user.client.ui.HasWidgets;
import com.google.gwt.user.client.ui.Label;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.view.client.SingleSelectionModel;

public class HelloView<T> extends Composite implements I_Display, I_Contained {

	private static HelloViewUiBinder uiBinder = GWT
			.create(HelloViewUiBinder.class);
	@UiField Button editProfile;
	@UiField Button editSettings;
	@UiField Button createOrder;
	@UiField Button showHistory;
	@UiField(provided=true) CellList<ViewOrderInfo> orderList = new CellList<ViewOrderInfo>(new AbstractCell<ViewOrderInfo>(){
		@Override
		public void render(Context context, ViewOrderInfo value, SafeHtmlBuilder sb) {
			// TODO
			Label lbl = new Label();
//			lbl.setText(value.getLabel());
			String[] s = value.getLabel().split(",");
			String l = s[0] + ", " + s[1];
			if (!value.getMLS().isEmpty())
				l += ", " + value.getMLS();
			lbl.setText(l);
				
			sb.appendHtmlConstant(lbl.getElement().getString());
		}
	});
	@UiField HTMLPanel details;
	@UiField Button logout;

	interface HelloViewUiBinder extends UiBinder<Widget, HelloView> {
	}

	public HelloView() {
		 final SingleSelectionModel<ViewOrderInfo> selectionModel = new SingleSelectionModel<ViewOrderInfo>();
		 orderList.setSelectionModel(selectionModel);
		 
		initWidget(uiBinder.createAndBindUi(this));
	}
	
	private I_HelloPresenter<T> presenter;
	public void setPresenter(I_HelloPresenter presenter) {
		this.presenter = presenter;
	}
	@Override
	public void setData(List<ViewOrderInfo> data) {
		orderList.setRowData(0, data);
	}
	@Override
	public HasClickHandlers getEditProfile() {
		return editProfile;
	}
	@Override
	public HasClickHandlers getEditSettings() {
		return editSettings;
	}
	@Override
	public HasClickHandlers getCreateOrder() {
		return createOrder;
	}
	@Override
	public HasClickHandlers getShowHistory() {
		return showHistory;
	}
	
	@Override
	public HasWidgets getDetailsArea() {
		return details;
	}
	@Override
	public SingleSelectionModel<ViewOrderInfo> getSelection() {
		// TODO Auto-generated method stub
		return (SingleSelectionModel<ViewOrderInfo>) orderList.getSelectionModel();
	}
	@Override
	public HasClickHandlers getLogout() {
		// TODO Auto-generated method stub
		return logout;
	}
}
