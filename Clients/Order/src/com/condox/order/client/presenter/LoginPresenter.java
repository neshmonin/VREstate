package com.condox.order.client.presenter;

import com.condox.order.client.context.IContext.Types;
import com.condox.order.client.context.BaseContext;
import com.condox.order.client.context.ContextTree;
import com.condox.order.client.utils.GET;
import com.condox.order.client.utils.Globals;
import com.condox.order.client.view.IView;
import com.condox.order.client.view.IViewContainer;
import com.google.gwt.event.shared.EventBus;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.user.client.Timer;
import com.google.gwt.user.client.ui.HasWidgets;
import com.google.gwt.user.client.ui.Widget;

public class LoginPresenter implements IPresenter {

	public interface IDisplay extends IView  {
		void setPresenter(LoginPresenter presenter);

		String getName();

		String getPassword();

		Boolean isGuestMode();

		Widget asWidget();
	}

	private EventBus eventBus;
	private ContextTree tree;
	private IDisplay display;

	public LoginPresenter(EventBus eventBus, ContextTree tree, IDisplay display) {
		this.eventBus = eventBus;
		this.tree = tree;
		this.display = display;
		this.display.setPresenter(this);
	}

//	@Override
//	public void go(IViewContainer container) {
//		container.setView(display);
////		container.clear();
////		container.add(display.asWidget());
//	}

	public void onLogin() {
		String name;
		String password;
		if (display.isGuestMode()) {
			name = "web";
			password = "web";
			tryLogin("visitor", name, password);
			// History.newItem("buildings");
		} else {
			name = display.getName();
			password = display.getPassword();
		}
	}

	private Timer keepAliveThread = null;
	private void tryLogin(String role, final String name, final String password) {
		String request = Globals.getLoginRequest(role, name, password);
		GET.send(request, new RequestCallback() {

			@Override
			public void onResponseReceived(Request request, Response response) {
				// TODO Auto-generated method stub
				JSONObject obj = JSONParser.parseStrict(response.getText()).isObject();
				String sid = obj.get("sid").isString().stringValue();
				//***********************************
				int keepAlivePeriodSec = 300;
				
				keepAliveThread = new Timer() {
					@Override
					public void run() {
						String request = Globals.getBaseUrl() + "program?q=sessionrenew&sid=" + Globals.getSID();
						GET.send(request);
					}
				};
				keepAliveThread.scheduleRepeating(keepAlivePeriodSec*1000);
				//***********************************
				Globals.setSID(sid);
				tree.setValue("user.name", name);
				tree.setValue("user.password", password);
				tree.setValue("user.sid", sid);
				tree.log();
//				History.newItem("buildings");
//				eventBus.fireEvent(new LoginedEvent());
				
				tree.next(new BaseContext(Types.BUILDINGS));
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
	}
}
