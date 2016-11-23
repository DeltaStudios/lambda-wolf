using UnityEngine;
using System;

[ExecuteInEditMode]
public class BezierSpline : MonoBehaviour {

	[SerializeField]
	private Vector3[] points;
	[SerializeField]
	private float[] starts;

	[SerializeField]
	private BezierControlPointMode[] modes;

	[SerializeField]
	private bool loop;

	public bool Loop {
		get {
			return loop;
		}
		set {
			loop = value;
			if (value == true) {
				modes[modes.Length - 1] = modes[0];
				SetControlPoint(0, points[0]);
			}
		}
	}

	public int ControlPointCount {
		get {
			return points.Length;
		}
	}

	public Vector3 GetControlPoint (int index) {
		return points[index];
	}

	public void SetControlPoint (int index, Vector3 point) {
		if (index % 3 == 0) {
			Vector3 delta = point - points[index];
			if (loop) {
				if (index == 0) {
					points[1] += delta;
					points[points.Length - 2] += delta;
					points[points.Length - 1] = point;
				}
				else if (index == points.Length - 1) {
					points[0] = point;
					points[1] += delta;
					points[index - 1] += delta;
				}
				else {
					points[index - 1] += delta;
					points[index + 1] += delta;
				}
			}
			else {
				if (index > 0) {
					points[index - 1] += delta;
				}
				if (index + 1 < points.Length) {
					points[index + 1] += delta;
				}
			}
		}
		points[index] = point;
		EnforceMode(index);
		RecalculateStarts ();
	}

	public BezierControlPointMode GetControlPointMode (int index) {
		return modes[(index + 1) / 3];
	}

	public void SetControlPointMode (int index, BezierControlPointMode mode) {
		int modeIndex = (index + 1) / 3;
		modes[modeIndex] = mode;
		if (loop) {
			if (modeIndex == 0) {
				modes[modes.Length - 1] = mode;
			}
			else if (modeIndex == modes.Length - 1) {
				modes[0] = mode;
			}
		}
		EnforceMode(index);
	}

	private void EnforceMode (int index) {
		int modeIndex = (index + 1) / 3;
		BezierControlPointMode mode = modes[modeIndex];
		if (mode == BezierControlPointMode.Free || !loop && (modeIndex == 0 || modeIndex == modes.Length - 1)) {
			return;
		}

		int middleIndex = modeIndex * 3;
		int fixedIndex, enforcedIndex;
		if (index <= middleIndex) {
			fixedIndex = middleIndex - 1;
			if (fixedIndex < 0) {
				fixedIndex = points.Length - 2;
			}
			enforcedIndex = middleIndex + 1;
			if (enforcedIndex >= points.Length) {
				enforcedIndex = 1;
			}
		}
		else {
			fixedIndex = middleIndex + 1;
			if (fixedIndex >= points.Length) {
				fixedIndex = 1;
			}
			enforcedIndex = middleIndex - 1;
			if (enforcedIndex < 0) {
				enforcedIndex = points.Length - 2;
			}
		}

		Vector3 middle = points[middleIndex];
		Vector3 enforcedTangent = middle - points[fixedIndex];
		if (mode == BezierControlPointMode.Aligned) {
			enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);
		}
		points[enforcedIndex] = middle + enforcedTangent;
	}

	public int CurveCount {
		get {
			return (points.Length - 1) / 3;
		}
	}

	int GetCurve(float t){
		t = Mathf.Clamp01 (t);

		int i;
		for (i = 0; i < CurveCount && starts[i] <= t; i++) {};
		return i-1;
	}

	public Vector3 GetPoint (float t) {
		int i = GetCurve (t);
		if (i != -1) {
			if (i == CurveCount - 1)
				t = (t - starts [i]) / (1f - starts [i]);
			else
				t = (t - starts [i]) / (starts [i + 1] - starts [i]);
			return transform.TransformPoint(Bezier.GetPointUniform(points[i*3], points[i*3 + 1], points[i*3 + 2], points[i*3 + 3], t));
		} 

		return Vector3.zero;

	}
	
	public Vector3 GetVelocity (float t) {
		int i = GetCurve (t);
		if (i != -1) {
			if (i == CurveCount - 1)
				t = (t - starts [i]) / (1f - starts [i]);
			else
				t = (t - starts [i]) / (starts [i + 1] - starts [i]);
			return transform.TransformPoint (Bezier.GetFirstDerivativeUniform (points [i * 3], points [i * 3 + 1], points [i * 3 + 2], points [i * 3 + 3], t)) - transform.position;
		} 

		return Vector3.zero;

	}
	
	public Vector3 GetDirection (float t) {
		return GetVelocity(t).normalized;
	}

	public void AddCurve () {
		Debug.Log ("adding curve");
		Vector3 point = points[points.Length - 1];
		Array.Resize(ref points, points.Length + 3);

		point.x += 1f;
		points[points.Length - 3] = point;
		point.x += 1f;
		points[points.Length - 2] = point;
		point.x += 1f;
		points[points.Length - 1] = point;

		Array.Resize(ref modes, modes.Length + 1);
		Array.Resize(ref starts, starts.Length + 1);

		modes[modes.Length - 1] = modes[modes.Length - 2];
		EnforceMode(points.Length - 4);
		RecalculateStarts ();

		if (loop) {
			points[points.Length - 1] = points[0];
			modes[modes.Length - 1] = modes[0];
			EnforceMode(0);
		}
	}
	
	public void Reset () {
		points = new Vector3[] {
			new Vector3(1f, 0f, 0f),
			new Vector3(2f, 0f, 0f),
			new Vector3(3f, 0f, 0f),
			new Vector3(4f, 0f, 0f)
		};
		starts = new float[]{ 0f };

		modes = new BezierControlPointMode[] {
			BezierControlPointMode.Free,
			BezierControlPointMode.Free
		};
	}
			
	void RecalculateStarts(){
		
		//This should be true: starts [0] = 0 for this to work
		for (int i = 1; i < CurveCount; i++) {
			starts [i] = starts[i-1] + Bezier.GetLength (points [(i-1) * 3], points [(i-1) * 3 + 1], points [(i-1) * 3 + 2], points [(i-1) * 3 + 3]);
		}
		float totalLength = starts [CurveCount - 1] + Bezier.GetLength (points [points.Length-4], points [points.Length-3], points [points.Length-2], points [points.Length-1]);
		for (int i = 1; i < CurveCount; i++) {
			starts [i] = starts[i]/totalLength;
		}
	}

	#if UNITY_EDITOR
	void Update(){
		RecalculateStarts ();
	}
	#endif
}