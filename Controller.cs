using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Controlls the Boards movement.
/// Forward movement, Turning, and stationary Rotation
/// </summary>
public class Controller : MonoBehaviour {

	public WheelCollider backLeftWheel;
	public WheelCollider backRightWheel;
	public WheelCollider frontLeftWheel;
	public WheelCollider frontRightWheel;

	public List<WheelCollider> wheels;
	//public Joystick moveJoystick;

	//eventually make all these part of a Player Stats Class
	private float maxSteer = 20f;
	private float maxSpeed = 10f;
	private float maxBrake = 100f;
	private float brakeClamp = 1.5f;

	//for steering
	private float steer = 0.0f;
	private float speed = 0.0f;
	private float brake = 0.0f;

	//for stationary rotation
	private float turnAnglePerFixedUpdate = 0.0f;
	private float angleVel = 0.3f;

	//added 5/22/14 for test purposes
	//private float steerAmt = 5f;
	//private float speedAmt = 10f;

	private bool isControlEnabled = true;
	private bool isControlNormal = true;

	// Use this for initialization
	void Start () 
	{
		//make sure the center Of mass is in the middle of the Rigid Body
		rigidbody.centerOfMass = new Vector3(0,0,0);

		wheels.Add(backLeftWheel);
		wheels.Add(backRightWheel);
		wheels.Add(frontLeftWheel);
		wheels.Add(frontRightWheel);

		//initialize controlles
		frontLeftWheel.steerAngle = steer;
		frontRightWheel.steerAngle = steer;
		backLeftWheel.brakeTorque = brake * maxBrake;
		backRightWheel.brakeTorque =  brake * maxBrake;		
		turnAnglePerFixedUpdate = steer * angleVel;
	}

	public void ResetWheelAngles()
	{
		frontLeftWheel.steerAngle = 0;
		frontRightWheel.steerAngle = 0;
		backLeftWheel.steerAngle = 0;
		backRightWheel.steerAngle = 0;
	}

	public void DisableControles()
	{
		isControlEnabled = false;
	}

	public void EnableControles()
	{
		isControlEnabled = true;
	}

	public void setControlsReversed()
	{
		if(isControlNormal)
		{
			isControlNormal = false;
		}
		else
		{
			isControlNormal = true;
		}
	}

	private void ReverseControls()
	{
		backLeftWheel.steerAngle = steer;
		backRightWheel.steerAngle = steer;
		frontLeftWheel.brakeTorque = brake * maxBrake;
		frontRightWheel.brakeTorque =  brake * maxBrake;
		speed = -speed;
		brake = -brake;
		
		turnAnglePerFixedUpdate = steer * angleVel;	
	}

	private void NormalizeControls()
	{
		frontLeftWheel.steerAngle = steer;
		frontRightWheel.steerAngle = steer;
		backLeftWheel.brakeTorque = brake * maxBrake;
		backRightWheel.brakeTorque =  brake * maxBrake;
		
		turnAnglePerFixedUpdate = steer * angleVel;	
	}

	public void MovementInput()
	{	
		/*
		//FOR JOYSTICK INPUT
		steer = moveJoystick.position.x * steerAmt;
		speed = moveJoystick.position.y * speedAmt;
		brake = -1 * Mathf.Clamp(moveJoystick.position.y, -1, 0);
		*/

		
		//FOR KEYBOARDINPUT		
		if(isControlEnabled)
		{
			steer = Input.GetAxis("Horizontal") * maxSteer;			
			speed = Input.GetAxis("Vertical") * maxSpeed;		

			speed = Mathf.Clamp(Input.GetAxis("Vertical"), 0, 1) * maxSpeed;		
			brake = -1 * Mathf.Clamp(Input.GetAxis("Vertical"), -1, 0)* brakeClamp;

		}
		if(isControlNormal)
		{
			NormalizeControls();
		}
		else
		{
			ReverseControls();
		}
	}

	public void HandleMovement()
	{
		//handle stationary rotation
		if(rigidbody.velocity.magnitude <= 1.2)//was .75
		{	
			Quaternion q = Quaternion.AngleAxis(turnAnglePerFixedUpdate, transform.up) * transform.rotation;//for new model
			rigidbody.MoveRotation(q);
		}
		//handle forward movement
		if((rigidbody.velocity.magnitude <= maxSpeed) )// && (!turning))
		{
			rigidbody.AddRelativeForce(Vector3.forward * speed);
		}
	}

	public bool OllieInput()
	{
		return (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.R));//left mouse
	}
}