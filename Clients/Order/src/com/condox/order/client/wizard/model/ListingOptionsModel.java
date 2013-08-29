package com.condox.order.client.wizard.model;

import com.condox.order.client.I_Model;
import com.condox.order.client.wizard.I_WizardStep;
import com.condox.order.client.wizard.WizardStep;
import com.condox.order.client.wizard.presenter.ListingOptionsPresenter;
import com.condox.order.client.wizard.presenter.LoginPresenter;
import com.condox.order.client.wizard.view.ListingOptionsView;
import com.condox.order.client.wizard.view.LoginView;
import com.google.gwt.user.client.ui.HasWidgets;

public class ListingOptionsModel extends WizardStep {

	public ListingOptionsModel(I_WizardStep parent) {
		super(parent);
		// TODO Auto-generated constructor stub
	}
	
	private String role = "";
	private String uid = "";
	private String pwd = "";
	private String sid = "";
	private String mls = "";
	private String urlVirtualTour = "";
	private String urlMoreInfo = "";
	
	public void setMls(String mls) {
		this.mls = mls;
	}

	public String getMls() {
		return mls;
	}

	
	public void setUrlVirtualTour(String urlVirtualTour) {
		this.urlVirtualTour = urlVirtualTour;
	}

	public String getUrlVirtualTour() {
		return urlVirtualTour;
	}

	public void setUrlMoreInfo(String urlMoreInfo) {
		this.urlMoreInfo = urlMoreInfo;
	}

	public String getUrlMoreInfo() {
		return urlMoreInfo;
	}

	// GETTERS
	public String getRole() {
		return this.role;
	}

	public String getUserLogin() {
		return this.uid;
	}

	public String getUserPassword() {
		return this.pwd;
	}

	public String getUserSid() {
		return this.sid;
	}

	// SETTERS
	public void setUserLogin(String uid) {
		this.uid = uid;
	}

	public void setUserPassword(String pwd) {
		this.pwd = pwd;
	}

	public void setUserSid(String sid) {
		this.sid = sid;
	}

//	@Override
	public boolean isValid() {
		/*
		 * boolean valid = true; valid &= !sid.isEmpty();
		 */
		boolean valid = true;
		valid &= "web".equals(uid);
		valid &= "web".equals(pwd);
		return valid;
	}

	private HasWidgets container = null;
	@Override
	public void go(HasWidgets container) {
		this.container = container;
		ListingOptionsPresenter presenter = new ListingOptionsPresenter(new ListingOptionsView(), this);
		presenter.go(container);
		super.go(container);
	}

	public void next() {
		getNextStep().go(container);
	}

	@Override
	protected I_WizardStep createNextStep() {
		children.put(this, new SummaryModel(this));
		return children.get(this);
	}

	public void prev() {
		getPrevStep().go(container);
	}
	
	
}
