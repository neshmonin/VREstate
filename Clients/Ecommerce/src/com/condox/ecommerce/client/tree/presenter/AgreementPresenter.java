package com.condox.ecommerce.client.tree.presenter;

import com.condox.clientshared.abstractview.Log;
import com.condox.clientshared.communication.GET;
import com.condox.clientshared.communication.Options;
import com.condox.clientshared.communication.PUT;
import com.condox.clientshared.communication.User;
import com.condox.clientshared.communication.User.UserRole;
import com.condox.clientshared.document.SuiteInfo;
import com.condox.clientshared.document.SuiteInfo.Status;
import com.condox.clientshared.tree.Data;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.ServerProxy;
import com.condox.ecommerce.client.UserInfo;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Actions;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.i18n.client.NumberFormat;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.json.client.JSONString;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.HasWidgets;
import com.google.gwt.user.client.ui.Widget;

public class AgreementPresenter implements I_Presenter {

	public static interface I_Display {
		void setPresenter(AgreementPresenter presenter);

		Widget asWidget();
	}

	private I_Display display = null;
	private EcommerceTree tree = null;

	@Override
	public void go(HasWidgets container) {
		container.clear();
		container.add(display.asWidget());
	}

	// Navigation events
	public void onCancel() {
		tree.next(Actions.Cancel);
	}

	public void onPrev() {
		tree.next(Actions.Prev);
	}

	private String viewOrderId = "";

	public void onProceed() {
		Data data = tree.getData(Field.UserInfo);
		UserInfo info = new UserInfo();
		info.fromJSONObject(data.asJSONObject());
		if (info.getRole() == UserRole.Visitor) {
			tree.next(Actions.Next);
			return;
		}
		
		String url = Options.URL_VRT;
		url += "program?";
		url += "&q=register";
		url += "&entity=viewOrder";
		url += "&ownerId=" + User.id;
		// url += "&pr=" + User.id;
		url += "&daysValid=1";
		url += "&product=prl";
		url += "&options=fp"; // TODO
		// MLS#
		String mls = getSuiteInfo(Field.SuiteInfo).getMLS();
		url += "&mls_id=" + mls;
		url += "&propertyType=suite"; // TODO
		url += "&propertyId=" + getSuiteInfo(Field.SuiteInfo).getId();
		url += "&sid=" + User.SID;
		GET.send(url, new RequestCallback() {

			@Override
			public void onResponseReceived(Request request, Response response) {
				Log.write(response.getStatusCode() + ": "
						+ response.getStatusText());
				if (response.getStatusCode() == 200) {
					final JSONObject order = JSONParser
							.parseLenient(response.getText()).isObject();
					viewOrderId = order.get("viewOrder-id").isString()
							.stringValue();
					viewOrderId = viewOrderId.replace("-", "");

					String url = Options.URL_VRT;
					url += "data/suite/"
							+ getSuiteInfo(Field.SuiteInfo).getId();
					url += "?sid=" + User.SID;
					GET.send(url, new RequestCallback() {

						@Override
						public void onResponseReceived(Request request,
								Response response) {
							JSONObject obj = JSONParser.parseLenient(
									response.getText()).isObject();
							/*
							 * {"id":3582, "version":[0,0,0,0,0,56,238,124],
							 * "buildingId":193, "levelNumber":-1,
							 * "floorName":"10", "name":"1011",
							 * "ceilingHeightFt":9, "showPanoramicView":true,
							 * "status":"AvailableRent", "position":
							 * {"lon":-79.3750991821289,
							 * "lat":43.646324157714844,
							 * "alt":30.90764045715332, "hdg":-32.75, "vhdg":0},
							 * "suiteTypeId":1393, "currentPrice":"1650.00",
							 * "currentPriceDisplay":"$1,650.00",
							 * "currentPriceCurrency":"CAD"}
							 */
							Data data = tree.getData(Field.SuiteInfo);
							if (data != null) {
								SuiteInfo info = new SuiteInfo();
								info.fromJSONObject(data.asJSONObject());

								if (obj.containsKey("currentPrice")) {
									NumberFormat fmt = NumberFormat
											.getDecimalFormat();
									fmt.overrideFractionDigits(2);
									String currentPrice = String.valueOf(info
											.getPrice());
									obj.put("currentPrice",
											new JSONString(currentPrice));
								}
								if (obj
										.containsKey("currentPriceDisplay")) {
									NumberFormat fmt = NumberFormat
											.getDecimalFormat();
									fmt.overrideFractionDigits(2);
									String currentPrice = fmt.format(info
											.getPrice());
									obj.put("currentPriceDisplay",
											new JSONString("$" + currentPrice));
								}
								if (obj
										.containsKey("currentPriceCurrency"))
									obj.put("currentPriceCurrency",
											new JSONString("CAD"));

								if (info.getStatus() == Status.AvailableRent)
									obj.put("status", new JSONString(
											"AvaibleRent"));
								if (info.getStatus() == Status.AvailableResale)
									obj.put("status", new JSONString(
											"AvaibleResale"));
							}

							String url = Options.URL_VRT;
							url += "data/suite/"
									+ getSuiteInfo(Field.SuiteInfo).getId();
							url += "?sid=" + User.SID;
							
								PUT.send(url, obj.toString(),
										new RequestCallback() {

											@Override
											public void onResponseReceived(
													Request request,
													Response response) {
												int status = response.getStatusCode();
												 if ((status == 200)||(status == 304)) {
													 HelloPresenter.selected = viewOrderId;
													 tree.next(Actions.Next);
												 }
												 else
													 ServerProxy.deleteOrder(
																viewOrderId, User.SID,
																new RequestCallback() {

																	@Override
																	public void onResponseReceived(
																			Request request,
																			Response response) {
																	}

																	@Override
																	public void onError(
																			Request request,
																			Throwable exception) {
																	}
																});
													 
											}

											@Override
											public void onError(
													Request request,
													Throwable exception) {
												ServerProxy.deleteOrder(
														viewOrderId, User.SID,
														new RequestCallback() {

															@Override
															public void onResponseReceived(
																	Request request,
																	Response response) {
															}

															@Override
															public void onError(
																	Request request,
																	Throwable exception) {
															}
														});

											}
										});
								
						}

						@Override
						public void onError(Request request, Throwable exception) {
							// TODO Auto-generated method stub

						}
					});};
					
			}

			@Override
			public void onError(Request request, Throwable exception) {
				// TODO Auto-generated method stub
				
			}
			});

	}

	// Data utils
	private String getString(Field key) {
		Data data = tree.getData(key);
		String s = (data == null) ? "" : data.asString();
		return s;
	}

	private int getInteger(Field key) {
		Data data = tree.getData(key);
		int s = (data == null) ? -1 : data.asInteger();
		return s;
	}

	private SuiteInfo getSuiteInfo(Field key) {
		Data data = tree.getData(key);
		if (data != null) {
			SuiteInfo info = new SuiteInfo();
			info.fromJSONObject(data.asJSONObject());
			return info;
		}
		return null;
	}

	//
	// private void loadData() {
	// String listing = getString(Field.Address);
	// String mls = getString(Field.MLS);
	// String price = getString(Field.Price);
	// String virtual_tour = getString(Field.VirtualTourUrl);
	// String more_info = getString(Field.MoreInfoUrl);
	//
	// String html =
	// "Listing: " + (listing.isEmpty()? "<<empty>>" : listing) + "<br>" +
	// "MLS# " + mls + "<br>" + " Price  " + (price.isEmpty()? "<<empty>>" :
	// price);
	// // "Third Party Virtual Tour<br>" +
	// // "	" + (virtual_tour.isEmpty()? "<<empty>>" : virtual_tour) + "<br>";
	//
	// html += "More Info Link<br>";
	// if (more_info.isEmpty())
	// html += "<<empty>>";
	// else
	// html += "<<a href=\"" + more_info + "\">>" + more_info + "<</a>>";
	//
	// // display.setData(html);
	//
	// }
	//
	// private void saveData() {
	// // node.setData(Field.SuiteId, new
	// Data(display.getSelectedSuite().getId()));
	// // node.setData(Field.SuiteName, new
	// Data(display.getSelectedSuite().getName()));
	// // node.setData(Field.Address, new
	// Data(display.getSelectedSuite().getAddress()));
	// // node.setData(Field.MLS, new
	// Data(display.getSelectedSuite().getMLS()));
	// }

	@Override
	public void setView(Composite view) {
		display = (I_Display) view;
		display.setPresenter(this);
	}

	@Override
	public void setTree(EcommerceTree tree) {
		this.tree = tree;
	}

}
