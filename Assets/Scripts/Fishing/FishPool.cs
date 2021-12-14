using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using MyBox;

public class FishPool : Singleton<FishPool>
{
    [SerializeField]
    private AnimationCurve _IncreasedChanceEfficiency;
    [SerializeField, ListDrawerSettings(Expanded = true)]
    private List<FishPoolItem> _FishPool;

    public string GetFish(FishBaitItem bait)
    {
        CreateTable(bait, out List<FishPoolItem> fishes, out float sumProbabilities);

        float number = Random.Range(0, sumProbabilities);
        foreach (FishPoolItem fish in fishes)
        {
            if (number > fish.ProbabilityRangeFrom && number < fish.ProbabilityRangeTo)
            {
                return fish.FishID;
            }
        }

        Debug.Log("no fish could be found");

        return string.Empty;
    }

    private void CreateTable(FishBaitItem bait, out List<FishPoolItem> fishes, out float sumProbabilities)
    {
        fishes = new List<FishPoolItem>();
        sumProbabilities = 0.0f;

        float maxRange = -float.MaxValue;
        foreach (FishPoolItem fish in _FishPool)
        {
            Stars baitStars = ((IStar)bait).Star;
            Stars fishStars = ((IStar)ItemTypeSettings.Instance.ItemTypeChunk[fish.FishID].Behaviour).Star;

            if ((int)fishStars > (int)baitStars)
                continue;

            float efficiency = ((IBait)bait).Efficiency;

            if (fish.ProbabilityWeight < 0f)
            {
                Debug.Log("negative probabilities not allowed");
                fish.ProbabilityWeight = 0f;
            }
            else
            {
                fish.ProbabilityRangeFrom = sumProbabilities;
                sumProbabilities += fish.ProbabilityWeight;
                fish.ProbabilityRangeTo = sumProbabilities;

                float range = fish.ProbabilityRangeTo - fish.ProbabilityRangeFrom;

                if (range > maxRange)
                    maxRange = range;
            }

            fishes.Add(fish);
        }

        sumProbabilities = 0.0f;
        foreach (FishPoolItem fish in fishes)
        {
            // higher efficiency means higher chance of getting rarer fish

            float range = fish.ProbabilityRangeTo - fish.ProbabilityRangeFrom;

            fish.ProbabilityRangeFrom = sumProbabilities;
            float increasedChance = _IncreasedChanceEfficiency.Evaluate(range / maxRange) * ((IBait)bait).Efficiency;
            sumProbabilities += fish.ProbabilityWeight + increasedChance;
            fish.ProbabilityRangeTo = sumProbabilities;
        }
    }

    [System.Serializable]
    private class FishPoolItem
    {
        [HorizontalGroup("Group")]
        [VerticalGroup("Group/Left"), LabelWidth(45)]
        public string FishID;
        [VerticalGroup("Group/Right"), LabelWidth(115)]
        [Tooltip("The higher the weight, the higher the chance of being picked"), Min(1)]
        public float ProbabilityWeight;

        [HideInInspector]
        public float ProbabilityRangeFrom;
        [HideInInspector]
        public float ProbabilityRangeTo;
    }
}
