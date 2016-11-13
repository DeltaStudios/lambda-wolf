using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {

	public AnimationCurve speedFunc; 
	[SerializeField]
	private float speed = 0; 
	private float force, time = 0, turning_direction_var_control = -1; 
	private RaycastHit hit; 
	private bool Following_Player = false, ImDead = false, turning = false; 
	private Vector3 direction, position2d; 
	private Rigidbody myRigid; 
	private GameObject player; 


	// Use this for initialization
	void Awake () {
		myRigid = gameObject.GetComponent<Rigidbody> ();
		direction = new Vector3 (0, -1f, 2); 
		player = GameObject.FindGameObjectWithTag ("Player");  //remember, if player is null Following_Player will always be false. 
		StartCoroutine (CheckGround()); 
		StartCoroutine (CalculateForce()); 
	}



	IEnumerator CheckGround(){
		//if you are on patrol do a raycast. If there is no ground infront of you, recalculate the direction in which you apply the force used to move, 
		//and wait one second. 
		while (!Following_Player&&!ImDead) {
			Physics.Raycast (transform.position, direction.normalized, out hit, 10f); 
			Debug.DrawLine (transform.position, transform.position + direction.normalized * 10f, Color.red);	
			if (hit.collider == null) {
				myRigid.velocity = Vector3.zero;  
				StartCoroutine (ReCalculateDirection ());
			}
			yield return new WaitForSeconds (0.5f); 
		}
		yield return null; 
	}
		
	IEnumerator CalculateForce(){
		while (!ImDead) {
			//check time. 
			if (time < 1)
				time += Time.fixedDeltaTime;
			else
				time = 1; 
			//evaluate v(t) and calculate the given force. 
			force = (Mathf.Pow ((speedFunc.Evaluate (time) * speed), 2) / 2f); 
			yield return StartCoroutine (ApplyForce ());  
		} 
		yield return null; 
	}

	IEnumerator ReCalculateDirection(){
		if (!Following_Player && !ImDead) {
			//move in the oposite direction you are facing. 
			direction.z *= -1f; 
			turning = true; 
			StartCoroutine (Turn()); 
			time = 0; 
		} else if (!ImDead && player != null) {
			direction = player.transform.position - transform.position; 
		} 
		yield return StartCoroutine(ApplyForce()); 
	}

	IEnumerator ApplyForce(){
		//add a force int he direction you are supposed to move. 
		if (myRigid.velocity.magnitude <= speed/2) {
			myRigid.AddForce (direction * force); 
		}
		yield return null; 
	}

	IEnumerator Turn(){ 
		turning_direction_var_control *= -1; 
		for (int n = 0; n < 18; n++) {
			transform.eulerAngles = new Vector3 (0, transform.eulerAngles.y + 10* turning_direction_var_control, 0); 
			yield return new WaitForSeconds (Time.fixedDeltaTime); 
		}
		yield return null; 
	}




}
