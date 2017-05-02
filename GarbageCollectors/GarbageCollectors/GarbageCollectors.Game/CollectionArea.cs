// Copyright (c) 2011-2017 Silicon Studio Corp. All rights reserved. (https://www.siliconstudio.co.jp)
// See LICENSE.md for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconStudio.Core;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Input;
using SiliconStudio.Xenko.Engine;
using SiliconStudio.Xenko.Particles.Components;

namespace GarbageCollectors
{
    public enum CollectionAreaState
    {
        Idle,
        Collecting,
    }

    public class CollectionArea : Grabber
    {
        private GameManager gameManager;

        [DataMemberIgnore]
        public Team Team { get; private set; }
        public CollectionAreaState State { get; private set; } = CollectionAreaState.Idle;
        public float CollectRadius = 0.2f;
        public float CollectionSpeed = 1.0f;

        public ParticleSystemComponent CollectEffect;
        public ParticleSystemComponent ContinuousEffect;

        public CollectionArea()
        {
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Update()
        {
            if (Team != null)
            {
                switch (State)
                { 
                    case CollectionAreaState.Idle:
                        IdleTick();
                        break;
                    case CollectionAreaState.Collecting:
                        CollectingTick();
                        break;
                }
            }
        }

        void IdleTick()
        {
            UpdateGrab();
            if (LastPullTarget != null)
            {
                Vector3 delta = LastPullTarget.Entity.Transform.WorldMatrix.TranslationVector -
                                Entity.Transform.WorldMatrix.TranslationVector;
                if (delta.Length() < CollectRadius)
                {
                    State = CollectionAreaState.Collecting;
                }
            }
        }

        void CollectingTick()
        {
            if (LastPullTarget == null)
            {
                State = CollectionAreaState.Idle;
                return;
            }

            Vector3 delta = LastPullTarget.Entity.Transform.WorldMatrix.TranslationVector -
                            Entity.Transform.WorldMatrix.TranslationVector;
            if (delta.Length() > CollectRadius)
            {
                State = CollectionAreaState.Idle;
                return;
            }

            if (LastPullTarget.Collect(this, CollectionSpeed * (float)Game.UpdateTime.Elapsed.TotalSeconds))
            {
                OnCollect(LastPullTarget);
                State = CollectionAreaState.Idle;
            }
        }

        private void OnCollect(Garbage garbage)
        {
            SceneSystem.SceneInstance.RootScene.Entities.Remove(garbage.Entity);
            Team.OnGarbageCollected(garbage);
            CollectEffect.ParticleSystem.Play();
            CollectEffect.ParticleSystem.ResetSimulation();
        }

        internal void AssignTeam(GameManager mgr, Team team)
        {
            Team = team;
            gameManager = mgr;
            ContinuousEffect.Color = team.Color;
        }
    }
}