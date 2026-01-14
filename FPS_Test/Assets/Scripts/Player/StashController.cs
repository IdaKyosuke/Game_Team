using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class StashController : MonoBehaviourPunCallbacks
{
    [SerializeField] UnityEvent m_onPassiveSkill;
    [SerializeField] GameObject m_stashManager;
    [SerializeField] Info_InventorySize m_inventortSize;
	[SerializeField] GameObject m_uiParentCanvs;
	[SerializeField] StashManager m_manager;
	[SerializeField] bool m_isPlayer = true;
	[SerializeField] Transform m_camera;
	private GameObject m_miniMap = null;

	private Rigidbody m_rb;         // レイの当たった敵を保管する用
    private GameObject m_rayTarget;
	private bool m_isDeath = false;

    public Info_InventorySize InventortSize => m_inventortSize;

	public bool IsOpen => m_manager.IsOpenInventory();

    void Awake()
    {
		m_uiParentCanvs.SetActive(true);
		m_rb = GetComponent<Rigidbody>();
    }

    private async void Start()
    {
        var token = this.GetCancellationTokenOnDestroy();
        await UniTask.WaitUntil(() => SceneManager.GetSceneByName("MapScene").isLoaded, cancellationToken: token);
		m_miniMap = GameObject.FindWithTag("MiniMap");
    }

    private void Update()
	{
		if (m_isDeath) return;
		if (!m_isPlayer) return;

		// 自身が生成したオブジェクトだけに移動処理を行う
		if (photonView.IsMine)
		{
			//前方にRayを飛ばす
			if (Physics.Raycast(m_camera.position, m_camera.forward, out var hit))
			{
				//Debug.Log("Hit : " + hit.transform.name);
				//Eキーが押されていなければ無視
				if (Input.GetKeyDown("e"))
				{
					//プレイヤー
					if (hit.transform.root.gameObject.CompareTag("Player"))
					{
						// レイの当たった敵を保管
						m_rayTarget = hit.transform.root.gameObject;

						m_stashManager.GetComponent<StashManager>().IsScavenger(true);

						if (m_rayTarget.TryGetComponent(out PhotonView view))
						{
							Debug.Log("Eをおした" + view);
							// rayが当たっているオブジェクトに自分へ情報を送るようリクエストする
							view.RPC(nameof(RequestInventoryData), view.Owner, photonView.ViewID);
						}
					}

                    //宝箱
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
				if (m_miniMap != null)
				{
					m_miniMap.SetActive(m_stashManager.GetComponent<StashManager>().ManageUiActiveInfo());
				}
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

		Debug.DrawRay(m_camera.position, m_camera.forward, Color.yellow);
	}

	// プレイヤーからリクエストをもらってデータを送り返す
	[PunRPC]
	void RequestInventoryData(int requestId)
	{
		PhotonView view = PhotonView.Find(requestId);
		// 死んでいなければreturn
		if (!view.GetComponent<PlayerController>().IsDeath) return;

		Debug.Log("view.RPC s : " + view);
		view.RPC(nameof(ReceiveInventoryData), view.Owner, GetComponent<Inventory_Info>().GetInfo(), GetManager().GetItemList());
		Debug.Log("view.RPC e");
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

	[PunRPC]
    public void OnDeathStash()
    {
		m_stashManager.GetComponent<StashManager>().Save();
		//Debug.Log("death");
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

	public void Save()
	{
		m_manager.Save();
	}
}