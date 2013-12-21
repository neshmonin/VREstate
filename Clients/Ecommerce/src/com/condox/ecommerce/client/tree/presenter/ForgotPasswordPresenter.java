package com.condox.ecommerce.client.tree.presenter;

import com.condox.clientshared.communication.I_Login;
import com.condox.clientshared.communication.User;
import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.container.I_Container;
import com.condox.clientshared.tree.Data;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.EcommerceTree.Actions;
import com.condox.ecommerce.client.tree.node.ForgotPasswordNode;
import com.condox.ecommerce.client.tree.node.LoginNode;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.Widget;

public class ForgotPasswordPresenter implements I_Presenter {

	public static interface I_Display extends I_Contained {
		void setPresenter(ForgotPasswordPresenter presenter);

		String getEmail();
		
		void setEmail(String value);

		Widget asWidget();
	}

	private I_Display display = null;
	private ForgotPasswordNode node = null;
	private EcommerceTree tree = null;

	public ForgotPasswordPresenter(I_Display newDisplay, ForgotPasswordNode newNode) {
		display = newDisplay;
		display.setPresenter(this);
		node = newNode;
		tree = node.getTree();
	}

	@Override
	public void go(I_Container container) {
		container.clear();
		container.add((I_Contained)display);
		
		Data data = node.getTree().getData(Field.UserEmail); 
		if (data != null) {
			String email = data.asString();
			display.setEmail(email);
		}
	}

	// Events
	public void onSubmit() {
		String email = display.getEmail().trim();
		Window.alert("TODO: send mail with new password.");
		tree.setData(Field.UserEmail, new Data(email));
		node.setState(Actions.Submit);
		node.next();
	}
}
