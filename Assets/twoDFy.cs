using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class twoDFy : MonoBehaviour {

	[SerializeField] Rigidbody2D counterpart;
	[SerializeField] BezierSpline ground;
	// Use this for initialization
	void Start () {
		transform.position = ground.GetPoint(counterpart.transform.position.x/100+0.5f)+Vector3.up*counterpart.transform.position.y;
		transform.rotation = Quaternion.LookRotation(ground.GetDirection(counterpart.transform.position.x / 100 + 0.5f));
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = ground.GetPoint(counterpart.transform.position.x/100+0.5f)+Vector3.up*counterpart.transform.position.y;
		transform.rotation = Quaternion.LookRotation(ground.GetDirection(counterpart.transform.position.x / 100 + 0.5f))*Quaternion.AngleAxis(counterpart.transform.rotation.z,Vector3.Cross(Vector3.up, ground.GetDirection(counterpart.transform.position.x/100+0.5f)));
	}
}
