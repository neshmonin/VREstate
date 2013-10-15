package com.condox.order.client.wizard.presenter;

import com.condox.clientshared.document.SuiteInfo;
import com.condox.order.client.I_Presenter;
import com.condox.order.client.wizard.I_WizardStep;
import com.condox.order.client.wizard.I_WizardStep.StepTypes;
import com.condox.order.client.wizard.model.ListingOptionsModel;
import com.condox.order.client.wizard.model.SuitesModel;
import com.google.gwt.user.client.ui.HasWidgets;
import com.google.gwt.user.client.ui.Widget;

public class ListingOptionsPresenter implements I_Presenter {

	public static interface I_Display {
		void setPresenter(ListingOptionsPresenter presenter);
		void setMLS(String value);
		void setVirtualTourURL(String value);
		void setMoreInfoURL(String value);
		String getMls();
		String getVirtualTourURL();
		String getMoreInfoURL();
		Widget asWidget();
	}

	private I_Display display = null;
	private ListingOptionsModel model = null;

	public ListingOptionsPresenter(I_Display display, ListingOptionsModel model) {
		this.display = display;
		this.display.setPresenter(this);
		this.model = model;
	}

	@Override
	public void go(HasWidgets container) {
		container.clear();
		container.add(this.display.asWidget());
		updateData();
	}

	public void onEnter() {
	}

	public void onPrev() {
		model.prev();
		
	}

	public void onNext() {
		model.setMls(display.getMls());
		model.setUrlVirtualTour(display.getVirtualTourURL());
		model.setUrlMoreInfo(display.getMoreInfoURL());
		model.next();
	}
	
	private void updateData() {
//		String sid = "";
		SuiteInfo selectedSuite = null;
		I_WizardStep step = model;
		while (step != null) {
			if (selectedSuite == null) {
				if (step.getStepType() == StepTypes.ListingOptionsModel) {
					selectedSuite = ((ListingOptionsModel)step).getSelectedSuite();
//					virtualTourURL = ((ListingOptionsModel)step).getUrlVirtualTour();
//					moreInfoURL = ((ListingOptionsModel)step).getUrlMoreInfo();
//					mls = ((ListingOptionsModel)step).getMls();
//					suiteId = ((ListingOptionsModel)step).getSuiteId();
				}
				else if (step.getStepType() == StepTypes.SuitesModel) {
					selectedSuite = ((SuitesModel)step).getSelected();
//					virtualTourURL = ((SuitesModel) step).getSelected().getVirtualTourURL();
//					moreInfoURL = ((SuitesModel) step).getSelected().getMoreInfoURL();
//					mls = ((SuitesModel) step).getSelected().getMLS();
				}
			}
			step = step.getPrevStep();
		}
		if (selectedSuite != null) {
			model.setUrlVirtualTour(selectedSuite.getVirtualTourURL());
			model.setUrlMoreInfo(selectedSuite.getMoreInfoURL());
			model.setMls(selectedSuite.getMLS());
			model.setSuiteId(String.valueOf(selectedSuite.getId()));
			display.setVirtualTourURL(model.getUrlVirtualTour());
			display.setMoreInfoURL(model.getUrlMoreInfo());
			display.setMLS(model.getMls());
		}
//		
////		String url = "https://vrt.3dcondox.com/data/inventory?";
//		String url = "https://vrt.3dcondox.com/vre/data/inventory?";
//		url += "building=" + buildingId;
//		url += "&sid=" + sid;
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
//					info.Parse(arr.get(index));
//					data.add(info);
//				}
//				display.setData(data);
//				// CreateTable();
//			}
//
//			@Override
//			public void onError(Request request, Throwable exception) {
//				// TODO Auto-generated method stub
//
//			}
//		});
	}

}