using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour 
{
	private IVehicleCtrl m_ctrl;
	private Vector2 m_steer;

	public void SetVehicle( IVehicleCtrl vehicle )
	{
		m_ctrl = vehicle;
	}

	public void OnEnterRight()
	{
		m_steer.x = 1;
		m_ctrl.SetSteering (m_steer);
	}

	public void OnLeaveRight()
	{
		m_steer.x = 0;
		m_ctrl.SetSteering (m_steer);
	}

	public void OnEnterLeft()
	{
		m_steer.x = -1;
		m_ctrl.SetSteering (m_steer);
	}

	public void OnLeaveLeft()
	{
		m_steer.x = 0;
		m_ctrl.SetSteering (m_steer);
	}

	public void OnEnterGas()
	{
		m_ctrl.SetForwardOn ();
	}

	public void OnLeaveGas()
	{
		m_ctrl.SetForwardOff ();
	}

	public void OnEnterBack()
	{
		m_ctrl.SetBackwardOn ();
	}

	public void OnLeaveBack()
	{
		m_ctrl.SetBackwardOff ();
	}
}


