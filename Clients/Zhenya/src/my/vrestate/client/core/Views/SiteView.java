package my.vrestate.client.core.Views;

import java.util.ArrayList;

import my.vrestate.client.Options;
import my.vrestate.client.Interactors.ILeftRightListener;
import my.vrestate.client.Interactors.IPanoramicViewListener;
import my.vrestate.client.Interactors.IShowSuiteInfoListener;
import my.vrestate.client.Interactors.IUpDownListener;
import my.vrestate.client.Interactors.LeftRightInteractor;
import my.vrestate.client.Interactors.PanoramicViewInteractor;
import my.vrestate.client.Interactors.ShowSuiteInfoInteractor;
import my.vrestate.client.Interactors.UpDownInteractor;
import my.vrestate.client.core.LookAt;
import my.vrestate.client.core.Point;
import my.vrestate.client.core.Site.IBuilding;
import my.vrestate.client.core.Site.ISite;
import my.vrestate.client.core.Site.ISiteLoadedListener;
import my.vrestate.client.core.Site.ISuite;
import my.vrestate.client.core.Site.Site;
import my.vrestate.client.interfaces.IShowSuiteInfoView;

import com.google.gwt.core.client.GWT;

public class SiteView extends View implements ILeftRightListener, IUpDownListener,IShowSuiteInfoListener,
	IShowSuiteInfoView, IPanoramicViewListener, ISiteLoadedListener{
	
	private my.vrestate.client.core.Camera Camera;
	private LookAt LookAt = null;
	private ISuite ActiveSuite;
	private LeftRightInteractor LeftRightInteractor = new LeftRightInteractor();
	private UpDownInteractor UpDownInteractor = new UpDownInteractor();
	private PanoramicViewInteractor PanoramicViewInteractor = new PanoramicViewInteractor();
	private ShowSuiteInfoInteractor ShowSuiteInfoInteractor = new ShowSuiteInfoInteractor(this);
	
	public SiteView(ISite site) {
		super(site);
	}

	
	
	@Override
	public void onSiteLoaded(Site site) {
		GWT.log("А сейчас будем двигать нашу камеру к нашим билдингам");
		LookAt look_at = this.getLookAt();
		Point center = site.getCenter();
		look_at.setLatitude(center.getLatitude());
		look_at.setLongitude(center.getLongitude());
		look_at.setAltitude(center.getAltitude());
		look_at.setHeading(45);
		look_at.setRange(200);
		look_at.setTilt(45);
		Options.CurrentLookAt = look_at;
		this.setLookAt(look_at);
		
		
		
///==================================
		LeftRightInteractor.addLeftRightListener(this);
		UpDownInteractor.addUpDownListener(this);
		PanoramicViewInteractor.addPanoramicViewListener(this);
		ShowSuiteInfoInteractor.addShowSuiteInfoListener(this);
		
		GEPlugin.addMouseEventListener(PanoramicViewInteractor);
		GEPlugin.addMouseEventListener(ShowSuiteInfoInteractor);
		GEPlugin.addMouseEventListener(LeftRightInteractor);
		GEPlugin.addMouseEventListener(UpDownInteractor);
		
		ShowSuiteInfoInteractor.setEnabled(true);
		ShowSuiteInfoInteractor.setVisible(true);
		PanoramicViewInteractor.setEnabled(true);
		PanoramicViewInteractor.setVisible(false);
		
//============================
		
		this.Update();
	}

	@Override
	public void onUpDown(double delta) {
		if (isVisible()) {
			LookAt look_at = this.getLookAt();
			double tilt = look_at.getTilt();
			tilt -= delta/10;
			tilt = tilt % 90;
			tilt = Math.max(tilt, 10);
			tilt = Math.min(tilt, 80);
			
			look_at.setTilt(tilt);
			Options.CurrentLookAt = look_at;
			this.setLookAt(look_at);
			this.Update();
		}
	}

	@Override
	public void onLeftRight(double delta) {
		if (isVisible()) { 
			LookAt look_at = this.getLookAt();
			double heading = look_at.getHeading();
	//		GWT.log("event_data" + event_data.);
	//		GWT.log("deltaX = " + delta);
			heading += delta/10;
			heading = heading % 360;
			look_at.setHeading(heading);
			this.setLookAt(look_at);
			Options.CurrentLookAt = look_at;
			this.Update();
		};
	}

	@Override
	public void onShowSuiteInfo(ISuite suite) {
		if (suite == null) {
			getSite().getGEPlugin().HideBalloon();
			PanoramicViewInteractor.setVisible(false);
		}else{
			suite.showInfo();
			ActiveSuite = suite;
			PanoramicViewInteractor.setVisible(true);
		};
		
	}

	@Override
	public void Update() {
//		GWT.log("Updating SiteView");
		for (IBuilding building : getSite().getBuildings()) {
			building.rePaint();
		}
	}

	@Override
	public ArrayList<ISuite> getSuites() {
		return getSite().getSuites();
	}

	@Override
	public void onPanoramicView() {
		this.setVisible(false);
//		ShowSuiteInfoInteractor.setPaused(true);
//		PanoramicViewInteractor.setPaused(true);
		getSite().getGEPlugin().HideBalloon();
//		setPaused(true);
		GWT.log("Go into panoramic view");
		new PanoramicView(ActiveSuite);
//		this.ShowSuiteInfoInteractor.setActive(false);
//		Window.alert("Panoramic view");
	}

	@Override
	public void setVisible(boolean visible) {
		super.setVisible(visible);
//		GWT.log("setPaused: " + pause);
		if (!visible)
			LookAt = this.getLookAt();
		else
			if(LookAt != null)
				this.setLookAt(LookAt);
		
		PanoramicViewInteractor.setVisible(visible);
//		GWT.log("x");
		ShowSuiteInfoInteractor.setVisible(visible);
//		GWT.log("x");
	}



	@Override
	public void setEnabled(boolean enable) {
		// TODO Auto-generated method stub
		
	}


}
