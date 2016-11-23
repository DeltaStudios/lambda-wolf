using UnityEngine;
using System.Collections;

public class Bear_Patrol : MonoBehaviour {

	private Rigidbody2D myRigid; 
	private RaycastHit2D eyeLevel, feet; 
	public AnimationCurve SpeedFunction; 
	public float Speed; 
	private float t, force; 

	public delegate void PatrolEvents (); 
	public static event PatrolEvents FoundPlayer; 


	void Start () {
		myRigid = this.gameObject.GetComponent<Rigidbody2D> (); 
		StartCoroutine (CheckForObstacles ()); 
	}

	IEnumerator CheckForObstacles(){
		//check if there is an obstacle infront of you (a gap or a wall)
		eyeLevel = Physics2D.Raycast(transform.position, transform.right, 3f); 
		feet = Physics2D.Raycast (transform.position, new Vector2(transform.right.x, -0.4f).normalized, 3f); 
		Debug.DrawLine (transform.position, transform.position + (3 * transform.right), Color.red); 
		Debug.DrawLine (transform.position, transform.position + (3 * new Vector3(transform.right.x, -0.4f).normalized), Color.red); 
		if (feet.collider == null) {
			StartCoroutine (TurnAround ()); 
		} else if(feet.collider!=null&&eyeLevel.collider!=null){
			if(feet.collider.tag!="Player" && eyeLevel.collider.tag != "Player"){
				StartCoroutine (TurnAround());
			}
		}else{
			StartCoroutine (Move ()); 
		}
		yield return null; 

	}
	IEnumerator TurnAround(){
		//stop all movemnet. 
		myRigid.velocity = Vector2.zero; 
		t = 0; 
		//turn 180 degreees. 
		for (int n = 0; n < 18; n++) {
			transform.Rotate (Vector2.up, 10f); 
			yield return new WaitForSeconds (2*Time.fixedDeltaTime);
		}
		yield return StartCoroutine(CheckForObstacles()); 
	}
	IEnumerator Move(){
		if (t >= 1) {
			t = 1; 
		} else {
			t += Time.fixedDeltaTime; 
		}
		//Debug.Log (t);
		//calculate force
		force = (Mathf.Pow((SpeedFunction.Evaluate(t)*Speed),2)/2f); 
		//apply force 
		if (myRigid.velocity.magnitude < Speed / 2) {
			myRigid.AddForce (transform.right * force); 
		}
		yield return new WaitForSeconds (0.25f); 
		yield return StartCoroutine (CheckForObstacles());
	}

	public void patrol_stop(){
		myRigid.velocity = Vector2.zero; 
		t = 0; 
		StopAllCoroutines (); 
	}
	public void patrol_start(){
		StartCoroutine(CheckForObstacles()); 
	}

	void OnTriggerEnter2D(Collider2D something){
		if (something.tag == "Player") {
			transform.GetComponent<CircleCollider2D> ().enabled = false; 
			patrol_stop (); 
			FoundPlayer (); 
			Debug.Log ("Found Player"); 
		}
	}
	

}
