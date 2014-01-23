package com.condox.ecommerce.client.tree.presenter;

import com.condox.clientshared.container.I_Container;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.google.gwt.user.client.ui.Composite;

public class MLSPresenter implements I_Presenter {

//	public static interface I_Display extends I_Contained {
//		void setPresenter(MLSPresenter presenter);
//		String getMLS();
//		void setMLS(String value);
//		Widget asWidget();
//		//----------------
//		void usingMLS(boolean checked);
//		
//		boolean isUsingMLS();
//	}
//	
//	private I_Display display = null;
//	private MLSModel model = null;
//	
//	public MLSPresenter(I_Display display, MLSModel model) {
//		this.display = display;
//		this.display.setPresenter(this);
//		this.model = model;
//		//-------------
//		boolean usingMLS = EcommerceTree.get(Field.USING_MLS).asBoolean();
//		this.display.usingMLS(usingMLS);
//		
//	}

	@Override
	public void go(I_Container container) {
//		display.setMLS("");
//		container.clear();
//		container.add((I_Contained)display);
	}

	@Override
	public void setView(Composite view) {
		// TODO Auto-generated method stub
		
	}

	@Override
	public void setTree(EcommerceTree tree) {
		// TODO Auto-generated method stub
		
	}
	
//	public void onPrev() {
//		model.prev();
//	}
//	
//	public void onNext() {
//		String mls = display.getMLS();
//		EcommerceTree.set(Field.MLS, new Data(mls));
//		boolean isUsingMLS = this.display.isUsingMLS();
//		EcommerceTree.set(Field.USING_MLS, new Data(isUsingMLS));
//		
//		if (mls.isEmpty())
//			EcommerceTree.transitState(State.Address);
//		else
//			EcommerceTree.transitState(State.MLS);
//
//		model.next();
//	}
}
