using UnityEngine;
using System.Collections;

public class Stats : MonoBehaviour {
	public int HP;
	public int totalHP;
	public int attack;
	public int speed;
	public int defense;


	public void updateHealth(int deltaHealth) {
		if((HP + deltaHealth) <= totalHP && (HP + deltaHealth) > 0) {
			HP += deltaHealth;
		}
		else if ((HP + deltaHealth) > totalHP) {
			HP = totalHP;
		}
		else {
			HP = 0;
		}
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}	

}
