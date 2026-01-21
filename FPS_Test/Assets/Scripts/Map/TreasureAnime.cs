using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureAnime : MonoBehaviourPunCallbacks
{
	Animator anime;
	bool m_isOpened = false;
    bool openTreasure = false;
	bool Transtion = false;

	public bool IsOpened => m_isOpened;

    // Start is called before the first frame update
    void Start()
	{
		anime = GetComponent<Animator>();
	}

	public void Open()
	{
		if (!Transtion && !openTreasure)
		{
			openTreasure = true;
			Transtion = true;
            m_isOpened = true;
            anime.SetBool("Open", true);
		}
	}

	public void OpenEnd()
	{
		Transtion = false;
	}

	public void CloseEnd()
	{
		Transtion = false;
	}

	public bool opentreasure
	{ 
		get { return openTreasure; }
		set { openTreasure = value; }
	}

	[PunRPC]
	public void IsOpen()
	{
		m_isOpened = true;
	}
}
