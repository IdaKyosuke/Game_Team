using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy List", menuName = "ScriptableObject/Create EnemyList")]
public class Spawn_EnemyList : ScriptableObject
{
	public List<GameObject> enemy = new List<GameObject>();
}
