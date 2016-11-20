using UnityEngine;
using System.Collections;
/* This scrip all it does is object pool a single projectile that has his position set to the origin when its not been used. This enemy shoots a raycast2d
 * into the foward direction and all the way down to the down direction, it simulates a scanning motion, there is no need to directly change the 
 * values of the raycast intervals nor the velocity of the scan, it can all be done in the inspector. 
 * 
 * On a side note. all the shoot and load projectle do is to change some values of the projectile so that this can correctly manage itself 
 * on the given context. 
*/

public class AI_Shooter : MonoBehaviour {

	[SerializeField][Range(1,10)]
	public float  scanSpeed = 2; 
	[SerializeField][Range(1,100)]
	private float scanRange = 1; 
	[SerializeField][Range(10,100)]
	private float scanSmoothness = 10; 
	public Transform projectile_instance, barrel; 
	private Projectile_Spirit projectile_script; 
	private Vector2 origin, target, direction, playerDirection; 
	private float scanDirectionControl = 1, angle = 0.5f; 
	private RaycastHit2D hit; 
	private bool imDead = false, alreadyShooting = false; 

	// Use this for initialization
	void Start () {
		//inicialize variables
		origin = new Vector2 (transform.right.x,-0.1f);//0.1f so that it doent try to go back from the other side when slerping.  
		target = transform.right.normalized; 
		projectile_script = projectile_instance.GetComponent<Projectile_Spirit> (); 
		//start the cycle. 
		StartCoroutine (Scan ()); 
		StartCoroutine (Die ()); 
	}
	
	// Update is called once per frame
	IEnumerator Scan() {
		direction = origin; //set the current direction to scan to the transform.right
		while(!imDead){
			hit = Physics2D.Raycast (transform.position, direction, scanRange); 
			Debug.DrawLine (transform.position, new Vector2(transform.position.x, transform.position.y) + direction*scanRange, Color.red);//this is so it can be seen from the inspector. 
			direction = Vector3.Slerp (origin, target, angle/scanSmoothness);//slerp so it does not shrink the ray lenght throught the motion. 

			angle+=scanDirectionControl;// the scan direction control just makes sure that once it reaches the end of the scan it goes back by substracting the 'angles': which is just the % of change whithin the slerp. 
			//if angle/smoohness will make sure to normalize the values given in the inspector so its never higher than one. the % of change whithin the slerp only goes from 0 to 1. 
			if ((angle / scanSmoothness) >= 1 || angle<=0) {
				scanDirectionControl *= -1; 
			}

			if (hit.collider!=null) {
				if (hit.collider.tag == "Player") {
					//if you find the player, get the direction in which to shoot the bullet and if you are not already shooting, shoot. 
					playerDirection = (hit.point - new Vector2 (transform.position.x, transform.position.y)).normalized; 
					if (!alreadyShooting) {//avoid calling the routing if you are already perfoming it by using this bool as a lock state. See shoot coroutine. 
						StartCoroutine (Shoot ()); 
						alreadyShooting = true; 
					}
					yield return new WaitForSeconds (1f);
				} else {
					playerDirection = Vector2.zero; 
				}
			}
			yield return new WaitForSeconds (1f/scanSpeed);
		}
		yield return null; 
	}
	IEnumerator Shoot(){
		//unleash the bullet
		yield return StartCoroutine (LoadProjectile ());//prepare the bullet to me shoot. Tip: You might want to increase the second to wait in this coroutine if you want some kind of animation to happen here. 
		projectile_script.Direction = playerDirection;  
		projectile_script.RestartBool = true; 
		alreadyShooting = false; //free the lock state. 
		yield return new WaitForSeconds (projectile_script.lifeTime);  
	}
	IEnumerator LoadProjectile(){
		//prepare the bullet. See the projectile_spirit script. 
		projectile_instance.position = barrel.position; 
		projectile_script.Direction = Vector2.zero; 
		projectile_script.RestartBool = false; 
		projectile_script.ResetTime = 0; 
		projectile_script.Trail = true; 
		yield return null;   
	}

	IEnumerator Die(){//this is the master control, if this enemy dies, everything stops. 
		while(true){
			if(imDead){
				gameObject.SetActive (false); //instead of this play some dead animation. 
				StopAllCoroutines (); 
			}
			yield return new WaitForSeconds (1f);
		}
	}


}
