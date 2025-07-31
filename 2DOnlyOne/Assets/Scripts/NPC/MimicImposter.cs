using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(NPCController))]
public class MimicAI : MonoBehaviour
{
    [Header("Mimic Settings")]
    public NPCController targetNPC;
    public float mimicDelay = 1.5f; // seconds
    public float positionThreshold = 0.1f; // minimum movement before logging new position

    private NPCController npcController;
    private NPCBehavior npcBehavior;

    private struct MimicFrame
    {
        public Vector3 position;
        public bool isIdle;
        public float timestamp;
    }

    private Queue<MimicFrame> recordedFrames = new Queue<MimicFrame>();
    private Vector3 lastRecordedPosition;

    void Start()
    {
        npcController = GetComponent<NPCController>();
        npcBehavior = GetComponent<NPCBehavior>();

        if (targetNPC == null)
        {
            Debug.LogWarning("MimicAI has no targetNPC assigned.");
            enabled = false;
        }

        lastRecordedPosition = targetNPC.transform.position;
    }

    void Update()
    {
        RecordTarget();
        ReplayFrames();
    }

    void RecordTarget()
    {
        float dist = Vector3.Distance(targetNPC.transform.position, lastRecordedPosition);
        if (dist > positionThreshold || targetNPC.IsIdleStateChanged())
        {
            MimicFrame frame = new MimicFrame
            {
                position = targetNPC.transform.position,
                isIdle = targetNPC.IsIdle(),
                timestamp = Time.time
            };
            recordedFrames.Enqueue(frame);
            lastRecordedPosition = targetNPC.transform.position;
        }
    }

    void ReplayFrames()
    {
        while (recordedFrames.Count > 0 && Time.time - recordedFrames.Peek().timestamp >= mimicDelay)
        {
            MimicFrame frame = recordedFrames.Dequeue();

            if (frame.isIdle)
            {
                npcController.StopCompletely();
            }
            else
            {
                npcController.MoveTo(frame.position);
            }
        }
    }
}
