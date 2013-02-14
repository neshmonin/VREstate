package com.condox.vrestate.client.view;

import com.condox.vrestate.client.Options;
import com.condox.vrestate.client.document.Document;
import com.condox.vrestate.client.document.ViewOrder.ProductType;
import com.condox.vrestate.client.view.GeoItems.BuildingGeoItem;
import com.condox.vrestate.client.view.GeoItems.IGeoItem;

public class BuildingView extends _SB_View {

	public BuildingView(IGeoItem buildingGeo) {
		super( buildingGeo );
	}

	@Override
	public void Select(String type, int id) {
		if (type.equals("building")) {
			BuildingGeoItem buildingGeoItem = _AbstractView.getBuildingGeoItem(id);
			if (theGeoItem.getId() == id) {
				if (!Options.isViewOrder() ||
					Document.targetViewOrder.getProductType() == ProductType.PublicListing ||
					Document.targetViewOrder.getProductType() == ProductType.Building3DLayout)
				{
					buildingGeoItem.onSelectionChanged(false);
					_AbstractView.Pop();
				}
			} else {
				buildingGeoItem.onSelectionChanged(true);
				BuildingGeoItem currGeoItem = (BuildingGeoItem)theGeoItem;
				currGeoItem.onSelectionChanged(false);
				_AbstractView.Pop_Push(new BuildingView(buildingGeoItem));
			}
		} else if (type.equals("suite")) {
			IGeoItem suiteGeo = _AbstractView.getSuiteGeoItem(id);
			SuiteView suiteView = new SuiteView(suiteGeo);
			_AbstractView.Push(suiteView);
		}
	}

}
