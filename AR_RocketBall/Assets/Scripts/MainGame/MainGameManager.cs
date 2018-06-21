using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class MainGameManager : MonoBehaviour 
{
	[SerializeField]
	private MainController m_controller;

	public const float CreateHeight = 0.3f;
	private const string VEHICLE_RES_PATH = "Vehicles/Car_1";

	private Vehicle m_vehicle;

	// Use this for initialization
	void Start () 
	{
		m_controller.gameObject.SetActive (false);
	}

	void CreateCar(Vector3 atPosition)
	{
		GameObject carObj = Instantiate (Resources.Load(VEHICLE_RES_PATH), atPosition, Quaternion.identity) as GameObject;
		m_vehicle = carObj.GetComponent<Vehicle> ();
		VehiclePlayerCtrl playCtrl = carObj.AddComponent<VehiclePlayerCtrl> ();

		m_controller.gameObject.SetActive (true);
		m_controller.SetVehicle (playCtrl);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (m_vehicle != null)
		{
			return;
		}

		if (Input.touchCount <= 0)
		{
			return;
		}

		var touch = Input.GetTouch(0);
		if (touch.phase == TouchPhase.Began)
		{
			var screenPosition = Camera.main.ScreenToViewportPoint(touch.position);
			ARPoint point = new ARPoint {
				x = screenPosition.x,
				y = screenPosition.y
			};

			List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface ().HitTest (point, 
				ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent);
			if (hitResults.Count > 0) 
			{
				foreach (ARHitTestResult hitResult in hitResults) 
				{
					Vector3 position = UnityARMatrixOps.GetPosition (hitResult.worldTransform);
					CreateCar (new Vector3 (position.x, position.y + CreateHeight, position.z));
					break;
				}
			}

		}

	}
}
