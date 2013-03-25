package com.condox.vrestate.client.filter;

import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;

import com.condox.vrestate.client.Log;
import com.condox.vrestate.client.Options;
import com.condox.vrestate.client.document.Document;
import com.condox.vrestate.client.document.Suite;
import com.condox.vrestate.client.view.I_AbstractView;
import com.condox.vrestate.client.view.ProgressBar;
import com.condox.vrestate.client.view._AbstractView;
import com.condox.vrestate.client.view.GeoItems.IGeoItem;
import com.google.gwt.core.client.Scheduler;
import com.google.gwt.core.client.Scheduler.ScheduledCommand;
import com.google.gwt.dom.client.IFrameElement;
import com.google.gwt.dom.client.Style;
import com.google.gwt.dom.client.Style.Position;
import com.google.gwt.dom.client.Style.Unit;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.event.dom.client.ClickHandler;
import com.google.gwt.event.logical.shared.CloseEvent;
import com.google.gwt.event.logical.shared.CloseHandler;
import com.google.gwt.event.logical.shared.OpenEvent;
import com.google.gwt.event.logical.shared.OpenHandler;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.DisclosurePanel;
import com.google.gwt.user.client.ui.DockPanel;
import com.google.gwt.user.client.ui.HasHorizontalAlignment;
import com.google.gwt.user.client.ui.HasVerticalAlignment;
import com.google.gwt.user.client.ui.HorizontalPanel;
import com.google.gwt.user.client.ui.RootPanel;
import com.google.gwt.user.client.ui.StackPanel;

public class Filter extends StackPanel implements I_FilterSection {
	public static boolean initialized = false;
	private static Filter instance = new Filter();
	private List<Suite> filteredIn_suites = null;

	ArrayList<I_FilterSection> sections = new ArrayList<I_FilterSection>();

	private Filter() {
		Create();
	}

	public static Filter get() {
		return instance;
	}

	private DisclosurePanel dpFilter = null;
	private IFrameElement frame = null;
	public Button btnApply = null;
	public Button btnReset = null;

	// ========================================

	/**
	 * @wbp.parser.entryPoint
	 */
	private void Create() {
		Log.write("Filter->Create()");
		RootPanel panel = RootPanel.get();
		panel.setWidth("");

		dpFilter = new DisclosurePanel("Filtered:");
		panel.add(dpFilter, 50, 50);
		dpFilter.setSize("250px", "23px");

		DockPanel dockPanel = new DockPanel();
		dpFilter.setContent(dockPanel);
		dockPanel.setSize("5cm", "4cm");
		dockPanel.setSpacing(10);

		HorizontalPanel horizontalPanel = new HorizontalPanel();
		dockPanel.add(horizontalPanel, DockPanel.SOUTH);
		horizontalPanel.setVerticalAlignment(HasVerticalAlignment.ALIGN_MIDDLE);
		horizontalPanel
				.setHorizontalAlignment(HasHorizontalAlignment.ALIGN_CENTER);
		horizontalPanel.setSize("104px", "20px");

		btnReset = new Button("New button");
		btnReset.addClickHandler(new ClickHandler() {
			public void onClick(ClickEvent event) {
				Reset();
				btnReset.setEnabled(false);
				Apply();
				btnApply.setEnabled(false);
			}
		});
		btnReset.setEnabled(false);
		btnReset.setText("Reset");
		horizontalPanel.add(btnReset);
		btnReset.setWidth("100px");

		btnApply = new Button("New button");
		btnApply.addClickHandler(new ClickHandler() {
			public void onClick(ClickEvent event) {
				Apply();
				btnApply.setEnabled(false);
			}
		});
		btnApply.setEnabled(false);
		btnApply.setText("Apply");
		horizontalPanel.add(btnApply);
		btnApply.setWidth("100px");

		StackPanel stackPanel = new StackPanel();
		dockPanel.add(stackPanel, DockPanel.CENTER);
		stackPanel.setSize("100%", "250px");


		dpFilter.getElement().getStyle().setZIndex(Integer.MAX_VALUE);

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

		frame = com.google.gwt.dom.client.Document.get().createIFrameElement();
		com.google.gwt.dom.client.Document.get().getBody().appendChild(frame);

		PriceSection priceSection = PriceSection.CreateSectionPanel("Price", stackPanel);
		if (priceSection != null) sections.add(priceSection);
		BedroomsSection bedroomsSection = BedroomsSection.CreateSectionPanel("Bedrooms", stackPanel);
		if (bedroomsSection != null) sections.add(bedroomsSection);
		BathroomSection bathroomSection = BathroomSection.CreateSectionPanel("Bathrooms", stackPanel);
		if (bathroomSection != null) sections.add(bathroomSection);
		AreaSection areaSection = AreaSection.CreateSectionPanel("Area", stackPanel);
		if (areaSection != null) sections.add(areaSection);
		BalconySection balconySection = BalconySection.CreateSectionPanel("Balconies", stackPanel);
		if (balconySection != null) sections.add(balconySection);
			
		Iterator<I_FilterSection> iterator = sections.iterator();
		while (iterator.hasNext())
			if (iterator.next() == null)
				iterator.remove();

		filteredIn_suites = new ArrayList<Suite>();
		this.filteredIn_suites.addAll(Document.get().getSuites());
		Init();

		Reset();
		UpdateSize();
		setVisible(false);
		initialized = true;
	}

	@Override
	public void Init() {
		for (I_FilterSection section : sections)
			section.Init();
	}

	private boolean isOpened = false;

	public void setVisible(boolean visible) {
		if (visible && Options.USE_FILTER) {
			dpFilter.setOpen(isOpened);
			dpFilter.setVisible(true);
			UpdateSize();
		} else {
			isOpened = dpFilter.isOpen();
			dpFilter.setVisible(false);
			UpdateSize();
		}
	}

	private void UpdateSize() {
		Scheduler.get().scheduleDeferred(new ScheduledCommand() {

			@Override
			public void execute() {
				Style style = frame.getStyle();
				style.setZIndex(Integer.MAX_VALUE - 1);
				style.setPosition(Position.ABSOLUTE);
				style.setLeft(dpFilter.getAbsoluteLeft(), Unit.PX);
				style.setTop(dpFilter.getAbsoluteTop(), Unit.PX);
				/*String style = "";
				style += "z-index: " + (Integer.MAX_VALUE - 1) + ";";
				style += " position: absolute;";
				style += " left: " + dpFilter.getAbsoluteLeft() + "px;";
				style += " top: " + dpFilter.getAbsoluteTop() + "px;";*/
				if (dpFilter.isVisible()) { 
					style.setWidth(dpFilter.getOffsetWidth(), Unit.PX);
					style.setHeight(dpFilter.getOffsetHeight(), Unit.PX);
					/*style += " width: " + dpFilter.getOffsetWidth()	+ "px;";
					style += " height: " + dpFilter.getOffsetHeight() + "px;";*/
				} else {
					style.setWidth(0, Unit.PX);
					style.setHeight(0, Unit.PX);
					/*style += " width: 0px;";
					style += " height: 0px;";*/
				}
				style.setBackgroundColor("white");
//				style += " background: white;";
//				frame.setAttribute( "style",style);
			}
		});
	}

	public void Apply() {
		for (I_FilterSection section : sections)
			section.Apply();

		this.filteredIn_suites.clear();
		_AbstractView.ApplyFilter();
		if (this.filteredIn_suites.size() == Document.get().getSuites().size())
			dpFilter.getHeaderTextAccessor().setText(
					"Selection Filter (" + Document.get().getSuites().size()
							+ " units available)");
		else
			dpFilter.getHeaderTextAccessor().setText(
					"Selection Filter (" + this.filteredIn_suites.size() + " out of "
							+ Document.get().getSuites().size() + ")");

		IGeoItem suiteGeo = Filter.get().getNextGeoItem();
		if (suiteGeo != null) {
			I_AbstractView currView = _AbstractView.getCurrentView();
			if (currView.getGeoItem().getType() != "suite")
				currView.Select(suiteGeo.getType(), suiteGeo.getId());
		}
	}

	@Override
	public void Reset() {
		for (I_FilterSection section : sections)
			section.Reset();

		this.filteredIn_suites.clear();
		this.filteredIn_suites.addAll(Document.get().getSuites());
		dpFilter.getHeaderTextAccessor().setText(
				"Selection Filter (" + Document.get().getSuites().size()
						+ " units available)");
	}

	@Override
	public boolean isFileredIn(Suite suite) {
		for (I_FilterSection section : sections)
			if (!section.isFileredIn(suite))
				return false;

		this.filteredIn_suites.add(suite);
		return true;
	}

	@Override
	public boolean isAny() {
		for (I_FilterSection section : sections)
			if (!section.isAny())
				return false;
		return true;
	}

	@Override
	public boolean isChanged() {
		for (I_FilterSection section : sections)
			if (section.isChanged())
				return true;
		return false;
	}
	
	public void onChanged() {
		if (isChanged()) {
			if (btnApply != null)
				btnApply.setEnabled(true);
			if (btnReset != null)
				btnReset.setEnabled(true);
		}
	}
	
	public int howManyFilteredIn() {
		return this.filteredIn_suites.size();
	}	
	
	public IGeoItem getNextGeoItem() {
		int currId = _AbstractView.getCurrentGeoItem().getId();
		if (this.filteredIn_suites == null) {
			this.filteredIn_suites = new ArrayList<Suite>();
			this.filteredIn_suites.addAll(Document.get().getSuites());
		}
			
		int size = this.filteredIn_suites.size();
		if (size == 0) return null;
		
		for (int i=0; i< size; i++) {
			Suite suite = this.filteredIn_suites.get(i);
			if (suite.getId() == currId) {
				Suite next = i+1 < size? this.filteredIn_suites.get(i+1) : 
										 this.filteredIn_suites.get(0);
				IGeoItem nextGeo = _AbstractView.getSuiteGeoItem(next.getId());
				return nextGeo;
			}				
		}

		Suite first = this.filteredIn_suites.get(0);
		IGeoItem firstGeo = _AbstractView.getSuiteGeoItem(first.getId());
		return firstGeo;
	}

	public IGeoItem getPrevGeoItem() {
		int currId = _AbstractView.getCurrentGeoItem().getId();
		if (this.filteredIn_suites == null) {
			this.filteredIn_suites = new ArrayList<Suite>();
			this.filteredIn_suites.addAll(Document.get().getSuites());
		}
			
		int size = this.filteredIn_suites.size();
		if (size == 0) return null;
		
		for (int i=0; i< size; i++) {
			Suite suite = this.filteredIn_suites.get(i);
			if (suite.getId() == currId) {
				Suite prev = i > 1 ? this.filteredIn_suites.get(i-1) : 
									 this.filteredIn_suites.get(size-1);
				IGeoItem nextGeo = _AbstractView.getSuiteGeoItem(prev.getId());
				return nextGeo;
			}				
		}

		Suite first = this.filteredIn_suites.get(0);
		IGeoItem firstGeo = _AbstractView.getSuiteGeoItem(first.getId());
		return firstGeo;
	}
}
