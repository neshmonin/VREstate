package my.vrestate.client.core.Views;

import my.vrestate.client.Interactors.BackViewInteractor;
import my.vrestate.client.Interactors.IBackViewListener;
import my.vrestate.client.Interactors.ILeftRightListener;
import my.vrestate.client.Interactors.IUpDownListener;
import my.vrestate.client.Interactors.LeftRightInteractor;
import my.vrestate.client.Interactors.UpDownInteractor;
import my.vrestate.client.core.Site.ISuite;

import com.google.gwt.core.client.GWT;

public class PanoramicView extends View implements ILeftRightListener, IUpDownListener, IBackViewListener{
	private double MIN_TILT = 60;
	private double DEFAULT_TILT = 90;
	private double MAX_TILT = 100;
	private double MIN_HEADING = 0;
	private double DEFAULT_HEADING = 0;
	private double MAX_HEADING = 0;
	private my.vrestate.client.core.Camera Camera = null;

	
	private ISuite Suite;
//	private IGEPlugin GEPlugin;
	private LeftRightInteractor LeftRightInteractor = new LeftRightInteractor();
	private UpDownInteractor UpDownInteractor = new UpDownInteractor();
	private BackViewInteractor BackViewInteractor = new BackViewInteractor();
//	private ShowSuiteInfoInteractor ShowSuiteInfoInteractor = new ShowSuiteInfoInteractor(this);
	
	public PanoramicView(ISuite suite) {
		super(suite.getBuilding().getSite());
		Suite = suite;
		GEPlugin = Suite.getBuilding().getSite().getGEPlugin();
		
		LeftRightInteractor.addLeftRightListener(this);
		UpDownInteractor.addUpDownListener(this);
		BackViewInteractor.addBackViewListener(this);
//		ShowSuiteInfoInteractor.addShowSuiteInfoListener(this);
		
		GEPlugin.addMouseEventListener(BackViewInteractor);
//		GEPlugin.addMouseEventListener(ShowSuiteInfoInteractor);
		GEPlugin.addMouseEventListener(LeftRightInteractor);
		GEPlugin.addMouseEventListener(UpDownInteractor);
		
//		ShowSuiteInfoInteractor.setEnabled(true);
//		ShowSuiteInfoInteractor.setPaused(false);
//		PanoramicViewInteractor.setEnabled(false);
//		PanoramicViewInteractor.setPaused(false);
		
		BackViewInteractor.setEnabled(true);
		BackViewInteractor.setVisible(true);
		
		
		
		
		Camera = GEPlugin.getCamera();
		Camera.setPosition(Suite.getPosition());

		DEFAULT_HEADING = Suite.getHDG();
		while (DEFAULT_HEADING < -180) DEFAULT_HEADING += 360;
		while (DEFAULT_HEADING >  180) DEFAULT_HEADING -= 360;

		MIN_HEADING = DEFAULT_HEADING - 45;
		while (MIN_HEADING < -180) MIN_HEADING += 360;
		while (MIN_HEADING >  180) MIN_HEADING -= 360;

		MAX_HEADING = DEFAULT_HEADING + 45;
		while (MAX_HEADING < -180) MAX_HEADING += 360;
		while (MAX_HEADING >  180) MAX_HEADING -= 360;

		
//		GWT.log("default:" + DEFAULT_HEADING);
//		GWT.log("min:" + MIN_HEADING);
//		GWT.log("max:" + MAX_HEADING);
		
		
		Camera.setTilt(DEFAULT_TILT);
		Camera.setHeading(DEFAULT_HEADING);
		GEPlugin.setCamera(Camera);
	}

	@Override
	public void Update() {
		// TODO Auto-generated method stub
		
	}

	@Override
	public void onUpDown(double delta) {
		Camera = this.getCamera();
		delta /= 10;
		double tilt = Camera.getTilt();
		tilt = (tilt + delta + 360) % 360;
		tilt = Math.max(tilt, MIN_TILT);
		tilt = Math.min(tilt, MAX_TILT);
		Camera.setTilt(tilt);
		this.setCamera(Camera);
//		GWT.log("new camera tilt:" + tilt);
//		Events.FireEvent(new Event(Type.PV_CENTERED, isCentered()));
		
	}

	@Override
	public void onLeftRight(double delta) {

//		GWT.log("ET_LEFT_RIGHT received by PanoramicView");
		Camera = this.getCamera();
		delta /= 10;
//		GWT.log("delta:" + delta);
		
		double heading = Camera.getHeading();
//		GWT.log("heading:" + heading);
		heading = heading - delta;
		
		double diff = heading - MAX_HEADING;
		if (diff > 180 && diff < 540)
			diff -= 360;
		if (diff < -180 && diff > -540)
			diff += 360;
		if (diff > 0)
			heading = MAX_HEADING;

		diff = heading - MIN_HEADING;
		if (diff > 180 && diff < 540)
			diff -= 360;
		if (diff < -180 && diff > -540)
			diff += 360;
		if (diff < 0)
			heading = MIN_HEADING;
		Camera.setHeading(heading);
		this.setCamera(Camera);
//		Events.FireEvent(new Event(Type.PV_CENTERED, isCentered()));
	}

	@Override
	public void onBackView() {
		GWT.log("onBackView");
		
//		GWT.log("Going back");
		BackViewInteractor.setVisible(false);
		
		LeftRightInteractor.removeLeftRightListener(this);
		UpDownInteractor.removeUpDownListener(this);
		BackViewInteractor.removeBackViewListener(this);
		GEPlugin.removeMouseEventListener(LeftRightInteractor);
		GEPlugin.removeMouseEventListener(UpDownInteractor);
		GEPlugin.removeMouseEventListener(BackViewInteractor);
		
		Back().setVisible(true);
////		GWT.log("X");
//		((SiteView)this.Back()).setPaused(false);
	}

	@Override
	public void setVisible(boolean visible) {
		// TODO Auto-generated method stub
		
	}

	@Override
	public void setEnabled(boolean enable) {
		// TODO Auto-generated method stub
		
	}

}
