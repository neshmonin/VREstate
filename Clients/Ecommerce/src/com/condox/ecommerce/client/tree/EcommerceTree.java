package com.condox.ecommerce.client.tree;

import com.google.gwt.user.client.ui.HasWidgets;

public class EcommerceTree extends Tree {
	
	public EcommerceTree(HasWidgets container) {
		super(container);
	}

	public enum Field {
		UserLogin,
		UserPassword,
		User,
		MLS,
		ProductType,
		SuiteId,
		SuiteName,
		Address,
		BuildingID,
		VirtualTourURL,
		MoreInfoURL,
		Email
	}

	public enum State {
		NotReady,
		Guest,
		MLS,
		Address,
		PrivateListing,
		PublicListing,
		Layout,
		OptionsReady,
		SummaryReady,
		EmailReady,
		BuildingReady,
		SuiteReady
	}
	
	
	@Override
	public void configureTree() {
//		registerNodeClass(BuildingsModel.simpleName, BuildingsModel.class);
//		registerNodeClass(EmailModel.simpleName, EmailModel.class);
//		registerNodeClass(ListingOptionsModel.simpleName, ListingOptionsModel.class);
//		registerNodeClass(LoginModel.simpleName, LoginModel.class);
//		registerNodeClass(MLSModel.simpleName, MLSModel.class);
//		registerNodeClass(ProductModel.simpleName, ProductModel.class);
//		registerNodeClass(SuitesModel.simpleName, SuitesModel.class);
//		registerNodeClass(SummaryModel.simpleName, SummaryModel.class);

		addLeaf("Root/"+
				"LoginModel.Guest/"+
				"MLSModel.MLS/"+
				"ListingOptionsModel.OptionsReady/"+
				"SummaryModel.SummaryReady/"+
				"EmailModel.EmailReady");
		addLeaf("Root/"+
				"LoginModel.Guest/"+
				"MLSModel.Address/"+
				"BuildingsModel.BuildingReady/"+
				"SuitesModel.SuiteReady/"+
				"ListingOptionsModel.OptionsReady/"+
				"SummaryModel.SummaryReady/"+
				"EmailModel.EmailReady");
		
		EcommerceTree.set(Field.ProductType, new Data("ListingPrivate"));
	}
}
