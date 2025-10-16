using Photon.Pun;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class PlayerMove : MonoBehaviourPunCallbacks
{
    [SerializeField] float m_jumpPower;
    [SerializeField] float m_gravity;
    [SerializeField] UnityEvent m_onPassiveSkill;
    [SerializeField] GameObject m_stashManager;
    [SerializeField] Info_InventorySize m_inventortSize;

    private Vector3 m_moveDirection;
    private CharacterController m_controller;
	private Rigidbody m_rb;

    private PlayerStatus m_playerStatus;
    private Condition m_condition;

    public Info_InventorySize InventortSize => m_inventortSize;

	// レイの当たった敵を保管する用
	private GameObject m_rayTarget;
	[SerializeField] StashManager m_manager;

	[SerializeField] bool m_isPlayer = true;
	

    void Start()
    {
		if (!m_isPlayer) return;
        m_controller = GetComponent<CharacterController>();
		m_rb = GetComponent<Rigidbody>();
        m_playerStatus = GetComponent<PlayerStatus>();
        m_condition = GetComponent<Condition>();
        m_moveDirection = Vector3.zero;
    }

    private void Update()
	{
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
					//プレイヤー以外は無視
					if (!hit.transform.gameObject.CompareTag("Player")) return;
					// レイの当たった敵を保管
					m_rayTarget = hit.transform.gameObject;

					m_stashManager.GetComponent<StashManager>().IsScavenger(true);

					if (m_rayTarget.TryGetComponent(out PhotonView view))
					{ 
						Debug.Log("Eをおした" + view);
						// rayが当たっているオブジェクトに自分へ情報を送るようリクエストする
						view.RPC(nameof(RequestInventoryData), view.Owner, photonView.ViewID);				
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
	void RequestInventoryData(int requestId)
	{
		PhotonView view = PhotonView.Find(requestId);

		/*
		Info_InventorySize info = GetComponent<Inventory_Info>().GetInfo();
		List<ItemList> dataList = GetManager().GetItemList();
		ItemList[] data = new ItemList[dataList.Count];
		//Debug.Log(view.Owner);
		for (int i = 0; i < dataList.Count; ++i)
		{
			data[i] = dataList[i];
		}
		*/

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

	void FixedUpdate()
	{
		/*
		if (!m_isPlayer) return;
		// 自身が生成したオブジェクトだけに移動処理を行う
		if (photonView.IsMine)
		{
			//自由落下
			m_moveDirection.y -= m_gravity * Time.deltaTime;

			//感電状態は移動不可
			if (m_condition.Current == ConditionType.Shock) return;

			//移動量の取得
			m_moveDirection = new Vector3(Input.GetAxis("Horizontal"), m_moveDirection.y, Input.GetAxis("Vertical"));
			if (m_controller.isGrounded)
			{
				if (Input.GetButton("Jump")) m_moveDirection.y = m_jumpPower;
			}

			//カメラの向きを考慮した移動量
			Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
			Vector3 moveVelocity = cameraForward * m_moveDirection.z + Camera.main.transform.right * m_moveDirection.x;
			moveVelocity = new Vector3(moveVelocity.x * m_playerStatus.Value.moveSpeed, m_moveDirection.y, moveVelocity.z * m_playerStatus.Value.moveSpeed);

			//移動
			m_controller.Move(moveVelocity * Time.deltaTime);

			//移動していれば回転させる
			Vector3 move = new Vector3(m_moveDirection.x, 0, m_moveDirection.z);
			if (move != Vector3.zero)
			{
				transform.rotation = Quaternion.Slerp(
					transform.rotation,
					Quaternion.LookRotation(move.normalized),
					0.2f
				);
			}
		}
		*/
    }

    public void OnDamage()
    {
        Debug.Log("Damage!!!!!!!");
    }

    public void OnDeath()
    {
        Debug.Log("Death!!!!!!!");
    }

	// 変更後のアイテムリストを返す
	public void ReturnItemList(List<ItemList> items)
	{
		if (!m_rayTarget || items == null) return;

		/*
		ItemList[] list = new ItemList[items.Count];
		for (int i = 0; i <  list.Length; i++)
		{
			list[i] = items[i];
		}
		*/

		/*
		if (m_rayTarget.TryGetComponent(out PhotonView view))
		{ 
			Debug.Log("Eをおした" + view);
			// rayが当たっているオブジェクトに自分へ情報を送るようリクエストする
			view.RPC(nameof(RequestInventoryData), view.Owner, photonView.ViewID);				
		}
		*/
		if (m_rayTarget.TryGetComponent(out PhotonView view))
		{
			view.RPC(nameof(RequestCopyItemList), view.Owner, items);
//			view.RPC(nameof(RequestCopyItemList), view.Owner, 1, items.ToArray());
//			view.RPC(nameof(RequestCopyItemList), view.Owner, 1);
			// ターゲットを空にする
			m_rayTarget = null;
		}
	}

	[PunRPC]
	void RequestCopyItemList(List<ItemList> list)
//	void RequestCopyItemList(int hoge, ItemList[] list)
//	void RequestCopyItemList(int list)
	{
		Debug.Log("RequestCopyItemList : " + list.Count);
		GetManager().CopyItemList(list);
	}

	public StashManager GetManager()
	{
		return m_manager;
	}
}