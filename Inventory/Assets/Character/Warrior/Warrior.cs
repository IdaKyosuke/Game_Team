using UnityEngine;

public class Warrior : PlayerStatus
{
    public void UniqueSkill()
    {
        Debug.Log("Warrior UniqueSkill");

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("ƒoƒt•t—^");
        }
    }
}
