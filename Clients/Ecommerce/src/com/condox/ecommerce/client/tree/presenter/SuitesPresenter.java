package com.condox.ecommerce.client.tree.presenter;

import com.condox.clientshared.container.I_Container;
import com.condox.ecommerce.client.I_Presenter;

public class SuitesPresenter implements I_Presenter {

//	public interface IDisplay extends I_Contained {
//		void setPresenter(SuitesPresenter presenter);
//
//		void setData(List<SuiteInfo> data);
//
//		Widget asWidget();
//
//		String getSuiteName();
//
//		String getSuiteFloorplan();
//
//		SuiteInfo getSelectedSuite();
//	}
//
//	private IDisplay display = null;
//	private List<SuiteInfo> data = new ArrayList<SuiteInfo>();
//	private SuitesModel model = null;
//
//	public SuitesPresenter(IDisplay display, SuitesModel model) {
//		this.display = display;
//		this.display.setPresenter(this);
//		this.model = model;
//	}
//
//	private void updateData() {
//		display.setData(null);
//		int buildingId = EcommerceTree.get(Field.BuildingID).asInteger();
//		
//		//	Sample: "https://vrt.3dcondox.com/data/inventory?"
//		String url = Options.URL_VRT;
//		url += "data/inventory?";
//		url += "building=" + buildingId;
//		url += "&sid=" + User.SID;
//		GET.send(url, new RequestCallback() {
//
//			@Override
//			public void onResponseReceived(Request request, Response response) {
//				// Log.write("answer:" + response.getText());
//				String json = response.getText();
//				JSONObject obj = JSONParser.parseStrict(json).isObject();
//				JSONArray arr = obj.get("inventory").isArray();
//				for (int index = 0; index < arr.size(); index++) {
//					SuiteInfo info = new SuiteInfo();
//					info.fromJSONObject(arr.get(index).isObject());
//					data.add(info);
//				}
//				display.setData(data);
//				// CreateTable();
//			}
//
//			@Override
//			public void onError(Request request, Throwable exception) {
//			}
//		});
//	}
//
//
//	public String getSelectedBuildingStreet() {
//		return "Floor:";//tree.getString("buildings.selected.street");
//	}

	@Override
	public void go(I_Container container) {
//		container.clear();
//		container.add((I_Contained)display);
//		updateData();
	}

//	public void onPrev() {
//		if (display.getSelectedSuite() != null) {
//			EcommerceTree.set(Field.SuiteId, new Data(display
//					.getSelectedSuite().getId()));
//			EcommerceTree.set(Field.SuiteName, new Data(display
//					.getSelectedSuite().getName()));
//			EcommerceTree.set(Field.Address, new Data(display
//					.getSelectedSuite().getAddress()));
//			EcommerceTree.set(Field.MLS, new Data(display.getSelectedSuite()
//					.getMLS()));
//		}
//		model.prev();
//	}
//
//	public void onNext() {
//		EcommerceTree.set(Field.SuiteId, new Data(display.getSelectedSuite().getId()));
//		EcommerceTree.set(Field.SuiteName, new Data(display.getSelectedSuite().getName()));
//		EcommerceTree.set(Field.Address, new Data(display.getSelectedSuite().getAddress()));
//		EcommerceTree.set(Field.MLS, new Data(display.getSelectedSuite().getMLS()));
//		EcommerceTree.transitState(State.SuiteReady);
//		model.next();
//	}

}
