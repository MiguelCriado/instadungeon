using UnityEngine;

public class ActorController : MonoBehaviour
{
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

	void Start ()
	{
		speed = freeSpeed;
	}
	

	void Update ()
	{
		GetInput();
	}

	void FixedUpdate()
	{
		UpdatePosition();
	}

	void GetInput()
	{
		UpdateVelocity();
	}

	void UpdateVelocity()
	{
		if (Input.GetKey(up))
		{
            if (useZVector)
            {
                velocity.z += speed;
            }
            else
            {
                velocity.y += speed;
            }
		}

		if (Input.GetKey(down))
		{
            if (useZVector)
            {
                velocity.z -= speed;
            }
            else
            {
                velocity.y -= speed;
            }
		}

		if (Input.GetKey(right))
		{
			velocity.x += speed;
		}

		if (Input.GetKey(left))
		{
			velocity.x -= speed;
		}

		Vector3.ClampMagnitude(velocity, maxSpeed);
		velocity *= speedDamping;

		if (velocity.magnitude < minSpeed)
		{
			velocity = new Vector3(0f, 0f, 0f);
		}
	}
	
	void UpdatePosition()
	{
		transform.Translate(velocity.x, velocity.y, velocity.z);
	}
}
