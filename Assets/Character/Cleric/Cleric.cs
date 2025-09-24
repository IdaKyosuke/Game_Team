using UnityEngine;

public class Cleric : PlayerStatus
{
    public override void Identity()
    { 
        Debug.Log("Cleric Identity");

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("•Ší‚Ì‚¿‘Ö‚¦");
        }
    }
}