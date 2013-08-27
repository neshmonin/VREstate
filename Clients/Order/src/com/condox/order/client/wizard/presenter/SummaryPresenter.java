package com.condox.order.client.wizard.presenter;

import com.condox.clientshared.abstractview.Log;
import com.condox.order.client.I_Presenter;
import com.condox.order.client.wizard.I_WizardStep;
import com.condox.order.client.wizard.model.BuildingsModel;
import com.condox.order.client.wizard.model.EmailModel;
import com.condox.order.client.wizard.model.LoginModel;
import com.condox.order.client.wizard.model.SummaryModel;
import com.google.gwt.user.client.ui.HasWidgets;
import com.google.gwt.user.client.ui.Widget;

public class SummaryPresenter implements I_Presenter {

	public static interface I_Display {
		void setPresenter(SummaryPresenter presenter);

		Widget asWidget();
	}

	private I_Display display = null;
	private SummaryModel model = null;

	public SummaryPresenter(I_Display display, SummaryModel model) {
		this.model = model;
		this.display = display;
		this.display.setPresenter(this);
	}

	@Override
	public void go(HasWidgets container) {
		container.clear();
		container.add(this.display.asWidget());
	}

	public void onPrev() {
		model.prev();
	}

	public String getSummary() {
		// **********************************
		// TODO —генерировать строку дл€ Summary
		String sid = "";
		String buildingId = "";
		I_WizardStep step = model;
		Log.write("1");
		while (step != null) {
			Log.write(step.getClass().getName());
			try {
				sid = ((LoginModel) step).getUserSid();
			} catch (Exception e) {
				e.printStackTrace();
			}
			step = step.getPrevStep();
		}
		Log.write("2");
		step = model;
		while (step != null) {
			try {
				buildingId = String.valueOf(((BuildingsModel) step)
						.getSelectedId());
				Log.write(buildingId);
			} catch (Exception e) {
				e.printStackTrace();
			}
			step = step.getPrevStep();
		}
		String html = "Summary: <br /> Session id: " + sid + "<br />"
				+ "Selected building: " + buildingId + "<br />";
		// **********************************
		return html;
	}
}
