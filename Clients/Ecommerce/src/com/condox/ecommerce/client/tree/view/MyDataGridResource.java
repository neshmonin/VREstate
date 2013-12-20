package com.condox.ecommerce.client.tree.view;
import com.google.gwt.user.cellview.client.*;

public interface MyDataGridResource extends DataGrid.Resources {
	@Source({ DataGrid.Style.DEFAULT_CSS, "DataGridOverride.css" })
	  MyDataGridStyle dataGridStyle();
}
