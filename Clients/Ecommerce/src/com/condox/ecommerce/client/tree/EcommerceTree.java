package com.condox.ecommerce.client.tree;

import java.util.ArrayList;
import java.util.List;

import com.condox.clientshared.container.I_Container;
import com.condox.clientshared.tree.Data;
import com.condox.ecommerce.client.tree.node.AbstractNode;
import com.condox.ecommerce.client.tree.node.AgreementNode;
import com.condox.ecommerce.client.tree.node.BuildingsNode;
import com.condox.ecommerce.client.tree.node.ChangingPasswordNode;
import com.condox.ecommerce.client.tree.node.DefaultsNode;
import com.condox.ecommerce.client.tree.node.ForgotPasswordNode;
import com.condox.ecommerce.client.tree.node.HelloAgentNode;
import com.condox.ecommerce.client.tree.node.LoginNode;
import com.condox.ecommerce.client.tree.node.NewOrderNode;
import com.condox.ecommerce.client.tree.node.OptionsNode;
import com.condox.ecommerce.client.tree.node.PickSuiteNode;
import com.condox.ecommerce.client.tree.node.SettingsNode;
import com.condox.ecommerce.client.tree.node.ShowHistoryNode;
import com.condox.ecommerce.client.tree.node.SummaryNode;
import com.condox.ecommerce.client.tree.node.UpdateAvatarNode;
import com.condox.ecommerce.client.tree.node.UpdateProfile1Node;
import com.condox.ecommerce.client.tree.node.UpdateProfile2Node;
import com.condox.ecommerce.client.tree.node.UsingMLSNode;


public class EcommerceTree {

	public I_Container container = new PopupContainer(); //TODO first version.
	
	public enum NodeStates {
		Close,
		ForgotPassword,	
		Submit, 
		Ok,
		Guest,
		Agent,
		Logout,
		Settings,
		ShowHistory,
		NewOrder,
		Cancel,
		Prev,
		Next,
		OneMore,
		Finish,
		UsingMLS,
		NotUsingMLS, Proceed, UpdateProfile, Apply, SelectAvatar
	}

	private AbstractNode currNode = null;
	private List<String> leafs = new ArrayList<String>();
	
	public void config() {
		
		// Forgot password node
//		leafs.add("DefaultsNode/LoginNode.ForgotPassword/ForgotPasswordNode.Close=>DefaultsNode/LoginNode");
//		leafs.add("DefaultsNode/LoginNode.ForgotPassword/ForgotPasswordNode.Submit/ChangingPasswordNode.Ok=>DefaultsNode/LoginNode");
		leafs.add("DefaultsNode/LoginNode.ForgotPassword/ChangingPasswordNode.Ok=>DefaultsNode/LoginNode");
		
		String login = "DefaultsNode/LoginNode";
				
//		// Guest node
//		final String guestNewOrder = login + ".Guest/NewOrderNode";
//		final String guestUsingMLS = guestNewOrder + ".Next/UsingMLSNode";
//		final String guestPickBuilding = guestUsingMLS + ".NotUsingMLS/BuildingsNode";
//		leafs.add(guestNewOrder + ".Cancel=>" + login);
//		leafs.add(guestUsingMLS + ".Cancel=>" + login);
//		leafs.add(guestPickBuilding + ".Cancel=>" + login);
//		
		
		// Agent node
		final String agentHello = login + ".Agent/HelloAgentNode";
		final String agentUpdateProfile1 = agentHello + ".UpdateProfile/UpdateProfile1Node";
		final String agentUpdateProfile2 = agentUpdateProfile1 + ".Next/UpdateProfile2Node";
		final String agentSettings = agentHello + ".Settings/SettingsNode";
		final String agentHistory = agentHello + ".ShowHistory/ShowHistoryNode";
		final String agentNewOrder = agentHello + ".NewOrder/NewOrderNode";
		final String agentUsingMLS = agentNewOrder + ".Next/UsingMLSNode";
		final String agentPickBuilding = agentUsingMLS + ".NotUsingMLS/BuildingsNode";
		final String agentPickSuite = agentPickBuilding + ".Next/PickSuiteNode";
		final String agentOrderOptions = agentPickSuite + ".Next/OptionsNode";
		final String agentOrderSummary = agentOrderOptions + ".Next/SummaryNode";
		final String agentAgreement = agentOrderSummary + ".Next/AgreementNode";
		
		// Update profile, step #1
		leafs.add(agentUpdateProfile1 + ".Close=>" + agentHello);
		leafs.add(agentUpdateProfile1 + ".Cancel=>" + agentHello);
		leafs.add(agentUpdateProfile1 + ".Finish=>" + agentHello);
		// Update profile, step #2
		leafs.add(agentUpdateProfile2 + ".Close=>" + agentHello);
		leafs.add(agentUpdateProfile2 + ".Cancel=>" + agentHello);
		leafs.add(agentUpdateProfile2 + ".Prev=>" + agentUpdateProfile1);
		leafs.add(agentUpdateProfile2 + ".Finish=>" + agentHello);
		// Settings
		leafs.add(agentSettings + ".Close=>" + agentHello);
		// Show history
		leafs.add(agentHistory + ".Close=>" + agentHello);
		// New order
		leafs.add(agentNewOrder + ".Cancel=>" + agentHello);
//		// Using MLS#
//		leafs.add(agentUsingMLS + ".Cancel=>" + agentHello);
//		leafs.add(agentUsingMLS + ".Prev=>" + agentHello);
		// Pick building
		leafs.add(agentPickBuilding + ".Cancel=>" + agentHello);
		leafs.add(agentPickBuilding + ".Prev=>" + agentUsingMLS);
		// Pick suite
		leafs.add(agentPickSuite + ".Cancel=>" + agentHello);
		leafs.add(agentPickSuite + ".Prev=>" + agentPickBuilding);
		// Order options
		leafs.add(agentOrderOptions + ".Cancel=>" + agentHello);
		leafs.add(agentOrderOptions + ".Prev=>" + agentPickSuite);
		// Order summary
		leafs.add(agentOrderSummary + ".Cancel=>" + agentHello);
		leafs.add(agentOrderSummary + ".Prev=>" + agentOrderOptions);		
		// Agreement
		leafs.add(agentAgreement + ".Cancel=>" + agentHello);
		leafs.add(agentAgreement + ".Prev=>" + agentOrderSummary);			
//		// SelectAvatar
//		leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.UpdateProfile/UpdateProfile1Node.Next/UpdateProfile2Node.SelectAvatar/UpdateAvatarNode.Close=>"
//				+ "DefaultsNode/LoginNode.Agent/HelloAgentNode.UpdateProfile/UpdateProfile1Node.Next/UpdateProfile2Node");				
		
		// Agent node
////	String agentNewOrder = login + ".Agent/NewOrderNode";
//	leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.Logout=>"
//			+ "DefaultsNode/LoginNode");
//	
//	// UpdateProdile, Step 1
//	leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.UpdateProfile/UpdateProfile1Node.Close=>"
//			+ "DefaultsNode/LoginNode.Agent/HelloAgentNode");
//	leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.UpdateProfile/UpdateProfile1Node.Cancel=>"
//			+ "DefaultsNode/LoginNode.Agent/HelloAgentNode");
//	leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.UpdateProfile/UpdateProfile1Node.Finish=>"
//			+ "DefaultsNode/LoginNode.Agent/HelloAgentNode");
//	
//	// UpdateProdile, Step 2
//	leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.UpdateProfile/UpdateProfile1Node.Next/UpdateProfile2Node.Close=>"
//			+ "DefaultsNode/LoginNode.Agent/HelloAgentNode");
//	leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.UpdateProfile/UpdateProfile1Node.Next/UpdateProfile2Node.Cancel=>"
//			+ "DefaultsNode/LoginNode.Agent/HelloAgentNode");
//	leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.UpdateProfile/UpdateProfile1Node.Next/UpdateProfile2Node.Prev=>"
//			+ "DefaultsNode/LoginNode.Agent/HelloAgentNode.UpdateProfile/UpdateProfile1Node");
//	leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.UpdateProfile/UpdateProfile1Node.Next/UpdateProfile2Node.Finish=>"
//			+ "DefaultsNode/LoginNode.Agent/HelloAgentNode");
//	// SelectAvatar
//	leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.UpdateProfile/UpdateProfile1Node.Next/UpdateProfile2Node.SelectAvatar/UpdateAvatarNode.Close=>"
//			+ "DefaultsNode/LoginNode.Agent/HelloAgentNode.UpdateProfile/UpdateProfile1Node.Next/UpdateProfile2Node");				
//	
//	leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.Settings/SettingsNode.Close=>"
//			+ "DefaultsNode/LoginNode.Agent/HelloAgentNode"); 
//	leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.ShowHistory/ShowHistoryNode.Close=>"
//			+ "DefaultsNode/LoginNode.Agent/HelloAgentNode"); 
//	leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.NewOrder/NewOrderNode.Cancel=>"
//			+ "DefaultsNode/LoginNode.Agent/HelloAgentNode");
//	
//	// Using MLS
//	leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.NewOrder/NewOrderNode.Next/UsingMLSNode.Cancel=>"
//			+ "DefaultsNode/LoginNode.Agent/HelloAgentNode"); 
//	leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.NewOrder/NewOrderNode.Next/UsingMLSNode.Prev=>"
//			+ "DefaultsNode/LoginNode.Agent/HelloAgentNode");
//	
//	leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.NewOrder/NewOrderNode.Next/UsingMLSNode.NotUsingMLS/BuildingsNode.Cancel=>"
//			+ "DefaultsNode/LoginNode.Agent/HelloAgentNode"); 
//	leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.NewOrder/NewOrderNode.Next/UsingMLSNode.NotUsingMLS/BuildingsNode.Prev=>"
//			+ "DefaultsNode/LoginNode.Agent/HelloAgentNode.NewOrder/NewOrderNode.Next/UsingMLSNode");
//	leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.NewOrder/NewOrderNode.Next/UsingMLSNode.NotUsingMLS/BuildingsNode.Next/PickSuiteNode.Cancel=>"
//			+ "DefaultsNode/LoginNode.Agent/HelloAgentNode"); 
//	leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.NewOrder/NewOrderNode.Next/UsingMLSNode.NotUsingMLS/BuildingsNode.Next/PickSuiteNode.Prev=>"
//			+ "DefaultsNode/LoginNode.Agent/HelloAgentNode.NewOrder/NewOrderNode.Next/UsingMLSNode.NotUsingMLS/BuildingsNode"); 
//	leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.NewOrder/NewOrderNode.Next/UsingMLSNode.NotUsingMLS/BuildingsNode.Next/PickSuiteNode.Next/OptionsNode.Cancel=>"
//			+ "DefaultsNode/LoginNode.Agent/HelloAgentNode"); 
//	leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.NewOrder/NewOrderNode.Next/UsingMLSNode.NotUsingMLS/BuildingsNode.Next/PickSuiteNode.Next/OptionsNode.Prev=>"
//			+ "DefaultsNode/LoginNode.Agent/HelloAgentNode.NewOrder/NewOrderNode.Next/UsingMLSNode.NotUsingMLS/BuildingsNode.Next/PickSuiteNode"); 
//	leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.NewOrder/NewOrderNode.Next/UsingMLSNode.NotUsingMLS/BuildingsNode.Next/PickSuiteNode.Next/OptionsNode.Next/SummaryNode.Cancel=>"
//			+ "DefaultsNode/LoginNode.Agent/HelloAgentNode"); 
//	leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.NewOrder/NewOrderNode.Next/UsingMLSNode.NotUsingMLS/BuildingsNode.Next/PickSuiteNode.Next/OptionsNode.Next/SummaryNode.Prev=>"
//			+ "DefaultsNode/LoginNode.Agent/HelloAgentNode.NewOrder/NewOrderNode.Next/UsingMLSNode.NotUsingMLS/BuildingsNode.Next/PickSuiteNode.Next/OptionsNode");
//	leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.NewOrder/NewOrderNode.Next/UsingMLSNode.NotUsingMLS/BuildingsNode.Next/PickSuiteNode.Next/OptionsNode.Next/SummaryNode.Next/AgreementNode.Cancel=>"
//			+ "DefaultsNode/LoginNode.Agent/HelloAgentNode");
//	leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.NewOrder/NewOrderNode.Next/UsingMLSNode.NotUsingMLS/BuildingsNode.Next/PickSuiteNode.Next/OptionsNode.Next/SummaryNode.Next/AgreementNode.Prev=>"
//			+ "DefaultsNode/LoginNode.Agent/HelloAgentNode.NewOrder/NewOrderNode.Next/UsingMLSNode.NotUsingMLS/BuildingsNode.Next/PickSuiteNode.Next/OptionsNode.Next/SummaryNode");
////	leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.NewOrder/NewOrderNode.Next/UsingMLSNode.NotUsingMLS/BuildingsNode.Next/PickSuiteNode.Next/OptionsNode.Next/SummaryNode.Next/AgreementNode.Proceed/=>"
////			+ "DefaultsNode/LoginNode.Agent/HelloAgentNode.NewOrder/NewOrderNode.Next/UsingMLSNode.NotUsingMLS/BuildingsNode.Next/PickSuiteNode.Next/OptionsNode.Next/SummaryNode");
	}
	
	public void next() {
		if (currNode == null)
			currNode = _createNode("DefaultsNode");
		String currPath = currNode.getLeaf();
//		Log.write("currPath = " + currPath);
	
		String nextPath = null;
		for (String leaf : leafs)
			if (leaf.startsWith(currPath))
				nextPath = leaf.substring(currPath.length());
//		Log.write("nextPath = " + nextPath);
		
		if (nextPath == null) return;
		if (nextPath.startsWith("/")) {	// Create next node
//			Log.write("Creating nextNode:");
			String nextNodeType = nextPath.substring(1);
			
			int index = nextNodeType.indexOf("/");
			if (index > 0) nextNodeType = nextNodeType.substring(0, index);
			
			index = nextNodeType.indexOf(".");
			if (index > 0) nextNodeType = nextNodeType.substring(0, index);
			
//			Log.write("nextNodeType = " + nextNodeType);
			AbstractNode nextNode = _createNode(nextNodeType);
			
			currNode = currNode.addChild(nextNode);
			
			currNode.go(this);
			
		} else if (nextPath.startsWith("=>")) {	// Jump to next node
//			Log.write("Jumping nextNode:");
			String nextNodeLeaf = nextPath.substring(2);
//			Log.write("nextNodeLeaf = " + nextNodeLeaf);
			while (currNode != null) {
				String s = currNode.getLeaf();
				s = s.substring(0, s.lastIndexOf("."));
//				Log.write(s);
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
		if ("UpdateProfile1Node".equals(nodeType))
			return new UpdateProfile1Node();
		if ("UpdateProfile2Node".equals(nodeType))
			return new UpdateProfile2Node();
		if ("UpdateAvatarNode".equals(nodeType))
			return new UpdateAvatarNode();
		if ("SettingsNode".equals(nodeType))
			return new SettingsNode();
		if ("ShowHistoryNode".equals(nodeType))
			return new ShowHistoryNode();
		if ("NewOrderNode".equals(nodeType))
			return new NewOrderNode();
		if ("UsingMLSNode".equals(nodeType))
			return new UsingMLSNode();
		if ("BuildingsNode".equals(nodeType))
			return new BuildingsNode();
		if ("PickSuiteNode".equals(nodeType))
			return new PickSuiteNode();
		if ("OptionsNode".equals(nodeType))
			return new OptionsNode();
		if ("SummaryNode".equals(nodeType))
			return new SummaryNode();
		if ("AgreementNode".equals(nodeType))
			return new AgreementNode();
		return null;
		
	}
	
	public Data getData(Field key) {
		AbstractNode node = currNode;
		while (node != null) {
			if (node.getData(key) != null)
				return node.getData(key);
			else
				node = node.getParent();
		} 
		return null;
	}
	
	public void setData(Field key, Data value) {
		if (key != null)
			currNode.setData(key, value);
	}
	//-----------------

	public enum Field {
		UserEmail,
		UserPassword,
		UserRole, 
		
		BuildingId,
		BuildingName,
		
		SuiteId,
		SuiteName,
		SuiteAddress,
		SuiteMLS,
		SuitePrice,
		
		FILTERING_BY_CITY, VirtualTourUrl, MoreInfoUrl // TODO review this constants
, UserInfo, SuiteSelected
		
	}
	
	public void close() {
		container.clear();
	}
}
