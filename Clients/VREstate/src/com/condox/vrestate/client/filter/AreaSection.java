package com.condox.vrestate.client.filter;


import java.util.ArrayList;

import com.condox.clientshared.abstractview.Log;
import com.condox.clientshared.document.SuiteType;
import com.condox.vrestate.client.view.GeoItems.SuiteGeoItem;
import com.google.gwt.event.dom.client.ChangeEvent;
import com.google.gwt.event.dom.client.ChangeHandler;
import com.google.gwt.event.logical.shared.ValueChangeEvent;
import com.google.gwt.event.logical.shared.ValueChangeHandler;
import com.google.gwt.i18n.client.NumberFormat;
import com.google.gwt.json.client.JSONNumber;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONString;
import com.google.gwt.user.client.ui.HorizontalPanel;
import com.google.gwt.user.client.ui.ListBox;
import com.google.gwt.user.client.ui.StackPanel;
import com.google.gwt.user.client.ui.VerticalPanel;

public class AreaSection extends VerticalPanel implements I_FilterSection {

	StackPanel stackPanel = null;

	private static AreaSection instance = null;
	private static MyCustomCheckBox cbAnyArea = null;
	private static ListBox lbMinArea = null;
	private static ListBox lbMaxArea = null;
	private ArrayList<Double> areas = new ArrayList<Double>();
	private I_FilterSectionContainer parentSection;

	private AreaSection(){super();}
	
	public static I_FilterSection CreateSectionPanel(I_FilterSectionContainer parentSection, 
			String sectionLabel,
			StackPanel stackPanel) {
		Log.write("AreaSection(" + sectionLabel + ")");
		//==============
		double min_area = Integer.MAX_VALUE;
		double max_area = Integer.MIN_VALUE;
		for (SuiteType suite_type : parentSection.getActiveSuiteTypes().values()) {
			double area = suite_type.getArea(); 
			if (area <= 0)
				continue;
			min_area = Math.min(min_area, area);
			max_area = Math.max(max_area, area);
		}
		if (min_area == Integer.MAX_VALUE || max_area == Integer.MIN_VALUE)
			return null;
		
		if (max_area <= min_area)
			return null;
		//==============
		
		instance = new AreaSection();
		instance.parentSection = parentSection;
		instance.stackPanel = stackPanel;  
		stackPanel.add(instance, "Area (any)", false);
		instance.setSize("100%", "150px");
		
		VerticalPanel vpArea = new VerticalPanel();
		vpArea.setSpacing(5);
		instance.add(vpArea);
		vpArea.setWidth("100%");
		
		cbAnyArea = new MyCustomCheckBox("Any area");
		cbAnyArea.addValueChangeHandler(new ValueChangeHandler<Boolean>() {

			@Override
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (cbAnyArea.getValue()) {
					lbMinArea.setSelectedIndex(0); // !!
					lbMaxArea.setSelectedIndex(lbMaxArea.getItemCount() - 1);	//!!
					instance.isAny = true;
				} //else 
					//cbAnyPrice.setValue(true,true);
				instance.Apply();
			}});
		vpArea.add(cbAnyArea);

		HorizontalPanel horizontalPanel_2 = new HorizontalPanel();
		horizontalPanel_2.setSpacing(5);
		vpArea.add(horizontalPanel_2);
		horizontalPanel_2.setSize("100%", "100px");

		lbMinArea = new ListBox();
		lbMinArea.addChangeHandler(new ChangeHandler() {
			public void onChange(ChangeEvent event) {
				lbMaxArea.setSelectedIndex(Math.max(
						lbMinArea.getSelectedIndex(),
						lbMaxArea.getSelectedIndex()));
				instance.isAny = lbMinArea.getSelectedIndex() == 0 &&
								 lbMaxArea.getSelectedIndex() == (lbMaxArea.getItemCount()-1);
				cbAnyArea.setValue(instance.isAny);
				instance.Apply();
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
				cbAnyArea.setValue(instance.isAny);
				instance.Apply();
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
		String areaUm = "Sq.Ft"; 
		for (SuiteType suite_type : getParentSectionContainer().getActiveSuiteTypes().values()) {
			double area = suite_type.getArea(); 
			if (area <= 0)
				continue;
			min_area = Math.min(min_area, area);
			max_area = Math.max(max_area, area);
			areaUm = suite_type.getAreaUm();
		}

		int diff = (int) (max_area - min_area);
		diff /= 10;

		int grid = 1;
		while (diff > grid)
			grid *= 10;

		int numOfEntries = 0;
		while (grid * (numOfEntries + 1) < min_area)
			numOfEntries++;

		if (numOfEntries == 0)
			grid /= 4;

		areas.clear();

		
		while (grid * numOfEntries < max_area) {
			areas.add((double) (grid * numOfEntries));
			numOfEntries++;
		}

		areas.add((double) (grid * numOfEntries));

		lbMinArea.clear();
		lbMaxArea.clear();
		for (Double item : areas) {
			// item = (item > 0) ? item : 100;
			int index = areas.indexOf(item);
			if (index == 0)
				lbMinArea.addItem("Min", item.toString());
			else if (index == areas.size() - 1)
				lbMaxArea.addItem("Max", item.toString());
			else {
				String area = NumberFormat.getDecimalFormat().format(item);
				lbMinArea.addItem(area + " " + areaUm, item.toString());
				lbMaxArea.addItem(area + " " + areaUm, item.toString());
			}
		}

		isAny = true;
	}

	@Override
	public void Reset() {
		cbAnyArea.setValue(true, true);
		lbMinArea.setEnabled(true);
		lbMinArea.setSelectedIndex(0);
		lbMaxArea.setEnabled(true);
		lbMaxArea.setSelectedIndex(lbMaxArea.getItemCount() - 1);
		isAny = true;
	}
	
	@Override
	public int StateHash() {
		int hash = hashCode() + cbAnyArea.StateHash();
		hash += lbMinArea.hashCode() * lbMinArea.getSelectedIndex();
		hash += lbMaxArea.hashCode() * lbMaxArea.getSelectedIndex();		
		
		return hash;
	}

	@Override
	public boolean isFilteredIn(SuiteGeoItem suiteGI) {
		if (isAny)
			return true;
		
		SuiteType type = suiteGI.suite.getSuiteType();
		double area = type.getArea();
		if (area <= areas.get(lbMinArea.getSelectedIndex()))
			return false;
		if (area >= areas.get(lbMaxArea.getSelectedIndex() + 1))
			return false;

		return true;
	}

	private boolean isAny = true;

	@Override
	public boolean isAny() {
		return isAny;
	}
	
	@Override
	public void Apply() {
		if (isAny)
			stackPanel.setStackText(stackPanel.getWidgetIndex(this), "Area (any)");
		else
			stackPanel.setStackText(stackPanel.getWidgetIndex(this), "Area");
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

	@Override
	public JSONObject toJSONObject() {
		JSONObject obj = new JSONObject();
		
		String name = getClass().getName();
		obj.put("name", new JSONString(name));
		
		double minAreaRange = areas.get(lbMinArea.getSelectedIndex());
		double maxAreaRange = areas.get(lbMaxArea.getSelectedIndex() + 1);

		if (minAreaRange == areas.get(0))
			minAreaRange = Integer.MIN_VALUE;
		if (maxAreaRange == areas.get(areas.size() - 1))
			maxAreaRange = Integer.MAX_VALUE;
		
		obj.put("minAreaRange", new JSONNumber(minAreaRange));
		obj.put("maxAreaRange", new JSONNumber(maxAreaRange));
		
		return obj;
	}

	@Override
	public void fromJSONObject(JSONObject obj) {
		if (obj == null) return;
		
		if (!obj.containsKey("name")) return;
		if (obj.get("name").isString() == null) return;
		String name = obj.get("name").isString().stringValue();
		if (!name.equals(getClass().getName())) return;
		
		Double minAreaRange = readDouble(obj,"minAreaRange");
		Double maxAreaRange = readDouble(obj,"maxAreaRange");
		if ((minAreaRange == null) ||
				(maxAreaRange == null))	return;
		if (minAreaRange > maxAreaRange) return;
		
		minAreaRange = Math.max(minAreaRange, areas.get(0));
		int i = areas.size() - 1;
		while (minAreaRange < areas.get(i)) i--;
		lbMinArea.setSelectedIndex(i);
			
		maxAreaRange = Math.min(maxAreaRange, areas.get(areas.size() - 1));
		i = 0;
		while (maxAreaRange > areas.get(i)) i++;
		lbMaxArea.setSelectedIndex(i - 1);
		
		
		// TODO !!
		lbMaxArea.setSelectedIndex(Math.max(
				lbMinArea.getSelectedIndex(),
				lbMaxArea.getSelectedIndex()));
		instance.isAny = lbMinArea.getSelectedIndex() == 0 &&
				lbMaxArea.getSelectedIndex() == (lbMaxArea.getItemCount()-1);
		cbAnyArea.setValue(instance.isAny);
		instance.Apply();
	}
	
	private Double readDouble(JSONObject obj, String key) {
		if ((obj == null) ||
			(!obj.containsKey(key)) ||
			(obj.get(key).isNumber() == null)) 
			return null;
		return obj.get(key).isNumber().doubleValue();
	}
}
