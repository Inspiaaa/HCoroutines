using Godot;
using System;
using System.Collections.Generic;

namespace HCoroutines
{
    public class CoroutineManager : Node
    {
        public static CoroutineManager Instance { get; private set; }
        public float DeltaTime { get; set; }

        private bool isIteratingActiveCoroutines = false;
        private HashSet<CoroutineBase> activeCoroutines = new HashSet<CoroutineBase>();
        private HashSet<CoroutineBase> coroutinesToDeactivate = new HashSet<CoroutineBase>();
        private HashSet<CoroutineBase> coroutinesToActivate = new HashSet<CoroutineBase>();

        public void StartCoroutine(CoroutineBase coroutine)
        {
            coroutine.manager = this;
            coroutine.OnEnter();
        }

        public void ActivateCoroutine(CoroutineBase coroutine)
        {
            if (isIteratingActiveCoroutines)
            {
                coroutinesToActivate.Add(coroutine);
            }
            else
            {
                activeCoroutines.Add(coroutine);
            }
        }

        public void DeactivateCoroutine(CoroutineBase coroutine)
        {
            if (isIteratingActiveCoroutines)
            {
                coroutinesToDeactivate.Add(coroutine);
            }
            else
            {
                activeCoroutines.Remove(coroutine);
            }
        }

        public override void _EnterTree()
        {
            Instance = this;
        }

        public override void _Process(float delta)
        {
            DeltaTime = delta;

            isIteratingActiveCoroutines = true;

            foreach (CoroutineBase coroutine in activeCoroutines)
            {
                if (coroutine.isAlive && coroutine.isPlaying)
                {
                    try
                    {
                        coroutine.Update();
                    }
                    catch (Exception e)
                    {
                        GD.PrintErr(e.ToString());
                    }
                }
            }

            isIteratingActiveCoroutines = false;

            foreach (CoroutineBase coroutine in coroutinesToActivate)
            {
                activeCoroutines.Add(coroutine);
            }
            coroutinesToActivate.Clear();

            foreach (CoroutineBase coroutine in coroutinesToDeactivate)
            {
                activeCoroutines.Remove(coroutine);
            }
            coroutinesToDeactivate.Clear();
        }
    }
}
