package com.condox.vrestate.client.filter;

import java.util.ArrayList;

import com.condox.vrestate.client.document.Document;
import com.condox.vrestate.client.document.Suite;
import com.google.gwt.event.dom.client.ChangeEvent;
import com.google.gwt.event.dom.client.ChangeHandler;
import com.google.gwt.user.client.ui.HorizontalPanel;
import com.google.gwt.user.client.ui.ListBox;
import com.google.gwt.user.client.ui.StackPanel;
import com.google.gwt.user.client.ui.VerticalPanel;

public class PriceSection extends VerticalPanel implements I_FilterSection {

	StackPanel stackPanel = null;

	private static PriceSection instance = null;
	private static ListBox cbMinPrice = null;
	private static ListBox cbMaxPrice = null;
	private ArrayList<Integer> prices = new ArrayList<Integer>();
	private int min_price_range = 0;
	private int max_price_range = 0;

	
	private PriceSection(){
		super();
	}
	
	public static PriceSection CreateSectionPanel(String sectionLabel, StackPanel stackPanel) {
		
		//==============
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
		//==============
		
		instance = new PriceSection();
		instance.stackPanel = stackPanel;  
		instance.setSpacing(5);
		stackPanel.add(instance, "Price (any)", false);
		instance.setSize("100%", "150px");

		HorizontalPanel horizontalPanel_1 = new HorizontalPanel();
		horizontalPanel_1.setSpacing(5);
		instance.add(horizontalPanel_1);
		horizontalPanel_1.setSize("100%", "100px");

		cbMinPrice = new ListBox();
		cbMinPrice.setEnabled(false);
		horizontalPanel_1.add(cbMinPrice);
		cbMinPrice.addChangeHandler(new ChangeHandler() {
			public void onChange(ChangeEvent event) {
				cbMaxPrice.setSelectedIndex(Math.max(
						cbMaxPrice.getSelectedIndex(),
						cbMinPrice.getSelectedIndex()));
				instance.isAny = cbMinPrice.getSelectedIndex() == 0 &&
								 cbMaxPrice.getSelectedIndex() == (cbMaxPrice.getItemCount()-1); 
				if (instance.isAny)
					instance.stackPanel.setStackText(instance.stackPanel.getWidgetIndex(instance), "Price (any)");
				else
					instance.stackPanel.setStackText(instance.stackPanel.getWidgetIndex(instance), "Price");
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
				instance.isAny = cbMinPrice.getSelectedIndex() == 0 &&
				 cbMaxPrice.getSelectedIndex() == (cbMaxPrice.getItemCount()-1); 
				if (instance.isAny)
					instance.stackPanel.setStackText(instance.stackPanel.getWidgetIndex(instance), "Price (any)");
				else
					instance.stackPanel.setStackText(instance.stackPanel.getWidgetIndex(instance), "Price");
			}
		});
		cbMaxPrice.setWidth("100%");
		
		return instance;
	}

	@Override
	public void Reset() {
		cbMinPrice.setEnabled(true);
		cbMinPrice.setSelectedIndex(0);
		cbMaxPrice.setEnabled(true);
		cbMaxPrice.setSelectedIndex(cbMaxPrice.getItemCount() - 1);
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
				cbMinPrice.addItem("Minimal", item.toString());
			else if (index == prices.size() - 1)
				cbMaxPrice.addItem("Maximal", item.toString());
			else {
				cbMinPrice.addItem("Min :$" + item, item.toString());
				cbMaxPrice.addItem("Max :$" + item, item.toString());
			}
		}
		isAny = true;
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
}
