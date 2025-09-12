using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName ="New Item", menuName = "ScriptableObject/Create Item")]
public class Test_Item : ScriptableObject
{
	// �A�C�e����r�p�ԍ�
	public int number = 0;

	// �A�C�e����
	public new string name = "New Item";

	// �A�C�e���A�C�R��
	public Sprite icon = null;

	// �A�C�e���̐���
	public string explain = "New Explain";

	// �A�C�e���̔̔����i
	public int buy_price = 0;

	// �A�C�e���̔��l
	public int sell_price = 0;

	// �A�C�e���̎��
	public enum Type
	{
		Material,
		ForHeal,
		ForMoney,
	}

	public Type type = Type.Material;

	// �A�C�e���̎�ނ��擾
	public string ItemType()
	{
		string name = "new type";

		switch(type)
		{
		case Type.Material:
			name = "�f�ރA�C�e��";
				break;

		case Type.ForHeal:
			name = "�񕜃A�C�e��";
				break;

		case Type.ForMoney:
			name = "�����A�C�e��";
			break;
		}
		return name;
	}

	// �g�p�ł��邩�ǂ���
	public bool canUse = false;

	// �g�p�ł���Ƃ��̌��ʂ̎��
	public enum Effect
	{
		Heal,	// �̗͉�
		AddHp,	// �ő�̗͑���
		AddAtk,	// �U���o�t
		RepairLight,	// �_���W�������Ŗ��邳���񕜂���
	}

	// �A�C�e����������
	public Effect effect = Effect.Heal;
	// ���ʗ�
	public int effectSize = 0;

	// ���ʂ��t�^�����v���C���[��ݒ肵�Ă��Ȃ��̂ň�U����
	/*
	// �g�p�ł��鎞�̌���
	public void Use()
	{
		if(effect == Effect.RepairLight)
		{
			GameObject light = GameObject.FindWithTag("lightManager");
			light.GetComponent<Light_Manager>().RepairLight();
		}
		else
		{
			GameObject PlayerStatus = GameObject.FindWithTag("playerStatus");
			switch(effect)
			{
			case Effect.Heal:
				PlayerStatus.GetComponent<Player_DungeonStatus>().Heal(effectSize);
				Debug.Log(PlayerStatus.GetComponent<Player_DungeonStatus>().GetCurrentHp());
				break;

			case Effect.AddHp:
				PlayerStatus.GetComponent<Player_DungeonStatus>().AddHp(effectSize);
				Debug.Log(PlayerStatus.GetComponent<Player_DungeonStatus>().GetMaxHp());
				break;

			case Effect.AddAtk:
				PlayerStatus.GetComponent<Player_DungeonStatus>().AddAtk(effectSize);
				Debug.Log(PlayerStatus.GetComponent<Player_DungeonStatus>().GetAtk());
				break;

			case Effect.RepairLight:
				break;
			}
		}

	}
	*/
}
