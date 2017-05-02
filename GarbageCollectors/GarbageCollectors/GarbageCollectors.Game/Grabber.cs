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
        
        public float PullStrengthMultiplier = 10.0f;
        public float BrakeStrengthMultiplier = 1.0f;

        private float PullFarFalloffRadius;

        protected Garbage LastPullTarget { get; private set; } = null;

        public override void Start()
        {
            Rigidbody = Entity.Get<RigidbodyComponent>();
            Rigidbody.IsKinematic = true;
            Rigidbody.IsTrigger = true;
            PullFarFalloffRadius = (Rigidbody.ColliderShape.Description as SphereColliderShapeDesc).Radius;
        }

        public override void Update()
        {
            UpdateGrab();
        }

        public void UpdateGrab()
        {
            float closestDistanceSquared = float.MaxValue;
            Vector3 closestDelta = Vector3.Zero;
            Garbage closest = null;
            foreach (var collision in Rigidbody.Collisions)
            {
                var a = collision.ColliderA;
                var b = collision.ColliderB;
                if (b == Rigidbody)
                    Utilities.Swap(ref a, ref b);
                var garbage = b.Entity.Get<Garbage>();
                if (garbage == null)
                    continue;
                if (!SelectTarget(garbage))
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
                
                // Far distance falloff
                float pullStrength = MathUtil.Clamp(1-(l / PullFarFalloffRadius), 0, 1);



                closestDelta.Z = 0.0f;
                Vector3 direction = Vector3.Normalize(closestDelta);

                //Vector3 newVelocity = closest.Rigidbody.LinearVelocity -
                //                      direction * pullStrength * PullStrengthMultiplier *
                //                      (float)Game.UpdateTime.Elapsed.TotalSeconds;
                //float length = newVelocity.Length();
                closest.Rigidbody.ApplyForce(-direction * pullStrength * PullStrengthMultiplier);
                Vector3 targetVelocity = Vector3.Normalize(closest.Rigidbody.LinearVelocity);
                closest.Rigidbody.ApplyForce(Vector3.Normalize(targetVelocity) * pullStrength * BrakeStrengthMultiplier);
            }

            LastPullTarget = closest;
        }

        protected virtual bool SelectTarget(Garbage garbage)
        {
            return true;
        }
    }
}
