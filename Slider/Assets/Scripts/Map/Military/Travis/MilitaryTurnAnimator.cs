using System.Collections.Generic;
using UnityEngine;

public class MilitaryTurnAnimator : Singleton<MilitaryTurnAnimator>
{
    public enum QueueStatus 
    {
        Off,
        Ready, // Might still be processing but send the next
        Processing, // Proccessing animations
    }

    private Queue<MGMove> moveQueue = new();
    private List<MGMove> activeMoves = new();
    private QueueStatus status = QueueStatus.Off;

    private float timeLastMoveExecuted;
    private MGMove lastMoveExecuted;

    private void Awake()
    {
        InitializeSingleton();
    }

    public static void AddNewMove(MGMove move)
    {
        _instance.moveQueue.Enqueue(move);
        _instance.CheckQueue();
    }

    private void CheckQueue()
    {
        if (moveQueue.Count == 0)
        {
            return;
        }

        switch (status)
        {
            case QueueStatus.Off:
            case QueueStatus.Ready:
                MGMove nextMove = moveQueue.Dequeue();
                activeMoves.Add(nextMove);
                ExecuteMove(nextMove);
                status = QueueStatus.Processing;
                break;
            case QueueStatus.Processing:
                // Queue is processing -- do nothing!
                if (Time.time - timeLastMoveExecuted > 2)
                {
                    Debug.LogError($"Time in queue took an unexpectedly long time. Last move was made by {lastMoveExecuted.unit.UnitTeam} {lastMoveExecuted.unit.UnitType}");
                    Debug.Log($"Allowing queue to process more.");
                    status = QueueStatus.Ready;
                    CheckQueue();
                }
                break;
        }
    }

    private void ExecuteMove(MGMove move)
    {
        status = QueueStatus.Processing;
        timeLastMoveExecuted = Time.time;
        lastMoveExecuted = move;
        move.unit.NPCController.AnimateMove(move, false, () => FinishMove(move));
    }

    private void FinishMove(MGMove move)
    {
        activeMoves.Remove(move);
        status = moveQueue.Count == 0 ? QueueStatus.Off : QueueStatus.Ready;
        CheckQueue();
    }
}