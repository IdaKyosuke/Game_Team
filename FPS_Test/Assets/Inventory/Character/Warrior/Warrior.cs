using UnityEngine;

public class Warrior : PlayerStatus
{
    public override void Identity()
    {
        Debug.Log("Warrior Identity");

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("ƒoƒt•t—^");
        }
    }
}
