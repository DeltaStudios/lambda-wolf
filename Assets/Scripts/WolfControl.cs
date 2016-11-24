using UnityEngine;
using System.Collections;

namespace LambdaWolf{
	
	abstract class WolfState{
		protected GameObject gameObject;

		protected virtual void Enter (){}
		protected virtual void Exit (){}

		public WolfState(GameObject gameObject){
			this.gameObject = gameObject;
		}
		public WolfState Transition(WolfState target){
			this.Exit ();
			target.Enter ();
			return target;
		}

		public virtual WolfState HandleInput (GameInput input){
			return this;
		}
		public virtual WolfState Update(){
			return this;
		}

		public virtual WolfState OnCollisionEnter2D(Collision2D col){
			return this;
		}
	}

	class Grounded : WolfState{
		private WolfControl wolfControl;
		private Rigidbody2D rigidbody;

		public Grounded(GameObject gameObject): base(gameObject){
			this.rigidbody = this.gameObject.GetComponent<Rigidbody2D> ();
			this.wolfControl = this.gameObject.GetComponent<WolfControl> ();
		}
			
		public override WolfState Update ()
		{
			
			if (!wolfControl.IsGrounded ()) {
				return Transition(new Airborne(this.gameObject));
			}

			return this;
		}

		public override WolfState HandleInput (GameInput input)
		{
			if (input.horizontal != 0) { // Moving
				float speed = wolfControl.walkingSpeed;

				// Run
				if (input.special2)
					speed = wolfControl.runningSpeed;

				//Start with an impulse
				if (Mathf.Abs(this.rigidbody.velocity.x) < speed/2) {
					this.rigidbody.velocity = new Vector2 (
						speed/2*Mathf.Ceil(input.horizontal),
						this.rigidbody.velocity.y);
				}

				// Then accelerate to maxSpeed using constant force
				if (Mathf.Abs (this.rigidbody.velocity.x) > speed) {
					this.rigidbody.velocity = new Vector2 (
						speed * Mathf.Ceil (input.horizontal),
						this.rigidbody.velocity.y);
				} else {
					this.rigidbody.AddForce (new Vector2 (
						50 * input.horizontal,
						0));
				}
			} else { // Not Moving

				// Stopping Force on character
				/*
				if (this.rigidbody.velocity.sqrMagnitude > 0.01) {
					this.rigidbody.velocity *= 0.8f;
				} else {
					this.rigidbody.velocity = Vector2.zero;
				}
				*/

				// Dig or Duck
				if (input.vertical < 0 && input.special)
					return Transition (new Digging (this.gameObject));
				else if (input.vertical < 0)
					return Transition (new Ducking (this.gameObject));
			}

			if (input.jump)
				return this.Transition(new Jumping(this.gameObject));

			return this;
		}
	}

	class Jumping : WolfState{
		private WolfControl wolfControl;
		private Rigidbody2D rigidbody;

		private float charge = 0f;
		private float deltaCharge = 0f;
		private float delta2Charge;
		private int direction = 0;

		public Jumping(GameObject gameObject): base(gameObject){
			this.rigidbody = this.gameObject.GetComponent<Rigidbody2D> ();
			this.wolfControl = this.gameObject.GetComponent<WolfControl> ();

			this.delta2Charge = 2 / wolfControl.jumpChargeTime/wolfControl.jumpChargeTime;
		}

		private void Jump(int direction){
			float velocityToReachHeight = Mathf.Sqrt (-2 * wolfControl.jumpHeight * Physics2D.gravity.y);

			// Full charge jump will push upwards, no charge jump will push forwards
			this.rigidbody.velocity += new Vector2(
				Mathf.Cos(charge/4*2*Mathf.PI)*direction,
				Mathf.Sin(charge/4*2*Mathf.PI)
			)*velocityToReachHeight;

		}

		public override WolfState HandleInput(GameInput input){
			if (input.horizontal > 0)
				this.direction = 1;
			else if (input.horizontal < 0)
				this.direction = -1;
			else
				this.direction = 0;

			if (!input.jump) {
				
				Jump (this.direction);
				return this.Transition (new Airborne (this.gameObject));
			}
			return this;
		}

		public override WolfState Update ()
		{
			if (!wolfControl.IsGrounded()) {
				return Transition (new Airborne (this.gameObject));
			}
			this.deltaCharge += this.delta2Charge * Time.deltaTime;
			this.charge += this.deltaCharge*Time.deltaTime;

			/*
			// Stopping Force on character
			if (this.rigidbody.velocity.sqrMagnitude > 0.01) {
				this.rigidbody.velocity -= new Vector2(5f,0)*Time.deltaTime;
			} else {
				this.rigidbody.velocity = Vector2.zero;
			}
			*/

			if (charge > 1f)
				charge = 1f;

			return this;
		}

	}
	class Airborne : WolfState{
		Rigidbody2D rigidbody;
		WolfControl wolfControl;
		public Airborne(GameObject gameObject): base(gameObject){
			this.rigidbody = gameObject.GetComponent<Rigidbody2D> ();
			this.wolfControl = gameObject.GetComponent < WolfControl> ();
		}

		public override WolfState Update ()
		{
			if(wolfControl.IsGrounded())
				return Transition(new Grounded (this.gameObject));
			return this;
		}

		public override WolfState HandleInput (GameInput input)
		{
			this.rigidbody.AddForce (new Vector2 (
				10*input.horizontal,
				0));
			
			return this;
		}
	}

	class Digging : WolfState{
		private WolfControl wolfControl;
		private float charge = 0;

		public Digging(GameObject gameObject): base(gameObject){
			this.wolfControl = gameObject.GetComponent<WolfControl>();
		}

		public override WolfState HandleInput(GameInput input){
			if (!input.special && input.horizontal == 0 && input.vertical < 0)
				return Transition (new Ducking (this.gameObject));
			return this;
		}

		public override WolfState Update ()
		{
			charge += Time.deltaTime;
			if (charge > wolfControl.diggingTime) {
				Debug.Log("Dug");
				var col = Physics2D.OverlapCircle(wolfControl.transform.position + Vector3.down * 0.5f, 1, wolfControl.itemMask);
				Debug.DrawLine(wolfControl.transform.position + Vector3.down * 0.5f, wolfControl.transform.position + Vector3.down * 1.5f);
				if(col != null && col.GetComponents(typeof(ICollectable)) != null) {
					
					ICollectable item = col.GetComponents(typeof(ICollectable)) [0] as ICollectable;
					wolfControl.GetComponent<Collector>().Collect(item);
				}
				return Transition (new Grounded(this.gameObject));
			}
			return this;
		}
	}

	class Ducking : WolfState{
		private WolfControl wolfControl;

		public Ducking(GameObject gameObject): base(gameObject){
			this.wolfControl = gameObject.GetComponent<WolfControl>();
		}

		public override WolfState HandleInput(GameInput input){
			if (input.vertical >= 0) {
				return Transition (new Grounded (this.gameObject));
			}
			if (input.special)
				return Transition (new Digging (this.gameObject));
			return this;
		}
	}


	[RequireComponent(typeof(Rigidbody2D))]
	public class WolfControl : MonoBehaviour  {
		
		public float walkingSpeed;
		public float runningSpeed;
		public float airborneSpeed;
		public float jumpForce;
		public float jumpChargeTime;
		public float diggingTime;
		public float jumpHeight;

		[SerializeField] private LayerMask whatIsGround;
		[SerializeField] public LayerMask itemMask;
		private WolfState state;

		void Awake(){
			state = new Grounded(gameObject);
		}
		// Use this for initialization
		void Start () {
		}
		
		// Update is called once per frame
		void Update () {
			state = state.Update ();
		}

		public void HandleInput (GameInput input)
		{
			state = state.HandleInput (input);
		}

		void OnCollisionEnter2D(Collision2D col){
			state = state.OnCollisionEnter2D (col);
		}

		public bool IsGrounded(){
			var collision = Physics2D.OverlapArea(
				new Vector2(transform.position.x-1,transform.position.y-transform.localScale.y/2),
				new Vector2(transform.position.x+1,transform.position.y-transform.localScale.y/2-0.1f),
				whatIsGround.value
			);

			return collision != null;
		}

	}
}