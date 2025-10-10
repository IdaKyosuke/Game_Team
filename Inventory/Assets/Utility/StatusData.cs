using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "ScriptableObject/Info PlayerData")]
public class StatusData : ScriptableObject
{
    //レベルごとのパラメータ
    [SerializeField] List<PlayerParameter> parameters;

    //指定レベルのステータスを取得
    public PlayerParameter GetStatus(int level) => parameters[level - 1];

    //最大レベルの取得
    public int MaxLevel => parameters.Count + 1;

    //インスペクター上で変更があればレベルを更新
    private void OnValidate()
    {
        for (int i = 0; i < parameters.Count; i++)
        {
            //新規作成
            if (parameters[i] == null) parameters[i] = new PlayerParameter(i + 1);

            //レベルの指定
            parameters[i].GetType()
                .GetField("level", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(parameters[i], i + 1);
        }
    }
}