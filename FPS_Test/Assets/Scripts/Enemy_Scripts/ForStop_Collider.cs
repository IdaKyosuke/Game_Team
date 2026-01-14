using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForStop_Collider : MonoBehaviour
{
	private bool m_isCheck = false;

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.CompareTag("playerModel"))
		{
			Debug.Log("hit player");

			m_isCheck = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("playerModel"))
		{
			Debug.Log("leave player");
			m_isCheck = false;
		}
	}

	public bool GetCheckFlg()
	{
		return m_isCheck;
	}
}
