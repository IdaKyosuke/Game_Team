using UnityEditor.EventSystems;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] float m_jumpPower;
    [SerializeField] float m_gravity;
    [SerializeField] UnityEvent m_onPassiveSkill;
    [SerializeField] GameObject m_stashManager;
    [SerializeField] Info_InventorySize m_inventortSize;


    private Vector3 m_moveDirection;
    private CharacterController m_controller;

    private PlayerStatus m_playerStatus;
    private Condition m_condition;

    public Info_InventorySize InventortSize => m_inventortSize;

    void Start()
    {
        m_controller = GetComponent<CharacterController>();
        m_playerStatus = GetComponent<PlayerStatus>();
        m_condition = GetComponent<Condition>();
        m_moveDirection = Vector3.zero;
    }

    private void Update()
    {
        //前方にRayを飛ばす
        if (Physics.Raycast(transform.position, transform.forward, out var hit))
        {
            //プレイヤー以外は無視
            if (!hit.transform.gameObject.CompareTag("Player")) return;

            Debug.Log("Hit!!!!!!!!!!!!");

            //Eキーが押されていなければ無視
            if (!Input.GetKeyDown("e")) return;

            //インベントリUIの表示
            m_stashManager.GetComponent<StashManager>().CreateStashUi(hit.transform.GetComponent<PlayerMove>().InventortSize);
        }
    }

    void FixedUpdate()
    {
        //自由落下
        m_moveDirection.y -= m_gravity * Time.deltaTime;

        //感電状態は移動不可
        if (m_condition.CurrentCondition == Condition.ConditionType.Shock) return;

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

    public void OnDamage()
    {
        Debug.Log("Damage!!!!!!!");
    }

    public void OnDeath()
    {
        Debug.Log("Death!!!!!!!");
    }
}