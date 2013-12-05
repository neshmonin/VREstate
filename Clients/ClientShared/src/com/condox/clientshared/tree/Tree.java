package com.condox.clientshared.tree;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.Map;

import com.condox.clientshared.abstractview.Log;
import com.condox.clientshared.container.I_Container;
import com.google.gwt.user.client.Cookies;

public abstract class Tree implements I_Tree {

	public static I_TreeNode currentNode = null;
	private static Tree instance = null;
//	private StackLayoutPanel stackLayoutPanel = new StackLayoutPanel(Unit.EM);

	public Tree() {
		instance = this;

	}
	
	public static void cancel() {
//		instance.popup.hide();
		if (instance.container != null)
			instance.container.clear();
	}
	
//	private DialogBox popup = new DialogBox();
	
//	@Override
//	public void activate(I_Container container) {
//		currentNode = getNextNode();
////		VerticalPanel vp = new VerticalPanel();
////		HorizontalPanel hp = new HorizontalPanel();
////		hp.getElement().setId("navBar");
////		hp.add(new Button(""));
////
////		HTMLPanel container = new HTMLPanel("");
////		container.setSize("750px", "500px");
////
////		vp.add(hp);
////		vp.add(container);
////		
////		popup.setWidget(vp);
////		popup.center();
//		
////		currentNode.go(container);
//	}
	
	private I_Container container = null;
	
	public void go(I_Container container) {
		this.container = container;
		currentNode = getNextNode();
		currentNode.go(container);
	}

//	@Override
//	public HasWidgets getContainer() {
//		return (HasWidgets) stackLayoutPanel.getWidget(0);
//	}
	
	private static String getNodePath() {
		I_TreeNode node = currentNode;
		String path = node.getStateString();
		while (node.getParent() != null) {
			node = node.getParent();
			path = node.getStateString() + "/" + path;
		}
		Log.write("path = " + path);
		return path;
	}
	
	public static Data get(String name) {
		I_TreeNode node = currentNode;
		String cookie = Cookies.getCookie(getNodePath() + ":" + name);
		if (cookie != null)
			return new Data(cookie);
		do {
			Data data = node.getData(name);
			if (data != null)
				return data;
			
			node = node.getParent();			
		} while (node != null);
		
		return null;
	}

	public static void set(String name, Data data) {
		// TODO
		// correct & remove hardcoded values
//		Calendar calendar = Calendar.getInstance();
//		calendar.add(Calendar.DATE, 10);
		
		Date date = new Date(System.currentTimeMillis());
		date.setTime(date.getTime() + 1000*60*60*24*10);
		
		
		Cookies.setCookie(getNodePath() + ":" + name, data.Value, date);
		currentNode.setData(name, data);
	}
	

	public static void transitState(String readyState) {
		currentNode.setState(readyState);
	}
	
	public static I_TreeNode getPrevNode() {
		currentNode = currentNode.getParent();
		return currentNode;
	}

	public static I_TreeNode getNextNode() {
		Log.write("Tree.getNextNode()");
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
		Log.write("Tree.createNextNode()");
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
//				Class<TreeNode> nextNodeClass = instance.nodeClasses.get(nextNodeType);
//				Object invoke = 
//				I_TreeNode nextNode = NodeFactory.create(nextNodeType);
				I_TreeNode nextNode = instance.createNode(nextNodeType);
				if (nextNode != null)
					nextNode.setParent(currentNode);

				return nextNode;
			}
		}
		
		return null;
	}
	
	public abstract I_TreeNode createNode(String nodeType);
	
	protected List<String> leafs = new ArrayList<String>();
	// Examples of leafFullPath:
	//  Login.Guest/MLS.ValidMLS/Options.Ready/Summary.Ready/Email.Ready
	//  Login.Guest/MLS.Address/Building.Ready/Suite.Ready/Options.Ready/Summary.Ready/Email.Ready
	public void addLeaf(String leafFullPath) {
		leafs.add(leafFullPath);
	}
	
//	protected Map<String, Class<TreeNode>> nodeClasses = new HashMap<String, Class<TreeNode>>();
//	
//	public void registerNodeClass(String simpleName, Class<TreeNode> nodeClass) {
//		//String simpleName = nodeClass.getSimpleName();
//		nodeClasses.put(simpleName, nodeClass);
//	}
	
	public abstract void configureTree();
}
