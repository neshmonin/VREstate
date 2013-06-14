package com.condox.order.client.context;


public class SuitesContext extends Context {
	@Override
	public String getName() {
		return "Suite#" + getValue("selectedSuite");
	}

	@Override
	public ContextType getType() {
		return IContext.ContextType.SUITES;
	}

	@Override
	public Boolean isValid() {
		boolean result = true;
		try {
			Integer index = Integer.valueOf(getValue("selectedSuite"));
			if ((index.intValue()%2) == 0)
				result = false;
		} catch (NumberFormatException e) {
			e.printStackTrace();
		}
		return result;
	}
	
	
}
