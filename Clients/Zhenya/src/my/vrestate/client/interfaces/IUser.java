package my.vrestate.client.interfaces;

import my.vrestate.client.core.IUserLoginedListener;

public interface IUser {
	public void Login();
	public String getSessionID();
	
	public void addUserLoginedListener(IUserLoginedListener listener);
	public void removeUserLoginedListener(IUserLoginedListener listener);
}
