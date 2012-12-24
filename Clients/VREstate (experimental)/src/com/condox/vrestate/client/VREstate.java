package com.condox.vrestate.client;

import com.condox.vrestate.client.document.Document;
import com.condox.vrestate.client.document.Site;
import com.condox.vrestate.client.document.Suite;
import com.condox.vrestate.client.ge.GE;
import com.condox.vrestate.client.view.SiteView;
import com.condox.vrestate.client.view.SuiteView;
import com.google.gwt.core.client.EntryPoint;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.user.client.Timer;
import com.google.gwt.user.client.Window;
import com.nitrous.gwt.earth.client.api.KmlObject;

public class VREstate implements EntryPoint {

	private void test() {
		String kml1 = ""
				+ "	<Placemark id = \"placemark\">"
				+ "		<name>1</name>"
				+ "			<Point>"
				+ "				<coordinates>-79.22557523359221,43.66339971461343,0</coordinates>"
				+ "			</Point>" + "</Placemark>";
		KmlObject obj1 = GE.getPlugin().parseKml(kml1);
		GE.getPlugin().getFeatures().appendChild(obj1);

		String kml2 = ""
				+ "	<Placemark id = \"placemark\">"
				+ "		<name>1</name>"
				+ "			<Point>"
				+ "				<coordinates>-79.22557523359221,43.66339971461343,0</coordinates>"
				+ "			</Point>" + "</Placemark>";
		KmlObject obj2 = GE.getPlugin().parseKml(kml2);
		GE.getPlugin().getFeatures().appendChild(obj2);

	}

	@Override
	public void onModuleLoad() {
		InitOptions();
		new Timer() {
			@Override
			public void run() {
				if (GE.isReady()) {
					cancel();
					// Log.write("GE inited!");
					// start();
					// test();

					if (Options.getViewOrderId() != null) {
						String url = Options.HOME_URL
								+ "data/view?type=viewOrder&id="
								+ Options.getViewOrderId() + "&SID=" + User.SID;
//						Window.open(url, "", "");

						GET.send(url, new RequestCallback() {

							@Override
							public void onResponseReceived(Request request,
									Response response) {
								String json = response.getText();
								// SlashView view = new SlashView();
								Document.get().Parse(json);
								Filter.get().setVisible(false);

								SiteView view = new SiteView(Document.get()
										.getSites().get(0));
								// View.Push(view);
								Suite suite = Document.get().getSuites().get(0);
								// SuiteView view1 = new
								// SuiteView(suite);
								view.Select("suite", suite.getId());
							}

							@Override
							public void onError(Request request,
									Throwable exception) {
								// TODO Auto-generated method stub

							}
						});
					} else {
						String url = Options.HOME_URL
								+ "data/view?type=site&id="
								+ Options.getSiteId() + "&SID=" + User.SID;
//						Window.open(url, "", "");
						GET.send(url, new RequestCallback() {

							@Override
							public void onResponseReceived(Request request,
									Response response) {
								String json = response.getText();
								// SlashView view = new SlashView();
								// View.Push(view);
								Document.get().Parse(json);

								Site site = Document.get().getSites().get(0);
								Filter.get().Init();
								new SiteView(site);
								Filter.get().setVisible(true);
								Filter.get().Apply();
							}

							@Override
							public void onError(Request request,
									Throwable exception) {
								// TODO Auto-generated method stub

							}
						});
					}
				}
			}
		}.scheduleRepeating(10);

	}

	private void InitOptions() {
		Options.Init();
		new Timer() {

			@Override
			public void run() {
				if (Options.isReady()) {
					// Log.write("Options ready!");
					cancel();
					// Log.write("SiteId: " + Options.SITE_ID);
					LoginUser();
				}
			}

		}.scheduleRepeating(10);
	};

	private void LoginUser() {
		User.Login();
		new Timer() {
			@Override
			public void run() {
				if (User.isReady()) {
					cancel();
					// Log.write("User logined!");
					StartGE();
				}
			}
		}.scheduleRepeating(10);
	};

	private void StartGE() {
		new GE().Init();
	};

	// public static void setBrowserWindowTitle(String newTitle) {
	// if (Document.get() != null) {
	// Document.get().setTitle(VRE.WINDOW_TITLE);
	// }
	// }

}
// private static final String EARTH_API_KEY =
// "ABQIAAAAm7LIvLNR-PkJLewH4qmS7hREGtQZq9OFJfHndXhPP8gxXzlLARQtA_EfZjc9zs77WO25FrLcaZ4ZVA";