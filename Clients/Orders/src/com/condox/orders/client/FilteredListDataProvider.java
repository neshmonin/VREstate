package com.condox.orders.client;

import java.util.ArrayList;
import java.util.List;

import com.condox.orders.client.Log;
import com.google.gwt.view.client.HasData;
import com.google.gwt.view.client.ListDataProvider;
import com.google.gwt.view.client.Range;

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
//		Log.write("setFilter");
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
//		Log.write("UpdateRowData");
        if (!hasFilter() || filter == null) { // we don't need to filter, so call base class  
            super.updateRowData(display, start, values);  
        } else {  
            int end = start + values.size();  
            Range range = display.getVisibleRange();  
            int curStart = range.getStart();  
            int curLength = range.getLength();  
            int curEnd = curStart + curLength;  
            if (start == curStart || (curStart < end && curEnd > start)) {  
                int realStart = curStart < start ? start : curStart;  
                int realEnd = curEnd > end ? end : curEnd;  
                int realLength = realEnd - realStart;  
                List<T> resulted = new ArrayList<T>(realLength);  
                for (int i = realStart - start; i < realStart - start + realLength; i++) {  
                    if (filter.isValid((T) values.get(i), getFilter())) {  
                        resulted.add((T) values.get(i));  
                    }  
                }  
                display.setRowData(realStart, resulted);  
                display.setRowCount(resulted.size());  
            }  
        }  
    }  

}
