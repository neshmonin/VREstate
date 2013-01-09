package com.condox.vrestate.client.filter;

import java.util.ArrayList;
import com.condox.vrestate.client.document.Document;
import com.condox.vrestate.client.document.Suite;
import com.condox.vrestate.client.document.SuiteType;
import com.google.gwt.event.dom.client.ChangeEvent;
import com.google.gwt.event.dom.client.ChangeHandler;
import com.google.gwt.user.client.ui.HorizontalPanel;
import com.google.gwt.user.client.ui.ListBox;
import com.google.gwt.user.client.ui.StackPanel;
import com.google.gwt.user.client.ui.VerticalPanel;

public class AreaSection extends VerticalPanel implements I_FilterSection {

	StackPanel stackPanel = null;

	private static AreaSection instance = null;
	private static ListBox lbMinArea = null;
	private static ListBox lbMaxArea = null;
	private ArrayList<Double> areas = new ArrayList<Double>();

	private AreaSection(){super();}
	
	public static AreaSection CreateSectionPanel(String sectionLabel, StackPanel stackPanel) {
		instance = new AreaSection();
		instance.stackPanel = stackPanel;  
		stackPanel.add(instance, "Area", false);
		instance.setSize("100%", "150px");

		HorizontalPanel horizontalPanel_2 = new HorizontalPanel();
		horizontalPanel_2.setSpacing(5);
		instance.add(horizontalPanel_2);
		horizontalPanel_2.setSize("100%", "100px");

		lbMinArea = new ListBox();
		lbMinArea.addChangeHandler(new ChangeHandler() {
			public void onChange(ChangeEvent event) {
				lbMinArea.setSelectedIndex(Math.min(
						lbMinArea.getSelectedIndex(),
						lbMaxArea.getSelectedIndex()));
				instance.isAny = lbMinArea.getSelectedIndex() == 0 &&
								 lbMaxArea.getSelectedIndex() == (lbMaxArea.getItemCount()-1);
			}
		});
		lbMinArea.setEnabled(false);
		horizontalPanel_2.add(lbMinArea);
		lbMinArea.setWidth("100%");

		lbMaxArea = new ListBox();
		lbMaxArea.addChangeHandler(new ChangeHandler() {
			public void onChange(ChangeEvent event) {
				lbMinArea.setSelectedIndex(Math.min(
						lbMinArea.getSelectedIndex(),
						lbMaxArea.getSelectedIndex()));
				instance.isAny = lbMinArea.getSelectedIndex() == 0 &&
				 lbMaxArea.getSelectedIndex() == (lbMaxArea.getItemCount()-1); 
			}
		});
		lbMaxArea.setEnabled(false);
		horizontalPanel_2.add(lbMaxArea);
		lbMaxArea.setWidth("100%");
		
		return instance;
	}
	
	@Override
	public void Init() {
		double min_area = Integer.MAX_VALUE;
		double max_area = Integer.MIN_VALUE;
		for (SuiteType suite_type : Document.get().getSuiteTypes()) {
			min_area = Math.min(min_area, suite_type.getArea());
			max_area = Math.max(max_area, suite_type.getArea());
		}

		int diff = (int) (max_area - min_area);
		diff /= 10;

		int a = 1;
		while (diff > a)
			a *= 10;

		int i = 0;
		while (a * (i + 1) < min_area)
			i++;

		areas.clear();

		while (a * i < max_area) {
			areas.add((double) (a * i));
			i++;
		}
		;
		areas.add((double) (a * i));

		lbMinArea.clear();
		lbMaxArea.clear();
		for (Double area : areas) {
			Double item = (area > 0) ? area : 100;
			lbMinArea.addItem("Min: " + item, item.toString() + " Sq.Ft.");
			lbMaxArea.addItem("Max: " + item, item.toString() + " Sq.Ft.");
		}		// TODO Auto-generated method stub

		isAny = true;
	}

	@Override
	public void Reset() {
		lbMinArea.setEnabled(true);
		lbMinArea.setSelectedIndex(0);
		lbMaxArea.setEnabled(true);
		lbMaxArea.setSelectedIndex(lbMaxArea.getItemCount() - 1);
		isAny = true;
	}

	@Override
	public boolean isFileredIn(Suite suite) {
		if (isAny)
			return true;
		
		SuiteType type = suite.getSuiteType();
		double area = type.getArea();
		if (area < areas.get(lbMinArea.getSelectedIndex()))
			return false;
		if (area > areas.get(lbMaxArea.getSelectedIndex()))
			return false;

		return true;
	}

	private boolean isAny = true;

	@Override
	public boolean isAny() {
		return isAny;
	}
}
