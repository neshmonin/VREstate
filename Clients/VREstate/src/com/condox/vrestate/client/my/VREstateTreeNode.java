package com.condox.vrestate.client.my;

import com.condox.clientshared.container.I_Container;
import com.condox.clientshared.tree.Data;
import com.condox.clientshared.tree.TreeNode;

public class VREstateTreeNode extends TreeNode {
	
	public enum Field {
		
	}
	
	public Data getData(Field key) {
		String name = key.name();
		if (dataRepository.containsKey(name))
			return dataRepository.get(name);
		return null;
	}
	
	public void setData(Field key, Data data) {
		dataRepository.put(key.name(), data);
	}

	@Override
	public void go(I_Container container) {
	}
}
