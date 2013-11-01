package com.condox.ecommerce.client.tree;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import com.condox.ecommerce.client.tree.EcommerceTree.State;
import com.google.gwt.core.shared.GWT;
import com.google.gwt.dom.client.Style.Unit;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.DialogBox;
import com.google.gwt.user.client.ui.HTMLPanel;
import com.google.gwt.user.client.ui.HasWidgets;
import com.google.gwt.user.client.ui.HorizontalPanel;
import com.google.gwt.user.client.ui.StackLayoutPanel;
import com.google.gwt.user.client.ui.VerticalPanel;

abstract class Tree implements I_Tree {

	private static I_TreeNode currentNode = null;
	private static Tree instance = null;
	private StackLayoutPanel stackLayoutPanel = new StackLayoutPanel(Unit.EM);

	public Tree(HasWidgets container) {
		instance = this;
		registerNodeClass(DefaultNode.simpleName, DefaultNode.class);
		currentNode = new DefaultNode();
		configureTree();
	}
	
	public static void cancel() {
		instance.popup.hide();
	}
	
	private DialogBox popup = new DialogBox();
	
	@Override
	public void activate() {
		currentNode = getNextNode();
		VerticalPanel vp = new VerticalPanel();
		HorizontalPanel hp = new HorizontalPanel();
		hp.getElement().setId("navBar");
		hp.add(new Button(""));

		HTMLPanel container = new HTMLPanel("");
		container.setSize("750px", "500px");

		vp.add(hp);
		vp.add(container);
		
		popup.setWidget(vp);
		popup.center();
		currentNode.go(container);
	}

	@Override
	public HasWidgets getContainer() {
		return (HasWidgets) stackLayoutPanel.getWidget(0);
	}

	public static Data get(EcommerceTree.Field name) {
		I_TreeNode node = currentNode;
		do {
			Data data = node.getData(name);
			if (data != null)
				return data;
			
			node = node.getParent();			
		} while (node != null);
		
		return null;
	}

	public static void set(EcommerceTree.Field name, Data data) {
		currentNode.setData(name, data);
	}

	public static void transitState(State readyState) {
		currentNode.setState(readyState);
	}
	
	public static I_TreeNode getPrevNode() {
		currentNode = currentNode.getParent();
		return currentNode;
	}

	public static I_TreeNode getNextNode() {
		// Log.write("" + children.toString());
		Map<I_TreeNode, I_TreeNode> children = currentNode.getChildren(); 
		I_TreeNode theNode = children.get(currentNode);
		if (theNode == null) {
			theNode = createNextNode();
			if (theNode != null)
				children.put(currentNode, theNode);
		}

		currentNode = theNode; 
		return theNode;
	}

	private static I_TreeNode createNextNode() {
		I_TreeNode node = currentNode;
		String fullPath = "";
		do {
			fullPath = node.getStateString() + "/" + fullPath;
			node = node.getParent();			
		} while (node != null);
		
		for (String leaf : instance.leafs) {
			if (leaf.startsWith(fullPath)) {
				String nextPath = leaf.substring(fullPath.length());
				String nextNodeType = nextPath.substring(0, nextPath.indexOf("."));
				Class<?> nextNodeClass = instance.nodeClasses.get(nextNodeType);
				I_TreeNode nextNode = GWT.create(nextNodeClass);
				nextNode.setParent(currentNode);

				return nextNode;
			}
		}
		
		return null;
	}
	
	protected List<String> leafs = new ArrayList<String>();
	// Examples of leafFullPath:
	//  Login.Guest/MLS.ValidMLS/Options.Ready/Summary.Ready/Email.Ready
	//  Login.Guest/MLS.Address/Building.Ready/Suite.Ready/Options.Ready/Summary.Ready/Email.Ready
	public void addLeaf(String leafFullPath) {
		leafs.add(leafFullPath);
	}
	
	protected Map<String, Class<?>> nodeClasses = new HashMap<String, Class<?>>();
	
	public void registerNodeClass(String simpleName, Class<?> nodeClass) {
		//String simpleName = nodeClass.getSimpleName();
		nodeClasses.put(simpleName, nodeClass);
	}
	
	public abstract void configureTree();
	
}
