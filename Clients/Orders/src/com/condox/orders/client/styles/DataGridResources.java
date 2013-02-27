package com.condox.orders.client.styles;

import com.google.gwt.user.cellview.client.DataGrid;
import com.google.gwt.user.cellview.client.DataGrid.Style;



public interface DataGridResources extends DataGrid.Resources {
	@Source ("MyStyles.css")
	Style dataGrid();
}
