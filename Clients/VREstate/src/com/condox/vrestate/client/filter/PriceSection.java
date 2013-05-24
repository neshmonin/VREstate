package com.condox.vrestate.client.filter;

import java.util.ArrayList;

import com.condox.vrestate.client.Log;
import com.condox.vrestate.client.document.Suite;
import com.condox.vrestate.client.view.GeoItems.SuiteGeoItem;
import com.google.gwt.event.dom.client.ChangeEvent;
import com.google.gwt.event.dom.client.ChangeHandler;
import com.google.gwt.event.logical.shared.ValueChangeEvent;
import com.google.gwt.event.logical.shared.ValueChangeHandler;
import com.google.gwt.i18n.client.NumberFormat;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.user.client.ui.CheckBox;
import com.google.gwt.user.client.ui.HorizontalPanel;
import com.google.gwt.user.client.ui.ListBox;
import com.google.gwt.user.client.ui.StackPanel;
import com.google.gwt.user.client.ui.VerticalPanel;

public class PriceSection extends VerticalPanel implements I_FilterSection {

	StackPanel stackPanel = null;

	public enum PriceType {
		Ownership,
		Rent
	}
	private static PriceSection instance = null;
	private CheckBox cbAnyPrice = null;
	private ListBox cbMinPrice = null;
	private ListBox cbMaxPrice = null;
	private ArrayList<Integer> prices = null;
	private String sectionLabel = "";
	private PriceType priceType;
	private I_FilterSectionContainer parentSection;

	private PriceSection() {
		super();
	}

	public static PriceSection CreateSectionPanel(I_FilterSectionContainer parentSection, 
			String sectionLabel,
			StackPanel stackPanel, PriceType priceType) {
		Log.write("PriceSection(" + sectionLabel + ")");
		// ==============
		int min_price = Integer.MAX_VALUE;
		int max_price = Integer.MIN_VALUE;
		for (SuiteGeoItem suiteGE : parentSection.getActiveSuiteGeoItems().values()) {
			Suite.Status status = suiteGE.suite.getStatus(); 
			if (status == Suite.Status.Sold)
				continue;

			if (priceType == PriceType.Ownership && 
				status == Suite.Status.AvailableRent)
				continue;
			else
			if (priceType == PriceType.Rent && 
				status != Suite.Status.AvailableRent)
				continue;
				
			int price = suiteGE.suite.getPrice();
			if (price > 0) {
				min_price = Math.min(min_price, price);
				max_price = Math.max(max_price, price);
			}
		}
		if (min_price == Integer.MAX_VALUE || max_price == Integer.MIN_VALUE)
			return null;
		
		if (max_price <= min_price)
			return null;
		// ==============

		instance = new PriceSection();
		instance.parentSection = parentSection;
		instance.prices = new ArrayList<Integer>();
		instance.sectionLabel = sectionLabel;
		instance.priceType = priceType;

		instance.stackPanel = stackPanel;
		instance.setSpacing(5);
		stackPanel.add(instance, instance.generateLabel(), false);
		instance.setSize("100%", "150px");

		VerticalPanel vpPrice = new VerticalPanel();
		vpPrice.setSpacing(5);
		instance.add(vpPrice);
		vpPrice.setWidth("100%");

		instance.cbAnyPrice = new CheckBox("Any price");
		instance.cbAnyPrice.addValueChangeHandler(new ValueChangeHandler<Boolean>() {

			@Override
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (instance.cbAnyPrice.getValue()) {
					instance.cbMinPrice.setSelectedIndex(0); // !!
					instance.cbMaxPrice.setSelectedIndex(instance.cbMaxPrice.getItemCount() - 1); // !!
					instance.isAny = true;
				} // else
				instance.Apply();
			}
		});
		vpPrice.add(instance.cbAnyPrice);

		HorizontalPanel horizontalPanel_1 = new HorizontalPanel();
		horizontalPanel_1.setSpacing(5);
		vpPrice.add(horizontalPanel_1);
		horizontalPanel_1.setSize("100%", "100px");

		instance.cbMinPrice = new ListBox();
		instance.cbMinPrice.setEnabled(false);
		horizontalPanel_1.add(instance.cbMinPrice);
		instance.cbMinPrice.addChangeHandler(new ChangeHandler() {
			public void onChange(ChangeEvent event) {
				instance.cbMaxPrice.setSelectedIndex(Math.max(
						instance.cbMaxPrice.getSelectedIndex(),
						instance.cbMinPrice.getSelectedIndex()));
				instance.isAny = instance.cbMinPrice.getSelectedIndex() == 0
						&& instance.cbMaxPrice.getSelectedIndex() == (instance.cbMaxPrice
								.getItemCount() - 1);
				instance.cbAnyPrice.setValue(instance.isAny);
				instance.Apply();
			}
		});
		instance.cbMinPrice.setWidth("100%");

		instance.cbMaxPrice = new ListBox();
		instance.cbMaxPrice.setEnabled(false);
		horizontalPanel_1.add(instance.cbMaxPrice);
		instance.cbMaxPrice.addChangeHandler(new ChangeHandler() {
			public void onChange(ChangeEvent event) {
				instance.cbMinPrice.setSelectedIndex(Math.min(
						instance.cbMinPrice.getSelectedIndex(),
						instance.cbMaxPrice.getSelectedIndex()));
				instance.isAny = instance.cbMinPrice.getSelectedIndex() == 0
						&& instance.cbMaxPrice.getSelectedIndex() == (instance.cbMaxPrice
								.getItemCount() - 1);
				instance.cbAnyPrice.setValue(instance.isAny);
				instance.Apply();
			}
		});
		instance.cbMaxPrice.setWidth("100%");

		return instance;
	}

	private String generateLabel() {
		return sectionLabel + (isAny ? " (any)" : "");
	}
	
	@Override
	public void Reset() {
		cbAnyPrice.setValue(true, true);
		cbMinPrice.setEnabled(true);
		cbMinPrice.setSelectedIndex(0); // !!
		cbMaxPrice.setEnabled(true);
		cbMaxPrice.setSelectedIndex(cbMaxPrice.getItemCount() - 1); //!!
		isAny = true;
	}

	@Override
	public int StateHash() {
		int hash = hashCode();
		if (cbAnyPrice.getValue()) hash += cbAnyPrice.hashCode();
		hash += cbMinPrice.hashCode() * cbMinPrice.getSelectedIndex();
		hash += cbMaxPrice.hashCode() * cbMaxPrice.getSelectedIndex();
		
		return hash;
	}
	
	@Override
	public boolean isFilteredIn(SuiteGeoItem suiteGI) {
		if (isAny)
			return true;

		int price = suiteGI.getPrice();
		if (prices.isEmpty())
			return true;
		if (price <= prices.get(cbMinPrice.getSelectedIndex()))
			return false;
		if (price >= prices.get(cbMaxPrice.getSelectedIndex() + 1))
			return false;
		return true;
	}

	@Override
	public void Init() {
		int min_price = Integer.MAX_VALUE;
		int max_price = Integer.MIN_VALUE;
		for (SuiteGeoItem suiteGI : getParentSectionContainer().getActiveSuiteGeoItems().values()) {
			Suite.Status status = suiteGI.suite.getStatus(); 
			if (status == Suite.Status.Sold)
				continue;

			if (priceType == PriceType.Ownership && 
				status == Suite.Status.AvailableRent)
				continue;
			else
			if (priceType == PriceType.Rent && 
				status != Suite.Status.AvailableRent)
				continue;
				
			int price = suiteGI.suite.getPrice();
			if (price > 0) {
				min_price = Math.min(min_price, price);
				max_price = Math.max(max_price, price);
			}
		}

		int diff = max_price - min_price;
		diff /= 10;

		int a = 1;
		while (diff > a)
			a *= 10;

		while (true)
		{
			int i = 0;
			while (a * (i + 1) < min_price)
				i++;
	
			prices.clear();
	
			// prices.add(Integer.MIN_VALUE);
	
			while (a * i < max_price) {
				prices.add(a * i);
				i++;
			}
			if (i >= 4) {
				prices.add(a * i);
				break;
			}

			a = a/2;
		}

		// prices.add(Integer.MAX_VALUE);

		cbMinPrice.clear();
		cbMaxPrice.clear();

		for (Integer item : prices) {
			int index = prices.indexOf(item);
			if (index == 0)
				cbMinPrice.addItem("Min", item.toString());
			else if (index == prices.size() - 1)
				cbMaxPrice.addItem("Max", item.toString());
			else {
				String price = NumberFormat.getDecimalFormat().format(item);
				cbMinPrice.addItem("$" + price, item.toString());
				cbMaxPrice.addItem("$" + price, item.toString());
			}
		}
		isAny = true;
//		Apply();
	}

	private boolean isAny = true;

	@Override
	public boolean isAny() {
		return isAny;
	}

	public JSONObject getJSON() {
		return null;
	}

	@Override
	public void Apply() {
		stackPanel.setStackText(stackPanel.getWidgetIndex(this), generateLabel());
		Filter.onChange();
	}

	@Override
	public void RemoveSection() {
		super.removeFromParent();
	}

	@Override
	public I_FilterSectionContainer getParentSectionContainer() {
		return parentSection;
	}
}
