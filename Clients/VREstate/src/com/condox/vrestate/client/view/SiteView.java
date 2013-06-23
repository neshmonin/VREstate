package com.condox.vrestate.client.view;

import com.condox.vrestate.client.view.GeoItems.BuildingGeoItem;
import com.condox.vrestate.client.view.GeoItems.SuiteGeoItem;
import com.condox.vrestate.shared.IGeoItem;

public class SiteView extends _SB_View {

	public SiteView(IGeoItem siteGeo) {
		super( siteGeo );
	}

	/* (non-Javadoc)
	 * @see com.condox.vrestate.client.view.IInteractiveView#Select(java.lang.String, int)
	 */
	@Override
	public void Select(String type, int id) {
		if (type.equals("building"))
		{
			BuildingGeoItem buildingGeo = _AbstractView.getBuildingGeoItem(id);
			BuildingView bldngView = new BuildingView(buildingGeo);
			buildingGeo.onSelectionChanged(true);
			_AbstractView.Push(bldngView);
		}
		else if (type.equals("suite"))
		{
			SuiteGeoItem suiteGeo = _AbstractView.getSuiteGeoItem(id);
			BuildingGeoItem buildingGeo = _AbstractView.getBuildingGeoItem(suiteGeo.getParent_id());
			
			BuildingView bldngView = new BuildingView(buildingGeo);
			buildingGeo.onSelectionChanged(true);
			_AbstractView.Push(bldngView);					

			_AbstractView.AddSelection(suiteGeo);
		}
	}
}
