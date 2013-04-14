window.onload = function() {
	var json = window.parent.VRT_getSuiteInfo();
	if (typeof json != 'undefined') {
		
		if (typeof json.name != 'undefined')
				document.getElementById('VRT_name').innerHTML = json.name;

		if (typeof json.bedrooms != 'undefined')
			document.getElementById('VRT_bedrooms').innerHTML = json.bedrooms;

		if (typeof json.area != 'undefined')
				document.getElementById('VRT_area').innerHTML = json.area;
		 //BATHROOMS
		 if (typeof json.bathrooms != 'undefined')
		 document.getElementById('VRT_bathrooms').innerHTML = json.bathrooms;
		 
		 if (typeof json.floor != 'undefined')
			 document.getElementById('VRT_floor').innerHTML = json.floor;
		 
		if (typeof json.price != 'undefined')
			document.getElementById('VRT_price').innerHTML = json.price;
		
		if (typeof json.floorplanUrl != 'undefined')
			document.getElementById('VRT_floorplan').src = json.floorplanUrl;
//		if (typeof json.balconies != 'undefined')
//			document.getElementById('VRT_balconies').innerHTML = "Balconies: "
//					+ json.balconies;
//
//		if (typeof json.virtualTour != 'undefined') {
//			document.getElementById('VRT_virtualTour').onclick = window.parent.VRT_showVirtualTour;
//			document.getElementById('VRT_virtualTour').style.cursor = 'pointer';
//		} else
//			document.getElementById('VRT_virtualTour').style.visibility = 'hidden';
//			
//		
//		if (typeof json.moreInfo != 'undefined'){
//			document.getElementById('VRT_moreInfo').onclick = window.parent.VRT_showMoreInfo;
//			document.getElementById('VRT_moreInfo').style.cursor = 'pointer';
//		} else
//			document.getElementById('VRT_moreInfo').style.visibility = 'hidden';
//		
//		// FLOORPLAN VIEW
//		if (typeof json.floorplanUrl != 'undefined') {
//			document.getElementById('VRT_interiorImg').src = "img/InternalButton.png";
//			document.getElementById('VRT_interiorImg').onclick = window.parent.VRT_showFloorplan;
//		} else
//			document.getElementById('VRT_interiorImg').src = "img/InternalButtonDisabled.png";
		// PANORAMIC VIEW
			document.getElementById('VRT_showPanoramicView').onclick = window.parent.VRT_showPanoramicView;

		window.parent.VRT_setBalloonSize(1500,890);
	}
}