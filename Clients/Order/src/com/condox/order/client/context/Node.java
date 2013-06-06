package com.condox.order.client.context;

import java.util.HashMap;
import java.util.Iterator;
import java.util.Map;
import java.util.Map.Entry;

public class Node {
	private IContext context;
	private Node parent;
//	private List<Node> children;
	private Map<IContext,Node> children;
	
	public Node() {
		super();
//		children = new ArrayList<Node>();
		children = new HashMap<IContext,Node>();
	}
	
	public Node(IContext context) {
		this();
		setContext(context);
	}
	
	public void setContext(IContext context) {
		this.context = context;
	}
	
	public IContext getContext() {
		return this.context;
	}
	
	public void setParent(Node parent) {
		this.parent = parent;
	}
	
	public Node getParent() {
		return this.parent;
	}
	
//	public List<Node> getChildren() {
//		return this.children;
//	}
	public Map<IContext,Node> getChildren() {
		return this.children;
	}
	
	public String getString() {
		if (getParent() != null)
			return getParent().getString() + " => " + getContext().getString() + childrenToString();
		return getContext().getString() + childrenToString();
	}
	
	public String childrenToString() {
		String result = "{";
		Iterator<Entry<IContext,Node>> iter = children.entrySet().iterator();
		while (iter.hasNext()) {
			Entry<IContext,Node> entry = iter.next();
			result += entry.getKey().getName() + " = " + entry.getValue().getContext().getName() + ", ";
		}
		result += "}";
		return result;
	}
	//-----------------------------------------------
//	public Node getChildByContext(IContext context) {
//		Iterator<IContext> iter = children.keySet().iterator();
//		while (iter.hasNext()) {
//			IContext curr = iter.next();
//			Log.write(context.getType().toString());
//			Log.write(curr.getType().toString());
//			if (context.getData().equals(curr.getData())) {
//				Log.write("+++");
//				return children.get(curr);
//			}
//		}
//		return null;
//	}
//	
	public void addChild(Node child) {
//		Log.write("addChild: " + child.getContext().getName());
		if (child != null) {
			this.children.put(getContext(), child);
			child.setParent(this);
		}
	}
	
//	public Boolean containsChild(IContext key) {
//		Log.write("containsChild:");
//		Log.write("  key:" + key.getName());
//		Iterator<IContext> iter = children.keySet().iterator();
//		while (iter.hasNext()) {
//			IContext curr = iter.next();
//			Log.write("  curr:" + key.getName());
//			if (curr.equals(key))
//				return true;
//		}
//		return false;
//	}
	
	public Node getChild(IContext key) {
//		if (containsChild(key))
			return children.get(key);
//		else
//			return null;
	}
	
	public Node getNextNode() {
		Node node = children.get(getContext());
		/*Log.write("=========");
		Log.write("childrens: " + childrenToString());
		Log.write("context: " + getContext().getString());
		Log.write("getNextNode: " + node);
		Log.write("=========");*/
		return node;
	}
}
