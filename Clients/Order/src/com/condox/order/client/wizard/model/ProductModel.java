package com.condox.order.client.wizard.model;

import com.condox.order.client.wizard.I_WizardStep;
import com.condox.order.client.wizard.WizardStep;
import com.condox.order.client.wizard.presenter.ProductPresenter;
import com.condox.order.client.wizard.view.ProductView;
import com.google.gwt.user.client.ui.HasWidgets;

public class ProductModel extends WizardStep {

	public ProductModel(I_WizardStep parent) {
		super(parent);
	}

	private boolean Listing = true;
	private boolean ListingPrivate = true;
	private boolean ListingShared = false;
	private boolean Layout = false;

	// GETTERS
	public boolean getListing() {
		return Listing;
	}

	public boolean getListingPrivate() {
		return ListingPrivate;
	}

	public boolean getListingShared() {
		return ListingShared;
	}

	public boolean getLayout() {
		return Layout;
	}

	// SETTERS
	public void setListing(boolean value) {
		Listing = value;
		Layout = !value;
	}

	public void setListingPrivate(boolean value) {
		ListingPrivate = value;
		ListingShared = !value;
	}

	public void setListingShared(boolean value) {
		ListingPrivate = !value;
		ListingShared = value;
	}

	public void setLayout(boolean value) {
		Listing = !value;
		Layout = value;
	}

	/*@Override
	public boolean isValid() {
		boolean valid = true;
		return valid;
	}*/

	private HasWidgets container = null;

	/**
	 * @wbp.parser.entryPoint
	 */
	@Override
	public void go(HasWidgets container) {
//		super.go(container, navigator);
		this.container = container;
		new ProductPresenter(new ProductView(), this).go(container);
		super.go(container);
	}
	
	public void prev() {
//		Log.write("ProductModel.prev");
		if (getPrevStep() != null)
			getPrevStep().go(container);		
	}
	
	public void next() {
//		Log.write("ProductModel.next");
		getNextStep().go(container);
	}

	@Override
	public int hashCode() {
		final int prime = 31;
		int result = 1;
		result = prime * result + (Layout ? 1231 : 1237);
		result = prime * result + (Listing ? 1231 : 1237);
		result = prime * result + (ListingPrivate ? 1231 : 1237);
		result = prime * result + (ListingShared ? 1231 : 1237);
		return result;
	}

	@Override
	public boolean equals(Object obj) {
		if (this == obj)
			return true;
		if (obj == null)
			return false;
		if (getClass() != obj.getClass())
			return false;
		ProductModel other = (ProductModel) obj;
		if (Layout != other.Layout)
			return false;
		if (Listing != other.Listing)
			return false;
		if (ListingPrivate != other.ListingPrivate)
			return false;
		if (ListingShared != other.ListingShared)
			return false;
		return true;
	}
	//***************************

	@Override
	protected I_WizardStep createNextStep() {
//		Log.write("new BuildingsModel");
		if (Listing)
			children.put(this, new MLSModel(this));
		else
			children.put(this, new BuildingsModel(this));
		return children.get(this);
	}

	@Override
	public String getNavURL() {
		return "Product type";
	}

	@Override
	public StepTypes getStepType() {
		return StepTypes.ProductModel;
	}
}
