using UnityEngine;
using System.Collections;

public class Consumer : MonoBehaviour {

	public void Consume(IConsumable item) {
		item.OnConsume(this);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
