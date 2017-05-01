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
using SiliconStudio.Xenko.Physics;

namespace GarbageCollectors
{
    public class Grabber : SyncScript
    {
        [DataMemberIgnore]
        public RigidbodyComponent Rigidbody { get; private set; }

        public float PullFalloffRadius = 10.0f;
        public float PullStrengthMultiplier = 10.0f;

        private Garbage lastPullTarget = null;

        public override void Start()
        {
            Rigidbody = Entity.Get<RigidbodyComponent>();
            Rigidbody.IsKinematic = true;
            Rigidbody.IsTrigger = true;
        }

        public override void Update()
        {
            float closestDistanceSquared = float.MaxValue;
            Vector3 closestDelta = Vector3.Zero;
            Garbage closest = null;
            foreach (var collision in Rigidbody.Collisions)
            {
                var a = collision.ColliderA;
                var b = collision.ColliderB;
                if(b == Rigidbody)
                    Utilities.Swap(ref a, ref b);
                var garbage = b.Entity.Get<Garbage>();
                if (garbage == null)
                    continue;
                Vector3 d = b.Entity.Transform.WorldMatrix.TranslationVector -
                    Entity.Transform.WorldMatrix.TranslationVector;
                float l = d.LengthSquared();
                if (l < closestDistanceSquared)
                {
                    closestDistanceSquared = l;
                    closestDelta = d;
                    closest = garbage;
                }
            }

            if (closest != null)
            {
                float l = closestDelta.Length();
                float pullStrength = (float)Math.Min(1.0f, Math.Pow(l / PullFalloffRadius, 2.0f));

                closestDelta.Z = 0.0f;
                Vector3 direction = Vector3.Normalize(closestDelta);
                closest.Rigidbody.LinearVelocity += -direction * pullStrength * PullStrengthMultiplier * (float)Game.UpdateTime.Elapsed.TotalSeconds;
            }

            lastPullTarget = closest;
        }
    }
}
