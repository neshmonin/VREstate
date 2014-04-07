package com.condox.ecommerce.client.tree.view;

import com.condox.clientshared.communication.User;
import com.condox.clientshared.communication.User.UserRole;
import com.condox.clientshared.document.SuiteInfo.Status;
import com.condox.ecommerce.client.tree.EcommerceTree.ListingType;
import com.condox.ecommerce.client.tree.presenter.OptionsPresenter;
import com.condox.ecommerce.client.tree.presenter.OptionsPresenter.I_Display;
import com.google.gwt.core.client.GWT;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.user.client.Timer;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.Label;
import com.google.gwt.user.client.ui.RadioButton;
import com.google.gwt.user.client.ui.TextBox;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.user.client.ui.VerticalPanel;
import com.google.gwt.user.client.ui.Image;

public class OptionsView extends Composite implements I_Display {

	private static OptionsViewUiBinder uiBinder = GWT
			.create(OptionsViewUiBinder.class);
	@UiField
	RadioButton rbForSale;
	@UiField
	RadioButton rbForRent;
	@UiField
	Button buttonCancel;
	@UiField
	Button buttonPrev;
	@UiField
	Button buttonNext;
	@UiField
	TextBox textPrice;
	@UiField
	TextBox textVirtualTourUrl;
	@UiField
	TextBox textMoreInfoUrl;
	@UiField
	TextBox textMLS;
	@UiField
	Label priceCaption;
	@UiField Image image;
	@UiField Label listingType;

	interface OptionsViewUiBinder extends UiBinder<Widget, OptionsView> {
	}

	private OptionsPresenter presenter = null;

	public OptionsView() {
		initWidget(uiBinder.createAndBindUi(this));
		Timer validate = new Timer() {

			@Override
			public void run() {
				int price = 0;
				if (!textPrice.getValue().isEmpty())
					try {
						price = Integer.valueOf(textPrice.getValue());
					} catch (NumberFormatException e) {
						textPrice.setValue("" + price);
						e.printStackTrace();
					}
				price = Math.max(price, 0);
				textPrice.setValue("" + price);
				boolean valid_sale_price = (!rbForSale.getValue() || price > 10000);
				boolean valid_rent_price = (!rbForRent.getValue() || price < 10000
						&& price > 0);
				// textPrice.setStyleDependentName("incorrect",
				// !(valid_sale_price && valid_rent_price));
				boolean valid = valid_sale_price && valid_rent_price;
				valid &= (textMLS.getValue().isEmpty() || textMLS.getValue()
						.matches("[A-Z]{1,1}[0-9]{7,7}"));
				buttonNext.setEnabled(valid);
				priceCaption.setText(rbForRent.getValue() ? "Price, $/m"
						: "Price, $");

			}
		};
		validate.scheduleRepeating(100);
	}

	@Override
	public void setPresenter(OptionsPresenter presenter) {
		this.presenter = presenter;
//		boolean usingMLS = presenter.isUsingMLS();
		// if (usingMLS) {
		// rbForRent.setEnabled(false);
		// rbForSale.setEnabled(false);
		// textMLS.setReadOnly(true);
		// textPrice.setReadOnly(true);
		// }

		if (User.role.equals(UserRole.Visitor)) {
			textMLS.setEnabled(false);
			rbForRent.setEnabled(false);
			rbForSale.setEnabled(false);
			textPrice.setEnabled(false);
		}
	}

	@UiHandler("buttonCancel")
	void onButtonCancelClick(ClickEvent event) {
		if (presenter != null)
			presenter.onCancel();
	}

	@UiHandler("buttonPrev")
	void onButtonPrevClick(ClickEvent event) {
		if (presenter != null)
			presenter.onPrev();
	}

	@UiHandler("buttonNext")
	void onButtonNextClick(ClickEvent event) {
		if (presenter != null)
			presenter.onNext();
	}

	// @Override
	// public String getMLS() {
	// return strMLS.getText();
	// }
	//
	// @Override
	// public String getPrice() {
	// return textPrice.getValue();
	// }

	@Override
	public String getVirtualTourUrl() {
		if ("http://".equals(textVirtualTourUrl.getValue()))
			return "";
		else
			return textVirtualTourUrl.getValue();
	}

	@Override
	public String getMoreInfoUrl() {
		if ("http://".equals(textMoreInfoUrl.getValue()))
			return "";
		else
			return textMoreInfoUrl.getValue();
	}

	@Override
	public String getMLS() {
		return textMLS.getValue();
	}

	@Override
	public void setMLS(String newMLS) {
		textMLS.setValue(newMLS);
		if (!getMLS().isEmpty()) {
			rbForRent.setEnabled(false);
			rbForSale.setEnabled(false);
			textMLS.setEnabled(false);
			textPrice.setEnabled(false);
		}
	}

	@Override
	public int getPrice() {
		return Integer.valueOf(textPrice.getValue());
	}

	@Override
	public void setPrice(int price) {
		textPrice.setValue("" + Math.max(price, 0));
	}

	@Override
	public Status getStatus() {
		if (rbForSale.getValue())
			return Status.AvailableResale;
		if (rbForRent.getValue())
			return Status.AvailableRent;
		return Status.AvailableRent;
	}

	@Override
	public void setStatus(Status newStatus) {
		switch (newStatus) {
		case AvailableRent:
			rbForRent.setValue(true);
			break;
		case AvailableResale:
			rbForSale.setValue(true);
			break;
		default:
			rbForSale.setValue(true);
			break;
		}
	}

	@Override
	public void setVirtualTourUrl(String newUrl) {
		textVirtualTourUrl.setValue(newUrl);
		if (textVirtualTourUrl.getValue().isEmpty())
			textVirtualTourUrl.setValue("http://");
	}

	@Override
	public void setMoreInfoUrl(String newUrl) {
		textMoreInfoUrl.setValue(newUrl);
		if (textMoreInfoUrl.getValue().isEmpty())
			textMoreInfoUrl.setValue("http://");
	}

	@Override
	public void setViewOrderType(ListingType type) {
		switch (type) {
		case PRIVATE:
			listingType.setText("Specify the options for the Private Listing:");
			image.setUrl("PrivateListing.png");
			break;
		case PUBLIC:
			listingType.setText("Specify the options for the Public Listing:");
			image.setUrl("PublicListing.png");
			break;
		}

	}

}
