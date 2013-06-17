package com.condox.order.client.presenter;

import java.util.ArrayList;
import java.util.List;

import com.condox.order.client.context.ContextTree;
import com.condox.order.client.utils.Globals;
import com.condox.order.client.utils.Log;
import com.condox.order.client.utils.PUT;
import com.condox.order.client.view.IView;
import com.condox.order.client.view.IViewContainer;
import com.condox.order.shared.SuiteInfo;
import com.google.gwt.event.shared.EventBus;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.http.client.URL;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONString;
import com.google.gwt.user.client.ui.Widget;

public class SubmitPresenter implements IPresenter {

	public interface IDisplay extends IView {
		void setPresenter(SubmitPresenter presenter);
		
		void setFloorplanUrl(String url);

		String getCustomerName();

		String getCustomerMail();

		String getCustomerPhone();
		
		String getCustomerPhoneExt();

		Boolean isListingPrivate();

		Widget asWidget();
	}

	private EventBus eventBus;
	private ContextTree tree;
	private IDisplay display;

	public SubmitPresenter(EventBus eventBus, ContextTree tree, IDisplay display) {
		this.eventBus = eventBus;
		this.tree = tree;
		this.display = display;
		this.display.setFloorplanUrl(tree.getValue("suite.floorplan"));
		this.display.setPresenter(this);
	}

	@Override
	public void stop() {
	}

	@Override
	public void go(IViewContainer container) {
		container.setView(display);
	}

	public void onCancel() {
		tree.prev();
	}

	public void onSubmit() {
		SubmitOrder();
	}

	// ================================
	private int countEmail = 2;

	private enum Types {
		PRIVATE, SHARED
	};

	private void SubmitOrder() {
		//*******Customer data****************//
		String CustomerName = display.getCustomerName();
		String CustomerEmail = display.getCustomerMail();
		String CustomerPhone = display.getCustomerPhone();
		String CustomerPhoneExt = display.getCustomerPhoneExt();
		/**/
		Types ListingType = display.isListingPrivate() ? Types.PRIVATE
				: Types.SHARED;
		//******************************************
		String BuildingName = tree.getValue("building.name");
		String BuildingAddress = tree.getValue("building.street");
		//******************************************
		String SuiteNum = tree.getValue("suite.name");
		
		JSONObject obj = new JSONObject();
		obj.put("CustomerName", new JSONString(CustomerName));
		obj.put("CustomerEmail", new JSONString(CustomerEmail));
		obj.put("CustomerPhone", new JSONString(CustomerPhone));
		if ((CustomerPhoneExt != null)&&(!CustomerPhoneExt.isEmpty()))
			obj.put("Ext", new JSONString(CustomerPhoneExt));
		obj.put("ListingType", new JSONString(ListingType.toString()));
		obj.put("BuildingName", new JSONString(BuildingName));
		obj.put("BuildingAddress", new JSONString(BuildingAddress)); // <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		obj.put("SuiteNum", new JSONString(SuiteNum));
		Log.write(obj.toString());
//		sendMail("submit", obj.toString(), "sales@3dcondox.com");
		// **********************************
		String mail = "" + "Dear " + CustomerName + ",\r\n" + "\r\n"
				+ "Thank you for submitting a request for ";
		switch (ListingType) {
		case PRIVATE:
			mail += "Private";
			break;
		case SHARED:
			mail += "Shared";
			break;
		}
		mail += " Interactive 3D Listing "
				+ "on the following property:\r\n"
				+ "\r\n"
				+ "\t"
				+ SuiteNum
				+ " - "
				+ BuildingAddress
				+ "  ("
				+ BuildingName
				+ ")"
				+ "\r\n"
				+ "\r\n"
				+ "You'll be contacted with one of our sales representatives shortly.\r\n"
				+ "\r\n"
				+ "If you did not order any products from 3D Condo Explorer, "
				+ "please reply to this message and inform us about this.\r\n"
				+ "\r\n" + "Thanks for your business,\r\n" + "\r\n"
				+ "3D Condo Explorer sales team.\r\n"
				+ "order.3dcondox.com\r\n" + "1-855-332-6630 ext.2";
		sendMail("notification", mail, CustomerEmail);
		// box.hide();
	}

	private void sendMail(String subject, String body, String receiver) {
		String url = Globals.getBaseUrl() + "program?q=salesMessage";
		url += "&subject=" + URL.encodeQueryString(subject);
		url += "&receiver=" + receiver;
		// url += "&testMode=true";
		PUT.send(url, body, new RequestCallback() {

			@Override
			public void onResponseReceived(Request request, Response response) {
				// TODO Auto-generated method stub
				/*
				 * countEmail--; if (countEmail == 0) { DialogBox box = new
				 * DialogBox(); SubmitOK message = new SubmitOK(box);
				 * message.setCustomerEmail(CustomerEmail);
				 * box.setWidget(message); box.center(); box.show(); }
				 */
			}

			@Override
			public void onError(Request request, Throwable exception) {
				// TODO Auto-generated method stub
				countEmail--;
			}
		});
	}
}
