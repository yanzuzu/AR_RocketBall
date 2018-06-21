using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleVehicleCamera : MonoBehaviour 
{
	public Transform Target;
	public float distance = 3f;
	public float height = 3f;

	private Camera m_camera;

	public void SetTarget(Transform target)
	{
		Target = target;
	}

	void Awake()
	{
		m_camera = GetComponent<Camera> ();
	}

	void Update()
	{
		if (Target == null)
		{
			return;
		}

		Vector3 pos = m_camera.transform.position;
		pos = Target.position - Vector3.forward * distance;
		pos.y = Target.position.y + height;
		m_camera.transform.position = pos;
		m_camera.transform.LookAt (Target);
	}
}
