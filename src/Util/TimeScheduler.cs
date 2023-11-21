using Godot;
using System;
using System.Collections.Generic;

namespace HCoroutines.Util
{
    public partial class TimeScheduler : Node
    {
        public static TimeScheduler Instance { get; private set; }

        private Dictionary<int, Action> actionsById = new();
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
            SceneTreeTimer timer = GetTree().CreateTimer(delay, processAlways: false);
            return ScheduleOnSignal(action, timer, "timeout");
        }

        public int ScheduleOnSignal(Action action, GodotObject obj, string signal)
        {
            int id = GetNextScheduleId();
            actionsById[id] = action;

            obj.Connect(
                signal,
                Callable.From(() => CallCallback(id)),
                (int)ConnectFlags.OneShot
            );

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