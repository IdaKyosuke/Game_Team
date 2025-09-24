using UnityEngine;

public class Thief : PlayerStatus
{
    public override void Identity()
    {
        Debug.Log("Thief Identity");
        
        if(Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("㩂�ݒu");
        }
    }
}