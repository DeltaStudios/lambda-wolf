using UnityEngine;
using System.Collections;


[System.Serializable]
public class BandAid : ICollectable, IConsumable, IDetectable {

	CollectableWrapper wrapperPrefab;

	public BandAid() {
		wrapperPrefab = Resources.Load("Collectable") as CollectableWrapper;
	}

	public GameObject itemPrefab;
	[SerializeField] private GameObject item;
	public int healQuantity = 20;

	public void OnTake (Collector c) {
		
	}

	public void OnDrop (Collector c) {
		CollectableWrapper wrapper = new GameObject().AddComponent<CollectableWrapper>();
		wrapper.SetCollectable(this);
		wrapper.OnDrop(c);
	}

	public void OnConsume (Consumer c) {
		c.GetComponent<Stats>().updateHealth(healQuantity);
	}

	public void OnDetect (Detector d) {

	}

}
