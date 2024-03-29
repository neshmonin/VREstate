package com.condox.ecommerce.client;

import com.condox.clientshared.abstractview.Log;
import com.condox.clientshared.communication.I_Login;
import com.condox.clientshared.communication.Options;
import com.condox.clientshared.communication.User;
import com.condox.clientshared.communication.User.UserRole;
import com.condox.ecommerce.client.resources.CSS;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.google.gwt.core.client.EntryPoint;
import com.google.gwt.event.logical.shared.ValueChangeEvent;
import com.google.gwt.event.logical.shared.ValueChangeHandler;
import com.google.gwt.event.shared.HandlerManager;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.user.client.History;

/**
 * Entry point classes define <code>onModuleLoad()</code>.
 */
public class Ecommerce implements EntryPoint, ValueChangeHandler<String> {
//	public enum Modes {
//		noTest, testDeleteOrder, testUpdateOrder
//	}
//
//	public static Modes mode = Modes.noTest;
	
	public static HandlerManager eventBus = new HandlerManager(null);
	public static AppController appViewer = new AppController(/*rpcService, */eventBus);
	
	/**
	 * This is the entry point method.
	 */
	public void onModuleLoad() {
		CSS.Instance.colors().ensureInjected();
		Options.Init();
		History.addValueChangeHandler(this);
		 History.newItem("login", false);
		 History.fireCurrentHistoryState();
	}

	@Override
	public void onValueChange(ValueChangeEvent<String> event) {
		String token = event.getValue();
		if ("login".equals(token))
			startWizard();
		else if ("orderNow".equals(token))
			startWizard();
//		else if ("testUpdateProfile".equals(token))
//			testUpdateProfile();
		History.newItem("", false);
	}

	private void startWizard() {
		EcommerceTree tree = new EcommerceTree();
		tree.config();
		tree.next(null);
	}

//	private void testUpdateProfile() {
//		String login = "adminan";
//		String password = "smelatoronto";
//		UserRole role = UserRole.SuperAdmin;
//		User.Login(new I_Login() {
//
//			@Override
//			public void onLoginSucceed() {
//				ServerProxy.getUserInfo(User.id, User.SID,
//						new RequestCallback() {
//
//							@Override
//							public void onResponseReceived(Request request,
//									Response response) {
//								JSONObject obj = JSONParser.parseLenient(
//										response.getText()).isObject();
//								UserInfo info = new UserInfo();
//								info.fromJSONObject(obj);
//								String nick = info.getNickName();
//								if (nick.startsWith("<changed>"))
//									nick = nick.substring(9);
//								else
//									nick = "<changed>" + nick;
//								info.setNickName(nick);
//
//								ServerProxy.setUserInfo(User.id, info
//										.toJSONObject().toString(), User.SID,
//										new RequestCallback() {
//
//											@Override
//											public void onResponseReceived(
//													Request request,
//													Response response) {
//												// TODO Auto-generated method
//												// stub
//												Log.popup();
//											}
//
//											@Override
//											public void onError(
//													Request request,
//													Throwable exception) {
//												// TODO Auto-generated method
//												// stub
//
//											}
//										});
//
//							}
//
//							@Override
//							public void onError(Request request,
//									Throwable exception) {
//
//							}
//						});
//			}
//
//			@Override
//			public void onLoginFailed(Throwable exception) {
//
//			}
//		}, login, password, role);
//	}
}
