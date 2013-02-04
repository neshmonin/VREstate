package com.condox.vrestate.client.filter;

import java.util.ArrayList;
import java.util.Iterator;

import com.condox.vrestate.client.Log;
import com.condox.vrestate.client.Options;
import com.condox.vrestate.client.document.Document;
import com.condox.vrestate.client.document.Suite;
import com.condox.vrestate.client.view._AbstractView;
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
import com.google.gwt.user.client.Window;
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
	private static int filteredIn_suites = 0;

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
		// rootLayoutPanel.add(dpFilter);
		// rootLayoutPanel.setWidgetLeftWidth(dpFilter, 50.0, Unit.PX, 200.0,
		// Unit.PX);
		// rootLayoutPanel.setWidgetTopHeight(dpFilter, 50.0, Unit.PX, 25.0,
		// Unit.PX);

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

		
		sections.add(PriceSection.CreateSectionPanel("Price", stackPanel));
		sections.add(BedroomsSection.CreateSectionPanel("Bedrooms", stackPanel));
		sections.add(BathroomSection.CreateSectionPanel("Bathrooms", stackPanel));
		sections.add(AreaSection.CreateSectionPanel("Area", stackPanel));
		sections.add(BalconySection.CreateSectionPanel("Balconies", stackPanel));
			
		Iterator<I_FilterSection> iterator = sections.iterator();
		while (iterator.hasNext())
			if (iterator.next() == null)
				iterator.remove();
		
		Init();

		Reset();
		UpdateSize();
		initialized = true;
	}

	@Override
	public void Init() {
		for (I_FilterSection section : sections)
			section.Init();
	}

	private boolean isOpened = false;

	public void setVisible(boolean visible) {
		Log.write("Filter->setVisible:" + visible);
		if (visible && Options.USE_FILTER) {
			dpFilter.setOpen(isOpened);
			dpFilter.setVisible(true);
			UpdateSize();
		} else {
			isOpened = dpFilter.isOpen();
			dpFilter.setVisible(false);
			UpdateSize();
			// dpFilter.setOpen(false);
			// frame.getStyle().setHeight(0, Unit.PX);
			// frame.getStyle().setWidth(0, Unit.PX);
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
		filteredIn_suites = 0;
		_AbstractView.ApplyFilter();
		if (Filter.filteredIn_suites == Document.get().getSuites().size())
			dpFilter.getHeaderTextAccessor().setText(
					"Selection Filter (" + Document.get().getSuites().size()
							+ " units available)");
		else
			dpFilter.getHeaderTextAccessor().setText(
					"Selection Filter (" + Filter.filteredIn_suites + " out of "
							+ Document.get().getSuites().size() + ")");
	}

	@Override
	public void Reset() {
		for (I_FilterSection section : sections)
			section.Reset();

		filteredIn_suites = Document.get().getSuites().size();
		dpFilter.getHeaderTextAccessor().setText(
				"Selection Filter (" + Document.get().getSuites().size()
						+ " units available)");
	}

	@Override
	public boolean isFileredIn(Suite suite) {
		for (I_FilterSection section : sections)
			if (!section.isFileredIn(suite))
				return false;

		filteredIn_suites ++;
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
		Log.write("onChanged");
		if (isChanged()) {
			if (btnApply != null)
				btnApply.setEnabled(true);
			if (btnReset != null)
				btnReset.setEnabled(true);
		}
	}
}
