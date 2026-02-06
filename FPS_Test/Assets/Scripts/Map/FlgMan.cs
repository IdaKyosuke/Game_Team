using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlgMan : SingletonBase<FlgMan>
{
    private bool m_isCreatedMaze = false;

    public bool IsCreatedMaze => m_isCreatedMaze;
    public void SetFlg(bool flg)
    {
        m_isCreatedMaze = flg;
    }
}
