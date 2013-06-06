package com.condox.order.client.context;

import com.condox.order.client.view.factory.ViewFactory;

public class Tree implements IContext {
	private Node root;
	private ViewFactory viewFactory;

	public Tree(ViewFactory viewFactory) {
		super();
		this.viewFactory = viewFactory;
	}

	public void setRoot(Node root) {
		this.root = root;
	}

	public Node getRoot() {
		return this.root;
	}

	// ----------------------------
	private Node selected;
	
	public void next(IContext defaultContext) {
//		Log.write("next");
		if (selected != null) {
			Node node = selected.getNextNode();
			
			if (node == null)
				node = new Node(defaultContext);
			selected.addChild(node);
			selected = node;
		} else
			selected = new Node(defaultContext);
		
		// TODO ??
		if (root == null)
			root = selected;
		
		
		updateView();
	}

	public void prev() {
//		Log.write("prev");
		if (selected != null) {
			Node parent = selected.getParent();
			if (parent != null) {
//				parent.addChild(selected);
				selected = parent;
			}
		}
		
		updateView();
	}
	
	private void updateView() {
		if (selected != null)
			viewFactory.showView(this);
	}
	
	public IContext getSelectedContext() {
		if (selected != null)
			if (selected.getContext() != null)
				return selected.getContext();
		return null;
	}

	public void log() {
//		Log.write("============");
		/* if (selected != null) {
			 Node node = selected;
			 String result = "";
			 while (node != null) {
				 result = node.getContext().getName() + node.childrenToString() + "=>" + result;
				 node = node.getParent();
			 }
			 Log.write(result);
		 }*/
	}

	// ----------------------------

	public Node backward(Node source) {
		if (source.getParent() != null)
			return source.getParent();
		return source;
	}

	@Override
	public void setValue(String key, String value) {
		if (this.selected != null)
			selected.getContext().setValue(key, value);
	}

	@Override
	public String getValue(String key) {
		Node node = selected;
		while (node != null) {
			if (node.getContext() != null) {
				String result = node.getContext().getValue(key);
				if (result != null)
					return result;
			}
			node = node.getParent();
		}
			
		/*if (node != null)
			if (node.getContext() != null) {
				String result =  node.getContext().getValue(key);
				if (result != null)
					return result;
				else if (node.getParent() != null)
					return node.getParent().getContext().
			}*/
		return null;
	}

	@Override
	public Object getData() {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public String getString() {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public Boolean isValid() {
		Node node = selected;
		while (node != null) {
			if (node.getContext() != null)
				if (!node.getContext().isValid())
					return false;
			node = node.getParent();
		}
		return true;
	}

	@Override
	public ContextType getType() {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public String getName() {
		// TODO Auto-generated method stub
		return null;
	}

	// public Node go(Node source, IContext data) {
	// Iterator<Node> iter = source.getChildren().iterator();
	// while(iter.hasNext()) {
	// Node child = iter.next();
	// i
	// }
	// //----------------------------
	//
	//
	//
	// return null;
	// }

}
