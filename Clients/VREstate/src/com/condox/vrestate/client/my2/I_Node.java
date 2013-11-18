package com.condox.vrestate.client.my2;

import com.condox.clientshared.tree.Data;



public interface I_Node {

	public I_Node getParent();
	
	public I_Node getChild();
	
	public Data getData(String name);
	
	public void setData(String name, Data value);
}
