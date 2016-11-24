using UnityEngine;
using System.Collections;

public interface ICollectable {
	void OnTake(Collector c);
	void OnDrop(Collector c);
}
