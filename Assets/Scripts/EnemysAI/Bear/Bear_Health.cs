using UnityEngine;
using System.Collections;

public class Bear_Health : MonoBehaviour {


	private float _health; 
	public float Health{ get { return _health; } set { _health = value; } }

	void Start(){
		//defaul health. 
		_health = 100f; 
	}

	IEnumerator Die(){
		while (true) {
			if (_health <= 0) {
				break; 
			}
			yield return new WaitForSeconds (1f); 
		}
		//run death animation. 
		Debug.Log("bear died"); 
		yield return null; 
	}

}
