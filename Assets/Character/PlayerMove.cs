using UnityEngine;
using UnityEngine.Events;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] float m_jumpPower;
    [SerializeField] float m_gravity;
    [SerializeField] UnityEvent m_onUniqueSkill;

    private Vector3 m_moveDirection;
    private CharacterController m_controller;

    private PlayerStatus m_playerStatus;

    void Start()
    {
        m_controller = GetComponent<CharacterController>();
        m_playerStatus = GetComponent<PlayerStatus>();
        m_moveDirection = Vector3.zero;
    }

    void Update()
    {
        //ユニークスキル
        if (Input.GetKeyDown("q"))
        {
            m_onUniqueSkill?.Invoke();
        }
    }

    void FixedUpdate()
    {
        //自由落下
        m_moveDirection.y -= m_gravity * Time.deltaTime;

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