using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Parent : MonoBehaviour
{
    [SerializeField] GameObject m_content;
    [SerializeField] GameObject m_equipments;

    public GameObject GetContent => m_content;
    public GameObject GetEquipments => m_equipments;
}
