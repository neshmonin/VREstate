package com.condox.ecommerce.client.tree.model;

import com.condox.clientshared.communication.GET;
import com.condox.clientshared.communication.Options;
import com.condox.clientshared.communication.User;
import com.condox.clientshared.container.I_Container;
import com.condox.clientshared.document.SuiteInfo;
import com.condox.clientshared.tree.Data;
import com.condox.clientshared.tree.I_TreeNode;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.EcommerceTreeNode;
import com.condox.ecommerce.client.tree.presenter.ListingOptionsPresenter;
import com.condox.ecommerce.client.tree.view.ListingOptionsView;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.json.client.JSONArray;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;

public class ListingOptionsModel extends EcommerceTreeNode {

	public static String simpleName = "ListingOptionsModel";

	public ListingOptionsModel() {}

	/*
	 * private String role = ""; private String uid = ""; private String pwd =
	 * ""; private String sid = "";
	 */

	private I_Container container = null;
	ListingOptionsPresenter presenter = new ListingOptionsPresenter(
			new ListingOptionsView(), this);

	@Override
	public void go(I_Container container) {
		this.container = container;
		updateData();
		super.go(container);
	}

	public void next() {
		I_TreeNode node = EcommerceTree.getNextNode();
		node.go(container);
	}

	public void prev() {
		I_TreeNode node = EcommerceTree.getPrevNode();
		if (node != null)
			node.go(container);
	}

	private void updateData() {
		String mls = EcommerceTree.get(Field.MLS).asString();

		if (!mls.isEmpty()) {
			String url = Options.URL_VRT + "data/inventory?";
			url += "mlsId=" + mls;
			url += "&sid=" + User.SID;
			GET.send(url, new RequestCallback() {

				@Override
				public void onResponseReceived(Request request,
						Response response) {
					// Log.write("HTTP ok:" + response.getStatusCode());
					if (response.getStatusCode() == 200) {
						String json = response.getText();
						JSONObject obj = JSONParser.parseStrict(json)
								.isObject();
						JSONArray arr = obj.get("inventory").isArray();
						SuiteInfo info = new SuiteInfo();
						info.fromJSONObject(arr.get(0).isObject());

						EcommerceTree.set(Field.MoreInfoURL, new Data(info.getMoreInfoURL()));
						EcommerceTree.set(Field.VirtualTourURL, new Data(info.getVirtualTourURL()));
						EcommerceTree.set(Field.SuiteId, new Data(info.getId()));
						EcommerceTree.set(Field.Address, new Data(info.getAddress()));
						
						presenter.go(container);
					} else
						new ModalMessage(
								"Sorry, this listing cannot be located in the 3D Condo Explorer's Database."
										+ "Please try another MLS#",
								"warning-icon.png").center();
					// CreateTable();
				}

				@Override
				public void onError(Request request, Throwable exception) {
					// Log.write("HTTP errot:" + response.getStatusCode());
				}
			});
		} else
			presenter.go(container);
	}

	@Override
	public String getNavURL() {
		return "Options";
	}

	@Override
	public String getStateString() {
		return simpleName + "." + getState();
	}
}
