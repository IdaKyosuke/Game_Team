using System.Numerics;

[System.Serializable]
public class ConditionParameter
{
	public int id;

    public string conditionType;    //ó‘ÔˆÙí‚Ìí—Ş
    public int grantRate;           //•t—^—¦(%)

    public int triggerCount;        //”­“®‰ñ”
    public int triggerInterval;     //”­“®ŠÔŠu(•b)
    public int triggerValue;        //Œø‰Ê—Ê
}
