using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class twoDFy : MonoBehaviour {

	[SerializeField] Transform counterpart;
	[SerializeField] BezierSpline ground;

	float groundLength;
	// Use this for initialization
	void Start () {
		groundLength = ground.GetLength ();
		transform.position = ground.GetPoint(
			(counterpart.position.x-ground.transform.position.x)/groundLength)
			+Vector3.up*(counterpart.position.y-ground.transform.position.y);
		
		transform.rotation = Quaternion.LookRotation (
			Vector3.Cross (
				ground.GetDirection ((counterpart.position.x - ground.transform.position.x) / groundLength
				),
				Vector3.up)
		) * counterpart.rotation;
	}
	
	// Update is called once per frame
	void Update () {
		if (counterpart == null)
			Destroy (gameObject);
		transform.position = ground.GetPoint(
			(counterpart.position.x-ground.transform.position.x)/groundLength)
			+Vector3.up*(counterpart.position.y-ground.transform.position.y);
		
		transform.rotation = Quaternion.LookRotation (
			Vector3.Cross (
				ground.GetDirection ((counterpart.position.x - ground.transform.position.x) / groundLength
				),
				Vector3.up)
		) * counterpart.rotation;
	}
}

