package com.condox.orders.client;

import com.condox.orders.client.document.Suite;
import com.google.gwt.core.client.GWT;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.event.dom.client.KeyUpEvent;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.http.client.URL;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONString;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.DialogBox;
import com.google.gwt.user.client.ui.RadioButton;
import com.google.gwt.user.client.ui.TextBox;
import com.google.gwt.user.client.ui.Widget;
import com.sun.org.apache.xerces.internal.util.URI;

public class Submit extends Composite {

	private static SubmitUiBinder uiBinder = GWT.create(SubmitUiBinder.class);
	private DialogBox box;
	@UiField Button btnCancel;
	@UiField Button btnSubmit;
	@UiField TextBox txtName;
	@UiField TextBox txtMail;
	@UiField TextBox txtPhone;
	@UiField RadioButton rbPrivateListing;
	@UiField RadioButton rbSharedListing;

	interface SubmitUiBinder extends UiBinder<Widget, Submit> {
	}

	public Submit() {
		initWidget(uiBinder.createAndBindUi(this));
		init();
	}

	public Submit(DialogBox box) {
		initWidget(uiBinder.createAndBindUi(this));
		this.box = box;
		init();
	}
	private void init() {
		Building selectedBuilding = Orders.selectedBuilding;
    	Suite selectedSuite = Orders.selectedSuite;
    	if (selectedBuilding == null)
    		return;
    	if (selectedSuite == null)
    		return;
    	
    	String str = selectedSuite.getName() + " - " +selectedSuite.getFloor_name();
    	str += selectedBuilding.getStreet() + ", ";
    	str += selectedBuilding.getCity() + ", ";
    	str += selectedBuilding.getPostal();
//    	lblInfo.setText(str);
	}
	
	private void ValidateInput() {
		String name = txtName.getText();
		String mail = txtMail.getText();
		String phone = txtPhone.getText();
		boolean isValid = true;
		isValid &= name.matches(".+");
		isValid &= mail.matches("\\S+@\\S+");
		isValid &= phone.matches("\\d+");
		btnSubmit.setEnabled(isValid);
	}

	@UiHandler("txtName")
	void onTxtNameKeyUp(KeyUpEvent event) {
		ValidateInput();
	}
	@UiHandler("txtMail")
	void onTxtMailKeyUp(KeyUpEvent event) {
		ValidateInput();
	}
	@UiHandler("txtPhone")
	void onTxtPhoneKeyUp(KeyUpEvent event) {
		ValidateInput();
	}
	@UiHandler("btnCancel")
	void onBtnCancelClick(ClickEvent event) {
//		History.back();
		box.hide();
	}
	@UiHandler("btnSubmit")
	void onBtnSubmitClick(ClickEvent event) {
//		Log.write("123");
//		Window.alert("213");
		SubmitOrder();
	}
	
	enum Type {PRIVATE, SHARED};
	
	private void SubmitOrder() {
		String CustomerName = txtName.getText();
		String CustomerEmail = txtMail.getText();
		String CustomerPhone = txtPhone.getText();
		Type ListingType = rbPrivateListing.getValue()? Type.PRIVATE : Type.SHARED;
//		String BuildingName = "Not implemented yet";
//		String BuildingAddress = "Not implemented yet";
//		String SuiteNum = "Not implemented yet";
		JSONObject obj = new JSONObject();
		obj.put("CustomerName", new JSONString(CustomerName));
		obj.put("CustomerEmail", new JSONString(CustomerEmail));
		obj.put("CustomerPhone", new JSONString(CustomerPhone));
		obj.put("ListingType", new JSONString(ListingType.toString()));
		Building selectedBuilding = Orders.selectedBuilding;
    	Suite selectedSuite = Orders.selectedSuite;
		obj.put("BuildingName", new JSONString(selectedBuilding.getName()));
		obj.put("BuildingAddress", new JSONString(selectedBuilding.getAddress()));
		obj.put("SuiteNum", new JSONString(selectedSuite.getName()));
//		Window.alert(obj.toString());
		sendMail("subject", obj.toString());
		box.hide();
	}
	
	private void sendMail(String subject, String body) {
		String url = Options.HOME_URL + "program?q=salesMessage";
		url += "&subject=" + URL.encodeQueryString(subject);
		url += "&testMode=true";
//		url += "&sid=" + User.SID;
		PUT.send(url, body, new RequestCallback(){

			@Override
			public void onResponseReceived(Request request, Response response) {
				// TODO Auto-generated method stub
				
			}

			@Override
			public void onError(Request request, Throwable exception) {
				// TODO Auto-generated method stub
				
			}});
	}
}
