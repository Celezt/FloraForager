[System.Serializable]
public struct CommissionData
{
    public string Title;
    public string Description;

    public int TimeLimit;
    public IObjective[] Objectives;

    public Relation MinRelation;
    public float RewardRelation;
    public float PenaltyRelation;

    public RewardPair[] Rewards;
    public string Giver;
}
