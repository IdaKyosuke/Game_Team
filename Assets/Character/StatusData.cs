using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Status")]
public class StatusData : ScriptableObject
{
    [Serializable]
    public class Parameters
    {
        [SerializeField] private int level;
        public int hp;
        public int mp;
        public int power;
        public int defense;
        public int speed;
        public int requiredExp;

        public Parameters(int level)
        {
            this.level = level;
        }
    }

    //レベルごとのパラメータ
    [SerializeField] private List<Parameters> parameters;

    public int MaxLevel
    { 
        get { return parameters.Count + 1; }
    }

    //インスペクター上で変更があればレベルを更新
    private void OnValidate()
    {
        for (int i = 0; i < parameters.Count; i++)
        {
            //新規作成
            if (parameters[i] == null) parameters[i] = new Parameters(i + 1);

            //レベルの指定
            parameters[i].GetType()
                .GetField("level", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(parameters[i], i + 1);
        }
    }

    public Parameters GetStatus(int level)
    { 
        return parameters[level - 1];
    }
}
