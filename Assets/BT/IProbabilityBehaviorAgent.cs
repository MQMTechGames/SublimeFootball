using UnityEngine;
using System.Collections.Generic;

public interface IProbabilityBehaviorAgent
{
    void FillProbabilities(int selectorId, List<int> nodesIds, List<float> probabilities);
}
