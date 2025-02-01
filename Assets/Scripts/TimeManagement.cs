using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Forest
{
    public class TimeManagement : MonoBehaviour
    {
        private TimeSpan elasped;
        private DateTime lastDate;

        private bool isPause;
        public TimeSpan CurrentElasped => elasped + (DateTime.Now - lastDate);

        private List<Timed> managed = new();

        public void Initialize()
        {
            elasped = TimeSpan.Zero;
            lastDate = DateTime.Now;
            isPause = true;
        }

        public TimeSpan SaveTime()
        {
            TimeSpan span = DateTime.Now - lastDate;
            elasped += span;
            lastDate = DateTime.Now;
            return elasped;
        }

        [ShowIf(nameof(isPause))]
        [Button]
        public void Resume()
        {
            if (isPause)
            {
                isPause = false;
                lastDate = DateTime.Now;
            }
        }

        [HideIf(nameof(isPause))]
        [Button]
        public void Pause()
        {
            if (!isPause)
            {
                isPause = true;
                SaveTime();
            }
        }

        public void Register(Timed t)
        {
            managed.Add(t);
            t.StartTime = CurrentElasped;
            t.Start();
        }

        public void Unregister(Timed t)
        {
            managed.Remove(t);
        }

        protected void FixedUpdate()
        {
            if (!isPause)
            {
                for (int i = 0; i < managed.Count; i++)
                {
                    Timed t = managed[i];
                    if (IsExpired(t))
                    {
                        t.Expire();

                        if (t.IsLooping)
                        {
                            t.StartTime += TimeSpan.FromSeconds(t.Duration);
                        }
                        else
                        {
                            Unregister(t);
                            i--;
                        }
                    }
                }
            }
        }

        public bool IsExpired(Timed t)
        {
            return t.HasDuration && t.StartTime + TimeSpan.FromSeconds(t.Duration) < CurrentElasped;
        }
    }

    public abstract class Timed
    {
        public TimeSpan StartTime;
        public abstract double Duration { get; }
        public bool HasDuration => Duration > 0;
        public abstract bool IsLooping { get; }

        public abstract void Start();
        public abstract void Expire();
    }

    public class Tick : Timed
    {
        public delegate void TickTrigger();
        public event TickTrigger OnTickTriggered;

        public Variable DurationTick;
        public override double Duration => DurationTick.ModifiedValue;
        public override bool IsLooping => true;

        public override void Start()
        {

        }

        public override void Expire()
        {
            OnTickTriggered?.Invoke();
        }
    }
}
