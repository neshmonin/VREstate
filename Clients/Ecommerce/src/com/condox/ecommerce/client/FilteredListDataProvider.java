package com.condox.ecommerce.client;

import java.util.ArrayList;
import java.util.List;

import com.google.gwt.view.client.HasData;
import com.google.gwt.view.client.ListDataProvider;

public class FilteredListDataProvider<T> extends ListDataProvider<T> {

	private String filterString;
	public final IFilter<T> filter;

	public FilteredListDataProvider(IFilter<T> filter) {
		this.filter = filter;
	}

	public String getFilter() {
		return filterString;
	}

	public void setFilter(String filterString) {
//		Log.write("" + filterString.isEmpty());
		this.filterString = filterString;
		refresh();
	}

	public void resetFilter() {
		filterString = null;
		refresh();
	}

	public boolean hasFilter() {
		return (filterString != null) && (!filterString.isEmpty());
	}

	@Override
	protected void updateRowData(HasData<T> display, int start, List<T> values) {
		List<T> resulted = new ArrayList<T>();
		for (int i = 0; i < values.size(); i++)
			if (!hasFilter() || filter == null)
				resulted.add((T) values.get(i));
			else if (filter.isValid((T) values.get(i), getFilter()))
				resulted.add((T) values.get(i));
		display.setRowData(0, resulted);
		display.setRowCount(resulted.size());
	}
}
