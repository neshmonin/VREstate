package com.condox.order.client.wizard.presenter;

import com.condox.clientshared.communication.GET;
import com.condox.clientshared.communication.Options;
import com.condox.order.client.I_Presenter;
import com.condox.order.client.wizard.model.LoginModel;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.http.client.URL;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.HasWidgets;
import com.google.gwt.user.client.ui.Widget;

public class LoginPresenter implements I_Presenter {

	public static interface I_Display {
		void setPresenter(LoginPresenter presenter);

		String getUserLogin();

		String getUserPassword();

		Widget asWidget();
	}

	private I_Display display = null;
	private LoginModel model = null;

	public LoginPresenter(I_Display display, LoginModel model) {
		this.display = display;
		this.display.setPresenter(this);
		this.model = model;
	}

	@Override
	public void go(HasWidgets container) {
		container.clear();
		container.add(this.display.asWidget());
	}

	public void onEnter() {
		final String uid = display.getUserLogin();
		final String pwd = display.getUserPassword();
		model.setUserLogin(uid);
		model.setUserPassword(pwd);
		
		if (!model.isValid()) {
			Window.alert("Not valid! Please, check and try again!");
			return;
		}
		
		String role = "visitor";
		String url = Options.getUserLogin(uid, pwd, role);
		url = URL.encode(url);

		// GET.send(url);
		GET.send(url, new RequestCallback() {

			@Override
			public void onResponseReceived(Request request, Response response) {
				JSONObject obj = JSONParser.parseStrict(response.getText()).isObject();
				String sid = obj.get("sid").isString().stringValue();
				model.setUserSid(sid);
				model.next();
			}

			@Override
			public void onError(Request request, Throwable exception) {

			}
		});
	}
}
