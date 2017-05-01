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
    public enum GarbageState
    {
        Appear,
        Idle,
    }

    /// <summary>
    /// AKA A collectible object
    /// </summary>
    public class Garbage : SyncScript
    {
        public GarbageState State { get; private set; } = GarbageState.Appear;
        public Entity Model { get; set; }

        [DataMemberIgnore]
        public RigidbodyComponent Rigidbody { get; private set; }
        public ModelComponent ModelComponent { get; private set; }
        
        public float InitialAngularVelocityMultiplier { get; set; } = 0.2f;
        public float InitialVelocityMultiplier { get; set; } = 1.0f;

        private static Random random = new Random();

        private float stateTimer = 0.0f;

        public override void Start()
        {
            Model.Transform.Scale = Vector3.Zero;
            ModelComponent = Model.Get<ModelComponent>();
            ModelComponent.Enabled = true;

            Model.Transform.Rotation = Quaternion.RotationYawPitchRoll(
                (float)random.NextDouble() * MathUtil.TwoPi,
                (float)random.NextDouble() * MathUtil.TwoPi,
                (float)random.NextDouble() * MathUtil.TwoPi);

            Rigidbody = Entity.Get<RigidbodyComponent>();
            Rigidbody.LinearFactor = new Vector3(1, 1, 0);
            Rigidbody.AngularFactor = new Vector3(1, 1, 1);
            Rigidbody.OverrideGravity = true;
            Rigidbody.Gravity = Vector3.Zero;
        }

        public override void Update()
        {
            // Spawning animation
            switch (State)
            {
                case GarbageState.Appear:
                    float t = stateTimer / 0.5f;
                    if (t >= 1.0f)
                    {
                        t = 1.0f;
                        State = GarbageState.Idle;
                        OnIdleEnter();
                    }
                    float curve = Bezier.Compute(t, .17f, .67f, .76f, 1.14f);
                    curve = curve < 1.0f ? curve : 2.0f - curve;
                    Model.Transform.Scale = new Vector3(curve);
                    break;
                case GarbageState.Idle:
                    break;
            }

            stateTimer += (float)Game.UpdateTime.Elapsed.TotalSeconds;
        }

        private void OnIdleEnter()
        {
            // TODO: Give initial velocity/spin
            Rigidbody.AngularVelocity = new Vector3(
                (float)random.NextDouble() * MathUtil.TwoPi,
                (float)random.NextDouble() * MathUtil.TwoPi,
                (float)random.NextDouble() * MathUtil.TwoPi) * InitialAngularVelocityMultiplier;
            Rigidbody.AngularDamping = 0.0f;

            Vector3 d = new Vector3(
                (float)random.NextDouble(),
                (float)random.NextDouble(),
                0);
            if (d.LengthSquared() > 0)
            {
                d.Normalize();
                Rigidbody.LinearVelocity = d * InitialVelocityMultiplier;
            }
        }
    }
}