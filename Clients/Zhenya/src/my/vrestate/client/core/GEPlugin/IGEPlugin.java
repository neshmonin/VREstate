package my.vrestate.client.core.GEPlugin;

import my.vrestate.client.core.Camera;
import my.vrestate.client.core.LookAt;


public interface IGEPlugin {
	public void Init();
	
//	Работа с камерой
	public void setLookAt(LookAt look_at);
	public LookAt getLookAt();
	public void setCamera(Camera look_at);
	public Camera getCamera();
	
//	public void UpdateDrawable(IDrawable drawable);
	
	public void ShowBalloon(int id, String data);
	public void HideBalloon();
	
	public void addPluginReadyListener(IPluginReadyListener listener);
	public void removePluginReadyListener(IPluginReadyListener listener);

	public void addMouseEventListener(IMouseEventListener listener);
	public void removeMouseEventListener(IMouseEventListener listener);

}
