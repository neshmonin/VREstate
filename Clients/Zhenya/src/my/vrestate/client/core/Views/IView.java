package my.vrestate.client.core.Views;

import my.vrestate.client.core.Camera;
import my.vrestate.client.core.LookAt;
import my.vrestate.client.core.Site.ISite;

public interface IView {
	public void Update();
	
	public ISite getSite();
	
	public IView Back();
	
	
	public void setLookAt(LookAt look_at);
	public LookAt getLookAt();
	public void setCamera(Camera camera);
	public Camera getCamera();

	public void setVisible(boolean visible);
	public void setEnabled(boolean enable);
}
