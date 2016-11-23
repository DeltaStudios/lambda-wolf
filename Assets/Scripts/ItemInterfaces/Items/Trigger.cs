using UnityEngine;
using System.Collections;

public class Trigger : MonoBehaviour, IInteractable {
	IObserver observer;

	public void Interact () {
		observer.OnTrigger();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
