package com.condox.vrestate.client.filter;


import java.util.ArrayList;
import java.util.HashMap;
import java.util.Iterator;
import java.util.Map;
import java.util.SortedMap;
import java.util.TreeMap;

import com.condox.clientshared.abstractview.IGeoItem;
import com.condox.clientshared.abstractview.I_AbstractView;
import com.condox.clientshared.abstractview.I_Progress;
import com.condox.clientshared.abstractview.Log;
import com.condox.clientshared.communication.Options;
import com.condox.clientshared.communication.Options.ROLES;
import com.condox.clientshared.document.SuiteType;
import com.condox.vrestate.client.view.ProgressBar;
import com.condox.vrestate.client.view._AbstractView;
import com.condox.vrestate.client.view.GeoItems.SuiteGeoItem;
import com.condox.vrestate.client.view.GeoItems.SuiteGeoItem.GeoStatus;
import com.google.gwt.core.client.GWT;
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
import com.google.gwt.json.client.JSONArray;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.json.client.JSONString;
import com.google.gwt.user.client.Cookies;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.DisclosurePanel;
import com.google.gwt.user.client.ui.DockPanel;
import com.google.gwt.user.client.ui.HasHorizontalAlignment;
import com.google.gwt.user.client.ui.HasVerticalAlignment;
import com.google.gwt.user.client.ui.HorizontalPanel;
import com.google.gwt.user.client.ui.RootPanel;
import com.google.gwt.user.client.ui.StackPanel;

public class Filter extends StackPanel implements I_FilterSectionContainer {
	public static boolean initialized = false;
	private static Filter instance = new Filter();
	private static SortedMap<Integer, SuiteGeoItem> filteredIn_suiteGeos = null;
	private static SortedMap<Integer, SuiteGeoItem> active_suiteGeos = null;
	public static FilterMessages i18n = (FilterMessages)GWT.create(FilterMessages.class);

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
		i18n = (FilterMessages)GWT.create(FilterMessages.class);
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
		horizontalPanel.setHorizontalAlignment(HasHorizontalAlignment.ALIGN_CENTER);
		horizontalPanel.setSize("104px", "20px");

		btnReset = new Button("New button");
		btnReset.addClickHandler(new ClickHandler() {
			public void onClick(ClickEvent event) {
				Reset();
				btnReset.setEnabled(false);
				ApplyAndSelect();
				btnApply.setEnabled(false);
				if (Options.ROLE != ROLES.KIOSK)
					saveToCookie();
			}
		});
		btnReset.setEnabled(false);
		btnReset.setText(i18n.reset());
		horizontalPanel.add(btnReset);
		btnReset.setWidth("100px");

		btnApply = new Button("New button");
		btnApply.addClickHandler(new ClickHandler() {
			public void onClick(ClickEvent event) {
				ApplyAndSelect();
				btnApply.setEnabled(false);
				if (Options.ROLE != ROLES.KIOSK)
					saveToCookie();
			}
		});
		btnApply.setEnabled(false);
		btnApply.setText(i18n.apply());
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

		getActiveSuiteGeoItems().putAll(_AbstractView.getSuiteGeoItems());
		
		I_FilterSection propertyTypeSection = 
			OwnershipSection.CreateSectionPanel(this, "Property Type", stackPanel);
		if (propertyTypeSection != null) sections.add(propertyTypeSection);
			
		Iterator<I_FilterSection> iterator = sections.iterator();
		while (iterator.hasNext())
			if (iterator.next() == null)
				iterator.remove();

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

	public void ApplyAndSelect() {
		Apply();
		IGeoItem suiteGeo = Filter.get().getNextGeoItem();
		if (suiteGeo != null) {
			I_AbstractView currView = _AbstractView.getCurrentView();
			if (currView.getGeoItem().getType() != "suite")
				currView.Select(suiteGeo.getType(), suiteGeo.getId());
		}
	}
	
	
	@Override
	public void Apply() {
		for (I_FilterSection section : sections)
			section.Apply();
		
		getFilteredInSuiteGeoItems().clear();

		I_Progress progressBar = new ProgressBar();
		progressBar.SetupProgress(I_Progress.ProgressType.Processing);

		int howMany = _AbstractView.getSuiteGeoItems().size();
		int count = 0;
		for (SuiteGeoItem suiteGeo : _AbstractView.getSuiteGeoItems().values()) {
			progressBar.UpdateProgress(count * 100 / howMany);
			if (suiteGeo.ShowIfFilteredIn())
				getFilteredInSuiteGeoItems().put(suiteGeo.getId(), suiteGeo);
			//else
			//	Log.write("Filter->Apply -> filtered out: suiteId="
			//			+suiteGeo.getId()+
			//			" suiteName="+suiteGeo.getName());
				
			count++;
		}
		progressBar.CleanupProgress();

		if (btnApply != null)
			btnApply.setEnabled(true);
		if (btnReset != null)
			btnReset.setEnabled(true);
		lastState = StateHash();

		if (getFilteredInSuiteGeoItems().size() == _AbstractView.getSuiteGeoItems().size())
			dpFilter.getHeaderTextAccessor()
				.setText(i18n.selectionFilter_UnitsAvailable(_AbstractView.getSuiteGeoItems().size()));
		else
			dpFilter.getHeaderTextAccessor()
				.setText(i18n.selectionFilter_Units_OutOf_Available(getFilteredInSuiteGeoItems().size(),
																_AbstractView.getSuiteGeoItems().size()));
	}

	@Override
	public int StateHash() {
		int hash = 9999;
		if (sections != null) {			
			for (I_FilterSection section : sections)
				hash += section.StateHash();
		}
		
		return hash;
	}

	@Override
	public void Reset() {
		getFilteredInSuiteGeoItems().putAll(getActiveSuiteGeoItems());
		for (I_FilterSection section : sections)
			section.Reset();
		Apply();
	}

	@Override
	public boolean isFilteredIn(SuiteGeoItem suiteGI) {
		if (suiteGI.getGeoStatus() == GeoStatus.Sold && !Options.getShowSold())
			return false;

		for (I_FilterSection section : sections)
			if (!section.isFilteredIn(suiteGI))
				return false;

		return true;
	}

	@Override
	public boolean isAny() {
		for (I_FilterSection section : sections)
			if (!section.isAny())
				return false;
		return true;
	}
	
	public int howManyFilteredIn() {
		return getFilteredInSuiteGeoItems().size();
	}	
	
	@Override
	public Map<Integer, SuiteGeoItem> getActiveSuiteGeoItems() {
		if (active_suiteGeos == null) {
			active_suiteGeos = new TreeMap<Integer, SuiteGeoItem>();
			active_suiteGeos.putAll(_AbstractView.getSuiteGeoItems());
		}
		return active_suiteGeos;
	}

	@Override
	public void setActiveSuiteGeoItems(Map<Integer, SuiteGeoItem> suites) {
		getActiveSuiteGeoItems().clear();
		getActiveSuiteGeoItems().putAll(suites);
	}
	
	public Map<Integer, SuiteGeoItem> getFilteredInSuiteGeoItems() {
		if (filteredIn_suiteGeos == null) {
			filteredIn_suiteGeos = new TreeMap<Integer, SuiteGeoItem>();
			filteredIn_suiteGeos.putAll(getActiveSuiteGeoItems());
		}
		return filteredIn_suiteGeos;
	}

	@Override
	public Map<Integer, SuiteType> getActiveSuiteTypes() {
		Map<Integer, SuiteType> suiteTypes = new HashMap<Integer, SuiteType>();
		for (SuiteGeoItem suiteGI : getActiveSuiteGeoItems().values()) {
			SuiteType type = suiteGI.suite.getSuiteType();
			if (type == null) continue;
			int typeId = type.getId();
			if (!suiteTypes.containsKey(typeId))
				suiteTypes.put(typeId, type);
		}			
			
		return suiteTypes;
	}


	public IGeoItem getNextGeoItem() {
		int currId = _AbstractView.getCurrentGeoItem().getId();
		int size = getFilteredInSuiteGeoItems().size();
		if (size == 0) return null;
		
		SuiteGeoItem[] suitesGIArray = getFilteredInSuiteGeoItems().values().toArray(new SuiteGeoItem[size]);
		for (int i=0; i< size; i++) {
			SuiteGeoItem suiteGI = suitesGIArray[i];
			if (suiteGI.suite.getId() == currId) {
				SuiteGeoItem nextGeo = i+1 < size? suitesGIArray[i+1] : suitesGIArray[0];
				return nextGeo;
			}				
		}

		SuiteGeoItem first = suitesGIArray[0];
		return first;
	}

	public IGeoItem getPrevGeoItem() {
		int currId = _AbstractView.getCurrentGeoItem().getId();
		int size = getFilteredInSuiteGeoItems().size();
		if (size == 0) return null;
		
		SuiteGeoItem[] suitesGIArray = getFilteredInSuiteGeoItems().values().toArray(new SuiteGeoItem[size]);
		for (int i=0; i< size; i++) {
			SuiteGeoItem suiteGI = suitesGIArray[i];
			if (suiteGI.suite.getId() == currId) {
				SuiteGeoItem prev = i > 1 ? suitesGIArray[i-1] : suitesGIArray[size-1];
				return prev;
			}				
		}

		SuiteGeoItem first = suitesGIArray[0];
		return first;
	}

	@Override
	public void RemoveSection() {
	}

	@Override
	public I_FilterSectionContainer getParentSectionContainer() {
		return null;
	}

	private static int lastState = -1;
	public static void onChange() {
		if (instance == null) return;
		
		if (instance.StateHash() != lastState) {
			if (instance.btnApply != null)
				instance.btnApply.setEnabled(true);
			if (instance.btnReset != null)
				instance.btnReset.setEnabled(true);
			
			instance.UpdateSize();
		}			
	}

	@Override
	public JSONObject toJSONObject() {
		JSONObject result = new JSONObject();
		result.put("name", new JSONString(this.getClass().getName()));
		
		JSONArray json_sections = new JSONArray();
		int index = 0;
		for (I_FilterSection section : sections) {
			json_sections.set(index++, section.toJSONObject());
		}
		result.put("sections", json_sections);
		
		return result;
	}

	@Override
	public void fromJSONObject(JSONObject json) {
		if (!json.containsKey("name")) return;
		if (!json.containsKey("sections")) return;
		
		if (json.get("name").isString() == null) return;
		if (json.get("sections").isArray() == null) return;
		
		String name = json.get("name").isString().stringValue();
		JSONArray arr = json.get("sections").isArray();
		
		if (name.equals(getClass().getName())) {
			for (I_FilterSection section : sections) {
				for (int i = 0; i < arr.size(); i++)
					section.fromJSONObject(arr.get(i).isObject());
			}
		}
	}
	
	private void saveToCookie() {
		String json = toJSONObject().toString();
		Log.write("saveToCookies, JSON: " + json);
		Cookies.setCookie("vreFilter", json);
	}
	
	public void loadFromCookies() {
		if (Options.ROLE == ROLES.KIOSK)
			return;
		
		String json = Cookies.getCookie("vreFilter");
		if ((json != null) && (!json.isEmpty())) {
			Log.write("loadFromCookies, JSON: " + json);
			JSONObject obj = JSONParser.parseLenient(json).isObject();
			if (obj != null)
				fromJSONObject(obj);
		}
	}
	
}
