package com.condox.ecommerce.client.tree;

import java.util.ArrayList;
import java.util.List;

import com.condox.clientshared.abstractview.Log;
import com.condox.clientshared.container.I_Container;
import com.condox.clientshared.tree.Data;
import com.condox.clientshared.tree.I_TreeNode;
import com.condox.clientshared.tree.Tree;
import com.condox.ecommerce.client.tree.node.AbstractNode;
import com.condox.ecommerce.client.tree.node.ChangingPasswordNode;
import com.condox.ecommerce.client.tree.node.DefaultsNode;
import com.condox.ecommerce.client.tree.node.ForgotPasswordNode;
import com.condox.ecommerce.client.tree.node.HelloAgentNode;
import com.condox.ecommerce.client.tree.node.LoginNode;


public class EcommerceTree extends Tree {
	
	public EcommerceTree() {
//		super();
//		currentNode = new DefaultNode();
//		configureTree();
	}
	//-----------------
//	public enum Nodes {
//		SIGN_IN,
//		FORGOT_PASSWORD,
//		CHANGING_PASSWORD,
//		HELLO,
//		
//		SETTINGS,
//		UPDATE1,
//		UPDATE2,
//		SHOW_HISTORY,
//		
//		NEW_ORDER,
//		NEW_ORDER_USING_MLS,
//		SELECT_BUILDING,
//		SELECT_SUITE,
//		OPTIONS,
//		SUMMARY,
//		AGREEMENT,
//		PROCEED,
//	}
	
	public I_Container container = new PopupContainer();
	
	public enum NodeStates {
		Close,
		ForgotPassword,	
		Submit, 
		Ok,
		Guest,
		Agent,
		Logout
		
	}

	private AbstractNode currNode = null;
	private List<String> leafs = new ArrayList<String>();
	
	public void config() {
		leafs.add("DefaultsNode/LoginNode.ForgotPassword/ForgotPasswordNode.Close=>" +
				"DefaultsNode/LoginNode");
		leafs.add("DefaultsNode/LoginNode.ForgotPassword/ForgotPasswordNode.Submit/ChangingPasswordNode.Ok=>" +
				"DefaultsNode/LoginNode");
		leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode");
		leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.Logout=>" +
				"DefaultsNode/LoginNode");
	}
	
	public void next(/*NodeStates state*/) {
//		Log.write("EcommerceTree.next():");
//		Log.write("state = " + state);
		
		if (currNode == null) {
			currNode = _createNode("DefaultsNode");
//			currNode.setState(NodeStates.ForgotPassword);
		}
		String currPath = currNode.getLeaf();
//		if (state != null)
//			currPath += "." + state;
		Log.write("currPath = " + currPath);
		String nextPath = null;
		for (String leaf : leafs)
			if (leaf.startsWith(currPath))
				nextPath = leaf.substring(currPath.length());
		Log.write("nextPath = " + nextPath);
		
		if (nextPath.startsWith("/")) {	// Create next node
//			Log.write("Creating nextNode:");
			String nextNodeType = nextPath.substring(1);
			
			int index = nextNodeType.indexOf("/");
			if (index > 0) nextNodeType = nextNodeType.substring(0, index);
			
			index = nextNodeType.indexOf(".");
			if (index > 0) nextNodeType = nextNodeType.substring(0, index);
			
			Log.write("nextNodeType = " + nextNodeType);
			AbstractNode nextNode = _createNode(nextNodeType);
			nextNode.setParent(currNode);
			currNode = nextNode;
			currNode.go(this);	// TODO or return existing child, if necessary
			
		} else if (nextPath.startsWith("=>")) {	// Jump to next node
//			Log.write("Jumping nextNode:");
			String nextNodeLeaf = nextPath.substring(2);
			Log.write("nextNodeLeaf = " + nextNodeLeaf);
			while (currNode != null) {
				String s = currNode.getLeaf();
				s = s.substring(0, s.lastIndexOf("."));
				Log.write(s);
				if (nextNodeLeaf.equals(s)) {
					currNode.go(this);
					break;
				}
				currNode = currNode.getParent();
			}
		}
//		String nextNode = nextPath.
		
	}
	
	private AbstractNode _createNode(String nodeType) {
		if ("DefaultsNode".equals(nodeType))
			return new DefaultsNode();
		if ("LoginNode".equals(nodeType))
			return new LoginNode();
		if ("ForgotPasswordNode".equals(nodeType))
			return new ForgotPasswordNode();
		if ("ChangingPasswordNode".equals(nodeType))
			return new ChangingPasswordNode();
		if ("HelloAgentNode".equals(nodeType))
			return new HelloAgentNode();
		return null;
		
	}
	//-----------------

	public enum Field {
		USING_MLS,
		FILTERING_BY_CITY,
		//----------
		UserLogin,
		UserPassword,
		UserId,
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
		Agent,
		History,
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
		addLeaf("Root/" + 
				"LoginModel.Agent/" + 
				"HelloNode.History/" +
				"HistoryNode.");
		
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
