using UnityEngine;
using System.Collections;

public class AI_Shooter : MonoBehaviour {

	[SerializeField][Range(1,10)]
	public float  scanSpeed = 2; 
	[SerializeField][Range(1,100)]
	private float scanRange = 1; 
	[SerializeField][Range(10,100)]
	private float scanSmoothness = 10; 

	private Vector3 origin, target, direction; 
	private float scanDirectionControl = 1, angle = 0.5f; 
	private RaycastHit hit; 

	// Use this for initialization
	void Start () {
		origin = new Vector3 (0,-1,0.1f); 
		target = transform.up.normalized; 
		StartCoroutine (Scan ()); 
	}
	
	// Update is called once per frame
	IEnumerator Scan() {
		direction = origin; 
		Physics.Raycast (transform.position, direction, out hit, scanRange); 
		Debug.DrawLine (transform.position, transform.position + direction*scanRange, Color.red);
		direction = Vector3.Slerp (origin, target, angle/scanSmoothness);
		angle+=scanDirectionControl;
		if ((angle / scanSmoothness) >= 1 || angle<=0) {
			scanDirectionControl *= -1; 
		}
		yield return new WaitForSeconds (1f/scanSpeed);
	}
	IEnumerator Shoot(){
		yield return StartCoroutine (LoadProjectile ()); 
	}
	IEnumerator LoadProjectile(){
		yield return null;  
	}


}
