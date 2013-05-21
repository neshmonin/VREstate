package com.condox.vrestate.client.filter;

import com.google.gwt.event.logical.shared.ValueChangeEvent;
import com.google.gwt.user.client.ui.CheckBox;

public class MyCustomCheckBox extends CheckBox implements I_State {
	
	public MyCustomCheckBox(String string) {
		super(string);
	}

	@Override
	public void setValue(Boolean value, boolean fireEvents) {
	      Boolean oldValue = getValue();
	      super.setValue(value, fireEvents);
	      if (oldValue.equals(value))
	    	  if (fireEvents)
	    		  ValueChangeEvent.fire(this, value);
	}

	@Override
	public int StateHash() {
		int hash = hashCode();
		if (getValue())
			hash += 123;
		return hash;
	}

}
