using UnityEngine;
using System.Collections;
using InControl;

public enum INPUT_TYPE
{
	LEFT,
	RIGHT,
	UP,
	DOWN,
	ATTACK
}


public class InputMaster : MonoBehaviour {

	InputDevice device;
	InputManager inputMgr;
	InputControl control;
	InControlManager manager;

	// Use this for initialization
	void Start ()
	{
		
	}
}
