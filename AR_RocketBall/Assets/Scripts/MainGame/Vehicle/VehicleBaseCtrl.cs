using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleBaseCtrl : MonoBehaviour , IVehicleCtrl
{
	protected Vehicle m_vehicle;

	protected VehicleInput m_vehicleInput = new VehicleInput();

	private bool m_lastJumpState = false;

	protected void Awake()
	{
		m_vehicle = GetComponent<Vehicle> ();
		if (m_vehicle == null)
		{
			Debug.LogErrorFormat ("can't get the vehicle obj");
			return;
		}
	}

	#region IVehicleCtrl implementation
	public void SetSteering (Vector2 steer)
	{
		m_vehicleInput.Steering = steer;
	}
	public void SetForwardOn ()
	{
		m_vehicleInput.Forward = 1f;
		m_vehicleInput.Backward = 0f;
	}
	public void SetForwardOff ()
	{
		m_vehicleInput.Forward = 0f;
	}
	public void SetBackwardOn ()
	{
		m_vehicleInput.Backward = -1f;
		m_vehicleInput.Forward = 0f;
	}
	public void SetBackwardOff ()
	{
		m_vehicleInput.Backward = 0f;
	}
	public void SetJumpOn ()
	{
		if (!m_lastJumpState)
		{
			m_vehicleInput.IsJump = true;
			m_lastJumpState = true;
		}
	}
	public void SetJumpOff ()
	{
		m_vehicleInput.IsJump = false;
		m_lastJumpState = false;
	}
	public void SetBoostOn ()
	{
		m_vehicleInput.IsBoost = true;
	}
	public void SetBoostOff ()
	{
		m_vehicleInput.IsBoost = false;
	}
    #endregion

    protected virtual void FixedUpdate()
	{
		m_vehicle.Drive (m_vehicleInput);
		m_vehicleInput.IsJump = false;
	}
}
