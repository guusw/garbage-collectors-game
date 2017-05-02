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
using SiliconStudio.Xenko.Rendering;
using SiliconStudio.Xenko.Rendering.Materials;

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

        [DataMemberIgnore]
        public ModelComponent ModelComponent { get; private set; }

        public Material MaterialTemplate;

        public float InitialAngularVelocityMultiplier { get; set; } = 0.2f;
        public float InitialVelocityMultiplier { get; set; } = 1.0f;

        private static Random random = new Random();

        private float stateTimer = 0.0f;

        public float CollectionPercentage { get; private set; } = 0.0f;
        public float CollectionDecaySpeed { get; set; } = 2.0f;
        private float timeSinceCollect = 0.0f;

        public override void Start()
        {
            Model.Transform.Scale = Vector3.Zero;
            ModelComponent = Model.Get<ModelComponent>();
            ModelComponent.Enabled = true;
            ModelComponent.Materials.Add(0, MaterialTemplate);

            Model.Transform.Rotation = Quaternion.RotationYawPitchRoll(
                (float)random.NextDouble() * MathUtil.TwoPi,
                (float)random.NextDouble() * MathUtil.TwoPi,
                (float)random.NextDouble() * MathUtil.TwoPi);

            Rigidbody = Entity.Get<RigidbodyComponent>();
            Rigidbody.LinearFactor = new Vector3(1, 1, 0);
            Rigidbody.AngularFactor = new Vector3(1, 1, 1);
            Rigidbody.OverrideGravity = true;
            Rigidbody.Gravity = Vector3.Zero;
            Rigidbody.CollisionGroup = CollisionFilterGroups.CustomFilter1;
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
                    // Curve + bounce
                    float curve = Bezier.Compute(t, .17f, .67f, .76f, 1.14f);
                    curve = curve < 1.0f ? curve : 2.0f - curve;
                    Model.Transform.Scale = new Vector3(curve);
                    break;
                case GarbageState.Idle:
                    break;
            }

            float dt = (float)Game.UpdateTime.Elapsed.TotalSeconds;
            stateTimer += dt;
            timeSinceCollect += dt;

            if (timeSinceCollect > 0.2f)
            {
                CollectionPercentage = Math.Max(0, CollectionPercentage - dt * CollectionDecaySpeed);
                if (CollectionPercentage <= 0)
                {
                    CollectionAnimTick();
                }
            }
            if (CollectionPercentage > 0.0f)
            {
                CollectionAnimTick();
            }
        }

        public bool Collect(CollectionArea area, float speed)
        {
            if (CollectionPercentage > 1.0f)
            {
                return true;
            }
            timeSinceCollect = 0.0f;
            CollectionPercentage += speed;
            return false;
        }

        private void CollectionAnimTick()
        {
            float scale = 1.0f - CollectionPercentage;
            Model.Transform.Scale = new Vector3(scale * .7f + .3f);
            SetGlow(CollectionPercentage);
        }

        private void OnIdleEnter()
        {
            Rigidbody.AngularVelocity = new Vector3(
                                            (float)random.NextDouble() * MathUtil.TwoPi,
                                            (float)random.NextDouble() * MathUtil.TwoPi,
                                            (float)random.NextDouble() * MathUtil.TwoPi) * 2.0f -
                                        1.0f * InitialAngularVelocityMultiplier;
            Rigidbody.AngularDamping = 0.0f;

            Vector3 d = new Vector3(
                            (float)random.NextDouble(),
                            (float)random.NextDouble(),
                            0.5f) * 2.0f - 1.0f;
            if (d.LengthSquared() > 0)
            {
                d.Normalize();
                Rigidbody.LinearVelocity = d * InitialVelocityMultiplier;
            }
        }

        private void SetGlow(float glow)
        {
            MaterialTemplate.Parameters.Set(MaterialKeys.EmissiveIntensity, glow);
        }
    }
}