using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class a : MonoBehaviour
{
	[SerializeField] GameObject A;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(A, transform.position, transform.rotation);
    }
}
