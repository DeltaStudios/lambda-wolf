using UnityEngine;
using System.Collections;

public class CollectableWrapper : MonoBehaviour, ICollectable {
	ICollectable rep;

	public void SetCollectable(ICollectable col) {
		this.rep = col;
	}

	public void OnTake (Collector c) {
		rep.OnTake(c);
		Destroy(gameObject);
	}

	public void OnDrop(Collector c) {

	}
}