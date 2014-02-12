package com.condox.ecommerce.client.tree.presenter;

import com.condox.clientshared.abstractview.Log;
import com.condox.clientshared.communication.GET;
import com.condox.clientshared.communication.Options;
import com.condox.clientshared.communication.User;
import com.condox.clientshared.document.SuiteInfo;
import com.condox.clientshared.tree.Data;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Actions;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.HasWidgets;
import com.google.gwt.user.client.ui.Widget;

public class SubmitGuestEmailPresenter implements I_Presenter {

	public static interface I_Display {
		void setPresenter(SubmitGuestEmailPresenter presenter);

		String getLogin();
		
		void setLogin(String value);

		Widget asWidget();
	}

	private I_Display display = null;
	private EcommerceTree tree = null;

	@Override
	public void go(HasWidgets container) {
		container.clear();
		container.add(display.asWidget());
		
//		Data data = tree.getData(Field.UserInfo); 
//		if (data != null) {
//			UserInfo info = new UserInfo();
//			info.fromJSONObject(data.asJSONObject());
//			display.setLogin(info.getLogin());
//		}
	}
	
	// TODO !!
	public static String guestEmail = "<none>"; 
	// Events
	public void onSubmit() {
		guestEmail = display.getLogin();
			
		String url = Options.URL_VRT;
		url += "program?";
		url += "&q=register";
		url += "&entity=viewOrder";
		url += "&ownerEmail=" + display.getLogin();
		// url += "&pr=" + User.id;
		url += "&daysValid=1";
		url += "&product=prl";
		url += "&options=fp"; // TODO
		url += "&paymentPending=CAD49.99";
		
		// MLS#
		String mls = getSuiteInfo(Field.SuiteInfo).getMLS();
		url += "&mls_id=" + mls;
		url += "&propertyType=suite"; // TODO
		url += "&propertyId=" + getSuiteInfo(Field.SuiteInfo).getId();
		url += "&sid=" + User.SID;
		GET.send(url, new RequestCallback() {

			@Override
			public void onResponseReceived(Request request, Response response) {
				Log.write(response.getStatusCode() + ": "
						+ response.getStatusText());
				if (response.getStatusCode() == 200)
					tree.next(Actions.Submit);
			}

			@Override
			public void onError(Request request, Throwable exception) {
				// TODO Auto-generated method stub
				
			}
			});

	}
		
		private SuiteInfo getSuiteInfo(Field key) {
			Data data = tree.getData(key);
			if (data != null) {
				SuiteInfo info = new SuiteInfo();
				info.fromJSONObject(data.asJSONObject());
				return info;
			}
			return null;
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

	public void onClose() {
		tree.next(Actions.Close);
	}
}
