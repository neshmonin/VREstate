package com.condox.ecommerce.client.tree;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import com.condox.clientshared.container.I_Container;
import com.condox.clientshared.tree.Data;
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

	public I_Container container = new PopupContainer(); // TODO first version.

	public enum Actions {
		PrevMLS, PrevAddress,
		Close, ForgotPassword, Submit, Ok, Guest, Agent, Logout, Settings, ShowHistory, NewOrder, Cancel, Prev, Next, OneMore, Finish, UsingMLS, UsingAddress, Proceed, UpdateProfile, Apply, SelectAvatar
	}

	private AbstractNode currNode = new DefaultsNode();
//	private List<String> leafs = new ArrayList<String>();
	private Node configNode = new Node("DefaultNode");

	class Node {
		private String nodeType = null;
		private Node parent = null;
		private Map<Actions, Node> actions = new HashMap<Actions, Node>();

		public Node(String newNodeType) {
			nodeType = newNodeType;
		}

		public void setParent(Node newParent) {
			parent = newParent;
		};

		public boolean hasParent() {
			return parent != null;
		};

		public void addAction(Actions state, Node nextNode) {
			actions.put(state, nextNode);
			nextNode.setParent(this);
		}

		public Node getNextNode(Actions state) {
			return actions.get(state);
		}
	}

	public void config() {
		final Node login = new Node("LoginNode");
		final Node agentHello = new Node("HelloAgentNode");
		final Node agentUpdateProfile1 = new Node("UpdateProfile1Node");
		final Node agentUpdateProfile2 = new Node("UpdateProfile2Node");
		final Node agentSettings = new Node("SettingsNode");
		final Node agentHistory = new Node("ShowHistoryNode");
		final Node newOrder = new Node("NewOrderNode");
		final Node usingMLS = new Node("UsingMLSNode");
		final Node pickBuilding = new Node("BuildingsNode");
		final Node pickSuite = new Node("PickSuiteNode");
		final Node options = new Node("OptionsNode");
		final Node summary = new Node("SummaryNode");
		final Node agreement = new Node("AgreementNode");

		configNode.addAction(null, login);
		login.addAction(Actions.Agent, agentHello);
		// Agent hello
		agentHello.addAction(Actions.Logout, login);
		agentHello.addAction(Actions.UpdateProfile, agentUpdateProfile1);
		agentHello.addAction(Actions.Settings, agentSettings);
		agentHello.addAction(Actions.ShowHistory, agentHistory);
		agentHello.addAction(Actions.NewOrder, /* newOrder */usingMLS);
		// Update profile, step #1
		agentUpdateProfile1.addAction(Actions.Close, agentHello);
		agentUpdateProfile1.addAction(Actions.Cancel, agentHello);
		agentUpdateProfile1.addAction(Actions.Next, agentUpdateProfile2);
		agentUpdateProfile1.addAction(Actions.Finish, agentHello);
		// Update profile, step #2
		agentUpdateProfile2.addAction(Actions.Close, agentHello);
		agentUpdateProfile2.addAction(Actions.Cancel, agentHello);
		agentUpdateProfile2.addAction(Actions.Prev, agentUpdateProfile1);
		agentUpdateProfile2.addAction(Actions.Finish, agentHello);
		// Settings
		agentSettings.addAction(Actions.Close, agentHello);
		// History
		agentHistory.addAction(Actions.Close, agentHello);
		// New order
		// Using MLS
		usingMLS.addAction(Actions.Close, agentHello);
		usingMLS.addAction(Actions.Cancel, agentHello);
		usingMLS.addAction(Actions.Prev, agentHello);
		usingMLS.addAction(Actions.UsingMLS, options);
		usingMLS.addAction(Actions.UsingAddress, pickBuilding);
		// Pick building
		pickBuilding.addAction(Actions.Close, agentHello);
		pickBuilding.addAction(Actions.Cancel, agentHello);
		pickBuilding.addAction(Actions.Prev, usingMLS);
		pickBuilding.addAction(Actions.Next, pickSuite);
		// Pick suite
		pickSuite.addAction(Actions.Close, agentHello);
		pickSuite.addAction(Actions.Cancel, agentHello);
		pickSuite.addAction(Actions.Prev, pickBuilding);
		pickSuite.addAction(Actions.Next, options);
		// Options
		options.addAction(Actions.Close, agentHello);
		options.addAction(Actions.Cancel, agentHello);
		options.addAction(Actions.UsingMLS, usingMLS);
		options.addAction(Actions.UsingAddress, pickSuite);
		options.addAction(Actions.Next, summary);
		// Summary
		summary.addAction(Actions.Close, agentHello);
		summary.addAction(Actions.Cancel, agentHello);
		summary.addAction(Actions.Prev, options);
		summary.addAction(Actions.Next, agreement);
		// Agreement
		agreement.addAction(Actions.Close, agentHello);
		agreement.addAction(Actions.Cancel, agentHello);
		agreement.addAction(Actions.Prev, summary);
//		summary.addAction(Actions.Next, agreement);
		
		// // Forgot password node
		// //
		// leafs.add("DefaultsNode/LoginNode.ForgotPassword/ForgotPasswordNode.Close=>DefaultsNode/LoginNode");
		// //
		// leafs.add("DefaultsNode/LoginNode.ForgotPassword/ForgotPasswordNode.Submit/ChangingPasswordNode.Ok=>DefaultsNode/LoginNode");
		// leafs.add("DefaultsNode/LoginNode.ForgotPassword/ChangingPasswordNode.Ok=>DefaultsNode/LoginNode");
		//
		// String login = "DefaultsNode/LoginNode";
		//
		// // Agent node
		// final String agentHello = login + ".Agent/HelloAgentNode";
		// final String agentUpdateProfile1 = agentHello
		// + ".UpdateProfile/UpdateProfile1Node";
		// final String agentUpdateProfile2 = agentUpdateProfile1
		// + ".Next/UpdateProfile2Node";
		// final String agentSettings = agentHello + ".Settings/SettingsNode";
		// final String agentHistory = agentHello +
		// ".ShowHistory/ShowHistoryNode";
		// final String agentNewOrder = agentHello + ".NewOrder/NewOrderNode";
		// final String agentUsingMLS = agentNewOrder + ".Next/UsingMLSNode";
		// final String agentPickBuilding = agentUsingMLS
		// + ".NotUsingMLS/BuildingsNode";
		// final String agentPickSuite = agentPickBuilding +
		// ".Next/PickSuiteNode";
		// final String agentOrderOptions = agentPickSuite +
		// ".Next/OptionsNode";
		// // final String agentOrderOptions2 = agentUsingMLS +
		// // ".UsingMLS/OptionsNode";
		// final String agentOrderSummary = agentOrderOptions
		// + ".Next/SummaryNode";
		// // final String agentOrderSummary2 = agentOrderOptions2 +
		// // ".Next/SummaryNode";
		//
		// final String agentAgreement = agentOrderSummary +
		// ".Next/AgreementNode";
		//
		// // Update profile, step #1
		// leafs.add(agentUpdateProfile1 + ".Close=>" + agentHello);
		// leafs.add(agentUpdateProfile1 + ".Cancel=>" + agentHello);
		// leafs.add(agentUpdateProfile1 + ".Finish=>" + agentHello);
		// // Update profile, step #2
		// leafs.add(agentUpdateProfile2 + ".Close=>" + agentHello);
		// leafs.add(agentUpdateProfile2 + ".Cancel=>" + agentHello);
		// leafs.add(agentUpdateProfile2 + ".Prev=>" + agentUpdateProfile1);
		// leafs.add(agentUpdateProfile2 + ".Finish=>" + agentHello);
		// // Settings
		// leafs.add(agentSettings + ".Close=>" + agentHello);
		// // Show history
		// leafs.add(agentHistory + ".Close=>" + agentHello);
		// // New order
		// leafs.add(agentNewOrder + ".Cancel=>" + agentHello);
		// // // Using MLS#
		// // leafs.add(agentUsingMLS + ".Cancel=>" + agentHello);
		// // leafs.add(agentUsingMLS + ".Prev=>" + agentHello);
		// // Pick building
		// leafs.add(agentPickBuilding + ".Cancel=>" + agentHello);
		// leafs.add(agentPickBuilding + ".Prev=>" + agentUsingMLS);
		// // Pick suite
		// leafs.add(agentPickSuite + ".Cancel=>" + agentHello);
		// leafs.add(agentPickSuite + ".Prev=>" + agentPickBuilding);
		// // Order options
		// leafs.add(agentOrderOptions + ".Cancel=>" + agentHello);
		// leafs.add(agentOrderOptions + ".Prev=>" + agentPickSuite);
		// // Order summary
		// leafs.add(agentOrderSummary + ".Cancel=>" + agentHello);
		// leafs.add(agentOrderSummary + ".Prev=>" + agentOrderOptions);
		// // Agreement
		// leafs.add(agentAgreement + ".Cancel=>" + agentHello);
		// leafs.add(agentAgreement + ".Prev=>" + agentOrderSummary);
	}

	public void next() {
		// if (currNode == null)
		// currNode = _createNode("DefaultsNode");
		Actions currState = currNode.getState();
		configNode = configNode.getNextNode(currState);

		boolean ready = false;
		AbstractNode node = currNode;
		while (node.hasParent()) {
			if (node.getName().equals(configNode.nodeType)) {
				ready = true;
				currNode = node;
				break;
			}
			node = node.getParent();
		}
		if (!ready) {
			AbstractNode nextNode = _createNode(configNode.nodeType);
			currNode = currNode.addChild(nextNode);
		}
		currNode.go(this);
		// if (currNode == null)
		// currNode = _createNode("DefaultsNode");
		// String currPath = currNode.getLeaf();
		// // Log.write("currPath = " + currPath);
		//
		// String nextPath = null;
		// for (String leaf : leafs)
		// if (leaf.startsWith(currPath))
		// nextPath = leaf.substring(currPath.length());
		// // Log.write("nextPath = " + nextPath);
		//
		// if (nextPath == null)
		// return;
		// if (nextPath.startsWith("/")) { // Create next node
		// // Log.write("Creating nextNode:");
		// String nextNodeType = nextPath.substring(1);
		//
		// int index = nextNodeType.indexOf("/");
		// if (index > 0)
		// nextNodeType = nextNodeType.substring(0, index);
		//
		// index = nextNodeType.indexOf(".");
		// if (index > 0)
		// nextNodeType = nextNodeType.substring(0, index);
		//
		// // Log.write("nextNodeType = " + nextNodeType);
		// AbstractNode nextNode = _createNode(nextNodeType);
		//
		// currNode = currNode.addChild(nextNode);
		//
		// currNode.go(this);
		//
		// } else if (nextPath.startsWith("=>")) { // Jump to next node
		// // Log.write("Jumping nextNode:");
		// String nextNodeLeaf = nextPath.substring(2);
		// // Log.write("nextNodeLeaf = " + nextNodeLeaf);
		// while (currNode != null) {
		// String s = currNode.getLeaf();
		// s = s.substring(0, s.lastIndexOf("."));
		// // Log.write(s);
		// if (nextNodeLeaf.equals(s)) {
		// currNode.go(this);
		// break;
		// }
		// currNode = currNode.getParent();
		// }
		// }
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

	// -----------------

	public enum Field {
		UserEmail, UserPassword, UserRole,

		BuildingId, BuildingName,

		SuiteId, SuiteName, SuiteAddress, SuiteMLS, SuitePrice,

		FILTERING_BY_CITY, VirtualTourUrl, MoreInfoUrl // TODO review this
														// constants
		, UserInfo, SuiteSelected, UsingMLS

	}

	public void close() {
		container.clear();
	}
}
