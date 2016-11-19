using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BezierSpline))]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class Path : MonoBehaviour {

	[SerializeField] int resolution;
	[SerializeField] float width = 1f;
	// Use this for initialization
	void Start(){
		DrawMesh ();
	}

	public void DrawMesh () {
		BezierSpline spline = GetComponent<BezierSpline> ();
		Mesh mesh = new Mesh ();
		GetComponent<MeshFilter> ().mesh = mesh;

		Vector3[] vertices = new Vector3[(resolution+1)*2];
		int[] triangles = new int[resolution*2*3];

		for (int i = 0; i < resolution; i++) {
			Vector3 position = transform.InverseTransformPoint(spline.GetPoint (((float) i) / resolution));
			Vector3 right = ( transform.InverseTransformPoint(spline.GetPoint (((float)i+1) / resolution))-position).normalized;
			Vector3 forward = Vector3.Cross (right, Vector3.up);

			vertices [i * 2] = position+forward*(width/2);
			vertices [i * 2 + 1] = position-forward*(width/2);

			// First triangle
			triangles[i * 2 * 3] = i * 2;
			triangles [i * 2 * 3 + 1] = i * 2 + 3;
			triangles [i * 2 * 3 + 2] = i * 2 + 1;

			// Second triangle
			triangles[i * 2 * 3 + 3] = i*2;
			triangles [i * 2 * 3 + 4] = i * 2 + 2;
			triangles [i * 2 * 3 + 5] = i * 2 + 3;
		}
			
		vertices[resolution*2] = transform.InverseTransformPoint(spline.GetPoint (1)) + Vector3.Cross(spline.GetDirection(1),Vector3.up)*(width/2);
		vertices[resolution*2+1] = transform.InverseTransformPoint(spline.GetPoint (1)) - Vector3.Cross(spline.GetDirection(1),Vector3.up)*(width/2);
		mesh.vertices = vertices;
		mesh.triangles = triangles;
	}

	#if UNITY_EDITOR
	void Update(){
		DrawMesh ();
	}
	#endif

}
