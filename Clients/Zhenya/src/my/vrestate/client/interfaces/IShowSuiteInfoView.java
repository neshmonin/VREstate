package my.vrestate.client.interfaces;

import java.util.ArrayList;

import my.vrestate.client.core.Site.ISuite;
import my.vrestate.client.core.Views.IView;

public interface IShowSuiteInfoView extends IView{
	public ArrayList<ISuite> getSuites();

}
