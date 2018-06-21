using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehiclePlayerCtrl : VehicleBaseCtrl 
{
	void Update()
	{
		#if UNITY_EDITOR
		if( Input.GetKey(KeyCode.K))
		{
			m_vehicleInput.Forward = 1f;
			m_vehicleInput.Backward = 0f;
		}else if( Input.GetKey(KeyCode.M) )
		{
			m_vehicleInput.Backward = -1f;
			m_vehicleInput.Forward = 0f;
		}else
		{
			m_vehicleInput.Forward = 0f;
			m_vehicleInput.Backward = 0f;
		}

		if( Input.GetKey(KeyCode.A))
		{
			m_vehicleInput.Steering.x = -1f;
		}else if( Input.GetKey(KeyCode.D))
		{
			m_vehicleInput.Steering.x = 1f;
		}else
		{
			m_vehicleInput.Steering.x = 0f;
		}

		if( Input.GetKey(KeyCode.W))
		{
			m_vehicleInput.Steering.y = 1f;
		}else if( Input.GetKey(KeyCode.S))
		{
			m_vehicleInput.Steering.y = -1f;
		}else
		{
			m_vehicleInput.Steering.y = 0f;
		}

		if( Input.GetKeyDown(KeyCode.L))
		{
			m_vehicleInput.IsJump = true;
		}else
		{
			m_vehicleInput.IsJump = false;
		}

		if( Input.GetKey(KeyCode.I))
		{
			m_vehicleInput.IsBoost = true;
		}else
		{
			m_vehicleInput.IsBoost = false;
		}
		#endif
	}
}
