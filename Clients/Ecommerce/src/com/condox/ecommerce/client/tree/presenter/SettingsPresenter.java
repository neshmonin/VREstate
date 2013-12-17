package com.condox.ecommerce.client.tree.presenter;

import com.condox.clientshared.communication.GET;
import com.condox.clientshared.communication.Options;
import com.condox.clientshared.communication.User;
import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.container.I_Container;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.EcommerceTree.NodeStates;
import com.condox.ecommerce.client.tree.node.SettingsNode;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.Widget;

public class SettingsPresenter implements I_Presenter {

	public static interface I_Display extends I_Contained {
		void setPresenter(SettingsPresenter presenter);

		Widget asWidget();
	}

	private I_Display display = null;
	private SettingsNode node = null;

	public SettingsPresenter(I_Display newDisplay, SettingsNode newNode) {
		display = newDisplay;
		display.setPresenter(this);
		node = newNode;
	}

	@Override
	public void go(I_Container container) {
		container.clear();
		container.add((I_Contained)display);
	}

//	Events
	public void onClose() {
		node.setState(NodeStates.Close);
		node.next();
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
				Window.alert("Error while changing login email.");
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
			}

			@Override
			public void onError(Request request, Throwable exception) {
				Window.alert("Error while changing password");
			}});
		
	}

}