using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour 
{
	bool _wasPresed = false;
	Vector3 _prevPos = Vector3.zero;
	bool _mousePressed = false;

	void Update()
	{
		SaveMouseStatus();
		ApplyMovement();
		ApplyZoom();
	}

	void SaveMouseStatus()
	{
		if(Input.GetMouseButtonDown(0))
		{
			_mousePressed = true;
		}
		else if(Input.GetMouseButtonUp(0))
		{
			_mousePressed = false;
		}
	}

	void ApplyMovement()
	{
		if(_mousePressed)
		{
			Vector3 currPos = Input.mousePosition;
			if(_wasPresed)
			{
				Move(currPos - _prevPos);
			}
			_wasPresed = true;
			_prevPos = currPos;
		}
		else
		{
			_wasPresed = false;
		}
	}

	void ApplyZoom()
	{
		float wheelFactor = Input.GetAxis("Mouse ScrollWheel");

		if ( wheelFactor < 0f)
		{
			camera.fieldOfView /= (1f + Mathf.Abs(wheelFactor));
		}
		if (Input.GetAxis("Mouse ScrollWheel") > 0f)
		{
			camera.fieldOfView *= (1f + wheelFactor);
		}
	}

	void Move(Vector3 dir)
	{
		Vector3 deltaRight = transform.right * dir.x;
		Vector3 deltaFront = transform.forward * dir.y;
		deltaFront.y = 0f;

		Vector3 newPos = transform.position + deltaRight + deltaFront;
		transform.position = newPos;
	}
}
