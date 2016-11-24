using UnityEngine;
using System.Collections;

public class TriggerArea : MonoBehaviour {

	public GameObject observer;
	IObserver observerObj;
	// Use this for initialization
	void Start () {
		observerObj = observer.GetComponents (typeof(IObserver)) [0] as IObserver;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D col) {
		observerObj.OnTrigger ();
	}
}
