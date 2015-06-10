using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour {

	public enum PlayerState {
		Idle, 
		Walking, 
		CarryingIdle, 
		CarryingWalking, 
		Throwing, 
		Taking, 
		BeingHit
	};

	public enum Facing {
		North, 
		East, 
		South, 
		West
	};

	public float speedDamping = 0.87f;
	public KeyCode up = KeyCode.W;
	public KeyCode down = KeyCode.S;
	public KeyCode right = KeyCode.D;
	public KeyCode left = KeyCode.A;
	public KeyCode action = KeyCode.Space;
	public float freeSpeed = 0.015f;
	public float carryingSpeed= 0.008f;
	public float speed;
	public float maxSpeed;
	public float minSpeed;
	public Transform head;
	public float dropStrenght = 3500;
	public float throwStrenght = 10000;
	public Animator anim;

	public PlayerState lastState = PlayerState.Idle;
	public PlayerState state = PlayerState.Idle;
	public Facing lastFacing = Facing.South;
	public Facing facing = Facing.South;

	public Vector2 velocity;
	public bool mayTakeRock = false;
	public bool wantsToTakeRock = false;
	public bool wantsToDropRock = false;
	public bool wantsToThrowRock = false;


	// Use this for initialization
	void Start () {
		speed = freeSpeed;
	}
	
	// Update is called once per frame
	void Update () {
		getInput();
		takeRock();
		dropRock();
		throwRock();
		checkWalking();
		checkFacing();
		// updateAnimation();
	}

	void FixedUpdate() {
		updatePosition();
	}

	void getInput() {
		updateVelocity();
		getAction();
	}

	void updateVelocity() {
		if (state == PlayerState.CarryingIdle
		    || state == PlayerState.CarryingWalking
		    || state == PlayerState.Walking
		    || state == PlayerState.Idle) {
			if (Input.GetKey(up)) {
				velocity.y += speed;
			}
			if (Input.GetKey(down)) {
				velocity.y -= speed;
			}
			if (Input.GetKey(right)) {
				velocity.x += speed;
			}
			if (Input.GetKey(left)) {
				velocity.x -= speed;
			}
		}
		Vector3.ClampMagnitude(velocity, maxSpeed);
		velocity *= speedDamping;
		if (velocity.magnitude < minSpeed) {
			velocity = new Vector3(0f, 0f, 0f);
		}
	}

	void checkWalking() {
		if (velocity.magnitude > 0) {
			if (state == PlayerState.CarryingIdle) {
				setState(PlayerState.CarryingWalking);
			} else {
				setState(PlayerState.Walking);
			}
		} else {
			if (state == PlayerState.Walking) {
				setState(PlayerState.Idle);
			} else if (state == PlayerState.CarryingWalking) {
				setState(PlayerState.CarryingIdle);
			}
		}
	}

	void checkFacing() {
		if (velocity.x != 0 || velocity.y != 0) {
			if (Mathf.Abs(velocity.y) >= Mathf.Abs(velocity.x)){
				if (velocity.y > 0) {
					facing = Facing.North;
				} else {
					facing = Facing.South;
				}
			} else {
				if (velocity.x > 0) {
					facing = Facing.East;
				} else {
					facing = Facing.West;
				}
			}
		}
	}

	void updateAnimation() {
		if (facing != lastFacing || state != lastState) {
			triggerFacing();
			triggerState ();
			lastFacing = facing;
			lastState = state;
		}
	}

	void triggerFacing() {
		if (facing == Facing.North) {
			anim.SetTrigger("North");
		} else if (facing == Facing.East) {
			anim.SetTrigger("East");
		} else if (facing == Facing.South) {
			anim.SetTrigger("South");
		} else {
			anim.SetTrigger("West");
		}
	}

	void triggerState() {
		if (state == PlayerState.Idle) {
			anim.SetTrigger("Idle");
		} else if (state == PlayerState.Walking || state == PlayerState.CarryingWalking) {
			anim.SetTrigger("Walk");
		}
	}

	void getAction() {
		if (Input.GetKeyDown(action)) {
			if (state == PlayerState.Taking 
			    || state == PlayerState.CarryingIdle 
			    || state == PlayerState.CarryingWalking) {
				wantsToDropRock = true;
			} else {
				wantsToTakeRock = true;
			}
		} else if (Input.GetKeyUp(action)) {
				wantsToDropRock = false;
				wantsToTakeRock = false;
		}
		if (state == PlayerState.CarryingIdle || state == PlayerState.CarryingWalking) {
			if (Input.GetMouseButtonDown(0)) {
				wantsToThrowRock = true;
			}
		}
	}

	void takeRock() {
		if (mayTakeRock && wantsToTakeRock && state != PlayerState.Taking) {
			setState(PlayerState.Taking);
			GameObject go = GameObject.FindGameObjectWithTag("Rock");
			Transform rock;
			if (go) {
				rock = go.transform;
				go.SendMessage("rockBeingTaken", true);
				StartCoroutine("takingRockRoutine", rock);
			}
		}
	}

	IEnumerator takingRockRoutine(Transform rock) {
		float moveSpeed = 0.5f;
		rock.GetComponent<Collider2D>().enabled = false;
		Vector3 targetPosition = this.transform.position;
		targetPosition.z = head.transform.localPosition.y;
		setState(PlayerState.Taking);
		while (rock.position != targetPosition) {
			rock.position = Vector3.MoveTowards(rock.position, targetPosition, moveSpeed * Time.deltaTime);
			yield return 0;
		}
		doneTakingRock(rock);
	}

	void doneTakingRock(Transform rock) {
		rock.gameObject.SendMessage("setHolderTransform", this.transform);
		rock.gameObject.SendMessage("rockBeingTaken", false);
		rock.gameObject.SendMessage("rockTaken", true);
		speed = carryingSpeed;
		setState(PlayerState.CarryingIdle);
	}

	void dropRock() {
		if (wantsToDropRock && 
			    (state == PlayerState.Taking
			 	|| state == PlayerState.CarryingIdle 
			 	|| state == PlayerState.CarryingWalking)) {
			StopCoroutine("takingRockRoutine");
			performThrowing(dropStrenght);
		}
	}

	void throwRock() {
		if (wantsToThrowRock) {
			if (state == PlayerState.CarryingIdle
			    || state == PlayerState.CarryingWalking) {
				performThrowing(throwStrenght);
			}
			wantsToThrowRock = false;
		}
	}

	void performThrowing(float force) {
		GameObject go = GameObject.FindGameObjectWithTag("Rock");
		GameObject target = GameObject.FindGameObjectWithTag("Target");
		Transform rock;
		if (go && target) {
			rock = go.transform;
			go.SendMessage("rockBeingTaken", false);
			go.SendMessage("rockTaken", false);
			go.SendMessage("rockBeingThrown", true);
			Vector2 direction;
			Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Rock"), true);
			direction = target.transform.position - this.transform.position;
			go.GetComponent<Rigidbody2D>().AddForce(direction.normalized * force);
			setState(PlayerState.Idle);
			mayTakeRock = false;
		}

	}
	
	void updatePosition() {
		transform.Translate(velocity.x, velocity.y, 0);

	}

	void setState(PlayerState state) {
		if (this.state != state){
			this.state = state;
			if (state == PlayerState.CarryingIdle
			    || state == PlayerState.CarryingWalking) {
				this.speed = carryingSpeed;
			} else if (state == PlayerState.Idle
			           || state == PlayerState.Walking) {
				this.speed = freeSpeed;
			} else if (state == PlayerState.Taking) {
				this.speed = 0f;
				this.velocity = new Vector3(0f, 0f, 0f);
			}
		}
	}

	void OnTriggerStay2D(Collider2D other) {
		if (other.tag == "Rock") { // we may take the rock
			mayTakeRock = true;
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.tag == "Rock") { // we cannot take the rock anymore
			mayTakeRock = false;
		}
	}
}
