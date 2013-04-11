window.onload = function() {
	var json = window.parent.VRT_getSuiteInfo();
	if (typeof json != 'undefined') {
		if (typeof json.name != 'undefined')
			if (typeof json.floor != 'undefined')
				document.getElementById('VRT_name_floor').innerHTML = json.name
						+ '(Floor ' + json.floor + ')';

		if (typeof json.bedrooms != 'undefined')
			document.getElementById('VRT_bedrooms').innerHTML = "Bedrooms: "
					+ json.bedrooms;

		if (typeof json.area != 'undefined')
			if (typeof json.areaUm != 'undefined')
				document.getElementById('VRT_area').innerHTML = 'Area: '
						+ json.area + " " + json.areaUm;
		// //BATHROOMS
		// if (typeof json.bathrooms != 'undefined')
		// document.getElementById('VRT_bathrooms').innerHTML = "Bathrooms: "
		// + json.bathrooms;
		if (typeof json.price != 'undefined')
			document.getElementById('VRT_price').innerHTML = json.price;
		if (typeof json.balconies != 'undefined')
			document.getElementById('VRT_balconies').innerHTML = "Balconies: "
					+ json.balconies;

		if (typeof json.virtualTour != 'undefined') {
			document.getElementById('VRT_virtualTour').onclick = window.parent.VRT_showVirtualTour;
			document.getElementById('VRT_virtualTour').style.cursor = 'pointer';
		} else
			document.getElementById('VRT_virtualTour').style.visibility = 'hidden';
			
		
		if (typeof json.moreInfo != 'undefined'){
			document.getElementById('VRT_moreInfo').onclick = window.parent.VRT_showMoreInfo;
			document.getElementById('VRT_moreInfo').style.cursor = 'pointer';
		} else
			document.getElementById('VRT_moreInfo').style.visibility = 'hidden';
		
		// INTERIOR BUTTON
		if (typeof json.floorplanUrl != 'undefined') {
			document.getElementById('VRT_interiorImg').src = "img/InternalButton.png";
			document.getElementById('VRT_interiorImg').onclick = window.parent.VRT_showFloorplan;
		} else
			document.getElementById('VRT_interiorImg').src = "img/InternalButtonDisabled.png";
		// EXTERIOR BUTTON
		if (typeof json.panoramicViewUrl != 'undefined') {
			document.getElementById('VRT_exteriorImg').src = "img/ExternalButton.png";
			document.getElementById('VRT_exteriorImg').onclick = window.parent.VRT_showPanoramicView;
		} else
			document.getElementById('VRT_exteriorImg').src = "img/ExternalButtonDisabled.png";

		window.parent.VRT_setBalloonSize(600, 200);
	}
}
