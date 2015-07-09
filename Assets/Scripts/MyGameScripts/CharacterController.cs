using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour {

    public bool useZVector;
	public float speedDamping = 0.87f;
	public KeyCode up = KeyCode.W;
	public KeyCode down = KeyCode.S;
	public KeyCode right = KeyCode.D;
	public KeyCode left = KeyCode.A;
	public float freeSpeed = 0.015f;
	public float speed;
	public float maxSpeed;
	public float minSpeed;

	public Vector3 velocity;


	// Use this for initialization
	void Start () {
		speed = freeSpeed;
	}
	
	// Update is called once per frame
	void Update () {
		getInput();
	}

	void FixedUpdate() {
		updatePosition();
	}

	void getInput() {
		updateVelocity();
	}

	void updateVelocity() {
		if (Input.GetKey(up)) {
            if (useZVector)
            {
                velocity.z += speed;
            }
            else
            {
                velocity.y += speed;
            }
		}
		if (Input.GetKey(down)) {
            if (useZVector)
            {
                velocity.z -= speed;
            }
            else
            {
                velocity.y -= speed;
            }
		}
		if (Input.GetKey(right)) {
			velocity.x += speed;
		}
		if (Input.GetKey(left)) {
			velocity.x -= speed;
		}

		Vector3.ClampMagnitude(velocity, maxSpeed);
		velocity *= speedDamping;
		if (velocity.magnitude < minSpeed) {
			velocity = new Vector3(0f, 0f, 0f);
		}
	}
	
	void updatePosition() {
		transform.Translate(velocity.x, velocity.y, velocity.z);
	}

}
