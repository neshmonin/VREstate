package com.condox.vrestate.client.view;



public interface IView {
	public void Update();
	public void Draw();
	public void Pop();
	abstract void setEnabled(boolean value);
}
