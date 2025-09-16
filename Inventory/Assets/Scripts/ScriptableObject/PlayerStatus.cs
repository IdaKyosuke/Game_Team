using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Status", menuName = "ScriptableObject/Create Status")]

// �v���C���[�̃X�e�[�^�X���V�[���ԂŎ󂯓n��
public class PlayerStatus : ScriptableObject
{
	// �������̍ő�l
	private int m_maxMoney = 999999;

	// �v���C���[���S���ɃX�e�[�^�X������������p
	[SerializeField] int initMoney = 0;
	[SerializeField] int initAtk = 0;
	[SerializeField] int initHp = 20;
	[SerializeField] int initLv = 1;

	// �����̒l
	public int firstAtk = 0;       // �U����
	public int firstMoney = 0;        // ������
	public int firstHp = 0;     // �̗͂̍ő�l
	public int currentHp = 0;   // ���݂̗̑�
	public int firstLv = 1;		// ���x��

	// �����i�K
	public int WeaponLevel = 0; // ���탌�x��
	public int BlackSmithLevel = 0; // �b���̃��x��
	public int ShopLevel = 0;	// ���X�̃��x��

	// �A�C�e�����X�g�͎Q�Ɠn������
	public List<Test_Item> itemList = new List<Test_Item>();	// �����A�C�e�����X�g
	public List<Test_Item> stashList = new List<Test_Item>();	// �A�C�e���{�b�N�X�̒��g

	// ---- �l��ύX����֐� ----
	// �U���͂�ύX����(�_���W�����p)
	public void SetAtk(int atk)
	{
		firstAtk = atk;
	}

	// �U���͂�ύX(�X�p)
	public void SetCityAtk(int atk)
	{
		initAtk = initAtk + atk;
		// �b�艮�ŋ������ꂽ����K�p����
		firstAtk = initAtk;
	}

	// ��������ύX����
	public void SetMoney(int money)
	{
		firstMoney = money;
		if(firstMoney >= m_maxMoney)
		{
			// ���������ő�l�ɕ␳����
			firstMoney = m_maxMoney;
		}
	}

	// �̗͂̍ő�l��ύX����
	public void SetMaxHp(int hp)
	{
		firstHp = hp;
	}

	// �̗͂̌��ݒl��ύX
	public void SetCurrentHp(int hp)
	{
		currentHp = hp;
	}

	// �_���W�������Ń��x����������
	public void SetLv(int lv) 
	{
		firstLv++;
	}

	// ���S���Ƀ��Z�b�g�����X�e�[�^�X
	public void ResetStatus()
	{
		firstMoney = initMoney;	// �����������Z�b�g
		itemList.Clear();		// �C���x���g�������Z�b�g
	}

	// �_���W�������o���Ƃ��ɐ�΃��Z�b�g�����X�e�[�^�X
	public void ReturnCity()
	{
		firstAtk = initAtk;     // �U���͂��X�ł̋����l�ɖ߂�
		firstHp = initHp;       // �̗͂��X�̒l�ɖ߂�
		currentHp = firstHp;    // �̗͂̌��ݒl���X�̗̑͂ɍ��킹��
		firstLv = initLv;		// Lv��1�ɖ߂�
	}

	// ����̋����l���グ��
	public void AddWeaponLevel()
	{
		WeaponLevel = WeaponLevel + 1;
	}

	// �b���̃��x����������
	public void AddBlackSmithLevel()
	{
		BlackSmithLevel = BlackSmithLevel + 1;
	}

	// ���X�̃��x����������
	public void AddShopLevel()
	{
		ShopLevel = ShopLevel + 1;
	}
}
