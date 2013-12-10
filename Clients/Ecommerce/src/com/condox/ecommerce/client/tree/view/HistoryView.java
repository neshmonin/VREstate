//package com.condox.ecommerce.client.tree.view;
//
//import com.condox.ecommerce.client.tree.presenter.HistoryPresenter;
//import com.condox.ecommerce.client.tree.presenter.HistoryPresenter.I_Display;
//import com.google.gwt.core.client.GWT;
//import com.google.gwt.json.client.JSONArray;
//import com.google.gwt.json.client.JSONObject;
//import com.google.gwt.json.client.JSONParser;
//import com.google.gwt.json.client.JSONValue;
//import com.google.gwt.uibinder.client.UiBinder;
//import com.google.gwt.uibinder.client.UiField;
//import com.google.gwt.user.client.ui.Composite;
//import com.google.gwt.user.client.ui.HTML;
//import com.google.gwt.user.client.ui.Widget;
//
//public class HistoryView extends Composite implements I_Display {
//
//	private static HistoryViewUiBinder uiBinder = GWT
//			.create(HistoryViewUiBinder.class);
//	@UiField HTML htmlHistoryData;
//
//	interface HistoryViewUiBinder extends UiBinder<Widget, HistoryView> {
//	}
//
//	@SuppressWarnings("unused")
//	private HistoryPresenter presenter = null;
//
//	public HistoryView() {
//		initWidget(uiBinder.createAndBindUi(this));
//	}
//
//	@Override
//	public void setPresenter(HistoryPresenter presenter) {
//		this.presenter = presenter;
//	}
//
//	@Override
//	public void setHistoryData(String data) {
//		String history = "";
//		JSONObject obj = JSONParser.parseLenient(data).isObject();
//		if (obj != null) {
////			int count = (int) obj.get("totalCount").isNumber().doubleValue();
//			JSONArray arr = obj.get("transactions").isArray();
//			for (int i = 0; i < arr.size(); i++) {
//				JSONValue item = arr.get(i);
//				history += item.toString() + "<br/>"; 
//			}
//		}
//		htmlHistoryData.setHTML(history);
//	}
//
////	@Override
////	public String getUserLogin() {
////		if (user)
////			return textUserLogin.getValue();
////		else if (guest)
////			return "web";
////		return "";
////
////	}
////
////	@Override
////	public String getUserPassword() {
////		if (user)
////			return textUserPassword.getValue();
////		else if (guest)
////			return "web";
////		return "";
////	}
////
////	private void updateButtonEnter() {
////		user = !textUserLogin.getValue().isEmpty();
////		user &= !textUserPassword.getValue().isEmpty();
////		guest = textUserLogin.getValue().isEmpty();
////		guest &= textUserPassword.getValue().isEmpty();
////		if (user) {
////			buttonEnter.setEnabled(true);
////			buttonEnter.setText("Order as a User");
////		} else if (guest) {
////			buttonEnter.setEnabled(true);
////			buttonEnter.setText("Order as a Guest");
////		} else {
////			buttonEnter.setEnabled(false);
////			buttonEnter.setText("Order");
////		}
////
////	}
//}
