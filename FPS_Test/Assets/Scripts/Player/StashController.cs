using Cysharp.Threading.Tasks;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StashController : MonoBehaviourPunCallbacks
{
	private float m_scavengerTime = 3.5f;    // 箱開けにかかる時間

	[SerializeField] Slider m_slider;
    [SerializeField] UnityEvent m_onPassiveSkill;
    [SerializeField] GameObject m_stashManager;
    [SerializeField] Info_InventorySize m_inventortSize;
	[SerializeField] GameObject m_uiParentCanvs;
	[SerializeField] StashManager m_manager;
	[SerializeField] bool m_isPlayer = true;
	[SerializeField] Transform m_camera;
	private GameObject m_miniMap = null;

	private PlayerStatus m_status;
    private Rigidbody m_rb;					// レイの当たった敵を保管する用
    private GameObject m_rayTarget;
	private bool m_isDeath = false;
	private bool m_nowScavenger = false;    // 現在箱開け中か状態か
	private float m_elapsedTime;                // 箱開けの経過時間

	private bool m_startReady = false;      // ゲーム開始時にインベントリを消したか

	[SerializeField] ExcelData m_excelData = null;	

	public Info_InventorySize InventortSize => m_inventortSize;

	public bool IsOpen => m_manager.IsOpenInventory();

	public bool NowScavenger => m_nowScavenger;

	public float ScavengerTime
	{
		get { return m_scavengerTime; }
		set { m_scavengerTime = value; }
    }

	public ExcelData GetExcel => m_excelData;

	void Awake()
    {
		m_uiParentCanvs.SetActive(true);
		m_rb = GetComponent<Rigidbody>();
        m_status = GetComponent<PlayerStatus>();
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
		if(!m_startReady)
		{
			m_stashManager.GetComponent<StashManager>().ManageUiActiveInfo();
			m_startReady = true;
		}

		// 自身が生成したオブジェクトだけに移動処理を行う
		if (photonView.IsMine)
		{
			//前方にRayを飛ばす
			if (Physics.Raycast(m_camera.position, m_camera.forward, out var hit))
			{
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

					// 宝箱
					if (hit.transform.gameObject.CompareTag("Treasure"))
					{
						// 誰かが触ってるかどうか
						if (hit.transform.GetComponent<TreasureBoxItem>().IsNowOpen)
						{
							return;
						}
						// 触ってない場合鍵がかかっているかどうか
						else if (hit.transform.GetComponent<TreasureBoxItem>().IsLock)
						{
							List<ItemList> items = new List<ItemList>(m_stashManager.GetComponent<StashManager>().GetItemList());

							// インベントリに鍵があるか確認
							foreach (var item in items)
							{
								if (item.ItemData.equipmentType == (int)EquipmentType.Key)
								{
									// インベントリから鍵を一つ消費
									m_stashManager.GetComponent<StashManager>().RemoveInventory(item);
									// 宝箱のロックを解除
									hit.transform.GetComponent<PhotonView>().RPC("SetLock", RpcTarget.All, false);
									break;
								}
							}
						}
					}

					return;
				}

				//長押しされている間だけ経過時間を加算
				if (Input.GetKey("e"))
				{
                    //宝箱
                    if (!hit.transform.gameObject.CompareTag("Treasure")) return;

					Debug.Log("treasure");

					hit.transform.TryGetComponent(out TreasureBoxItem treasureBox);

					// 誰かが開いてるかどうか
					if (treasureBox.IsNowOpen)
					{
						// 自分以外
						if (!m_nowScavenger)
						{
							Debug.Log("cant open");
							return;
						}
					}
					else
					{
						treasureBox.GetComponent<PhotonView>().RPC("SetNowOpen", RpcTarget.All, true);
					}

					//既にインベントリを開いているとき or 鍵がかかっている時は無視
					if (!m_nowScavenger && !treasureBox.IsLock) m_slider.gameObject.SetActive(true);

					// 箱開け速度の補正
					float openSpeedRate = m_status.Total.openSpeed / 100.0f;

					//未開封の箱なら経過時間を加算
					if (!m_nowScavenger && !treasureBox.IsOpened)
					{
						if (!treasureBox.IsLock)
						{
							// レイの当たった箱を保管
							m_rayTarget = hit.transform.gameObject;
							m_nowScavenger = true;
						}
						else
						{
							return;
						}
                    }
					// すでに空いている宝箱を調べた時
					else if(treasureBox.IsOpened)
					{
						m_rayTarget = hit.transform.gameObject;
					}

					if (m_nowScavenger && m_rayTarget == hit.transform.gameObject)
					{
						// 同じ箱を見た時
						m_elapsedTime += Time.deltaTime * openSpeedRate;
						m_slider.value = m_elapsedTime / m_scavengerTime;
						if (m_elapsedTime < m_scavengerTime) return;
					}

					// 経過時間と箱開け状態をリセット
					m_elapsedTime = 0;
                    m_slider.gameObject.SetActive(false);

					m_stashManager.GetComponent<StashManager>().IsScavenger(true);

                    if (m_rayTarget.TryGetComponent(out PhotonView view))
                    {
                        // rayが当たっているオブジェクトに自分へ情報を送るようリクエストする
                        view.RPC("RequestTreasureData", view.Owner, photonView.ViewID);

						// 宝箱を空いた状態にする
						view.RPC("IsOpen", RpcTarget.All);
                    }
                }

				if (Input.GetKeyUp("e"))
				{
					m_elapsedTime = 0;
                    m_slider.gameObject.SetActive(false);

					// 宝箱を開き切ってるかどうか
					if (!IsOpen)
					{
						// 宝箱にRayが当たっているかどうか
						if (m_rayTarget && m_rayTarget.transform.CompareTag("Treasure"))
						{
							m_nowScavenger = false;
							m_rayTarget.GetComponent<PhotonView>().RPC("SetNowOpen", RpcTarget.All, false);
						}
					}
				}
            }

			if (Input.GetKeyDown("tab"))
			{
				if (m_miniMap != null)
				{
					// 宝箱を開いていた時
					if (m_rayTarget && m_rayTarget.transform.CompareTag("Treasure"))
					{
						m_nowScavenger = false;
						if (m_rayTarget.TryGetComponent(out PhotonView view))
						{
							// 宝箱の開け状態をfalseにする
							view.RPC("SetNowOpen", RpcTarget.All, false);
						}
					}

					// インベントリを閉じたときに自分のコピーにも反映させる
					photonView.RPC(nameof(RequestCopyItemList), RpcTarget.All, GetManager().GetItemList());

					m_miniMap.SetActive(m_stashManager.GetComponent<StashManager>().ManageUiActiveInfo());
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
		if (!photonView.GetComponent<PlayerController>().IsDeath) return;

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
		// 宝箱を開いていた時
		if (m_rayTarget && m_rayTarget.transform.CompareTag("Treasure"))
		{
			m_nowScavenger = false;
			if (m_rayTarget.TryGetComponent(out PhotonView view))
			{
				// 宝箱の開け状態をfalseにする
				view.RPC("SetNowOpen", RpcTarget.All, false);
			}
		}

		// インベントリを閉じたときに自分のコピーにも反映させる
		photonView.RPC(nameof(RequestCopyItemList), RpcTarget.All, GetManager().GetItemList());

		m_miniMap.SetActive(m_stashManager.GetComponent<StashManager>().ManageUiActiveInfo());
		//m_stashManager.GetComponent<StashManager>().Save();
		//Debug.Log("death");
		m_isDeath = true;
    }

	// 変更後のアイテムリストを返す
	public void ReturnItemList(List<ItemList> items)
	{
		if (!m_rayTarget || items == null) return;

		if (m_rayTarget.TryGetComponent(out PhotonView view))
		{
			view.RPC("RequestCopyItemList", RpcTarget.All, items);
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

	public void DeleteInventory()
	{
		m_manager.DeleteInventory();
	}
}