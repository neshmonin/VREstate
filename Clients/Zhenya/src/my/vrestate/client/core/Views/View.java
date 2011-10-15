package my.vrestate.client.core.Views;

import java.util.ArrayList;
import java.util.Stack;

import com.google.gwt.core.client.GWT;

import my.vrestate.client.Interactors.IInteractor;
import my.vrestate.client.core.Camera;
import my.vrestate.client.core.LookAt;
import my.vrestate.client.core.GEPlugin.GEPlugin;
import my.vrestate.client.core.GEPlugin.IGEPlugin;
import my.vrestate.client.core.Site.ISite;
import my.vrestate.client.core.Site.ISiteLoadedListener;
import my.vrestate.client.core.Site.ISuite;

public abstract class View implements IView {
	private ISite Site;
	protected IGEPlugin GEPlugin;
	private boolean Visible = true;
	
	private static Stack<IView> ViewStack = new Stack<IView>();
	private ArrayList<IInteractor> Interactors = new ArrayList<IInteractor>();

	public View(ISite site) {
		Site = site;
		GEPlugin = Site.getGEPlugin();
		
		if(ViewStack.size() > 0)
			ViewStack.peek().setVisible(false);
		ViewStack.push(this);
//		ViewStack.peek().setPaused(false);
	}

	public IView Back() {
//		if (ViewStack.isEmpty()) return null;
		if (ViewStack.size() == 1) return ViewStack.peek();
		if (ViewStack.size() > 1) {
			ViewStack.pop();
			return ViewStack.peek();
		}
//		GWT.log("closing view");
//		ViewStack.peek().setPaused(true);
//		if (ViewStack.size() > 1)
//			ViewStack.pop().setPaused(false);
//		else
//			ViewStack.peek().setPaused(false);
//		return ViewStack.peek();
		return null;
	};
	
	
	
	
	

	public Camera getCamera() {
		return GEPlugin.getCamera();
	};
	
	public void setCamera(Camera camera) {
		GEPlugin.setCamera(camera);
	};
	
	public LookAt getLookAt() {
		return GEPlugin.getLookAt();
	};
	
	public void setLookAt(LookAt look_at) {
		GEPlugin.setLookAt(look_at);
	};
	
	public ISite getSite() {
		return Site;
	}
	
	public void setVisible(boolean visible) {
		Visible = visible;
	}
	public boolean isVisible() {
		return Visible;
	}
}
