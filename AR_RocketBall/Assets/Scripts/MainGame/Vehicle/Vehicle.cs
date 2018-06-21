using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class VehicleInput
{
	public Vector2 Steering;
	public float Forward;
	public float Backward;
	public bool IsJump = false;
	public bool IsBoost = false;
}

public class VehiclePhyState
{
	public bool IsJumping = false;
	public bool IsOnGround = false;
	public bool IsBicycle = false;
	public float LastJumpTime = 0;
}

public class Vehicle : MonoBehaviour
{
	[SerializeField]
	private VehiclePhyAttr m_physicAttr = new VehiclePhyAttr();
	[SerializeField]
	private bool m_isUpdateWheelPos = true;

    private VehiclePhyState m_phyState = new VehiclePhyState ();
    public VehiclePhyState PhyState
    {
        get { return m_phyState; }
    }

    private	VehicleInput m_input = new VehicleInput();
    public VehicleInput Input
    {
        get { return m_input; }
    }

    private List<WheelCollider> m_wheelColliders = new List<WheelCollider>();
	private List<GameObject> m_wheelMeshes = new List<GameObject>();

    
    private GameObject m_root;
	public GameObject Root
	{
		get { return m_root; }
	}

	private Transform m_transform;
	public Transform RootTrans
	{
		get { return m_transform; }
	}

	private Rigidbody m_rootRig;
    public Rigidbody RootRig
    {
        get { return m_rootRig; }
    }

    public List<WheelCollider> WheelColliders
	{
		get{ return m_wheelColliders; }
	}

	public float TopSpeed
	{
		get { return m_physicAttr.Topspeed; }
	}
				

	public float CurrentSpeed
	{
		get { return m_velocityMagnitude; }
	}

    public float LowSpeedAngle
    {
        get { return m_physicAttr.LowSpeedAngle; }
    }

    public float HighSpeedAngle
    {
        get { return m_physicAttr.HighSpeedAngle; }
    }

    public bool IsGround
	{
		get
		{
			for (int i = 0; i < m_wheelColliders.Count; i++)
			{
				if (m_wheelColliders [i].isGrounded)
				{
					return true;
				}
			}
			return false;
		}
	}

	public bool IsFullGround
	{
		get
		{
			for (int i = 0; i < m_wheelColliders.Count; i++)
			{
				if (!m_wheelColliders [i].isGrounded)
				{
					return false;
				}
			}
			return true;
		}
	}

	private float m_steerAngle = 0f;
	private int m_gearNum = 0;
	private float m_gearFactor = 0;
	private float m_oldRotation = 0;
	private float m_currentTorque = 0;
	private bool m_isWall = false;
	private bool m_isCollisionGround = false;

	private float m_velocityMagnitude = 0f;
	private float m_angularVelocityMagnitude = 0f;

	public bool UseWallGravity()
	{
		WheelHit hit = new WheelHit();

		m_isWall = true;
		for(int cnt = 0; cnt < m_wheelColliders.Count; cnt++)
		{
			if(!m_wheelColliders[cnt].GetGroundHit(out hit))
				continue;

			if(hit.collider.tag == "Wall")
				return true;
		}
		m_isWall = false;
		return false;
	}
		
	public float CurrentSteerAngle{ get { return m_steerAngle; }}

	private bool m_lastHandBrake = false;

	// Use this for initialization
	protected virtual void Awake()
	{
		m_rootRig = GetComponent<Rigidbody>();
		m_currentTorque = m_physicAttr.FullTorqueOverAllWheels - (m_physicAttr.TractionControl*m_physicAttr.FullTorqueOverAllWheels);

		m_root = gameObject;
		m_transform = transform;

		WheelCollider wheelCollider;
		wheelCollider =  m_root.GetChildComponent<WheelCollider> ("WheelCollider_FR");
		if (wheelCollider != null)
		{
			m_wheelColliders.Add (wheelCollider);
		}
		wheelCollider =  m_root.GetChildComponent<WheelCollider> ("WheelCollider_FL");
		if (wheelCollider != null)
		{
			m_wheelColliders.Add (wheelCollider);
		}
		wheelCollider =  m_root.GetChildComponent<WheelCollider> ("WheelCollider_RR");
		if (wheelCollider != null)
		{
			m_wheelColliders.Add (wheelCollider);
		}
		wheelCollider =  m_root.GetChildComponent<WheelCollider> ("WheelCollider_RL");
		if (wheelCollider != null)
		{
			m_wheelColliders.Add (wheelCollider);
		}

		Transform wheelTrans;
		wheelTrans = m_root.GetChildComponent<Transform> ("Wheel_FR");
		if (wheelTrans != null)
		{
			m_wheelMeshes.Add (wheelTrans.gameObject);
		}
		wheelTrans = m_root.GetChildComponent<Transform> ("Wheel_FL");
		if (wheelTrans != null)
		{
			m_wheelMeshes.Add (wheelTrans.gameObject);
		}
		wheelTrans = m_root.GetChildComponent<Transform> ("Wheel_RR");
		if (wheelTrans != null)
		{
			m_wheelMeshes.Add (wheelTrans.gameObject);
		}
		wheelTrans = m_root.GetChildComponent<Transform> ("Wheel_RL");
		if (wheelTrans != null)
		{
			m_wheelMeshes.Add (wheelTrans.gameObject);
		}
			
		m_rootRig.centerOfMass = m_physicAttr.CentreOfMassOffset;
		m_rootRig.maxAngularVelocity = m_physicAttr.MaxAngularVelocity; 
	}
		
	public void Drive(VehicleInput input)
	{
		m_input = input;
		m_velocityMagnitude = m_rootRig.velocity.magnitude;
		m_angularVelocityMagnitude = m_rootRig.angularVelocity.magnitude;

		bool isOnGround = IsGround;

		if (isOnGround)
		{
			m_isCollisionGround = false;
		}

		if (!m_phyState.IsOnGround)
		{
			if (m_phyState.IsJumping && isOnGround)
			{
				m_phyState.IsJumping = false;
				m_phyState.IsBicycle = false;
			}
		}
		m_phyState.IsOnGround = isOnGround;

		//clamp input values
		float steering = Mathf.Clamp(input.Steering.x, -1, 1);
		float forward = Mathf.Clamp(input.Forward, -1, 1);
		float bacward = Mathf.Clamp(input.Backward, -1, 1);

		float accel = 0f;
		bool isHandBrake = m_input.Backward < 0f && Mathf.Abs (m_input.Steering.x) >= 0.5f;

		if (m_input.IsBoost)
		{
			accel = 1f;
		} else if (isHandBrake)
		{
			accel = 0f;
		} else
		{
			if (forward != 0)
			{
				accel = forward;
			} else if (bacward != 0)
			{
				accel = bacward;
			} else
			{
				accel = 0f;
			}

		}
			
		float speedFactor = CurrentSpeed / TopSpeed;

		m_steerAngle = steering * Mathf.Lerp (m_physicAttr.LowSpeedAngle, m_physicAttr.HighSpeedAngle, speedFactor);

		int frontWheelCount = Mathf.CeilToInt( m_wheelColliders.Count/2.0f );
		for (int i = 0; i < frontWheelCount; i++)
		{
			m_wheelColliders[i].steerAngle = m_steerAngle;
		}
			
		SteerHelper();
		ApplyDrive(accel);
		Boost ();

		if (m_lastHandBrake != isHandBrake)
		{
			HandBreak (isHandBrake);
			m_lastHandBrake = isHandBrake;
		}
			
		CapSpeed();

		AddDownForce();

		TractionControl();

		WallGravity ();

		Jump ();

		AirMove (isOnGround);

		CheckIsStuck ();

		if (IsGround && accel == 0)
		{
			m_rootRig.drag = m_physicAttr.NoMoveDrag;
		} else
		{
			m_rootRig.drag = m_physicAttr.NormalDrag;
		}

		UpdateWheelPos ();

		CheckIsOutSide ();
	}
		
	private void UpdateWheelPos()
	{
		if (!m_isUpdateWheelPos)
		{
			return;
		}

		for (int i = 0; i < m_wheelMeshes.Count; i++)
		{
			Quaternion quat;
			Vector3 position;
			m_wheelColliders[i].GetWorldPose(out position, out quat);
			m_wheelMeshes[i].transform.position = position;
			m_wheelMeshes[i].transform.rotation = quat;
		}
	}

	private void GearChanging()
	{
		float f = Mathf.Abs(CurrentSpeed/TopSpeed);
		float upgearlimit = (1/(float) m_physicAttr.NoOfGears)*(m_gearNum + 1);
		float downgearlimit = (1/(float) m_physicAttr.NoOfGears)*m_gearNum;

		if (m_gearNum > 0 && f < downgearlimit)
		{
			m_gearNum--;
		}

		if (f > upgearlimit && (m_gearNum < (m_physicAttr.NoOfGears - 1)))
		{
			m_gearNum++;
		}
	}
		
	private void CalculateGearFactor()
	{
		float f = (1/(float) m_physicAttr.NoOfGears);
		// gear factor is a normalised representation of the current speed within the current gear's range of speeds.
		// We smooth towards the 'target' gear factor, so that revs don't instantly snap up or down when changing gear.
		var targetGearFactor = Mathf.InverseLerp(f*m_gearNum, f*(m_gearNum + 1), Mathf.Abs(CurrentSpeed/TopSpeed));
		m_gearFactor = Mathf.Lerp(m_gearFactor, targetGearFactor, Time.deltaTime*5f);
	}
		
	private void SetUseGravity(bool usGravity)
	{
		m_rootRig.useGravity = usGravity;
	}

	private void WallGravity()
	{
		UseWallGravity ();

		if (m_isWall) 
		{
			SetUseGravity (false);
			m_rootRig.AddForce ( -transform.up * 3.0f , ForceMode.Acceleration );

		} else {
			SetUseGravity (true);
		}
	}

	private void CapSpeed()
	{
		float topSpeed = m_input.IsBoost ? m_physicAttr.BoostTopspeed : m_physicAttr.Topspeed;
		if (m_velocityMagnitude > topSpeed)
		{
			m_rootRig.velocity = topSpeed * m_rootRig.velocity.normalized;
		}
	}
		
	private void ApplyDrive(float accel)
	{
		float thrustTorque;
		thrustTorque = accel * (m_currentTorque / m_wheelColliders.Count);
		for (int i = 0; i < m_wheelColliders.Count; i++)
		{
			m_wheelColliders[i].motorTorque = thrustTorque;
		}
	}
		
	private void SteerHelper()
	{
		for (int i = 0; i < m_wheelColliders.Count; i++)
		{
			WheelHit wheelhit;
			m_wheelColliders[i].GetGroundHit(out wheelhit);
			if (wheelhit.normal == Vector3.zero)
				return; // wheels arent on the ground so dont realign the rigidbody velocity
		}

		// this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
		if (Mathf.Abs(m_oldRotation - transform.eulerAngles.y) < 10f)
		{
			var turnadjust = (transform.eulerAngles.y - m_oldRotation) * m_physicAttr.SteerHelper;
			Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
			m_rootRig.velocity = velRotation * m_rootRig.velocity;
		}
		m_oldRotation = transform.eulerAngles.y;

		if (m_isWall) {

			float dot = Vector3.Dot (m_rootRig.velocity.normalized, transform.forward);

			if( dot>0)
				m_rootRig.velocity = transform.forward * m_velocityMagnitude;
			else
				m_rootRig.velocity = -transform.forward * m_velocityMagnitude;

			if (m_velocityMagnitude > m_physicAttr.Topspeed) {
				m_rootRig.velocity = m_rootRig.velocity.normalized * m_physicAttr.Topspeed;
			}
		}
	}


	// this is used to add more grip in relation to speed
	private void AddDownForce()
	{
		if ( m_rootRig == null )
			return;
		m_rootRig.AddForce(-transform.up*m_physicAttr.Downforce* m_velocityMagnitude);
	}

	// crude traction control that reduces the power to wheel if the car is wheel spinning too much
	private void TractionControl()
	{
		WheelHit wheelHit;
		for (int i = 0; i < m_wheelColliders.Count; i++)
		{
			m_wheelColliders[i].GetGroundHit(out wheelHit);

			AdjustTorque(wheelHit.forwardSlip);
		}
	}


	private void AdjustTorque(float forwardSlip)
	{
		if (forwardSlip >= m_physicAttr.SlipLimit && m_currentTorque >= 0)
		{
			m_currentTorque -= 10 * m_physicAttr.TractionControl;
		}
		else
		{
			m_currentTorque += 10 * m_physicAttr.TractionControl;
			if (m_currentTorque > m_physicAttr.FullTorqueOverAllWheels)
			{
				m_currentTorque = m_physicAttr.FullTorqueOverAllWheels;
			}
		}
	}
	public void HandBreak( bool isHandBreak)
	{
		int frontWheelCount = Mathf.CeilToInt( m_wheelColliders.Count/2.0f );
		for (int i = frontWheelCount; i < m_wheelColliders.Count; i++)
		{
			if (isHandBreak) {
				WheelFrictionCurve friction = m_wheelColliders [i].sidewaysFriction;
				friction.stiffness = 0.01f;
				m_wheelColliders [i].sidewaysFriction = friction;

				m_wheelColliders [i].brakeTorque = m_physicAttr.BrakeTorque;
			} else {

				WheelFrictionCurve friction = m_wheelColliders [i].sidewaysFriction;
				friction.stiffness = 1f;
				m_wheelColliders [i].sidewaysFriction = friction;

				m_wheelColliders [i].brakeTorque = 0;
			}
		}

		if (isHandBreak)
		{
			float dir = m_input.Steering.x > 0 ? 1 : -1;
			m_rootRig.AddRelativeTorque (dir * Vector3.up * m_physicAttr.BrakeTorqueForce, ForceMode.VelocityChange);
		}
	}
		
	public void StopWheels()
	{
		for(int i=0;i<m_wheelColliders.Count;++i)
		{
			if (null != m_wheelColliders[i])
			{
				m_wheelColliders[i].brakeTorque = float.MaxValue;
				//m_WheelColliders[i].enabled = false;
				if (null != m_wheelColliders[i].attachedRigidbody)
				{
					m_wheelColliders[i].attachedRigidbody.velocity = Vector3.zero;
					m_wheelColliders[i].attachedRigidbody.angularVelocity = Vector3.zero;
					m_wheelColliders[i].attachedRigidbody.isKinematic = true;
					m_wheelColliders[i].transform.localRotation = Quaternion.identity;
					m_wheelColliders[i].attachedRigidbody.Sleep();
				}
			}
		}
	}

	public void ReleaseWheels()
	{
		for(int i=0;i<m_wheelColliders.Count;++i)
		{
			if (null != m_wheelColliders[i])
			{
				m_wheelColliders[i].enabled = true;
				m_wheelColliders[i].brakeTorque = 0f;
				if (null != m_wheelColliders[i].attachedRigidbody)
				{
					m_wheelColliders[i].attachedRigidbody.isKinematic = false;
				}
			}
		}
	}

	private void Jump()
	{
		if (m_phyState.IsJumping)
		{
			// bicycle
			BicycleJump();
			return;
		}

		if (!m_input.IsJump)
		{
			return;
		}

		m_phyState.LastJumpTime = Time.time;
		m_phyState.IsJumping = true;
		m_rootRig.AddForce (transform.up * m_physicAttr.JumpForce, ForceMode.VelocityChange);
		m_rootRig.angularVelocity = Vector3.zero;
	}

	private void BicycleJump()
	{
		if (!m_input.IsJump)
		{
			return;
		}

		if (m_phyState.IsBicycle)
		{
			return;
		}

		float diffTime = Time.time - m_phyState.LastJumpTime;
		if (diffTime >= m_physicAttr.BicycleDiffTime)
		{
			return;
		}
			
		m_phyState.IsBicycle = true;

		m_rootRig.angularVelocity = Vector3.zero;

		Vector3 dir = Vector3.zero;
		if (m_input.Steering.x != 0)
        {
			if (m_input.Steering.x > 0)
			{
				dir = transform.right;
				dir.y = 0.2f;
				dir.Normalize ();
			} else
			{
				dir = -transform.right;
				dir.y = 0.2f;
				dir.Normalize ();
			}


            float delta = m_input.Steering.x > 0 ? 1 : -1;
            m_rootRig.AddRelativeTorque(delta * Vector3.back * m_physicAttr.DoubleJumpSide, ForceMode.VelocityChange);
			m_rootRig.AddForce(dir * m_physicAttr.DoubleJumpSide_impulse, ForceMode.VelocityChange);
        }
        else 
        {
			if (m_input.Steering.y >= 0)
			{
				dir = transform.forward;
				dir.y = 0.2f;
				dir.Normalize ();
			} else
			{
				dir = -transform.forward;
				dir.y = 0.2f;
				dir.Normalize ();
			}

			float delta = m_input.Steering.y >= 0 ? 1 : -1;
			m_rootRig.AddRelativeTorque(delta*Vector3.right * m_physicAttr.DoubleJumpMain, ForceMode.VelocityChange);
			m_rootRig.AddForce(dir * m_physicAttr.DoubleJumpMain_impulse, ForceMode.VelocityChange);
        }

		Vector3 v = m_rootRig.velocity;
		v.y = 0;
		v *= 0.5f;
		m_rootRig.AddForce (v, ForceMode.VelocityChange);
    }

    private int m_curCountryID = -1;
    private GameObject m_curFlag;
	public GameObject CurFlag
	{
		get{ return m_curFlag; }
	}

    public void SetCountryFlag(int countryID)
	{
        if (m_curCountryID == countryID)
        {
            return;
        }

        if (m_curFlag != null)
        {
            GameObject.Destroy(m_curFlag);
        }

        FindTopperRoot();

        if (m_topperRoot == null)
        {
            Debug.LogError("Can't find TopperRoot on " + gameObject.name);
            return;
        }

        m_curCountryID = countryID;

        GameObject flag = Resources.Load<GameObject>("Flags/" + GameUtility.Flags[m_curCountryID]) as GameObject;
        m_curFlag = GameObject.Instantiate(flag);
        m_curFlag.transform.SetParent(m_topperRoot);
        m_curFlag.transform.localPosition = Vector3.zero;
        m_curFlag.transform.localRotation = Quaternion.identity;
        m_curFlag.transform.localScale = Vector3.one;
	}


    private Transform m_topperRoot;
    private void FindTopperRoot()
    {
        if (m_topperRoot == null)
        {
            m_topperRoot = gameObject.FindChild("TopperRoot").transform;
        }
    }


	private void AirMove( bool isOnGround )
	{
		if (isOnGround)
		{
			return;
		}

		if (m_phyState.IsBicycle)
		{
			return;
		}

		if (Mathf.Abs(m_input.Steering.x) > 0.1f)
		{
            float delta = m_input.Steering.x > 0 ? 1 : -1;
            m_rootRig.AddRelativeTorque (delta * Vector3.up * m_physicAttr.AirMoveY_force, ForceMode.VelocityChange);
		}

		if (Mathf.Abs(m_input.Steering.y) > 0.1f)
		{
            float delta = m_input.Steering.y > 0 ? 1 : -1;
            m_rootRig.AddTorque (delta * m_transform.right * m_physicAttr.AirMoveX_force, ForceMode.VelocityChange);
		}
	}

	private void CheckIsStuck()
	{
		if (!m_isCollisionGround)
		{
			return;
		}

		if (m_velocityMagnitude >= 0.1f || m_angularVelocityMagnitude >= 0.1f)
		{
			return;
		}

		m_isCollisionGround = false;
		m_rootRig.velocity = Vector3.zero;

		m_rootRig.AddForce( Vector3.up * m_physicAttr.RevertUpForce, ForceMode.VelocityChange);
		StartCoroutine (RevertBody ());

	}

	IEnumerator RevertBody()
	{
		yield return new WaitForSeconds (0.1f);
		m_rootRig.AddRelativeTorque( Vector3.forward * m_physicAttr.RevertRotateForce, ForceMode.VelocityChange );
	}
		

	private void OnCollisionEnter(Collision collision)
	{
		if (!gameObject.activeSelf)
			return;
		if (collision.gameObject.CompareTag(Consts.Tag.GROUND))
		{
			m_isCollisionGround = true;
		}
	}

	private void Boost()
	{
		if (!m_input.IsBoost)
		{
			return;
		}

		Vector3 direction = Vector3.zero;
		if (!IsFullGround)
		{
			direction = m_transform.forward * m_physicAttr.BoostForceAir;
		} else
		{
			direction = m_transform.forward * m_physicAttr.BoostForceGround;
		}
		m_rootRig.AddForce (direction, ForceMode.Acceleration);
	}

	private void CheckIsOutSide()
	{
		if (Mathf.Abs (transform.position.x) > 30 ||
			Mathf.Abs (transform.position.y) > 30 ||
			Mathf.Abs (transform.position.z) > 30)
		{
			m_rootRig.velocity = Vector3.zero;
			m_rootRig.angularVelocity = Vector3.zero;
			m_transform.position = Vector3.zero;
		}
	}

	public void Explosion (Vector3 explosionPosition)
	{
		m_rootRig.velocity = Vector3.zero;
		explosionPosition -= Vector3.up * (explosionPosition.y + 5);

		if (transform.position.z > 20f)
		{
			transform.position += new Vector3(0,0.1f, (20f - transform.position.z) * 1.5f);
		}
		else if (transform.position.z < -20f)
		{
			transform.position += new Vector3(0,0.1f, (-20f - transform.position.z) * 1.5f);
		}
		if (explosionPosition.z > 19)
		{
			explosionPosition.z += 6f;
		}
		else if (explosionPosition.z < -19)
		{
			explosionPosition.z -= 6f;
		}

		RootRig.AddExplosionForce (m_rootRig.mass * 1000f, explosionPosition, 1000f);
	}
}
