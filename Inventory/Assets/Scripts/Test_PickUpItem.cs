using UnityEngine;

public class Test_PickUpItem : MonoBehaviour
{
	// �A�C�e���f�[�^
	[SerializeField] Test_Item m_item;

    // Start is called before the first frame update
    void Start()
	{
		// �X�N���v�^�u���I�u�W�F�N�g����摜���擾����
		GetComponent<SpriteRenderer>().sprite = m_item.icon;
	}

	public void PickUp()
	{
		if (Test_Inventory.instance.AddItem(m_item))
		{
			Debug.Log(m_item.name + " ����肵�܂����B");
			Destroy(gameObject);
		}
		else
		{
			Debug.Log("�o�b�O�������ς��� " + m_item.name + " ���󂯎��܂���ł����B");
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			PickUp();
		}
	}
}
