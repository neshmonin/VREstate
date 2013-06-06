package com.condox.order.client.context;


public class BuildingsContext extends Context {

	@Override
	public String getName() {
		return "Building #" + getValue("selectedBuilding");
	}

	@Override
	public ContextType getType() {
		return IContext.ContextType.BUILDINGS;
	}

	@Override
	public Boolean isValid() {
		boolean result = true;
		try {
			Integer index = Integer.valueOf(getValue("selectedBuilding"));
			if ((index.intValue()%3) == 0)
				result = false;
		} catch (NumberFormatException e) {
			e.printStackTrace();
		}
		return result;
	}
	
	

}
