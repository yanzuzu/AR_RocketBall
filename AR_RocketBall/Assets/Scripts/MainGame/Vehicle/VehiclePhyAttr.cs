using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VehiclePhyAttr
{
	public Vector3 CentreOfMassOffset = Vector3.zero;
	[Range(0, 1)] 
	public float SteerHelper = 1; // 0 is raw physics , 1 the car will grip in the direction it is facing
	[Range(0, 1)] 
	public float TractionControl = 1; // 0 is no traction control, 1 is full interference
	public float FullTorqueOverAllWheels = 800f;
	public float Downforce = 100f;
	public float Topspeed = 7f;
	public float BoostTopspeed = 12f;
	public int NoOfGears = 5;
	public float RevRangeBoundary = 1f;
	public float SlipLimit = 1f;
	public float BrakeTorque = float.MaxValue;

	[Header("Revert")]
	public float RevertUpForce = 1.6f;
	public float RevertRotateForce = 12f;

	public float LowSpeedAngle = 30;
	public float HighSpeedAngle = 18;

	public float JumpForce = 3.0f;
	public float SecondJumpForce = 1.8f;

	[Header("DoubleJump")]
	public float DoubleJumpMain = 27.1f;
	public float DoubleJumpMain_impulse = 10.0f;
	public float DoubleJumpSide = 25.8f;
	public float DoubleJumpSide_impulse = 3.6f;
	public float BicycleDiffTime = 0.8f;

	[Header("AirMove")]
	public float AirMoveY_force = 0.25f;
	public float AirMoveX_force = 0.25f;

	[Header("Boost")]
	public float BoostForceGround = 7f;
	public float BoostForceAir = 7f;

	public float MaxAngularVelocity = 40f;

	public float NormalDrag = 0.4f;
	public float NoMoveDrag = 4f;

	public float BrakeTorqueForce = 10f;
}
