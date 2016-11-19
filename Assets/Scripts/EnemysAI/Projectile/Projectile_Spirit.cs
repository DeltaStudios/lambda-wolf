using UnityEngine;
using System.Collections;
/* This script all it does is set the projectile in motion once it is given a direction, most likely it will be the player's direction. After a certain
 * amount of seconds (3 by default) the bullet will return to the origin to wait to be used again. If it hits the player it will also return to the origin. 
 * The speed is algo set to 5 by default. All the stats here are managed within the AI_Shooter script. Every enemy shooter has one bullet to object pool with. 
 * All this script is responsible for is to reset it self when the time is right. AI_Shooter will specify the position in which to "spawn" and where to shoot. 
*/

public class Projectile_Spirit : MonoBehaviour {


	private Vector2 direction; 
	private float timeToReset = 0;
	public float speed = 5f, lifeTime = 3f; //the bullet lives 3 second by default. 
	private bool startCounting = false; 
	private TrailRenderer myTrail; 
	public Vector2 Direction{
		set{ direction = value; }
	}
	public float ResetTime{
		set{ timeToReset = value; }
	}
	public bool RestartBool {
		set{ startCounting = value; }
	}	
	public bool Trail {
		set{ myTrail.enabled = value; }
	}


	// Use this for initialization
	void Start () {
		myTrail = gameObject.GetComponent<TrailRenderer> (); 
		transform.position = Vector2.zero; 
	}

	void FixedUpdate(){
		transform.Translate (direction.normalized * speed * Time.fixedDeltaTime); 
		if (startCounting) {
			timeToReset += Time.fixedDeltaTime; 
		}
		if (timeToReset >= 3f) {
			transform.position = Vector2.zero; 
			direction = Vector2.zero; 
			timeToReset = 0; 
			startCounting = false; 
		}
	}

	void OnTriggerEnter2D(Collider2D something){
		if (something.tag == "Player") {
			Trail = false; 
			transform.position = Vector2.zero;//or whatever the reset position is. 
			direction = Vector2.zero; 
			//damage the player
		} 
	}
	

}
