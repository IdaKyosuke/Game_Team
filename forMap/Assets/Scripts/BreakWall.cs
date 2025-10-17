using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BreakWall : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.CompareTag("Wall"))
		{
			Debug.Log(other.gameObject);
			Destroy(other.gameObject);
			Destroy(gameObject);
		}
	}
}
