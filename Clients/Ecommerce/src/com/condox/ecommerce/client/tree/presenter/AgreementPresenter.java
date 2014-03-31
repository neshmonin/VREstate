package com.condox.ecommerce.client.tree.presenter;

import com.condox.clientshared.communication.User;
import com.condox.clientshared.communication.User.UserRole;
import com.condox.clientshared.document.SuiteInfo;
import com.condox.clientshared.document.SuiteInfo.Status;
import com.condox.clientshared.tree.Data;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.ServerProxy;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Actions;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.api.I_RequestCallback;
import com.condox.ecommerce.client.tree.api.ServerAPI;
import com.google.gwt.core.client.GWT;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.event.dom.client.ClickHandler;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.i18n.client.NumberFormat;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONString;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.DialogBox;
import com.google.gwt.user.client.ui.HasWidgets;
import com.google.gwt.user.client.ui.Widget;
import com.condox.ecommerce.client.tree.view.*;

public class AgreementPresenter implements I_Presenter {

	public static interface I_Display {
		void setPresenter(AgreementPresenter presenter);

		Widget asWidget();
	}

	private I_Display display = null;
	private EcommerceTree tree = null;

	@Override
	public void go(HasWidgets container) {
		if (UserRole.Visitor.equals(User.role)) {
			tree.next(Actions.Next);
			return;
		}
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
		if (User.role == UserRole.Visitor)
			tree.next(Actions.Next);
		else
			registerListing();
	}

	private void registerListing() {
		Data data = tree.getData(Field.ListingType);
		EcommerceTree.ListingType viewOrderType = EcommerceTree.ListingType
				.values()[data.asInteger()];

		switch (viewOrderType) {
		case PRIVATE:
		case PUBLIC:
			ServerAPI.registerViewOrder(User.id, viewOrderType,
					getSuiteInfo(Field.SuiteInfo).getMLS(),
					getSuiteInfo(Field.SuiteInfo).getId(), User.SID,
					new I_RequestCallback() {

						@Override
						public void onSuccess(JSONObject obj) {
							viewOrderId = obj.get("viewOrder-id").isString()
									.stringValue();
							viewOrderId = viewOrderId.replace("-", "");
							requestSuite();
						}

						@Override
						public void onError(String errMessage) {
							// Error message.
							final DialogBox warning = new DialogBox();
							WarningPresenter.I_Display widget = new WarningView();
							if (errMessage.isEmpty())
								widget.setMessage("Error while creating listing.");
							else
								widget.setMessage("Error while creating listing - "
										+ errMessage);
							widget.getOK().addClickHandler(new ClickHandler() {

								@Override
								public void onClick(ClickEvent event) {
									warning.hide();
									tree.next(Actions.Cancel);
								}
							});
							warning.add(widget.asWidget());
							warning.setGlassEnabled(true);
							warning.center();
						}
					});
			break;
		// break;
		}

	}

	private void requestSuite() {
		ServerAPI.requestSuite(getSuiteInfo(Field.SuiteInfo).getId(), User.SID,
				new I_RequestCallback() {

					@Override
					public void onSuccess(JSONObject obj) {
						String original = obj.toString();
						Data data = tree.getData(Field.SuiteInfo);
						if (data != null) {
							SuiteInfo info = new SuiteInfo();
							info.fromJSONObject(data.asJSONObject());

							NumberFormat fmt = NumberFormat.getDecimalFormat();
							fmt.overrideFractionDigits(2);
							String currentPrice = String.valueOf(info
									.getPrice());
							obj.put("currentPrice",
									new JSONString(currentPrice));
							fmt = NumberFormat.getDecimalFormat();
							fmt.overrideFractionDigits(2);
							currentPrice = fmt.format(info.getPrice());
							obj.put("currentPriceDisplay", new JSONString("$"
									+ currentPrice));

							obj.put("currentPriceCurrency", new JSONString(
									"CAD"));

							EcommerceTree.ListingType viewOrderType = EcommerceTree.ListingType
									.values()[tree.getData(Field.ListingType)
									.asInteger()];
							if (EcommerceTree.ListingType.PUBLIC
									.equals(viewOrderType)) {
								if (info.getStatus() == Status.AvailableRent)
									obj.put("status", new JSONString(
											"AvailableRent"));
								if (info.getStatus() == Status.AvailableResale)
									obj.put("status", new JSONString(
											"AvailableResale"));
							}
						}
						GWT.log("Original data: " + original);
						GWT.log("Updated data: " + obj.toString());
						if (!original.equals(obj.toString()))
							updateSuite(obj.toString());
					}

					@Override
					public void onError(String errMessage) {
						ServerProxy.deleteOrder(viewOrderId, User.SID, new RequestCallback() {
							
							@Override
							public void onResponseReceived(Request request, Response response) {
								tree.next(Actions.Cancel);
							}
							
							@Override
							public void onError(Request request, Throwable exception) {
								
							}
						});
						// Error message.
						final DialogBox warning = new DialogBox();
						WarningPresenter.I_Display widget = new WarningView();
						if (errMessage.isEmpty())
							widget.setMessage("Error while requesting suite.");
						else
							widget.setMessage("Error while requesting suite - "
									+ errMessage);
						widget.getOK().addClickHandler(new ClickHandler() {

							@Override
							public void onClick(ClickEvent event) {
								warning.hide();
								tree.next(Actions.Cancel);
							}
						});
						warning.add(widget.asWidget());
						warning.setGlassEnabled(true);
						warning.center();

					}
				});
	}

	private void updateSuite(String data) {
		ServerAPI.updateSuite(getSuiteInfo(Field.SuiteInfo).getId(), User.SID,
				data, new I_RequestCallback() {

					@Override
					public void onSuccess(JSONObject obj) {
						// TODO Auto-generated method stub
						tree.next(Actions.Next);
					}

					@Override
					public void onError(String errMessage) {
						ServerProxy.deleteOrder(viewOrderId, User.SID, new RequestCallback() {
							
							@Override
							public void onResponseReceived(Request request, Response response) {
								tree.next(Actions.Cancel);
							}
							
							@Override
							public void onError(Request request, Throwable exception) {
								
							}
						});
						// Error message.
						final DialogBox warning = new DialogBox();
						WarningPresenter.I_Display widget = new WarningView();
						if (errMessage.isEmpty())
							widget.setMessage("Error while updating suite.");
						else
							widget.setMessage("Error while updating suite - "
									+ errMessage);
						widget.getOK().addClickHandler(new ClickHandler() {

							@Override
							public void onClick(ClickEvent event) {
								warning.hide();
								tree.next(Actions.Cancel);
							}
						});
						warning.add(widget.asWidget());
						warning.setGlassEnabled(true);
						warning.center();

					}
				});
	}

	// Data utils
	private SuiteInfo getSuiteInfo(Field key) {
		Data data = tree.getData(key);
		if (data != null) {
			SuiteInfo info = new SuiteInfo();
			info.fromJSONObject(data.asJSONObject());
			return info;
		}
		return null;
	}

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
