using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StashController : MonoBehaviourPunCallbacks
{
    [SerializeField] UnityEvent m_onPassiveSkill;
    [SerializeField] GameObject m_stashManager;
    [SerializeField] Info_InventorySize m_inventortSize;
	[SerializeField] GameObject m_uiParentCanvs;
	[SerializeField] StashManager m_manager;
	[SerializeField] bool m_isPlayer = true;

	
	private Rigidbody m_rb;         // レイの当たった敵を保管する用
    private GameObject m_rayTarget;
	private bool m_isDeath = false;

    public Info_InventorySize InventortSize => m_inventortSize;

	public bool IsOpen => m_manager.IsOpenInventory();

    void Start()
    {
		m_uiParentCanvs.SetActive(true);
		m_isPlayer = photonView.IsMine;
		if (!m_isPlayer) return;
		m_rb = GetComponent<Rigidbody>();
    }

    private void Update()
	{
		if (m_isDeath) return;
		if (!m_isPlayer) return;

		// 自身が生成したオブジェクトだけに移動処理を行う
		if (photonView.IsMine)
		{
			//前方にRayを飛ばす
			if (Physics.Raycast(transform.position, transform.forward, out var hit))
			{
				//Debug.Log("Hit!!!!!!!!!!!!");
				//Eキーが押されていなければ無視
				if (Input.GetKeyDown("e"))
				{
					//プレイヤー
					if (hit.transform.gameObject.CompareTag("Player"))
					{
						// レイの当たった敵を保管
						m_rayTarget = hit.transform.gameObject;

						m_stashManager.GetComponent<StashManager>().IsScavenger(true);

						if (m_rayTarget.TryGetComponent(out PhotonView view))
						{
							Debug.Log("Eをおした" + view);
							// rayが当たっているオブジェクトに自分へ情報を送るようリクエストする
							view.RPC(nameof(RequestInventoryData), view.Owner, photonView.ViewID, true);
						}
					}

					if (hit.transform.gameObject.CompareTag("Treasure"))
					{
						// レイの当たった敵を保管
						m_rayTarget = hit.transform.gameObject;

						m_stashManager.GetComponent<StashManager>().IsScavenger(true);

						if (m_rayTarget.TryGetComponent(out PhotonView view))
						{ 
							Debug.Log("Eをおした" + view);
							// rayが当たっているオブジェクトに自分へ情報を送るようリクエストする
							view.RPC("RequestTreasureData", view.Owner, photonView.ViewID);				
						}
					}
				}
			}

			if (Input.GetKeyDown("tab"))
			{
				m_stashManager.GetComponent<StashManager>().ManageUiActiveInfo();
			}

			if(m_stashManager.GetComponent<StashManager>().IsOpenInventory())
			{
				if (Input.GetKeyDown("1"))
				{
					m_stashManager.GetComponent<StashManager>().AddItemInventory();
				}
				else if (Input.GetKeyDown("2"))
				{
					m_stashManager.GetComponent<StashManager>().AddItemStash();
				}
			}
		}
		else
		{
			m_rb.isKinematic = true;
		}
	}

	// プレイヤーからリクエストをもらってデータを送り返す
	[PunRPC]
	void RequestInventoryData(int requestId, bool isPlayer)
	{
		if (isPlayer)
		{
			PhotonView view = PhotonView.Find(requestId);

			Debug.Log("view.RPC s : " + view);
			view.RPC(nameof(ReceiveInventoryData), view.Owner, GetComponent<Inventory_Info>().GetInfo(), GetManager().GetItemList());
			Debug.Log("view.RPC e");
		}
	}

	[PunRPC]
	void ReceiveInventoryData(Info_InventorySize info, List<ItemList> list)
	{
		Debug.Log("PUNRPC : " + list.Count);
		//インベントリUIの表示
		m_stashManager.transform.GetComponent<StashManager>().CreateStashUi(info, list);
	}


    public void OnDamage()
    {
        Debug.Log("Damage!!!!!!!");
    }

    public void OnDeath()
    {
        Debug.Log("Death!!!!!!!");
		m_isDeath = true;
    }

	// 変更後のアイテムリストを返す
	public void ReturnItemList(List<ItemList> items)
	{
		if (!m_rayTarget || items == null) return;

		if (m_rayTarget.TryGetComponent(out PhotonView view))
		{
			view.RPC(nameof(RequestCopyItemList), view.Owner, items);
			// ターゲットを空にする
			m_rayTarget = null;
		}
	}

	[PunRPC]
	void RequestCopyItemList(List<ItemList> list)
	{
		Debug.Log("RequestCopyItemList : " + list.Count);
		GetManager().CopyItemList(list);
	}

	public StashManager GetManager()
	{
		return m_manager;
	}

	public override void OnLeftRoom()
	{
		Debug.Log("LeftRoom!");
		photonView.RPC(nameof(RequestOnDeath), photonView.Owner);
		base.OnLeftRoom();
	}

	[PunRPC]
	void RequestOnDeath()
	{
		m_isDeath = true;
	}
}