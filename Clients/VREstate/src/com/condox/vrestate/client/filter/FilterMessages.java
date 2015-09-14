package com.condox.vrestate.client.filter;

import com.google.gwt.i18n.client.Messages;

public interface FilterMessages extends Messages {
	String selectionFilter();
	String selectionFilter_UnitsAvailable(int availableUnits);
	String selectionFilter_Units_OutOf_Available(int filteredUnits, int availableUnits);
	String reset();
	String apply();
	String price();
	String price_any();
	String rent();
	String rent_any();
	String bedrooms();
	String bedrooms_any();
	String bathrooms();
	String bathrooms_any();
	String area();
	String area_any();
	String balconies();
	String balconies_any();
	String balconies_terraces();
	String balconies_terraces_any();
	String terraces();
	String terraces_any();
	String anyOr();
	String studio();
	String oneBedroom();
	String twoBedroom();
	String threeBedroom();
	String fourBedroom();
	String fiveBedroom();
	String sixBedroom();
	String sevenBedroom();
	String oneBathroom();
	String one05Bathroom();
	String twoBathroom();
	String two05Bathroom();
	String dontCare();
	String withBalcony();
	String withoutBalcony();
	String withTerrace();
	String withBalconyOrTerrace();
	String none();
	String min();
	String max();
	String priceValue(String formattedPrice);
	String sqM();
	String sqFt();
}
