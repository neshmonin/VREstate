package com.condox.ecommerce.client;

import com.condox.clientshared.container.I_Container;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.HasWidgets;

public interface I_Presenter {
	void setView(Composite view);
	void setTree(EcommerceTree tree);
	void go(HasWidgets container);
}
