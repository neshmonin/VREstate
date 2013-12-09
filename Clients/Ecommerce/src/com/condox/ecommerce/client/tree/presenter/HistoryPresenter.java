package com.condox.ecommerce.client.tree.presenter;

import com.condox.clientshared.communication.GET;
import com.condox.clientshared.communication.Options;
import com.condox.clientshared.communication.User;
import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.container.I_Container;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.model.HistoryNode;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.user.client.ui.Widget;

public class HistoryPresenter implements I_Presenter {

	public static interface I_Display extends I_Contained {
		void setPresenter(HistoryPresenter presenter);

		void setHistoryData(String data);

		Widget asWidget();
	}

	private I_Display display = null;
	@SuppressWarnings("unused")
	private HistoryNode node = null;

	public HistoryPresenter(I_Display display, HistoryNode node) {
		this.display = display;
		this.display.setPresenter(this);
		this.node = node;
	}

	@Override
	public void go(final I_Container container) {
		String request = Options.URL_VRT + "data/ft?"
				+ "userId=" + EcommerceTree.get(Field.UserId).asString()
				+ "&sid=" + User.SID;
		GET.send(request, new RequestCallback() {

			@Override
			public void onResponseReceived(Request request, Response response) {
				if (!response.getText().isEmpty())
					display.setHistoryData(response.getText());
				container.clear();
				container.add((I_Contained)display);
			}

			@Override
			public void onError(Request request, Throwable exception) {
				// TODO Auto-generated method stub
				
			}});
	}

//	public void onLogin() {
//		final String uid = display.getUserLogin();
//		final String pwd = display.getUserPassword();
//		EcommerceTree.set(Field.UserLogin, new Data(uid));
//		EcommerceTree.set(Field.UserPassword, new Data(pwd));
//		
////		if (!model.isValid()) {
////			Window.alert("Not valid! Please, check and try again!");
////			return;
////		}
//		
//		String role = "visitor";
////		String url = Options.getUserLogin(uid, pwd, role);
////		EcommerceTree.transitState(State.Guest); // for role == "visitor"
//
////		url = URL.encode(url);
//		
//		String request = Options.URL_VRT + "program?q=login&role=visitor"
//				+ "&uid=" + uid
//				+ "&pwd=" + pwd;
//
//		// GET.send(url);
//		User.Login(this, request);
//	}
//
//	@Override
//	public void onLoginSucceed() {
//		EcommerceTree.transitState(State.Agent);
//		if (display.getUserLogin().equalsIgnoreCase("web"))
//			if (display.getUserPassword().equalsIgnoreCase("web"))
//				EcommerceTree.transitState(State.Guest);
//		model.next();
//	}
//
//	@Override
//	public void onLoginFailed(Throwable exception) {}
}
