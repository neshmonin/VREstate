package com.condox.vrestate.client.filter;

import java.text.DecimalFormat;
import java.util.ArrayList;
import java.util.Formatter;

import com.condox.vrestate.client.Log;
import com.condox.vrestate.client.document.Document;
import com.condox.vrestate.client.document.Suite;
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

	private static PriceSection instance = null;
	private static CheckBox cbAnyPrice = null;
	private static ListBox cbMinPrice = null;
	private static ListBox cbMaxPrice = null;
	private static ArrayList<Integer> prices = new ArrayList<Integer>();
	private int min_price_range = 0;
	private int max_price_range = 0;
	private int last_min_price_range = 0;
	private int last_max_price_range = 0;

	private PriceSection() {
		super();
	}

	public static PriceSection CreateSectionPanel(String sectionLabel,
			StackPanel stackPanel) {

		// ==============
		int min_price = Integer.MAX_VALUE;
		int max_price = Integer.MIN_VALUE;
		for (Suite suite : Document.get().getSuites()) {
			min_price = Math.min(min_price, suite.getPrice());
			max_price = Math.max(max_price, suite.getPrice());
		}
		if (min_price <= 0)
			return null;
		if (max_price <= 0)
			return null;
		if (max_price <= min_price)
			return null;
		// ==============

		instance = new PriceSection();
		instance.stackPanel = stackPanel;
		instance.setSpacing(5);
		stackPanel.add(instance, "Price (any)", false);
		instance.setSize("100%", "150px");

		VerticalPanel vpPrice = new VerticalPanel();
		vpPrice.setSpacing(5);
		instance.add(vpPrice);
		vpPrice.setWidth("100%");

		cbAnyPrice = new CheckBox("Any price");
		cbAnyPrice.addValueChangeHandler(new ValueChangeHandler<Boolean>() {

			@Override
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (cbAnyPrice.getValue()) {
					cbMinPrice.setSelectedIndex(0); // !!
					cbMaxPrice.setSelectedIndex(cbMaxPrice.getItemCount() - 1); // !!
					instance.isChanged = true;
					instance.isAny = true;
				} // else
				UpdateSectionCaption();
			}
		});
		vpPrice.add(cbAnyPrice);

		HorizontalPanel horizontalPanel_1 = new HorizontalPanel();
		horizontalPanel_1.setSpacing(5);
		vpPrice.add(horizontalPanel_1);
		horizontalPanel_1.setSize("100%", "100px");

		cbMinPrice = new ListBox();
		cbMinPrice.setEnabled(false);
		horizontalPanel_1.add(cbMinPrice);
		cbMinPrice.addChangeHandler(new ChangeHandler() {
			public void onChange(ChangeEvent event) {
				cbMaxPrice.setSelectedIndex(Math.max(
						cbMaxPrice.getSelectedIndex(),
						cbMinPrice.getSelectedIndex()));
				instance.isAny = cbMinPrice.getSelectedIndex() == 0
						&& cbMaxPrice.getSelectedIndex() == (cbMaxPrice
								.getItemCount() - 1);
				cbAnyPrice.setValue(instance.isAny);
				UpdateSectionCaption();
			}
		});
		cbMinPrice.setWidth("100%");

		cbMaxPrice = new ListBox();
		cbMaxPrice.setEnabled(false);
		horizontalPanel_1.add(cbMaxPrice);
		cbMaxPrice.addChangeHandler(new ChangeHandler() {
			public void onChange(ChangeEvent event) {
				cbMinPrice.setSelectedIndex(Math.min(
						cbMinPrice.getSelectedIndex(),
						cbMaxPrice.getSelectedIndex()));
				instance.isAny = cbMinPrice.getSelectedIndex() == 0
						&& cbMaxPrice.getSelectedIndex() == (cbMaxPrice
								.getItemCount() - 1);
				cbAnyPrice.setValue(instance.isAny);
				UpdateSectionCaption();
			}
		});
		cbMaxPrice.setWidth("100%");

		return instance;
	}

	@Override
	public void Reset() {
		cbAnyPrice.setValue(true, true);
		cbMinPrice.setEnabled(true);
		// cbMinPrice.setSelectedIndex(0); // !!
		cbMaxPrice.setEnabled(true);
		// cbMaxPrice.setSelectedIndex(cbMaxPrice.getItemCount() - 1); //!!
		isAny = true;
	}

	@Override
	public boolean isFileredIn(Suite suite) {
		if (isAny)
			return true;

		int price = suite.getPrice();
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
		for (Suite suite : Document.get().getSuites()) {
			min_price = Math.min(min_price, suite.getPrice());
			max_price = Math.max(max_price, suite.getPrice());
		}

		int diff = max_price - min_price;
		diff /= 10;

		int a = 1;
		while (diff > a)
			a *= 10;

		int i = 0;
		while (a * (i + 1) < min_price)
			i++;

		prices.clear();

		// prices.add(Integer.MIN_VALUE);

		while (a * i < max_price) {
			prices.add(a * i);
			i++;
		}
		prices.add(a * i);
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

	public int getMinPriceRange() {
		return min_price_range;
	}

	public int getMaxPriceRange() {
		return max_price_range;
	}

	private boolean isAny = true;

	@Override
	public boolean isAny() {
		return isAny;
	}

	private static void UpdateSectionCaption() {
		if (instance.isAny)
			instance.stackPanel
					.setStackText(instance.stackPanel.getWidgetIndex(instance),
							"Price (any)");
		else
			instance.stackPanel.setStackText(
					instance.stackPanel.getWidgetIndex(instance), "Price");
		instance.isChanged = true;
		if (Filter.initialized == true)
			Filter.get().onChanged();
	}

	private boolean isChanged = false;
	
	@Override
	public boolean isChanged() {
		return isChanged;
	}

	private JSONObject getJSON() {
		return null;
	}

	@Override
	public void Apply() {
		isChanged = false;
		if (Filter.initialized == true)
			Filter.get().onChanged();
	}
}
