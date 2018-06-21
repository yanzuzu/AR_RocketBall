using UnityEngine;

public interface IVehicleCtrl 
{
	void SetSteering (Vector2 steer);

	void SetForwardOn();
	void SetForwardOff();

	void SetBackwardOn();
	void SetBackwardOff();

	void SetJumpOn();
	void SetJumpOff();

	void SetBoostOn();
	void SetBoostOff();
}
