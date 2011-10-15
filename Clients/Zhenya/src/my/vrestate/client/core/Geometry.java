package my.vrestate.client.core;

import java.util.ArrayList;

public class Geometry {
	private ArrayList<ArrayList<double[]>> Lines = new ArrayList<ArrayList<double[]>>();

	public double[] getPoint(int LineIndex, int PointIndex) {
		double[] Result = { Lines.get(LineIndex).get(PointIndex)[0],
				Lines.get(LineIndex).get(PointIndex)[1],
				Lines.get(LineIndex).get(PointIndex)[2] };
		return Result;
	};

	public void addLine() {
		Lines.add(new ArrayList<double[]>());
	};

	public void addPoint(double X, double Y, double Z) {
		double[] XYZ = { X, Y, Z };
		Lines.get(Lines.size() - 1).add(XYZ);
	}
}
