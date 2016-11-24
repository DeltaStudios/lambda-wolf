using UnityEngine;
using System.Collections;

public class Bear_Combat : MonoBehaviour {

	private Stats stats;
	private Rigidbody2D myRigid; 
	private Transform player; 
	private RaycastHit2D front, back; 
	private float DistanceFromPlayer = 0, t, force; 
	public AnimationCurve SpeedFunction; 
	public float Speed; 

	public Animator animator;


	void Start () {
		stats = this.gameObject.GetComponent<Stats> (); 
		myRigid = this.gameObject.GetComponent<Rigidbody2D> (); 
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Transform>(); 
		Bear_Patrol.FoundPlayer += combat_start; 
	}

	IEnumerator CheckPlayerDirection(){
		//Debug.Log ((player.position.x - transform.position.x) + ", " + transform.right.x); 
		if (((player.position.x - transform.position.x) < 0f && transform.right.x >0f)||((player.position.x - transform.position.x) > 0f && transform.right.x <0f)){
			StartCoroutine (TurnAround ());
		} else{
			StartCoroutine (CheckDistanceFromPlayer ()); 
		} 
		yield return null; 
	}
	IEnumerator TurnAround(){
		t = 0; 
		myRigid.velocity = Vector2.zero; 
		for (int n = 0; n < 18; n++) {
			transform.Rotate (Vector2.up, 10f); 
			yield return new WaitForSeconds (2*Time.fixedDeltaTime);
		}
		//Debug.Log (transform.right); 
		yield return StartCoroutine(CheckPlayerDirection()); 
	}
	IEnumerator CheckDistanceFromPlayer(){
		DistanceFromPlayer = Mathf.Abs(player.position.x - transform.position.x); 
		//Debug.Log (DistanceFromPlayer);
		//Fix later consider width of 2 entities
		if (DistanceFromPlayer < 3f) {
			StartCoroutine (Attack ()); 
		} else {
			StartCoroutine (GetClose ()); 
		}
		yield return null; 
	}
	IEnumerator GetClose(){
		if (t >= 1) {
			t = 1; 
		} else {
			t += 2*Time.fixedDeltaTime; 
		}
		//Debug.Log (t); 
		//calculate force
		animator.SetFloat("speed",Speed);
		force = (Mathf.Pow((SpeedFunction.Evaluate(t)*Speed),2)/2f); 
		//apply force 
		if (myRigid.velocity.magnitude < Speed / 2) {
			myRigid.AddForce (transform.right * force); 
		}
		yield return new WaitForSeconds (0.25f); 
		yield return StartCoroutine (CheckPlayerDirection());
	}
	IEnumerator Attack(){
		animator.SetTrigger("attack");
		myRigid.velocity = Vector2.zero;
		//Do some damage to player.
		Debug.Log("attacking");
		player.GetComponent<Stats>().updateHealth(-stats.attack);
		yield return new WaitForSeconds (1f);
		yield return StartCoroutine(CheckPlayerDirection()); 
	}

	public void combat_stop(){
		myRigid.velocity = Vector2.zero; 
		StopAllCoroutines (); 
	}
	public void combat_start(){
		StartCoroutine (CheckPlayerDirection ()); 
	}

}
