using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Collector : MonoBehaviour {

	public List<ICollectable> inventory = new List<ICollectable>();

	public void Collect (ICollectable item) {
		item.OnTake(this);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
