using UnityEngine;
using System.Collections;

namespace LambdaWolf{
	[RequireComponent(typeof(WolfControl))]
	public class WolfUserControl : MonoBehaviour {

		private WolfControl wolfControl;
		// Use this for initialization
		void Start () {
			wolfControl = GetComponent<WolfControl> ();
		}
		
		// Update is called once per frame
		void FixedUpdate () {
			GameInput input = new GameInput {
				horizontal = Input.GetAxisRaw ("Horizontal"),
				vertical = Input.GetAxisRaw ("Vertical")
			};

			if (Input.GetButton ("Jump")) {
				input.jump = true;
			}
			if (Input.GetButton ("Special")) {
				input.special = true;
			}

			if (Input.GetButton ("Special2")) {
				input.special2 = true;
			}
			wolfControl.HandleInput(input);

		}
	}
}