package com.condox.ecommerce.client.tree.presenter;

import com.condox.clientshared.communication.GET;
import com.condox.clientshared.communication.Options;
import com.condox.clientshared.communication.User;
import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.tree.Data;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.UserInfo;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Actions;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.view.WarningView;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.HasWidgets;
import com.google.gwt.user.client.ui.Widget;

public class SettingsPresenter implements I_Presenter {

	public static interface I_Display extends I_Contained {
		void setPresenter(SettingsPresenter presenter);
		void setEmail(String newEmail);
		Widget asWidget();
	}

	private I_Display display = null;
	private EcommerceTree tree = null;

	@Override
	public void go(HasWidgets container) {
		Data data = tree.getData(Field.UserInfo);
		if (data != null) {
			UserInfo info = new UserInfo();
			info.fromJSONObject(data.asJSONObject());
			display.setEmail(info.getEmail());
		}
		container.clear();
		container.add(display.asWidget());
	}

//	Events
	public void onClose() {
		tree.next(Actions.Close);
	}

	public void onChangeEmail(String newEmail) {
		// TODO validate newEmail
		String url = Options.URL_VRT + "/program?q=chlogin" +
				"&sid=" + User.SID + 
				"&newLogin=" + newEmail;
		GET.send(url, new RequestCallback(){

			@Override
			public void onResponseReceived(Request request, Response response) {
				Window.alert("Please check your mail and access sent link.");
			}

			@Override
			public void onError(Request request, Throwable exception) {
				Window.alert("Error while changing primary email.");
			}});
	}

	public void onChangePassword(String oldPassword, String newPassword,
			String newPassword2) {
		// TODO validate params
		if (!newPassword.equals(newPassword2)) {
			Window.alert("Password not equals! Please correct and try again.");
			return;
		}
		
		String url = Options.URL_VRT + "/program?q=chpwd" +
				"&sid=" + User.SID + 
				"&pwd=" + oldPassword +
				"&npwd=" + newPassword;
		GET.send(url, new RequestCallback(){

			@Override
			public void onResponseReceived(Request request, Response response) {
				if (response.getStatusCode() == 200)
					Window.alert("Password changed");
				else {
					WarningPresenter warning = new WarningPresenter(new WarningView(), null);
					String message = "Status code: " + response.getStatusCode() + "<br />";
					message += "Status text: " + response.getStatusText() + "<br />";
					message += "Response text: " + response.getText();
					warning.setMessage(message);
					warning.go(null);
				}
			}

			@Override
			public void onError(Request request, Throwable exception) {
				Window.alert("Error while changing password");
			}});
		
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
