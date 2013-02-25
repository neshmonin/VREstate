package com.condox.orders.client.page.submit;

import com.condox.orders.client.Log;
import com.condox.orders.client.Options;
import com.condox.orders.client.Orders;
import com.condox.orders.client.PUT;
import com.condox.orders.client.page.building.Building;
import com.condox.orders.client.page.suite.Suite;
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
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.DialogBox;
import com.google.gwt.user.client.ui.HTML;
import com.google.gwt.user.client.ui.RadioButton;
import com.google.gwt.user.client.ui.TextBox;
import com.google.gwt.user.client.ui.Widget;

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
	@UiField HTML urlFloorplan;

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
    	
    	String str = selectedSuite.getName() + " - " +selectedSuite.getFloorName();
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
		isValid &= phone.matches("\\d{10,}");
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
	
	private String CustomerName = "";
	private String CustomerEmail = "";
	
	private int countEmail = 0;
	
	private void SubmitOrder() {
		countEmail = 2;
		CustomerName = txtName.getText();
		CustomerEmail = txtMail.getText();
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
		obj.put("BuildingAddress", new JSONString(selectedBuilding.getStreet()));	//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		obj.put("SuiteNum", new JSONString(selectedSuite.getName()));
		Log.write(obj.toString());
		sendMail("submit", obj.toString(),"sales@3dcondox.com");
		//**********************************
		String mail = "" +
		"Dear " + CustomerName + ",\r\n" +
		"\r\n" +
		"Thank you for submitting a request for ";
		switch (ListingType) {
		case PRIVATE:
			mail += "Private";
			break;
		case SHARED:
			mail += "Shared";
			break;
		}
		mail += " Interactive 3D Listing " +
		"on the following property:\r\n" +
		"\r\n" +
		"\t" + box.getText() + "\r\n" + 
		"\r\n" +
		"You'll be contacted with one of our sales representatives shortly.\r\n" +
		"\r\n" +
		"If you did not order any products from 3D Condo Explorer, " +
		"please reply to this message and inform us about this.\r\n" +
		"\r\n" +
		"Thanks for your business,\r\n" +
		"\r\n" +
		"3D Condo Explorer sales team.\r\n" +
		"order.3dcondox.com\r\n" +
		"1-855-332-6630 ext.2";
		sendMail("notification", mail, CustomerEmail);
		box.hide();
	}
	
	private void sendMail(String subject, String body, String receiver) {
		String url = Options.HOME_URL + "program?q=salesMessage";
		url += "&subject=" + URL.encodeQueryString(subject);
		url += "&receiver=" + receiver;
//		url += "&testMode=true";
		PUT.send(url, body, new RequestCallback(){

			@Override
			public void onResponseReceived(Request request, Response response) {
				// TODO Auto-generated method stub
				countEmail--;
				if (countEmail == 0) {
					DialogBox box = new DialogBox();
					SubmitOK message = new SubmitOK(box);
					message.setCustomerEmail(CustomerEmail);
					box.setWidget(message);
					box.center();
					box.show();
				}
			}

			@Override
			public void onError(Request request, Throwable exception) {
				// TODO Auto-generated method stub
				countEmail--;
			}});
	}
	
	public void setFloorPlanUrl(String url) {
		if (!url.isEmpty())
			urlFloorplan.setHTML("Do you want to check the floorplan for this suite?<br>" +
					"<a href=\"" + url + "\" target=\"_blank\">Show me...</a>");
		else
			urlFloorplan.setHTML("");
//		Do you want to check the floorplan for this suite?
//				Show me...
	}
}
