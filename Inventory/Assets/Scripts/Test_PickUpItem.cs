using UnityEngine;

public class Test_PickUpItem : MonoBehaviour
{
	// アイテムデータ
	[SerializeField] Test_Item m_item;

    // Start is called before the first frame update
    void Start()
	{
		// スクリプタブルオブジェクトから画像を取得する
		GetComponent<SpriteRenderer>().sprite = m_item.icon;
	}

	public void PickUp()
	{
		if (Test_Inventory.instance.AddItem(m_item))
		{
			Debug.Log(m_item.name + " を入手しました。");
			Destroy(gameObject);
		}
		else
		{
			Debug.Log("バッグがいっぱいで " + m_item.name + " が受け取れませんでした。");
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
