using UnityEngine;
using System.Collections;	

public class PlayerInput : MonoBehaviour 
{
	public float maxSpeed = 10f;
	public float acceleration = 35f;
	public float jumpSpeed = 8f;
	public float jumpDuration = 150f;

	public bool enableDoubleJump = true;
	public bool wallHitDoubleJumpOverride = true;

	bool _canDoubleJump = true;
	bool _jumpKeyDown = false;
	float _jumpDuration = 0f;
	bool _canVariableJump = false;
	

	Renderer _renderer;
	Rigidbody2D _rb2D;

	void Awake ()
	{
		_renderer = GetComponent<Renderer>();
		_rb2D = GetComponent<Rigidbody2D>();
	}

	void Update ()
	{
		float horizontal = Input.GetAxis("Horizontal");

		if (horizontal < -0.1f)
		{
			if (_rb2D.velocity.x > -maxSpeed)
			{
				_rb2D.AddForce(new Vector2(-acceleration, 0f));
			}
			else
			{
				_rb2D.velocity = new Vector2(-maxSpeed, _rb2D.velocity.y);
			}
		}
		else if (horizontal > 0.1f)
		{
			if (_rb2D.velocity.x < maxSpeed)
			{
				_rb2D.AddForce(new Vector2(acceleration, 0f));
			}
			else
			{
				_rb2D.velocity = new Vector2(maxSpeed, _rb2D.velocity.y);
			}
		}

		bool onTheGround = IsOnGround ();

		float vertical = Input.GetAxis("Vertical");

		if (onTheGround)
		{
			_canDoubleJump = true;
		}

		if (vertical > 0.1f)
		{
			if (_jumpKeyDown == false) //first frame
			{
				_jumpKeyDown = true;

				if (onTheGround || (_canDoubleJump && enableDoubleJump) || wallHitDoubleJumpOverride)
				{
					bool wallHit = false;
					int wallHitDirection = 0;

					bool leftWallHit = IsOnWallLeft ();
					bool rightWallHit = IsOnWallRight ();

					if (horizontal != 0f)
					{
						if (leftWallHit)
						{
							wallHit = true;
							wallHitDirection = 1;
						}
						else if (rightWallHit)
						{
							wallHit = true;
							wallHitDirection = -1;
						}
					}

					if (wallHit == false)
					{
						if (onTheGround || (_canDoubleJump && enableDoubleJump))
						{
							_rb2D.velocity = new Vector2(_rb2D.velocity.x, jumpSpeed);

							_jumpDuration = 0f;
							_canVariableJump = true;
						}
					}
					else
					{
						_rb2D.velocity = new Vector2(jumpSpeed * wallHitDirection, jumpSpeed);
						_jumpDuration = 0f;
						_canVariableJump = true;
					}

					if (onTheGround == false && wallHit == false)
					{
						_canDoubleJump = false;
					}
				}
			}
			else if (_canVariableJump) //second frame 
			{
				_jumpDuration += Time.deltaTime;

				if (_jumpDuration < jumpDuration / 1000)
				{
					_rb2D.velocity = new Vector2(_rb2D.velocity.x, jumpSpeed);
				}
			}
		}
		else
		{
			_jumpKeyDown = false;
			_canVariableJump = false;
		}
	}

	bool IsOnGround ()
	{
		float lengthToSearch = 0.1f;
		float colliderThreshold = 0.001f;

		Vector2 lineStart = new Vector2 (transform.position.x, transform.position.y - _renderer.bounds.extents.y - colliderThreshold);
		Vector2 vectorToSearch = new Vector2(transform.position.x, lineStart.y - lengthToSearch);

		RaycastHit2D hit = Physics2D.Linecast(lineStart, vectorToSearch);

		return hit;
	}

	bool IsOnWallLeft ()
	{
		bool retVal = false;

		float lengthToSearch = 0.1f;
		float colliderThreshold = 0.01f;

		Vector2 lineStart = new Vector2 (transform.position.x - _renderer.bounds.extents.x - colliderThreshold, transform.position.y);
		Vector2 vectorToSearch = new Vector2(lineStart.x - lengthToSearch, transform.position.y);

		RaycastHit2D hitLeft = Physics2D.Linecast(lineStart, vectorToSearch);

		retVal = hitLeft;

		if (retVal)
		{
			if (hitLeft.collider.GetComponent<NoSlideJump>())
			{
				retVal = false;
			}
		}

		return retVal;
	}

	bool IsOnWallRight ()
	{
		bool retVal = false;
		
		float lengthToSearch = 0.1f;
		float colliderThreshold = 0.01f;
		
		Vector2 lineStart = new Vector2 (transform.position.x + _renderer.bounds.extents.x + colliderThreshold, transform.position.y);
		Vector2 vectorToSearch = new Vector2(lineStart.x + lengthToSearch, transform.position.y);
		
		RaycastHit2D hitRight = Physics2D.Linecast(lineStart, vectorToSearch);
		
		retVal = hitRight;
		
		if (retVal)
		{
			if (hitRight.collider.GetComponent<NoSlideJump>())
			{
				retVal = false;
			}
		}
		
		return retVal;
	}
}
