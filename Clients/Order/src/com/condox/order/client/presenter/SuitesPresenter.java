package com.condox.order.client.presenter;

import java.util.ArrayList;
import java.util.List;

import com.condox.clientshared.communication.GET;
import com.condox.clientshared.document.SuiteInfo;
import com.condox.order.client.context.BaseContext;
import com.condox.order.client.context.ContextTree;
import com.condox.order.client.context.IContext.Types;
import com.condox.order.client.utils.Globals;
import com.condox.order.client.view.IView;
import com.condox.order.client.view.IViewContainer;
import com.google.gwt.event.shared.EventBus;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.json.client.JSONArray;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.user.client.ui.Widget;

public class SuitesPresenter implements IPresenter {

	public interface IDisplay extends IView {
		void setPresenter(SuitesPresenter presenter);
		String getSuiteName();
		String getSuiteFloorplan();
		void setData(List<SuiteInfo> data);
		Widget asWidget();
	}

//	private EventBus eventBus;
	private ContextTree tree;
	private IDisplay display;
	private List<SuiteInfo> data = new ArrayList<SuiteInfo>();

	public SuitesPresenter(EventBus eventBus, ContextTree tree, IDisplay display) {
//		this.eventBus = eventBus;
		this.tree = tree;
		this.display = display;
		this.display.setPresenter(this);
	}

//	@Override
//	public void go(HasWidgets container) {
//		container.clear();
//		container.add(display.asWidget());
//		updateData();
//	}

	private void updateData() {
//		Log.write("--");
//		Log.write("tree: " + tree);
//		Log.write("value: " + tree.getValue("building.id"));
		tree.log();
//		Log.write(tree.getValue("building.id"));
		Integer buildingId = Integer.valueOf(tree.getValue("building.id"));
//		Log.write("--");
		String sid = tree.getValue("user.sid");
//		Log.write("--");

		String request = Globals.getBaseUrl() + "data/inventory?";
		request += "building=" + buildingId;
		request += "&sid=" + sid;
		GET.send(request, new RequestCallback() {

			@Override
			public void onResponseReceived(Request request, Response response) {
//				Log.write("answer:" + response.getText());
				String json = response.getText();
				JSONObject obj = JSONParser.parseStrict(json).isObject();
				JSONArray arr = obj.get("inventory").isArray();
				for (int index = 0; index < arr.size(); index++) {
					SuiteInfo info = new SuiteInfo();
					info.Parse(arr.get(index));
					data.add(info);
				}
				display.setData(data);
				// CreateTable();
			}

			@Override
			public void onError(Request request, Throwable exception) {
				// TODO Auto-generated method stub

			}
		});
	}

	@Override
	public void stop() {
		// TODO Auto-generated method stub
		
	}

	@Override
	public void go(IViewContainer container) {
		// TODO Auto-generated method stub
		container.setView(display);
		updateData();
	}
	
	public void prev() {
		tree.prev();
	}
	
	public void onSubmit() {
		tree.setValue("suite.name", display.getSuiteName());
		tree.setValue("suite.floorplan", display.getSuiteFloorplan());
		tree.next(new BaseContext(Types.SUBMIT));
	}
	
	public String getSelectedBuildingStreet() {
		String street = tree.getValue("building.street");
		street = (street != null)? street : "";
		return street;
	}
}
