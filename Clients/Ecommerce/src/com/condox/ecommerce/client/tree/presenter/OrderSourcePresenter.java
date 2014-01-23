package com.condox.ecommerce.client.tree.presenter;

import java.util.ArrayList;
import java.util.List;

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
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.json.client.JSONArray;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.Widget;

public class OrderSourcePresenter implements I_Presenter {

	public static interface I_Display extends I_Contained {
		void setPresenter(OrderSourcePresenter presenter);

		boolean isUsingMLS();

		void setUsingMLS(boolean value);

		String getMLS();

		void setMLS(String value);
		
		void setMLSSuggestions(List<String> value);

		Widget asWidget();
	}

	private I_Display display = null;
	private EcommerceTree tree = null;

	@Override
	public void go(I_Container container) {
		Data data = tree.getData(Field.UsingMLS);
		if (data != null) {
			display.setUsingMLS(data.asBoolean());
		}
		data = tree.getData(Field.SuiteInfo);
		if (data != null) {
			SuiteInfo info = new SuiteInfo();
			info.fromJSONObject(data.asJSONObject());
			display.setMLS(info.getMLS());
		}

//		List<Data> values1 = tree.getBrotherData(Field.SuiteMLS);
//		List<String> values2 = new ArrayList<String>();
//		for (Data item : values1)
//			if (item != null)
//				values2.add(item.asString());
//		display.setMLSSuggestions(values2);
		
		container.clear();
		container.add((I_Contained) display);
	}

	// Navigation events
	public void onCancel() {
		tree.next(Actions.Cancel);
	}

	public void onNext() {
		tree.setData(Field.UsingMLS, new Data(display.isUsingMLS()));

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
								tree.setData(Field.SuiteInfo,
										new Data(info));
//								tree.setData(Field.SuiteMLS,
//										new Data(display.getMLS()));
								tree.next(Actions.UsingMLS);
							} else {
								Window.alert("Please, check your MLS#");
							}
						}

						@Override
						public void onError(Request request, Throwable exception) {
							// TODO Auto-generated method stub

						}
					});
		} else
			tree.next(Actions.UsingAddress);

	}

	public void onPrev() {
		tree.next(Actions.Prev);
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
