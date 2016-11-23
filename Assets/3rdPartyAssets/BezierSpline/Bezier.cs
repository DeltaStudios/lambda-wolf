using UnityEngine;

public static class Bezier {

	public static Vector3 GetPoint (Vector3 p0, Vector3 p1, Vector3 p2, float t) {
		t = Mathf.Clamp01(t);
		float oneMinusT = 1f - t;
		return
			oneMinusT * oneMinusT * p0 +
			2f * oneMinusT * t * p1 +
			t * t * p2;
	}

	public static Vector3 GetFirstDerivative (Vector3 p0, Vector3 p1, Vector3 p2, float t) {
		return
			2f * (1f - t) * (p1 - p0) +
			2f * t * (p2 - p1);
	}

	public static Vector3 GetPoint (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
		t = Mathf.Clamp01(t);
		float OneMinusT = 1f - t;
		return
			OneMinusT * OneMinusT * OneMinusT * p0 +
			3f * OneMinusT * OneMinusT * t * p1 +
			3f * OneMinusT * t * t * p2 +
			t * t * t * p3;
	}

	private static float GetUniformT(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t){
		int precision = 100;
		float[] arcLengths = new float[precision];
		arcLengths[0] = 0;

		for(var i = 1; i < precision; i += 1) {
			arcLengths[i] = arcLengths[i-1] + Vector3.Distance(GetPoint(p0,p1,p2,p3,((float)i)/precision), GetPoint(p0,p1,p2,p3,((float)i+1)/precision));
		}

		float targetLength = t * arcLengths[arcLengths.Length-1];

		// Find index
		int low = 0;
		int high = arcLengths.Length;
		int index = 0;
		while (low < high) {
			index = low + (high - low) / 2;
			if (arcLengths[index] < targetLength) {
				low = index + 1;

			} else {
				high = index;
			}
		}
		if (arcLengths[index] > targetLength) {
			index--;
		}

		// Calculate t value
		float lengthBefore = arcLengths[index];
		if (lengthBefore == targetLength) {
			return index / arcLengths [arcLengths.Length - 1];

		} else if (index + 1 == arcLengths.Length) {
			return 1f;
		} else{
			return (index + (targetLength - lengthBefore) / (arcLengths[index + 1] - lengthBefore)) / arcLengths.Length;
		}
	}

	public static Vector3 GetPointUniform(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t){
		return GetPoint (p0,p1,p2,p3,GetUniformT (p0,p1,p2,p3,t));
	}

	public static Vector3 GetFirstDerivative (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
		t = Mathf.Clamp01(t);
		float oneMinusT = 1f - t;
		return
			3f * oneMinusT * oneMinusT * (p1 - p0) +
			6f * oneMinusT * t * (p2 - p1) +
			3f * t * t * (p3 - p2);
	}

	public static Vector3 GetFirstDerivativeUniform(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t){
		return GetFirstDerivative (p0,p1,p2,p3,GetUniformT (p0,p1,p2,p3,t));
	}

	public static float GetLength (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3){
		Vector3 pastPoint = GetPoint (p0,p1,p2,p3,0);
		float length = 0;
		for (int i = 1; i < 100; i++) {
			Vector3 currentPoint = GetPoint (p0,p1,p2,p3,((float) i) / 100);
			length += (pastPoint - currentPoint).magnitude;
			pastPoint = currentPoint;
		}
		return length;
	}
}