package com.condox.ecommerce.client.tree.presenter;

import com.condox.clientshared.communication.User;
import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.container.I_Container;
import com.condox.clientshared.document.SuiteInfo;
import com.condox.clientshared.tree.Data;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.ServerProxy;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Actions;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.node.UsingMLSNode;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.json.client.JSONArray;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.Widget;

public class UsingMLSPresenter implements I_Presenter {

	public static interface I_Display extends I_Contained {
		void setPresenter(UsingMLSPresenter presenter);

		boolean isUsingMLS();

		void setUsingMLS(boolean value);

		String getMLS();

		void setMLS(String value);

		Widget asWidget();
	}

	private I_Display display = null;
	private UsingMLSNode node = null;
	private EcommerceTree tree = null;

	public UsingMLSPresenter(I_Display newDisplay, UsingMLSNode newNode) {
		display = newDisplay;
		display.setPresenter(this);
		node = newNode;
		tree = node.getTree();
	}

	@Override
	public void go(I_Container container) {
		Data data = tree.getData(Field.UsingMLS);
		if (data != null) {
			display.setUsingMLS(data.asBoolean());
		}
		data = tree.getData(Field.SuiteMLS);
		if (data != null) {
			display.setMLS(data.asString());
		}

		container.clear();
		container.add((I_Contained) display);
	}

	// Navigation events
	public void onCancel() {
		node.next(Actions.Cancel);
	}

	public void onNext() {
		if (display.isUsingMLS()) {
			String mls = display.getMLS();
			ServerProxy.getSuiteInfoFromMLS(mls, User.SID,
					new RequestCallback() {

						@Override
						public void onResponseReceived(Request request,
								Response response) {
							if (!response.getText().isEmpty()) {
								JSONObject obj = JSONParser.parseStrict(
										response.getText()).isObject();
								JSONArray arr = obj.get("inventory").isArray();
								SuiteInfo info = new SuiteInfo();
								info.fromJSONObject(arr.get(0).isObject());
								tree.setData(Field.SuiteSelected,
										new Data(info));
								tree.setData(Field.UsingMLS,
										new Data(display.isUsingMLS()));
								tree.setData(Field.SuiteMLS,
										new Data(display.getMLS()));
								node.next(Actions.UsingMLS);
							} else {Window.alert("Please, check your MLS");};
						}

						@Override
						public void onError(Request request, Throwable exception) {
							// TODO Auto-generated method stub

						}
					});
		} else
			node.next(Actions.UsingAddress);

	}

	public void onPrev() {
		node.next(Actions.Prev);
	}
}
