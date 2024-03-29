package com.condox.ecommerce.client.tree.view;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.Iterator;
import java.util.List;

import com.condox.clientshared.communication.User;
import com.condox.clientshared.communication.User.UserRole;
import com.condox.clientshared.document.SuiteInfo;
import com.condox.ecommerce.client.resources.CSS;
import com.condox.ecommerce.client.tree.presenter.PickSuitePresenter;
import com.condox.ecommerce.client.tree.presenter.PickSuitePresenter.IDisplay;
import com.google.gwt.cell.client.ClickableTextCell;
import com.google.gwt.cell.client.FieldUpdater;
import com.google.gwt.core.client.GWT;
import com.google.gwt.event.dom.client.ClickEvent;
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
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.Label;
import com.google.gwt.user.client.ui.PopupPanel;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.view.client.ListDataProvider;

public class PickSuiteView extends Composite implements IDisplay {

	private static PickSuiteViewUiBinder uiBinder = GWT
			.create(PickSuiteViewUiBinder.class);
	// private String userRole = "";
	private PickSuitePresenter presenter;
	private List<SuiteInfo> data;
	private ListDataProvider<Floor> dataProvider = new ListDataProvider<Floor>();

	@UiField(provided = true)
	DataGrid<Floor> dataGrid = new DataGrid<Floor>();
	@UiField
	Button buttonCancel;
	@UiField
	Button buttonPrev;

	interface PickSuiteViewUiBinder extends UiBinder<Widget, PickSuiteView> {
	}

	public PickSuiteView() {
		initWidget(uiBinder.createAndBindUi(this));
		setSelectEvent();
	}

	private native void setSelectEvent()/*-{
		var instance = this;
		$wnd.onSelectSuite = function(id) {
			instance.@com.condox.ecommerce.client.tree.view.PickSuiteView::onSelectSuite(I)(id);
		}
	}-*/;

	private void onSelectSuite(int id) {
		// Window.alert("" + id);
		Iterator<SuiteInfo> infos = data.iterator();
		while (infos.hasNext()) {
			SuiteInfo info = infos.next();
			if (info.getId() == id) {
				this.selected = info;
				// presenter.onSubmit();
				// presenter.onPayNow();
				presenter.onNext();
				return;
			}
		}
	}

	@Override
	public void setPresenter(PickSuitePresenter presenter) {
		this.presenter = presenter;
	}

	private PopupPanel loading = new PopupPanel();

	@Override
	public void setData(String newUserRole, List<SuiteInfo> data) {
		// userRole = newUserRole;
		if (data == null) {
			loading.clear();
			loading.setModal(true);
			loading.setGlassEnabled(true);
			loading.add(new Label("Loading, please wait..."));
			loading.center();
		} else {
			loading.hide();
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
			Collections.sort(dataProvider.getList(), new Comparator<Floor>() {

				@Override
				public int compare(Floor o1, Floor o2) {
					/*
					 * int A = Integer.valueOf(o1.getName()); int B =
					 * Integer.valueOf(o2.getName()); return (A > B) ? 1 : -1;
					 */
					String A = o1.getName();
					String B = o2.getName();
					return A.compareTo(B);
				}
			});
			CreateDataGrid();
		}
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
		dataGrid.setColumnWidth(nameColumn, "100px");
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
		Column<Floor, String> dataColumn = new Column<Floor, String>(
				new ClickableTextCell(anchorRenderer2)) {
			@Override
			public String getValue(Floor object) {
				return object.getValue();
			}
		};
		dataColumn.setFieldUpdater(new FieldUpdater<Floor, String>() {
			@Override
			public void update(int index, Floor object, String value) {
				/*
				 * selectedFloor = object; presenter.onSubmit();
				 */
			}
		});
		dataGrid.addColumn(dataColumn, "");

		if (!dataProvider.getDataDisplays().contains(dataGrid)) {
			dataProvider.addDataDisplay(dataGrid);
		}

		// ================================
		String s = "Loading suits list, please wait for few seconds..";
		Label loadingLabel = new Label(s);
		loadingLabel.setStylePrimaryName("my-loading-label");
		// dataGrid.setLoadingIndicator(loadingLabel);
		dataGrid.setEmptyTableWidget(loadingLabel);

	}

	// **********************************
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

			for (SuiteInfo suite : suites) {
				String disabled = "";
				String style = "";

				// Visitor role && no MLS#
				if ((User.role.equals(UserRole.Visitor))
						&& ((suite.getMLS() == null) || (suite.getMLS()
								.isEmpty()))) {
					disabled = " disabled=\"true\" ";
					style = CSS.Instance.colors().suite_disabled();
				} else

					switch (suite.getStatus()) {
					case ResaleAvailable:
						style = CSS.Instance.colors().suite_resale();
						break;
					case AvailableRent:
						style = CSS.Instance.colors().suite_rent();
						break;
					default:
						style = CSS.Instance.colors().suite_default();
					}

				result += "<button type=\"button\" class=\"" + style + "\" ";
				result += "onclick=\"onSelectSuite(" + suite.getId() + ")\"";
				result += disabled + "title=\"" + suite.getTooltip() + "\">";
				result += suite.getName();
				result += "</button> ";
			}
			return result;
		}
	}

	private SuiteInfo selected;

	@Override
	public SuiteInfo getSuiteSelected() {
		return this.selected;
	}

	@UiHandler("buttonPrev")
	void onButtonPrevClick(ClickEvent event) {
		if (presenter != null)
			presenter.onPrev();
	}

	@UiHandler("buttonCancel")
	void onButtonCancelClick(ClickEvent event) {
		if (presenter != null)
			presenter.onCancel();
	}
}
