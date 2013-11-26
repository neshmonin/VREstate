package com.condox.ecommerce.client.tree;

import java.util.ArrayList;
import java.util.List;

import com.condox.clientshared.tree.Data;
import com.condox.clientshared.tree.I_TreeNode;
import com.condox.clientshared.tree.Tree;


public class EcommerceTree extends Tree {
	
	public EcommerceTree() {
		super();
		currentNode = new DefaultNode();
		configureTree();
	}
	//-----------------
	public enum Nodes {
		SIGN_IN,
		FORGOT_PASSWORD,
		CHANGING_PASSWORD,
		HELLO,
		
		SETTINGS,
		UPDATE1,
		UPDATE2,
		SHOW_HISTORY,
		
		NEW_ORDER,
		NEW_ORDER_USING_MLS,
		SELECT_BUILDING,
		SELECT_SUITE,
		OPTIONS,
		SUMMARY,
		AGREEMENT,
		PROCEED,
	}
	
	private List<String> leafs = new ArrayList<String>();
	
	private void config() {
		
		leafs.add("SIGN_IN/FORGOT_PASSWORD/CHANGING_PASSWORD/SIGN_IN");
		leafs.add("SIGN_IN/HELLO/SETTINGS");
		leafs.add("SIGN_IN/HELLO/UPDATE1");
		leafs.add("SIGN_IN/HELLO/");
		leafs.add("SIGN_IN/HELLO/");
	}
	//-----------------

	public enum Field {
		USING_MLS,
		FILTERING_BY_CITY,
		//----------
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
		EcommerceTree.set(Field.USING_MLS.name(), new Data(true));
		EcommerceTree.set(Field.FILTERING_BY_CITY.name(), new Data("Toronto"));
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
