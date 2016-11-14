using UnityEngine;
using System.Collections;

public class Combat : MonoBehaviour {

	public delegate void CombatEvents(); 
	public static event CombatEvents foundPlayer, lostPlayer; 
	public Transform player; 

	// Use this for initialization
	void Awake () {
		if (player == null) {
			player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Transform> (); 
		}
		StartCoroutine (CheckPlayerProximity ()); 
	}

	IEnumerator CheckPlayerProximity(){
		while (player!=null) {
			if (Vector3.Distance (player.position, transform.position) <= 7f) {
				foundPlayer (); 
			} else if(Vector3.Distance (player.position, transform.position) >= 10f){
				lostPlayer (); 
			}
			yield return new WaitForSeconds(1f);
		}
	}

}
