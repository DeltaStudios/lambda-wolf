using UnityEngine;
using System.Collections;

public class Elevator : MonoBehaviour, IObserver {
	public float speed = 0.5f;
	public Vector3 target;

	public void OnTrigger() {
		StartCoroutine(Elevate());
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator Elevate () {
		Vector2 pastPosition = new Vector2(transform.position.x, transform.position.y);
		Vector2 pos = new Vector2(transform.position.x, transform.position.y);
		Vector2 target2d = new Vector2(target.x, target.y);

		while((target2d-pos).magnitude > 0.1f) {
			pos += speed * (target2d-pos).normalized * Time.fixedDeltaTime;
			transform.position = new Vector3(pos.x, pos.y, transform.position.z);
			yield return new WaitForFixedUpdate();
		}
		target = pastPosition;
	}
}
