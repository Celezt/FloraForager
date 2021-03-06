using System.Collections.Generic;

[System.Serializable]
public struct CommissionData
{
    public string Title;
    public string Description;

    public bool Repeatable;
    public bool HasTimeLimit;
    public int TimeLimit;
    public IObjective[] Objectives;

    public Relation MinRelation;
    public float RewardRelation;
    public float PenaltyRelation;

    public ItemAsset[] Rewards;
    public string Giver;

    public bool Completed;

    public Dictionary<string, string> AcceptDialogue;
    public Dictionary<string, string> CompleteDialogue;
}
