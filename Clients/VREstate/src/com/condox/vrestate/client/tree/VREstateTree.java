package com.condox.vrestate.client.tree;

import com.condox.clientshared.tree.Data;
import com.condox.clientshared.tree.I_TreeNode;
import com.condox.clientshared.tree.Tree;

public class VREstateTree extends Tree {

	public VREstateTree() {
		super();
		currentNode = new DefaultNode();
		configureTree();
	}

	public enum Field {
	}

	public enum State {
		NotReady,
		VREstate,
		Regions
	}
	
	
	@Override
	public void configureTree() {
		addLeaf("Root/"+
				"Login.VREstate");
		addLeaf("Root/"+
				"Login.Regions");
		
//		EcommerceTree.set(Field.ProductType.name(), new Data("ListingPrivate"));
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
