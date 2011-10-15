package my.vrestate.client.core;

import java.util.ArrayList;

import my.vrestate.client.Options;
import my.vrestate.client.interfaces.IUser;

import com.google.gwt.core.client.GWT;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.user.client.Window;


public class User implements IUser {
	
	private ArrayList<IUserLoginedListener> UserLoginedListeners = new ArrayList<IUserLoginedListener>();
	
	public void addUserLoginedListener(IUserLoginedListener listener) {
		this.UserLoginedListeners.add(listener);
	}
	public void removeUserLoginedListener(IUserLoginedListener listener) {
		this.UserLoginedListeners.remove(listener);
	}
	private void riseUserLogined() {
		GWT.log("Rising UserLogined ready");
		for (IUserLoginedListener listener : UserLoginedListeners)
			listener.onUserLogined();
	}
	
	public User Instance = this;
	public void Login() {
		GWT.log("User try to login...");
		Requests.Add("program?q=login&uid=admin&pwd=admin",onLogin);
	}
	
	private RequestCallback onLogin = new RequestCallback(){
		@Override
		public void onResponseReceived(Request request,Response response) {
//								Window.alert(response.getText());
			JSONObject answer = (JSONObject) JSONParser.parseStrict(response.getText());
			Options.SessionID = answer.get("sid").toString().replace("\"","");
			GWT.log(answer.get("sid").toString());
			Instance.riseUserLogined();
		}
		@Override
		public void onError(Request request, Throwable exception) {
		};
	};

	public void setSessionId(String session_id) {
	}


	@Override
	public String getSessionID() {
		// TODO Auto-generated method stub
		return null;
	}

};