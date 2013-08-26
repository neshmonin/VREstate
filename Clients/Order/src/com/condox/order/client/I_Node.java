package com.condox.order.client;

public interface I_Node {
	public I_Node getPrev();
	public I_Node getNext();
//	public I_Node findModel(I_Model model);
	void log();
	public void setParent(I_Node parent);
}
