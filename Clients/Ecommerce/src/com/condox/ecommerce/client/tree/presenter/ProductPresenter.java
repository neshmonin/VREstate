package com.condox.ecommerce.client.tree.presenter;

import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.HasWidgets;

public class ProductPresenter implements I_Presenter {

//	public static interface I_Display extends I_Contained {
//		void setPresenter(ProductPresenter presenter);
//
//		boolean getListing();
//
//		boolean getPrivate();
//
//		boolean getShared();
//
//		boolean getLayout();
//
//		void setListing(boolean value);
//
//		void setPrivate(boolean value);
//
//		void setShared(boolean value);
//
//		void setLayout(boolean value);
//
//		Widget asWidget();
//	}
//
//	private I_Display display = null;
//	private ProductModel model = null;
//
//	public ProductPresenter(I_Display display, ProductModel model) {
//		this.display = display;
//		this.display.setPresenter(this);
//		this.model = model;
//	}

	@Override
	public void go(HasWidgets container) {
//		String productType = EcommerceTree.get(Field.ProductType).asString();
//		display.setListing(productType != "Layout");
//		display.setPrivate(productType == "ListingPrivate");
//		display.setShared(productType == "ListingPublic");
//		display.setLayout(productType == "Layout");
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
//		if (display.getListing()) {
//			if (display.getPrivate()) {
//				EcommerceTree.set(Field.ProductType, new Data("ListingPrivate"));
//				EcommerceTree.transitState(State.PrivateListing);
//			}
//			else {
//				EcommerceTree.set(Field.ProductType, new Data("ListingPublic"));
//				EcommerceTree.transitState(State.PublicListing);
//			}
//		} else {
//			EcommerceTree.set(Field.ProductType, new Data("Layout"));
//			EcommerceTree.transitState(State.Layout);
//		}
//
//		model.next();
//	}
}
