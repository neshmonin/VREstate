package com.condox.order.client.presenter;

import java.util.ArrayList;
import java.util.List;

import com.condox.order.client.context.BaseContext;
import com.condox.order.client.context.ContextTree;
import com.condox.order.client.context.IContext.Types;
import com.condox.order.client.utils.GET;
import com.condox.order.client.utils.Globals;
import com.condox.order.client.view.IView;
import com.condox.order.client.view.IViewContainer;
import com.condox.order.shared.BuildingInfo;
import com.google.gwt.event.shared.EventBus;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.json.client.JSONArray;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.Widget;

public class BuildingsPresenter implements IPresenter {

	public interface IDisplay extends IView {
		BuildingInfo getSelectedBuilding();

		void setPresenter(BuildingsPresenter presenter);

		void setData(List<BuildingInfo> data, BuildingInfo selected);

		Widget asWidget();
	}

//	private EventBus eventBus;
	private ContextTree tree;
	private IDisplay display;
	private List<BuildingInfo> data = new ArrayList<BuildingInfo>();
	private BuildingInfo selected;

	public BuildingsPresenter(EventBus eventBus, ContextTree tree, IDisplay display) {
//		this.eventBus = eventBus;
		this.tree = tree;
		this.display = display;
		this.display.setPresenter(this);
	}

	private void updateData() {
		GET.send(Globals.getBuildingsListRequest(), new RequestCallback() {

			@Override
			public void onResponseReceived(Request request, Response response) {
				String json = response.getText();
				JSONObject obj = JSONParser.parseStrict(json).isObject();
				JSONArray arr = obj.get("buildings").isArray();

				Integer id = null;
				try {
					id = Integer.valueOf(tree.getValue("building.id"));
				} catch (NumberFormatException e1) {
//					e1.printStackTrace();
				}
					
				data.clear();
				for (int index = 0; index < arr.size(); index++) {
					BuildingInfo info = new BuildingInfo();
					info.Parse(arr.get(index));
					data.add(info);
					if (id != null)
						if (id.equals(info.getId()))
							selected = info;
				}
				display.setData(data, selected);
			}

			@Override
			public void onError(Request request, Throwable exception) {

			}
		});
	}

	public void onNext() {
		BuildingInfo info = display.getSelectedBuilding();
		try {
			tree.setValue("building.id", String.valueOf(info.getId()));
			tree.setValue("building.name", info.getName());
			tree.setValue("building.street", info.getStreet());
		} catch (Exception e) {
			Window.alert("Please, select a building!");
			return;
//			e.printStackTrace();
		}
		tree.next(new BaseContext(Types.SUITES));
	}

	@Override
	public void stop() {

	}

	@Override
	public void go(IViewContainer container) {
		container.setView(display);
		updateData();
	}
}
