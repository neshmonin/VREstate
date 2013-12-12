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
		UsingMLS,
		NotUsingMLS, Proceed
	}

	private AbstractNode currNode = null;
	private List<String> leafs = new ArrayList<String>();
	
	public void config() {
		// Forgot password node
//		leafs.add("DefaultsNode/LoginNode.ForgotPassword/ForgotPasswordNode.Close=>DefaultsNode/LoginNode");
//		leafs.add("DefaultsNode/LoginNode.ForgotPassword/ForgotPasswordNode.Submit/ChangingPasswordNode.Ok=>DefaultsNode/LoginNode");
		leafs.add("DefaultsNode/LoginNode.ForgotPassword/ChangingPasswordNode.Ok=>DefaultsNode/LoginNode");
		
		// Guest node
		leafs.add("DefaultsNode/LoginNode.Guest/NewOrderNode.Cancel=>DefaultsNode/LoginNode");
		leafs.add("DefaultsNode/LoginNode.Guest/NewOrderNode.Next/UsingMLSNode.Cancel=>DefaultsNode/LoginNode");
		leafs.add("DefaultsNode/LoginNode.Guest/NewOrderNode.Next/UsingMLSNode.NotUsingMLS/BuildingsNode.Cancel=>DefaultsNode/LoginNode");
		
		// Agent node
		leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.Logout=>DefaultsNode/LoginNode");
		leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.Settings/SettingsNode.Close=>DefaultsNode/LoginNode.Agent/HelloAgentNode"); 
		leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.ShowHistory/ShowHistoryNode.Close=>DefaultsNode/LoginNode.Agent/HelloAgentNode"); 
		leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.NewOrder/NewOrderNode.Cancel=>DefaultsNode/LoginNode.Agent/HelloAgentNode"); 
		leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.NewOrder/NewOrderNode.Next/UsingMLSNode.Cancel=>DefaultsNode/LoginNode.Agent/HelloAgentNode"); 
		leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.NewOrder/NewOrderNode.Next/UsingMLSNode.NotUsingMLS/BuildingsNode.Cancel=>"
				+ "DefaultsNode/LoginNode.Agent/HelloAgentNode"); 
		leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.NewOrder/NewOrderNode.Next/UsingMLSNode.NotUsingMLS/BuildingsNode.Prev=>"
				+ "DefaultsNode/LoginNode.Agent/HelloAgentNode.NewOrder/NewOrderNode.Next/UsingMLSNode");
		leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.NewOrder/NewOrderNode.Next/UsingMLSNode.NotUsingMLS/BuildingsNode.Next/PickSuiteNode.Cancel=>"
				+ "DefaultsNode/LoginNode.Agent/HelloAgentNode"); 
		leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.NewOrder/NewOrderNode.Next/UsingMLSNode.NotUsingMLS/BuildingsNode.Next/PickSuiteNode.Prev=>"
				+ "DefaultsNode/LoginNode.Agent/HelloAgentNode.NewOrder/NewOrderNode.Next/UsingMLSNode.NotUsingMLS/BuildingsNode"); 
		leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.NewOrder/NewOrderNode.Next/UsingMLSNode.NotUsingMLS/BuildingsNode.Next/PickSuiteNode.Next/OptionsNode.Cancel=>"
				+ "DefaultsNode/LoginNode.Agent/HelloAgentNode"); 
		leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.NewOrder/NewOrderNode.Next/UsingMLSNode.NotUsingMLS/BuildingsNode.Next/PickSuiteNode.Next/OptionsNode.Prev=>"
				+ "DefaultsNode/LoginNode.Agent/HelloAgentNode.NewOrder/NewOrderNode.Next/UsingMLSNode.NotUsingMLS/BuildingsNode.Next/PickSuiteNode"); 
		leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.NewOrder/NewOrderNode.Next/UsingMLSNode.NotUsingMLS/BuildingsNode.Next/PickSuiteNode.Next/OptionsNode.Next/SummaryNode.Cancel=>"
				+ "DefaultsNode/LoginNode.Agent/HelloAgentNode"); 
		leafs.add("DefaultsNode/LoginNode.Agent/HelloAgentNode.NewOrder/NewOrderNode.Next/UsingMLSNode.NotUsingMLS/BuildingsNode.Next/PickSuiteNode.Next/OptionsNode.Next/SummaryNode.Prev=>"
				+ "DefaultsNode/LoginNode.Agent/HelloAgentNode.NewOrder/NewOrderNode.Next/UsingMLSNode.NotUsingMLS/BuildingsNode.Next/PickSuiteNode.Next/OptionsNode");
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
			
//			nextNode.setParent(currNode);
//			currNode = nextNode;
			currNode = currNode.addChild(nextNode);
			
			currNode.go(this);	// TODO or return existing child, if necessary
			
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
		
		FILTERING_BY_CITY, BuildingID, SuiteId, SuiteName, Address, MLS, Price, VirtualTourUrl, MoreInfoUrl, // TODO review this constants
		
	}
}
