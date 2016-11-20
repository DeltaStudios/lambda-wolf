using UnityEngine;
using System.Collections;
/*this script uses only coroutines, it works as a single big cicle that chains routines together one after another, taking 
	decisions along the way, imagine an ol' school  flow chart. There are only 2 global bool control variables, if it found
	the player t/f or if he is dead t/f. If he is dead, all the cicle shuts down inmediately. 
*/

public class AI_Patroler: MonoBehaviour {
	[SerializeField]
	private AnimationCurve SpeedFunc; 
	[SerializeField]
	private float speed = 0, range = 1; 
	private float time = 0, force = 0, targetAngle = 0, turning_direction_var_control = 1;
	private Vector2 direction, groundCheckRayCastDirection; 
	private bool foundPlayer = false, imDead = false; 
	private RaycastHit2D hit, hit2;
	private Rigidbody2D myRigid; 
	private Transform player; 

	void Start () {
		//inicializing variables. 
		if (transform.right.x < 0) {
			groundCheckRayCastDirection = new Vector2 (-2f, -1f);

		} else {
			groundCheckRayCastDirection = new Vector2 (2f, -1f);
			turning_direction_var_control = -1; 
		}
		direction = Vector2.zero; 
		myRigid = gameObject.GetComponent<Rigidbody2D> (); 
		player = gameObject.GetComponent<Transform> (); 

		//start the cicle
		StartCoroutine(CheckForPlayer()); 
		StartCoroutine (Die ()); 
	}
	
	IEnumerator CheckForPlayer(){
		//check for player infront of me. 
		hit = Physics2D.Raycast (transform.position, transform.right.normalized , 3f); 
		Debug.DrawLine (transform.position, transform.position + (transform.right.normalized * 3f), Color.red); //just so you can see it in the inspector. 
		if (hit.collider != null) {
			if (hit.collider.tag == "Player") {
				foundPlayer = true; 
			}
		} else {
			foundPlayer = false; 
		}
		//if the player if infront of me. 
		if (foundPlayer) {
			StartCoroutine (CheckProximity ()); 
		}
		//if the player is not infornt of me. 
		else {
			StartCoroutine (CheckForGround ()); 
		}
		yield return null; 
	}
	IEnumerator CheckForGround(){
		//check if there is ground ahead. 
		hit2 = Physics2D.Raycast(transform.position, groundCheckRayCastDirection.normalized, 3f); //needs layering. 
		Debug.DrawLine (transform.position, new Vector2(transform.position.x, transform.position.y) + (groundCheckRayCastDirection.normalized * 3f), Color.yellow); //just so you can see it in the inspector. 
	
		//if there is ground 
		if (hit.collider != null && hit2.collider != null) {
			if (hit.collider.tag != "Player" && hit.collider.tag != "Player") {
				StartCoroutine (Turn ()); 
				
			} 
		} else if(hit2.collider!=null){
			if (hit2.collider.tag != "Player") {
				StartCoroutine (CalculateDirection ()); 
			}
		} 
		//if there is no ground
		else {
			StartCoroutine (Turn ()); 
		}
		yield return null; 
	}
	IEnumerator CheckProximity(){
		//check if the player es close enought to hit him. 
		//if he is close enough. 
		if (hit.collider!=null) {
			StartCoroutine (Attack ()); 
		}
		//if he is not
		else{
			StartCoroutine(CalculateDirection()); 
		}
		yield return null;
	}
	IEnumerator CalculateDirection(){
		//calculate in which direction to move depending ont the context. 
		if (foundPlayer) {
			direction = player.position - transform.position; 
			direction.y = 0; 
		} else {
			direction = transform.right.normalized; 
		}
		//if you are calculating the direction it means you are moving so update the time for the speed function. 
		time += Time.fixedDeltaTime; 
		//once you finish
		StartCoroutine(CalculateForce()); 
		yield return null;
	}
	IEnumerator CalculateForce(){
		//calculate how much force to apply, remember to reset the timer on the correct context. 
		force = (Mathf.Pow(SpeedFunc.Evaluate(time) * speed,2f)/2f); 
		//once you finish
		StartCoroutine(ApplyForce()); 
		yield return null; 
	}
	IEnumerator ApplyForce(){
		//apply the correct amount of force. 
		if (myRigid.velocity.magnitude < speed/2) {//this will limit the velocity
			myRigid.AddForce(direction*force); 
		}
		yield return new WaitForSeconds(2*Time.fixedDeltaTime); 
		StartCoroutine (CheckForPlayer ()); 
	}
	IEnumerator Turn(){
		//stop all movement and restart the timer for speed function.
		myRigid.velocity = Vector2.zero; 
		time = 0; 

		turning_direction_var_control *= -1; 
		for (int n = 0; n < 18; n++) {
			transform.eulerAngles = new Vector3 (0, transform.eulerAngles.y + 10* turning_direction_var_control, 0); 
			yield return new WaitForSeconds (Time.fixedDeltaTime); 
		}
		groundCheckRayCastDirection.x *= -1; 
		StartCoroutine (CheckForPlayer ()); 
	}
	IEnumerator Attack(){
		//do some damage to the player. 
		if (player != null && hit.collider != null) {
			if (hit.collider.tag == "Player") {
				Debug.Log ("Hiting");
			}
		}

		//reset move timer. 
		time = 0.1f; 
		yield return new WaitForSeconds(1f);
		StartCoroutine (CheckForPlayer ()); 
	}

	IEnumerator Die(){
		while(true){
			if(imDead){
				gameObject.SetActive (false); //instead of this play some dead animation. 
				StopAllCoroutines (); 
			}
			yield return new WaitForSeconds (1f);
		}
	}


}
