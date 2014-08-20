using UnityEngine;

namespace MQMTech.AI.Knowledge
{
    public enum KnowledgeStatus
    {
        NEW,
        PROCESSED,
        DONE,
    }

    public class BaseKnowledge
    {
        public KnowledgeStatus Status { get; set; }

        public BaseKnowledge()
        {
            Status = KnowledgeStatus.NEW;
        }
    }
}
