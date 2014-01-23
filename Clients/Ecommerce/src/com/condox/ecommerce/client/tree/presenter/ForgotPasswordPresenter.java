package com.condox.ecommerce.client.tree.presenter;

import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.container.I_Container;
import com.condox.clientshared.tree.Data;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.UserInfo;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Actions;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.Widget;

public class ForgotPasswordPresenter implements I_Presenter {

	public static interface I_Display extends I_Contained {
		void setPresenter(ForgotPasswordPresenter presenter);

		String getEmail();
		
		void setEmail(String value);

		Widget asWidget();
	}

	private I_Display display = null;
	private EcommerceTree tree = null;

	@Override
	public void go(I_Container container) {
		container.clear();
		container.add((I_Contained)display);
		
		Data data = tree.getData(Field.UserInfo); 
		if (data != null) {
			UserInfo info = new UserInfo();
			info.fromJSONObject(data.asJSONObject());
			display.setEmail(info.getLogin());
		}
	}

	// Events
	public void onSubmit() {
		String email = display.getEmail().trim();
		tree.setData(Field.EmailToRecoverPassword, new Data(email));
		tree.next(Actions.Submit);
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
