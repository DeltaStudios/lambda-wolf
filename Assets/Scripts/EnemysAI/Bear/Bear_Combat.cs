using UnityEngine;
using System.Collections;

public class Bear_Combat : MonoBehaviour, IDamagable {

	private Bear_Health myHealth;
	private Rigidbody2D myRigid; 
	private Transform player; 
	private RaycastHit2D front, back; 
	private Vector2 direction; 
	private float DistanceFromPlayer = 0, t, force; 
	public AnimationCurve SpeedFunction; 
	public float Speed; 



	void Start () {
		myHealth = this.gameObject.GetComponent<Bear_Health> (); 
		myRigid = this.gameObject.GetComponent<Rigidbody2D> (); 
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Transform>(); 
		Bear_Patrol.FoundPlayer += combat_start; 
	}

	IEnumerator CheckPlayerDirection(){
		if ((player.position.x - transform.position.x) < 0) {
			direction = -transform.right; 
			StartCoroutine (TurnAround ()); 
		} else{
			direction = transform.right; 
			StartCoroutine (CheckDistanceFromPlayer ()); 
		} 
		yield return null; 
	}
	IEnumerator TurnAround(){
		for (int n = 0; n < 18; n++) {
			transform.Rotate (Vector2.up, 10f); 
			yield return new WaitForSeconds (2*Time.fixedDeltaTime);
		}
		yield return StartCoroutine(CheckDistanceFromPlayer()); 
	}
	IEnumerator CheckDistanceFromPlayer(){
		DistanceFromPlayer = player.position.x - transform.position.x; 
		if (DistanceFromPlayer < 2f) {
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
			t += Time.fixedDeltaTime; 
		}
		//calculate force
		force = (Mathf.Pow((SpeedFunction.Evaluate(t)*Speed),2)/2f); 
		//apply force 
		if (myRigid.velocity.magnitude < Speed / 2) {
			myRigid.AddForce (direction * force); 
		}
		yield return new WaitForSeconds (0.25f); 
		yield return StartCoroutine (CheckPlayerDirection());
	}
	IEnumerator Attack(){
		//Do some damage to player. 
		Debug.Log("Hitting Player"); 
		yield return StartCoroutine(CheckPlayerDirection()); 
	}

	public void combat_stop(){
		myRigid.velocity = Vector2.zero; 
		StopAllCoroutines (); 
	}
	public void combat_start(){
		StartCoroutine (CheckPlayerDirection ()); 
	}

	public void DamageMe(float Damage, Vector2 force){
		//lower this objects health and apply nock back. 
		myRigid.AddForce(force); 
		myHealth.Health-=Damage; 
	}
	public void DamageMe(Vector2 force){
		//calculate damage. 
		float Damage = force.magnitude; 
		//lower health and apply nockback.  
		myRigid.AddForce(force); 
		myHealth.Health-=Damage; 
	}

}
