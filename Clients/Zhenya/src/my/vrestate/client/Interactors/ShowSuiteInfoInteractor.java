package my.vrestate.client.Interactors;

import java.util.ArrayList;

import com.google.gwt.core.client.GWT;

import my.vrestate.client.core.ClientPosition;
import my.vrestate.client.core.Point;
import my.vrestate.client.core.GEPlugin.GEPlugin;
import my.vrestate.client.core.GEPlugin.IGEPlugin;
import my.vrestate.client.core.GEPlugin.IMouseEventListener;
import my.vrestate.client.core.GEPlugin.MouseEventData;
import my.vrestate.client.core.Site.ISuite;
import my.vrestate.client.core.Views.SiteView;
import my.vrestate.client.interfaces.IShowSuiteInfoView;

public class ShowSuiteInfoInteractor extends Interactor implements IMouseEventListener{

	private IShowSuiteInfoView View = null;
	public ShowSuiteInfoInteractor(IShowSuiteInfoView view) {
		View = view;
	}
	
	@Override
	public void onMouseEvent(MouseEventData mouse_event_data) {
		 if(isEnabled() && isVisible()) {
			switch (mouse_event_data.Type) {
			case ME_CLICK: {
//				fireShowSuiteInfo(null);
				double mouse_x = mouse_event_data.X;
				double mouse_y = mouse_event_data.Y;
				
//				IGEPlugin ge_plugin = View.getSite().getGEPlugin();
	//			ge_plugin.HideBalloon();
				fireShowSuiteInfo(null);
				ArrayList<ISuite> suites = View.getSuites();
				for (ISuite suite : suites) {
					if (suite.isVisible()) {
						ClientPosition xy = GEPlugin.getClientPosition(suite.getPosition());
						double suite_x = xy.X;
						double suite_y = xy.Y;
						double dx = Math.abs(mouse_x - suite_x);
						double dy = Math.abs(mouse_y - suite_y);
						if ((dx < 10)&&(dy < 10)) {
//							suite.showInfo();
							fireShowSuiteInfo(suite);
						};
					};
				}
				break;
			}
			}
		 }
	}
private ArrayList<IShowSuiteInfoListener> ShowSuiteInfoListeners = new ArrayList<IShowSuiteInfoListener>();
	
	public void addShowSuiteInfoListener(IShowSuiteInfoListener listener) {
		this.ShowSuiteInfoListeners.add(listener);
	}
	public void removeShowSuiteInfoListener(IShowSuiteInfoListener listener) {
		this.ShowSuiteInfoListeners.remove(listener);
	}
	private void fireShowSuiteInfo(ISuite suite) {
		for (IShowSuiteInfoListener listener : ShowSuiteInfoListeners)
			if (isEnabled() && isVisible())
				listener.onShowSuiteInfo(suite);
	}

}
