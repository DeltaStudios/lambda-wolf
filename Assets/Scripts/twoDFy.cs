using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class twoDFy : MonoBehaviour {

	[SerializeField] Transform counterpart;
	[SerializeField] BezierSpline ground;
	// Use this for initialization
	void Start () {
		transform.position = ground.GetPoint(counterpart.position.x/100+0.5f)+Vector3.up*counterpart.position.y;
		transform.rotation = Quaternion.LookRotation(ground.GetDirection(counterpart.position.x / 100 + 0.5f));
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = ground.GetPoint(counterpart.position.x/100+0.5f)+Vector3.up*counterpart.position.y;
		transform.rotation = Quaternion.LookRotation(Vector3.Cross(ground.GetDirection(counterpart.position.x / 100 + 0.5f), Vector3.up))*counterpart.rotation;
	}
}
