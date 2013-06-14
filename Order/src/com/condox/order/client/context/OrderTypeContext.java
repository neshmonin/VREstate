package com.condox.order.client.context;


public class OrderTypeContext extends Context {
	
	public OrderTypeContext() {
		setValue("orderType", "private");
	}
	
	@Override
	public String getName() {
		return "Order#" + getValue("order.type");
	}

	@Override
	public ContextType getType() {
		return IContext.ContextType.ORDER_TYPE;
	}

	@Override
	public Boolean isValid() {
		boolean result = true;
		/*try {
			Integer index = Integer.valueOf(getValue("selectedSuite"));
			if ((index.intValue()%2) == 1)
				result = false;
		} catch (NumberFormatException e) {
			e.printStackTrace();
		}*/
		return result;
	}
	
	
}
