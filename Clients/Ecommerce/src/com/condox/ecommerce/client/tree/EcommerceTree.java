package com.condox.ecommerce.client.tree;

import com.condox.clientshared.tree.Data;
import com.condox.clientshared.tree.I_TreeNode;
import com.condox.clientshared.tree.Tree;


public class EcommerceTree extends Tree {
	
	public EcommerceTree() {
		super();
		currentNode = new DefaultNode();
		configureTree();
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
		
		EcommerceTree.set(Field.ProductType.name(), new Data("ListingPrivate"));
	}


	@Override
	public I_TreeNode createNode(String nodeType) {
		return NodeFactory.create(nodeType);
	}


	public static Data get(Field key) {
		return Tree.get(key.name());
	}


	public static void set(Field key, Data data) {
		// TODO Auto-generated method stub
		set(key.name(), data);
	}


	public static void transitState(State key) {
		// TODO Auto-generated method stub
		transitState(key.name());
	}
	
}
