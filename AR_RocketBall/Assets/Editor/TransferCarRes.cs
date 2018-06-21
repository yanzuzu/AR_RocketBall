using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TransferCarRes : MonoBehaviour 
{

	[MenuItem("Assets/Transfer Car Assets")]
	private static void TransferCarAsset()
	{
		GameObject carTrans = GameObject.Instantiate(Selection.activeObject) as GameObject;
		Transform [] childs = carTrans.GetComponentsInChildren<Transform> ();

		for (int i = 0; i < childs.Length; i++)
		{
			if (childs [i].name.IndexOf ("Wheel_FL") != -1 || childs [i].name.IndexOf ("Wheel_fl") != -1
				|| childs [i].name.IndexOf ("fl_wheel") != -1)
			{
				CreateWheelCollider ("FL", carTrans.transform, childs [i].transform);
				continue;
			}

			if (childs [i].name.IndexOf ("Wheel_FR") != -1 || childs [i].name.IndexOf ("Wheel_fr") != -1
				|| childs [i].name.IndexOf ("fr_wheel") != -1)
			{
				CreateWheelCollider ("FR", carTrans.transform, childs [i].transform);
				continue;
			}

			if (childs [i].name.IndexOf ("Wheel_RR") != -1 || childs [i].name.IndexOf ("Wheel_rr") != -1
				|| childs [i].name.IndexOf ("rr_wheel") != -1 || childs [i].name.IndexOf ("Wheel_BR") != -1)
			{
				CreateWheelCollider ("RR", carTrans.transform, childs [i].transform);
				continue;
			}

			if (childs [i].name.IndexOf ("Wheel_RL") != -1 || childs [i].name.IndexOf ("Wheel_rl") != -1 
				|| childs [i].name.IndexOf ("rl_wheel") != -1 || childs [i].name.IndexOf ("Wheel_BL") != -1)
			{
				CreateWheelCollider ("RL", carTrans.transform, childs [i].transform);
				continue;
			}

			if (childs [i].name.IndexOf ("Wheel_F") != -1)
			{
				CreateWheelCollider ("FL", carTrans.transform, childs [i].transform);
				continue;
			}

			if (childs [i].name.IndexOf ("Wheel_R") != -1)
			{
				CreateWheelCollider ("RL", carTrans.transform, childs [i].transform);
				continue;
			}
		}

		carTrans.ChangeLayer ("car");
        carTrans.tag = Consts.Tag.CAR;

        carTrans.transform.localScale = Vector3.one * 0.1f;
		BoxCollider boxCollider = carTrans.AddComponent<BoxCollider> ();
		boxCollider.center = new Vector3 (0, 2f, 0);
		boxCollider.size = new Vector3 (5f, 2.5f, 6f);

		Rigidbody rigid = carTrans.AddComponent<Rigidbody> ();
		rigid.mass = 500f;
		rigid.drag = 0.1f;
		rigid.angularDrag = 4f;
		rigid.interpolation = RigidbodyInterpolation.Interpolate;
		rigid.collisionDetectionMode = CollisionDetectionMode.Continuous;

		carTrans.AddComponent<Vehicle> ();

		GameObject topperRoot = new GameObject ();
		topperRoot.transform.SetParent (carTrans.transform);
		topperRoot.name = "TopperRoot";

		GameObject boostRoot = new GameObject ();
		boostRoot.transform.SetParent (carTrans.transform);
		boostRoot.name = "BoostRoot";
	}

	private static void CreateWheelCollider(string name, Transform carTrans, Transform child)
	{
		child.gameObject.name = "Wheel_" + name;
		GameObject obj = new GameObject ();
		obj.name = "WheelCollider_" + name ;
		obj.transform.SetParent (carTrans);
		obj.transform.position = child.position;
		WheelCollider wheelCollider = obj.AddComponent<WheelCollider> ();
		wheelCollider.mass = 90f;
		wheelCollider.radius = 0.5f;
		wheelCollider.wheelDampingRate = 100000;
		wheelCollider.suspensionDistance = 0.3f;
		wheelCollider.forceAppPointDistance = 1f;
		wheelCollider.center = Vector3.zero;

		JointSpring jointSpring = wheelCollider.suspensionSpring;
		jointSpring.spring = 60000f;
		jointSpring.damper = 6000f;
		jointSpring.targetPosition = 0.1f;
		wheelCollider.suspensionSpring = jointSpring;

		WheelFrictionCurve wheelforwardFrictionCurve = wheelCollider.forwardFriction;
		wheelforwardFrictionCurve.extremumSlip = 0.4f;
		wheelforwardFrictionCurve.extremumValue = 1f;
		wheelforwardFrictionCurve.asymptoteSlip = 0.8f;
		wheelforwardFrictionCurve.asymptoteValue = 1f;
		wheelforwardFrictionCurve.stiffness = 1f;
		wheelCollider.forwardFriction = wheelforwardFrictionCurve;

		WheelFrictionCurve wheelSideWayFrictionCurve = wheelCollider.sidewaysFriction;
		wheelSideWayFrictionCurve.extremumSlip = 0.2f;
		wheelSideWayFrictionCurve.extremumValue = 1f;
		wheelSideWayFrictionCurve.asymptoteSlip = 0.5f;
		wheelSideWayFrictionCurve.asymptoteValue = 1f;
		wheelSideWayFrictionCurve.stiffness = 1f;
		wheelCollider.sidewaysFriction = wheelSideWayFrictionCurve;

		child.SetParent (obj.transform);
		child.localPosition = Vector3.zero;
	}

    [MenuItem("Assets/Set Car Boost Root Position")]
    private static void SetCarBoostRootPosition()
    {
        GameObject[] cars = Selection.gameObjects;

        for (int i = 0; i < cars.Length; i++)
        {
            GameObject wheelRL = cars[i].FindChildByPath("WheelCollider_RL");

            GameObject boostRoot = cars[i].FindChildByPath("BoostRoot");
            boostRoot.transform.position = new Vector3(0, wheelRL.transform.position.y, wheelRL.transform.position.z);
        }
    }
}
