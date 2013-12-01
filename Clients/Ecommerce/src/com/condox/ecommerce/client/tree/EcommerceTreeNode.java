package com.condox.ecommerce.client.tree;

import com.condox.clientshared.abstractview.Log;
import com.condox.clientshared.container.I_Container;
import com.condox.clientshared.tree.Data;
import com.condox.clientshared.tree.I_TreeNode;
import com.condox.clientshared.tree.TreeNode;

public abstract class EcommerceTreeNode extends TreeNode {
	
	public Data getData(EcommerceTree.Field key) {
		String name = key.name();
		if (dataRepository.containsKey(name))
			return dataRepository.get(name);
		return null;
	}
	
	public void setData(EcommerceTree.Field key, Data data) {
		dataRepository.put(key.name(), data);
	}

	@Override
	public void go(I_Container container) {
//		I_TreeNode item = this;
//		String str = item.getNavURL();
//		item = item.getParent();
//		while (item != null) {
//			if (!item.getNavURL().isEmpty())
//				str = item.getNavURL() + " &#187; " + str;
//			item = item.getParent();
//		}
////		str = "&#187;";
////		Log.write(str);
		
		// TODO correct NavBar
//		setNavText(str);
	}
	
	private native void setNavText(String str) /*-{
		$doc.getElementById('navBar').rows[0].cells[0].innerHTML = str;
	}-*/;
	
	public abstract String getNavURL();
}
