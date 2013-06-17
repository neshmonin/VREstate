package com.condox.order.client.view;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.Iterator;
import java.util.List;

import com.condox.order.client.presenter.SuitesPresenter;
import com.condox.order.shared.BuildingInfo;
import com.condox.order.shared.SuiteInfo;
import com.google.gwt.cell.client.ClickableTextCell;
import com.google.gwt.cell.client.FieldUpdater;
import com.google.gwt.core.client.GWT;
import com.google.gwt.core.client.JavaScriptObject;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.event.dom.client.ClickHandler;
import com.google.gwt.safehtml.shared.SafeHtml;
import com.google.gwt.safehtml.shared.SafeHtmlBuilder;
import com.google.gwt.text.shared.AbstractSafeHtmlRenderer;
import com.google.gwt.text.shared.SafeHtmlRenderer;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.user.cellview.client.Column;
import com.google.gwt.user.cellview.client.ColumnSortEvent.ListHandler;
import com.google.gwt.user.cellview.client.DataGrid;
import com.google.gwt.user.cellview.client.TextColumn;
import com.google.gwt.user.client.Element;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.DockLayoutPanel;
import com.google.gwt.user.client.ui.FlowPanel;
import com.google.gwt.user.client.ui.Hyperlink;
import com.google.gwt.user.client.ui.Label;
import com.google.gwt.user.client.ui.VerticalPanel;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.view.client.ListDataProvider;
import com.google.gwt.view.client.SelectionChangeEvent;
import com.google.gwt.view.client.SingleSelectionModel;

public class SuitesView extends Composite implements SuitesPresenter.IDisplay {

	private static SuitesViewUiBinder uiBinder = GWT
			.create(SuitesViewUiBinder.class);
	private SuitesPresenter presenter;
	private List<SuiteInfo> data;
	private ListDataProvider<Floor> dataProvider = new ListDataProvider<Floor>();
	
	@UiField(provided=true) DataGrid<Floor> dataGrid = new DataGrid<Floor>();
	@UiField DockLayoutPanel dockPanel;

	interface SuitesViewUiBinder extends UiBinder<Widget, SuitesView> {
	}

	public SuitesView() {
		initWidget(uiBinder.createAndBindUi(this));
		setSelectEvent();
	}
	
	private native void setSelectEvent()/*-{
		var instance = this;
		$wnd.onSelectSuite = function(id){
			instance.@com.condox.order.client.view.SuitesView::onSelectSuite(I)(id);
		}
	}-*/;
	
	private void onSelectSuite(int id) {
//		Window.alert("" + id);
		Iterator<SuiteInfo> infos = data.iterator();
		while (infos.hasNext()) {
			SuiteInfo info = infos.next();
			if (info.getId() == id) {
				this.selected = info;
				presenter.onSubmit();
				return;
			}
		}
	}

	@Override
	public void setPresenter(SuitesPresenter presenter) {
		this.presenter = presenter;
	}

	@Override
	public void setData(List<SuiteInfo> data) {
		this.data = data;
		dataProvider.getList().clear();
		Iterator<SuiteInfo> infos = this.data.iterator();
		while (infos.hasNext()) {
			SuiteInfo info = infos.next();
			String floorName = info.getFloorName();
			Iterator<Floor> floors = dataProvider.getList().iterator();
			while (floors.hasNext()) {
				Floor floor = floors.next();
				if (floor.getName().equals(floorName)) {
					floor.add(info);
					info = null;
					break;
				}
			}
			if (info != null) {
				Floor floor = new Floor();
				dataProvider.getList().add(floor);
				floor.add(info);
			}
		}
		Collections.sort(dataProvider.getList(),
				new Comparator<Floor>() {

					@Override
					public int compare(Floor o1, Floor o2) {
						int A = Integer.valueOf(o1.getName());
						int B = Integer.valueOf(o2.getName());
						return (A > B) ? 1 : -1;
					}
				});
		CreateDataGrid();
	}
	private void CreateDataGrid() {
		// ==============================================
		// dataGrid = new DataGrid<Building>();
		int count = dataGrid.getColumnCount();
		for (int i = 0; i < count; i++)
			dataGrid.removeColumn(0);

		ListHandler<Floor> sortHandler = new ListHandler<Floor>(
				dataProvider.getList());
		dataGrid.addColumnSortHandler(sortHandler);
		// buildings.setKeyboardSelectionPolicy(KeyboardSelectionPolicy.ENABLED);
		// -------------------------
		// -------------------------
		// //
		
		// Add a text column to show the no of the floor.
		TextColumn<Floor> nameColumn = new TextColumn<Floor>() {
			@Override
			public String getValue(Floor object) {
				return "Floor " + object.getName();
			}
		};
		nameColumn.setSortable(true);
		sortHandler.setComparator(nameColumn, new Comparator<Floor>() {
			@Override
			public int compare(Floor A, Floor B) {
				return A.getName().compareTo(B.getName());
			}
		});
		dataGrid.setColumnWidth(nameColumn, "200px");
		dataGrid.addColumn(nameColumn, presenter.getSelectedBuildingStreet());

		// -------------------------
		// Show names.
		SafeHtmlRenderer<String> anchorRenderer2 = new AbstractSafeHtmlRenderer<String>() {
			@Override
			public SafeHtml render(String object) {
				SafeHtmlBuilder sb = new SafeHtmlBuilder();
				sb.appendHtmlConstant(object);
				return sb.toSafeHtml();
			}
		};
		
		// dataColumn
		Column<Floor, String> dataColumn = new Column<Floor, String>(new ClickableTextCell(
				anchorRenderer2)) {
			@Override
			public String getValue(Floor object) {
				return object.getValue();
			}
		};
		dataColumn.setFieldUpdater(new FieldUpdater<Floor, String>() {
			@Override
			public void update(int index, Floor object, String value) {
				/*selectedFloor = object;
				presenter.onSubmit();*/
			}
		});
		dataGrid.addColumn(dataColumn, "");

		if (!dataProvider.getDataDisplays().contains(dataGrid)) {
			dataProvider.addDataDisplay(dataGrid);
		}
		
	}
	//**********************************
	private class Floor {
		private ArrayList<SuiteInfo> suites = new ArrayList<SuiteInfo>();
		private String name = "";

		public void add(SuiteInfo suite) {
			suites.add(suite);
			if (name.isEmpty())
				name = suite.getFloorName();
		}

		public void sort() {
			Collections.sort(suites, new Comparator<SuiteInfo>() {

				@Override
				public int compare(SuiteInfo arg0, SuiteInfo arg1) {
					int result = String.CASE_INSENSITIVE_ORDER.compare(
							arg0.getName(), arg1.getName());
					return result;
				}
			});
		}

		public String getName() {
			return name;
		}

		public String getValue() {
			sort();
			String result = "";
			/*VerticalPanel vp = new VerticalPanel();
			Label label1 = new Label("label1");
			Label label2 = new Label("label2");
			Hyperlink link1 = new Hyperlink("link1","1");
			Hyperlink link2 = new Hyperlink("link2","2");
			vp.add(label1);
			vp.add(label2);
			vp.add(link1);
			vp.add(link2);
			result = vp.toString();*/
			
			for (SuiteInfo suite : suites) {
				String disabled = "";
				switch (suite.getStatus()) {
				case Sold:
					break;
				default:
					disabled += " disabled=\"true\" ";
				}

				result += "<button ";
				result += "type=\"button\"" + " onclick=\"onSelectSuite("
						+ suite.getId() + ")\"" + "style=\"width:50px\""
						+ "class=\"btnSelectSuite\"" + disabled + "title=\""
						+ suite.getTooltip() + "\">";
				result += suite.getName();
				result += "</button> ";
			}
			return result;
		}
	}

	private SuiteInfo selected;
	@Override
	public String getSuiteName() {
		if (selected != null)
			return selected.getName();
		return null;
	}
	@UiHandler("hyperlink")
	void onHyperlinkClick(ClickEvent event) {
		presenter.prev();
	}

	@Override
	public String getSuiteFloorplan() {
		if (selected != null)
			return selected.getFloorplan_url();
		return null;
	}
	
}
