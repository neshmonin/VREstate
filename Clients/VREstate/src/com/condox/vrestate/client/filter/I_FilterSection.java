package com.condox.vrestate.client.filter;

import com.condox.vrestate.client.document.Suite;

public interface I_FilterSection {
	public void Init();
	public void Reset();
	public boolean isFileredIn(Suite suite);
	public boolean isAny();
}