using Photon.Pun;
using UnityEngine;

public class EquipmentStatus : MonoBehaviour
{
    [SerializeField] EquipmentParameter m_statusData;   //装備の基礎ステータス(ScriptableObject)
    [SerializeField] EquipmentData m_passiveSkillData;  //装備のパッシブスキル     

    private EquipmentParameter m_totalStatus;           //装備の総合ステータス

    public EquipmentParameter TotalStatus => m_totalStatus;

    private void Start()
    {
        //Null対策(後で消す)
        Init();
    }

    public void Init(int id = -1)
    {
        //idが設定されていなければ抽選
        if (id == -1)
        {
            //ランダムでパッシブスキルを設定 (1 / 2)
            int rand = Random.Range(0, 2);
            if (rand == 1)
            {
                rand = Random.Range(0, m_passiveSkillData.EquipmentAbility.Count);
                m_statusData.id = rand;
            }
        }

        //装備の総合ステータスを計算
        m_totalStatus = m_statusData;
        m_totalStatus += m_passiveSkillData.EquipmentAbility[m_statusData.id];
    }

	public void SetPassive()
	{
		//武器なら状態異常付与のスキルを取得
		if (GetComponent<Item_Object>().GetWeaponType() == EquipmentType.Weapon)
		{
			//transform.root.GetComponent<Condition>().Grant = (ConditionType)m_passiveSkillData.EquipmentAbility[m_statusData.id].condition;
		}
	}
}