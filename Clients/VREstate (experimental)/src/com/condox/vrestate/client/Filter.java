package com.condox.vrestate.client;

import java.util.ArrayList;

import com.condox.vrestate.client.document.Document;
import com.condox.vrestate.client.document.Suite;
import com.condox.vrestate.client.document.SuiteType;
import com.condox.vrestate.client.view.View;
import com.google.gwt.dom.client.Element;
import com.google.gwt.dom.client.Style.Unit;
import com.google.gwt.event.dom.client.ChangeEvent;
import com.google.gwt.event.dom.client.ChangeHandler;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.event.dom.client.ClickHandler;
import com.google.gwt.event.logical.shared.CloseEvent;
import com.google.gwt.event.logical.shared.CloseHandler;
import com.google.gwt.event.logical.shared.OpenEvent;
import com.google.gwt.event.logical.shared.OpenHandler;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.CheckBox;
import com.google.gwt.user.client.ui.DisclosurePanel;
import com.google.gwt.user.client.ui.DockPanel;
import com.google.gwt.user.client.ui.HasHorizontalAlignment;
import com.google.gwt.user.client.ui.HasVerticalAlignment;
import com.google.gwt.user.client.ui.HorizontalPanel;
import com.google.gwt.user.client.ui.ListBox;
import com.google.gwt.user.client.ui.RadioButton;
import com.google.gwt.user.client.ui.RootLayoutPanel;
import com.google.gwt.user.client.ui.StackPanel;
import com.google.gwt.user.client.ui.VerticalPanel;
import com.google.gwt.event.logical.shared.ValueChangeHandler;
import com.google.gwt.event.logical.shared.ValueChangeEvent;

public class Filter {
	private static Filter instance = new Filter();
	public static int filtered_suites = 0;
	public static int all_suites = 0;

	private Filter() {
		Create();
	}

	public static Filter get() {
		return instance;
	}

	private DisclosurePanel dpFilter = null;
	// Цена
	private VerticalPanel vpPrices = null;
	private ListBox cbMinPrice = null;
	private ListBox cbMaxPrice = null;
	// Bedrooms
	private VerticalPanel vpBedrooms = null;
	private CheckBox cbStudio = null;
	private CheckBox cbOneBedrooms = null;
	private CheckBox cbTwoBedrooms = null;
	private CheckBox cbThreeBedrooms = null;
	private CheckBox cbFourBedrooms = null;
	private CheckBox cbFiveBedrooms = null;
	// Bathrooms
	private VerticalPanel vpBathrooms = null;
	private CheckBox cbOneBathroom = null;
	private CheckBox cbTwoBathrooms = null;
	private CheckBox cbThreeBathrooms = null;
	private CheckBox cbFourBathrooms = null;
	private CheckBox cbFiveBathrooms = null;
	// Areas
	private VerticalPanel vpAreas = null;
	private ListBox lbMinArea = null;
	private ListBox lbMaxArea = null;

	// Balconies/Terrases
	private VerticalPanel vpBalconies = null;
	private RadioButton rbBalconyAny = null;
	private RadioButton rbBalconyYes = null;
	private RadioButton rbBalconyNo = null;

	// ========================================

	/**
	 * @wbp.parser.entryPoint
	 */
	private void Create() {
		RootLayoutPanel rootLayoutPanel = RootLayoutPanel.get();

		dpFilter = new DisclosurePanel("Filtered:", true);

		dpFilter.addOpenHandler(new OpenHandler<DisclosurePanel>() {
			public void onOpen(OpenEvent<DisclosurePanel> event) {
				UpdateSize();
			}
		});
		dpFilter.addCloseHandler(new CloseHandler<DisclosurePanel>() {
			public void onClose(CloseEvent<DisclosurePanel> event) {
				UpdateSize();
			}
		});
		rootLayoutPanel.add(dpFilter);
		dpFilter.setWidth("300px");
		rootLayoutPanel.setWidgetLeftWidth(dpFilter, 50.0, Unit.PX, 200.0,
				Unit.PX);
		rootLayoutPanel.setWidgetTopHeight(dpFilter, 50.0, Unit.PX, 25.0,
				Unit.PX);

		DockPanel dockPanel = new DockPanel();
		dockPanel.setSpacing(10);
		dpFilter.setContent(dockPanel);
		dockPanel.setSize("300px", "400px");

		HorizontalPanel horizontalPanel = new HorizontalPanel();
		dockPanel.add(horizontalPanel, DockPanel.SOUTH);
		horizontalPanel.setVerticalAlignment(HasVerticalAlignment.ALIGN_BOTTOM);
		horizontalPanel
				.setHorizontalAlignment(HasHorizontalAlignment.ALIGN_CENTER);
		horizontalPanel.setSize("100%", "36px");

		Button btnReset = new Button("New button");
		btnReset.addClickHandler(new ClickHandler() {
			public void onClick(ClickEvent event) {
				Reset();
			}
		});
		btnReset.setText("Reset");
		horizontalPanel.add(btnReset);
		btnReset.setWidth("100px");

		Button btnApply = new Button("New button");
		btnApply.addClickHandler(new ClickHandler() {
			public void onClick(ClickEvent event) {
				Apply();
			}
		});
		btnApply.setText("Apply");
		horizontalPanel.add(btnApply);
		btnApply.setWidth("100px");

		StackPanel stackPanel = new StackPanel();
		dockPanel.add(stackPanel, DockPanel.CENTER);
		stackPanel.setSize("100%", "150px");

		// PRICES
		vpPrices = new VerticalPanel();
		vpPrices.setSpacing(5);
		stackPanel.add(vpPrices, "Prices", false);
		vpPrices.setSize("100%", "150px");

		HorizontalPanel horizontalPanel_1 = new HorizontalPanel();
		horizontalPanel_1.setSpacing(5);
		vpPrices.add(horizontalPanel_1);
		horizontalPanel_1.setSize("100%", "100px");

		cbMinPrice = new ListBox();
		cbMinPrice.setEnabled(false);
		horizontalPanel_1.add(cbMinPrice);
		cbMinPrice.addChangeHandler(new ChangeHandler() {
			public void onChange(ChangeEvent event) {
				cbMaxPrice.setSelectedIndex(Math.max(
						cbMaxPrice.getSelectedIndex(),
						cbMinPrice.getSelectedIndex()));
				// Apply();
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
				// Apply();
			}
		});
		cbMaxPrice.setWidth("100%");
		// PRICES

		vpBedrooms = new VerticalPanel();
		stackPanel.add(vpBedrooms, "Bedrooms", false);
		vpBedrooms.setSize("100%", "150px");

		cbStudio = new CheckBox("Studio");
		cbStudio.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (isAllBedroomsUnchecked())
					cbStudio.setValue(true, true);
			}
		});
		cbStudio.setEnabled(false);
		vpBedrooms.add(cbStudio);

		cbOneBedrooms = new CheckBox("One bedroom");
		cbOneBedrooms.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (isAllBedroomsUnchecked())
					cbOneBedrooms.setValue(true, true);
			}
		});
		cbOneBedrooms.setEnabled(false);
		vpBedrooms.add(cbOneBedrooms);

		cbTwoBedrooms = new CheckBox("Two bedrooms");
		cbTwoBedrooms.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (isAllBedroomsUnchecked())
					cbTwoBedrooms.setValue(true, true);
			}
		});
		cbTwoBedrooms.setEnabled(false);
		vpBedrooms.add(cbTwoBedrooms);

		cbThreeBedrooms = new CheckBox("Three bedrooms");
		cbThreeBedrooms.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (isAllBedroomsUnchecked())
					cbThreeBedrooms.setValue(true, true);
			}
		});
		cbThreeBedrooms.setEnabled(false);
		vpBedrooms.add(cbThreeBedrooms);

		cbFourBedrooms = new CheckBox("Four bedrooms");
		cbFourBedrooms.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (isAllBedroomsUnchecked())
					cbFourBedrooms.setValue(true, true);
			}
		});
		cbFourBedrooms.setEnabled(false);
		vpBedrooms.add(cbFourBedrooms);

		cbFiveBedrooms = new CheckBox("Five and more bedrooms");
		cbFiveBedrooms.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (isAllBedroomsUnchecked())
					cbFiveBedrooms.setValue(true, true);
			}
		});
		cbFiveBedrooms.setEnabled(false);
		vpBedrooms.add(cbFiveBedrooms);

		vpBathrooms = new VerticalPanel();
		vpBathrooms.setSpacing(5);
		stackPanel.add(vpBathrooms, "Bathrooms", false);
		vpBathrooms.setSize("100%", "150px");

		cbOneBathroom = new CheckBox("One bathroom");
		cbOneBathroom.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (isAllBathroomsUnchecked())
					cbOneBathroom.setValue(true, true);
			}
		});
		vpBathrooms.add(cbOneBathroom);

		cbTwoBathrooms = new CheckBox("Two bathrooms");
		cbTwoBathrooms.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (isAllBathroomsUnchecked())
					cbTwoBathrooms.setValue(true, true);
			}
		});
		vpBathrooms.add(cbTwoBathrooms);

		cbThreeBathrooms = new CheckBox("Three bathrooms");
		cbThreeBathrooms.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (isAllBathroomsUnchecked())
					cbThreeBathrooms.setValue(true, true);
			}
		});
		vpBathrooms.add(cbThreeBathrooms);

		cbFourBathrooms = new CheckBox("Four bathrooms");
		cbFourBathrooms.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (isAllBathroomsUnchecked())
					cbFourBathrooms.setValue(true, true);
			}
		});
		vpBathrooms.add(cbFourBathrooms);

		cbFiveBathrooms = new CheckBox("Five and more bathrooms");
		cbFiveBathrooms.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (isAllBathroomsUnchecked())
					cbFiveBathrooms.setValue(true, true);
			}
		});
		vpBathrooms.add(cbFiveBathrooms);

		vpAreas = new VerticalPanel();
		stackPanel.add(vpAreas, "Areas", false);
		vpAreas.setSize("100%", "150px");

		HorizontalPanel horizontalPanel_2 = new HorizontalPanel();
		horizontalPanel_2.setSpacing(5);
		vpAreas.add(horizontalPanel_2);
		horizontalPanel_2.setSize("100%", "100px");

		lbMinArea = new ListBox();
		lbMinArea.addChangeHandler(new ChangeHandler() {
			public void onChange(ChangeEvent event) {
				lbMinArea.setSelectedIndex(Math.min(
						lbMinArea.getSelectedIndex(),
						lbMaxArea.getSelectedIndex()));
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
			}
		});
		lbMaxArea.setEnabled(false);
		horizontalPanel_2.add(lbMaxArea);
		lbMaxArea.setWidth("100%");

		// BALCONY
		vpBalconies = new VerticalPanel();
		vpBalconies.setSpacing(10);
		stackPanel.add(vpBalconies, "Balconies / Terrases", false);
		vpBalconies.setSize("100%", "150px");

		rbBalconyYes = new RadioButton("new name", "Yes");
		vpBalconies.add(rbBalconyYes);

		rbBalconyNo = new RadioButton("new name", "No");
		vpBalconies.add(rbBalconyNo);

		rbBalconyAny = new RadioButton("new name", "Any");
		rbBalconyAny.setValue(true);
		vpBalconies.add(rbBalconyAny);

		Element filter_container = com.google.gwt.dom.client.Document.get().getElementById("filter");
		filter_container.appendChild(dpFilter.getElement());
	}

	private ArrayList<Integer> prices = new ArrayList<Integer>();
	private ArrayList<Double> areas = new ArrayList<Double>();
	
	private boolean isAllBedroomsUnchecked() {
		if(cbStudio.getValue()&&(cbStudio.isEnabled()))
			return false;
		if(cbOneBedrooms.getValue()&&(cbOneBedrooms.isEnabled()))
			return false;
		if(cbTwoBedrooms.getValue()&&(cbTwoBedrooms.isEnabled()))
			return false;
		if(cbThreeBedrooms.getValue()&&(cbThreeBedrooms.isEnabled()))
			return false;
		if(cbFourBedrooms.getValue()&&(cbFourBedrooms.isEnabled()))
			return false;
		if(cbFiveBedrooms.getValue()&&(cbFiveBedrooms.isEnabled()))
			return false;
		return true;
	}
	
	private boolean isAllBathroomsUnchecked() {
		if(cbOneBathroom.getValue()&&(cbOneBathroom.isEnabled()))
			return false;
		if(cbTwoBathrooms.getValue()&&(cbTwoBathrooms.isEnabled()))
			return false;
		if(cbThreeBathrooms.getValue()&&(cbThreeBathrooms.isEnabled()))
			return false;
		if(cbFourBathrooms.getValue()&&(cbFourBathrooms.isEnabled()))
			return false;
		if(cbFiveBathrooms.getValue()&&(cbFiveBathrooms.isEnabled()))
			return false;
		return true;
	}

	public void Init() {
		// PRICE
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

		while (a * i < max_price) {
			prices.add(a * i);
			i++;
		}
		;
		prices.add(a * i);

		cbMinPrice.clear();
		cbMaxPrice.clear();
		for (Integer item : prices) {
			cbMinPrice.addItem("Min: $" + item, item.toString());
			cbMaxPrice.addItem("Max: $" + item, item.toString());
		}
		// BEDROOMS
		boolean studio = false;
		boolean one = false;
		boolean two = false;
		boolean three = false;
		boolean four = false;
		boolean five = false;
		for (SuiteType suite_type : Document.get().getSuiteTypes()) {
			if (suite_type.getBedrooms() >= 5)
				five = true;
			else
				switch (suite_type.getBedrooms()) {
				case 0:
					studio = true;
					break;
				case 1:
					one = true;
					break;
				case 2:
					two = true;
					break;
				case 3:
					three = true;
					break;
				case 4:
					four = true;
					break;
				}
		}
		cbStudio.setEnabled(studio);
		cbOneBedrooms.setEnabled(one);
		cbTwoBedrooms.setEnabled(two);
		cbThreeBedrooms.setEnabled(three);
		cbFourBedrooms.setEnabled(four);
		cbFiveBedrooms.setEnabled(five);
		// BATHROOMS
		one = false;
		two = false;
		three = false;
		four = false;
		five = false;
		for (SuiteType suite_type : Document.get().getSuiteTypes()) {
			if (suite_type.getBathrooms() >= 5)
				five = true;
			else
				switch (suite_type.getBathrooms()) {
				case 0:
				case 1:
					one = true;
					break;
				case 2:
					two = true;
					break;
				case 3:
					three = true;
					break;
				case 4:
					four = true;
					break;
				}
		}
		cbOneBathroom.setEnabled(one);
		cbTwoBathrooms.setEnabled(two);
		cbThreeBathrooms.setEnabled(three);
		cbFourBathrooms.setEnabled(four);
		cbFiveBathrooms.setEnabled(five);
		// AREAs
		double min_area = Integer.MAX_VALUE;
		double max_area = Integer.MIN_VALUE;
		for (SuiteType suite_type : Document.get().getSuiteTypes()) {
			min_area = Math.min(min_area, suite_type.getArea());
			max_area = Math.max(max_area, suite_type.getArea());
		}

		diff = (int) (max_area - min_area);
		diff /= 10;

		a = 1;
		while (diff > a)
			a *= 10;

		i = 0;
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
		}
		Reset();
	}

	private int min_price_range = 0;
	private int max_price_range = 0;

	public int getMinPriceRange() {
		return min_price_range;
	}

	public int getMaxPriceRange() {
		return max_price_range;
	}

	private boolean isOpened = false;

	public void setVisible(boolean visible) {
		if ((visible) && (Options.getViewOrderId() == null) && (Options.USE_FILTER)) {
			UpdateSize();
			dpFilter.setOpen(isOpened);
		} else {
			isOpened = dpFilter.isOpen();
			dpFilter.setOpen(false);
			Element filter_container = com.google.gwt.dom.client.Document.get().getElementById("filter");
			filter_container.getStyle().setHeight(0, Unit.PX);
			filter_container.getStyle().setWidth(0, Unit.PX);
		}
	}

	public void UpdateSize() {
		Element filter_container = com.google.gwt.dom.client.Document.get().getElementById("filter");
		if (dpFilter != null) {
			filter_container.getStyle().setHeight(dpFilter.getOffsetHeight(),
					Unit.PX);
			filter_container.getStyle().setWidth(dpFilter.getOffsetWidth(),
					Unit.PX);
		}
		;
	}

	public void Apply() {
		// TODO
		Filter.filtered_suites = 0;
		Filter.all_suites = 0;
		View.ApplyFilter();
		if (Filter.filtered_suites == Filter.all_suites)
			dpFilter.getHeaderTextAccessor().setText(
					"Selection Filter (" + Filter.all_suites
							+ " units available)");
		else
			dpFilter.getHeaderTextAccessor().setText(
					"Selection Filter (" + Filter.filtered_suites + " out of "
							+ Filter.all_suites + ")");
	}

	public void Reset() {
		// PRICE
		cbMinPrice.setEnabled(true);
		cbMinPrice.setSelectedIndex(0);
		cbMaxPrice.setEnabled(true);
		cbMaxPrice.setSelectedIndex(cbMaxPrice.getItemCount() - 1);
		// BEDROOMS
//		cbStudio.setEnabled(true);
		cbStudio.setValue(true);
//		cbOneBedrooms.setEnabled(true);
		cbOneBedrooms.setValue(true);
//		cbTwoBedrooms.setEnabled(true);
		cbTwoBedrooms.setValue(true);
//		cbThreeBedrooms.setEnabled(true);
		cbThreeBedrooms.setValue(true);
//		cbFourBedrooms.setEnabled(true);
		cbFourBedrooms.setValue(true);
//		cbFiveBedrooms.setEnabled(true);
		cbFiveBedrooms.setValue(true);
		// BATHROOMS
//		cbOneBathroom.setEnabled(true);
		cbOneBathroom.setValue(true);
//		cbTwoBathrooms.setEnabled(true);
		cbTwoBathrooms.setValue(true);
//		cbThreeBathrooms.setEnabled(true);
		cbThreeBathrooms.setValue(true);
//		cbFourBathrooms.setEnabled(true);
		cbFourBathrooms.setValue(true);
//		cbFiveBathrooms.setEnabled(true);
		cbFiveBathrooms.setValue(true);
		// AREAs
		lbMinArea.setEnabled(true);
		lbMinArea.setSelectedIndex(0);
		lbMaxArea.setEnabled(true);
		lbMaxArea.setSelectedIndex(lbMaxArea.getItemCount() - 1);
		// BALCONIES
		rbBalconyAny.setValue(true);
		Apply();
	}

	public boolean isPriceFiltered(double price) {
		if (price < prices.get(cbMinPrice.getSelectedIndex()))
			return false;
		if (price > prices.get(cbMaxPrice.getSelectedIndex()))
			return false;
		return true;
	}

	public boolean isBedroomsFiltered(int bedrooms) {
		if (cbStudio.getValue() && bedrooms == 0)
			return true;
		else if (cbOneBedrooms.getValue() && bedrooms == 1)
			return true;
		else if (cbTwoBedrooms.getValue() && bedrooms == 2)
			return true;
		else if (cbThreeBedrooms.getValue() && bedrooms == 3)
			return true;
		else if (cbFourBedrooms.getValue() && bedrooms == 4)
			return true;
		else if (cbFiveBedrooms.getValue() && bedrooms > 4)
			return true;
		return false;
	}

	public boolean isBathroomsFiltered(int bathrooms) {
		if (cbOneBathroom.getValue() && bathrooms == 0)
			return true;
		else if (cbOneBathroom.getValue() && bathrooms == 1)
			return true;
		else if (cbTwoBathrooms.getValue() && bathrooms == 2)
			return true;
		else if (cbThreeBathrooms.getValue() && bathrooms == 3)
			return true;
		else if (cbFourBathrooms.getValue() && bathrooms == 4)
			return true;
		else if (cbFiveBathrooms.getValue() && bathrooms > 4)
			return true;
		return false;
	}

	public boolean isAreaFiltered(double area) {
		if (area < areas.get(lbMinArea.getSelectedIndex()))
			return false;
		if (area > areas.get(lbMaxArea.getSelectedIndex()))
			return false;
		return true;
	}

	public boolean isBalconyFiltered(boolean presented) {
		if (rbBalconyAny.getValue())
			return true;
		return (presented == rbBalconyYes.getValue());
	}
}
