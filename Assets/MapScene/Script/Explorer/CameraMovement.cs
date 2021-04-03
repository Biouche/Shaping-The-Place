using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

	public int m_PlayerNumber = 1;              // Used to identify which tank belongs to which player.  This is set by this tank's manager.
	public float m_Speed = 12f;                 // How fast the tank moves forward and back.
	public float m_TurnSpeed = 180f;            // How fast the tank turns in degrees per second.

	private string m_MovementAxisName;          // The name of the input axis for moving forward and back.
	private string m_TurnAxisName;              // The name of the input axis for turning.
	private string m_TurnUpDownAxisName;              // The name of the input axis for turning.
	private string m_StrafeAxisName;          // The name of the input axis for moving forward and back.
	private string m_FlyAxisName;              // The name of the input axis for turning.

	private Rigidbody m_Rigidbody;              // Reference used to move the tank.



	private float m_MovementInputValue;         // The current value of the movement input.
	private float m_TurnInputValue;             // The current value of the turn input.
	private float m_TurnUpDownInputValue;             // The current value of the turn input.
	private float m_StrafeInputValue;         // The current value of the movement input.
	private float m_FlyInputValue;             // The current value of the turn input.

	// Use this for initialization
	void Start () {
		// The axes names are based on player number.
		m_MovementAxisName = "LeftVertical" +m_PlayerNumber;
		m_TurnAxisName = "RightHorizontal" +m_PlayerNumber;
		m_TurnUpDownAxisName = "UpDownVertical"+m_PlayerNumber;
		m_StrafeAxisName = "LeftHorizontal"+m_PlayerNumber ;
		m_FlyAxisName = "RightVertical"+m_PlayerNumber;
	}

	private void OnEnable ()
	{
		// When the tank is turned on, make sure it's not kinematic.
		m_Rigidbody.isKinematic = false;

		// Also reset the input values.
		m_MovementInputValue = 0f;
		m_TurnInputValue = 0f;
		m_TurnUpDownInputValue = 0f;
		m_StrafeInputValue = 0f;
		m_FlyInputValue = 0f;
	}

	private void OnDisable ()
	{
		// When the tank is turned off, set it to kinematic so it stops moving.
		m_Rigidbody.isKinematic = true;
	}

	// Update is called once per frame
	void Update () {
		// Store the value of both input axes.
		m_MovementInputValue = Input.GetAxis (m_MovementAxisName);
		m_TurnInputValue = Input.GetAxis (m_TurnAxisName);
		m_TurnUpDownInputValue = Input.GetAxis (m_TurnUpDownAxisName);
		m_StrafeInputValue = Input.GetAxis (m_StrafeAxisName);
		m_FlyInputValue = Input.GetAxis (m_FlyAxisName);
	}
		
	private void Awake ()
	{
		m_Rigidbody = GetComponent<Rigidbody> ();
	}

	private void FixedUpdate ()
	{
		// Adjust the rigidbodies position and orientation in FixedUpdate.
		Move ();
		Turn ();
		TurnUpDown ();
		Strafe ();
		Fly ();
	}

	private void Strafe ()
	{
		// Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.
		Vector3 movement = transform.right * m_StrafeInputValue * m_Speed * Time.deltaTime;

		// Apply this movement to the rigidbody's position.
		m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
	}

	private void Fly ()
	{
		// Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.
		Vector3 movement = transform.up * m_FlyInputValue * m_Speed * Time.deltaTime;

		// Apply this movement to the rigidbody's position.
		m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
	}

	private void Move ()
	{
		// Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.
		Vector3 movement = -transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;

		// Apply this movement to the rigidbody's position.
		m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
	}


	private void Turn ()
	{
		// Determine the number of degrees to be turned based on the input, speed and time between frames.
		float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

		// Make this into a rotation in the y axis.
		Quaternion turnRotation = Quaternion.Euler (0f, turn, 0f);

		// Apply this rotation to the rigidbody's rotation.
		m_Rigidbody.MoveRotation (m_Rigidbody.rotation * turnRotation);
	}

	private void TurnUpDown ()
	{
		// Determine the number of degrees to be turned based on the input, speed and time between frames.
		float turn = m_TurnUpDownInputValue * m_TurnSpeed * Time.deltaTime;

		// Make this into a rotation in the y axis.
		Quaternion turnRotation = Quaternion.Euler (turn, 0f, 0f);

		// Apply this rotation to the rigidbody's rotation.
		m_Rigidbody.MoveRotation (m_Rigidbody.rotation * turnRotation);
	}

}
