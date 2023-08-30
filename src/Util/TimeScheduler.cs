using Godot;
using System;
using System.Collections.Generic;

namespace HCoroutines.Util
{
    public class TimeScheduler : Node
    {
        public static TimeScheduler Instance { get; private set; }

        private Dictionary<int, Action> actionsById = new Dictionary<int, Action>();
        private int idCounter = 0;

        public override void _EnterTree()
        {
            Instance = this;
        }

        private int GetNextScheduleId()
        {
            int id = idCounter;
            // Allow for integer overflow to wrap around to the beginning.
            idCounter = unchecked(idCounter + 1);
            return id;
        }

        public int Schedule(Action action, float delay)
        {
            SceneTreeTimer timer = GetTree().CreateTimer(delay, pauseModeProcess: false);
            return ScheduleOnSignal(action, timer, "timeout");
        }

        public int ScheduleOnSignal(Action action, Godot.Object obj, string signal)
        {
            int id = GetNextScheduleId();
            actionsById[id] = action;
            obj.Connect(signal, this, "CallCallback", new Godot.Collections.Array(id), (int)ConnectFlags.Oneshot);
            return id;
        }

        public void CancelSchedule(int scheduleId)
        {
            actionsById.Remove(scheduleId);
        }

        private void CallCallback(int id)
        {
            if (actionsById.TryGetValue(id, out Action action))
            {
                action.Invoke();
                actionsById.Remove(id);
            }
        }
    }
}