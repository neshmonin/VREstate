package com.condox.ecommerce.client;

import com.condox.clientshared.abstractview.Log;
import com.condox.clientshared.communication.Options;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.PopupContainer;
import com.google.gwt.core.client.EntryPoint;
import com.google.gwt.event.logical.shared.ValueChangeEvent;
import com.google.gwt.event.logical.shared.ValueChangeHandler;
import com.google.gwt.user.client.History;

/**
 * Entry point classes define <code>onModuleLoad()</code>.
 */
public class Ecommerce implements EntryPoint, ValueChangeHandler<String> {
	/**
	 * This is the entry point method.
	 */
	public void onModuleLoad() {
		Options.Init();
		History.addValueChangeHandler(this);
//		History.newItem("login");
	}

	@Override
	public void onValueChange(ValueChangeEvent<String> event) {
		String token = event.getValue();
//		Log.write(token);
		if ("login".equals(token))
			startWizard();
		else if("orderNow".equals(token))
			startWizard();
		History.newItem("", false);
	}
	
	private void startWizard() {
		EcommerceTree tree = new EcommerceTree();
		tree.go(new PopupContainer());
//		tree.activate(new PopupContainer());
	}
}
