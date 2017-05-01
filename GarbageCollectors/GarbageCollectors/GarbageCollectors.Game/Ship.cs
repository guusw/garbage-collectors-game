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
    public class Ship : SyncScript
    {
        public static readonly float ZDepth = 0.0f;
        public static readonly float InputEpsilon = 0.0001f;

        [DataMemberIgnore]
        public float Rotation
        {
            get => Entity.Transform.RotationEulerXYZ.Z;
            set => Entity.Transform.Rotation = Quaternion.RotationZ(value);
        }

        [DataMemberIgnore]
        public RigidbodyComponent Rigidbody { get; private set; }

        [DataMemberIgnore]
        public Vector2 Velocity => Rigidbody.LinearVelocity.XY();
        [DataMemberIgnore]
        public Vector2 Position => Entity.Transform.Position.XY();
        [DataMemberIgnore]
        public Vector2 Forward => Vector2.Normalize(Entity.Transform.WorldMatrix.Up.XY());

        public AngleSingle RotationSpeed = new AngleSingle(200.0f, AngleType.Degree);
        public float AcclerationSpeed = 3.0f;
        public float MaximumSpeed = 1.0f;

        private InputState input;

        public override void Start()
        {
            // After input
            Priority = 2;

            Rigidbody = Entity.Get<RigidbodyComponent>();
            Rigidbody.LinearFactor = new Vector3(1,1,0);
            Rigidbody.AngularFactor = new Vector3(0,0,1);
            Rigidbody.OverrideGravity = true;
            Rigidbody.Gravity = Vector3.Zero;
        }

        public override void Update()
        {
            // Apply forces
            Vector2 newVelocity = Velocity;

            float speedMult = (float)Game.UpdateTime.Elapsed.TotalSeconds;

            if (Math.Abs(input.Rotation) > InputEpsilon)
            {
                float addRotaton = RotationSpeed.Radians * input.Rotation;
                Rigidbody.AngularVelocity = new Vector3(0, 0, addRotaton);
            }
            else
            {
                Rigidbody.AngularVelocity = Vector3.Zero;
            }

            if (Math.Abs(input.Acceleration) > InputEpsilon)
            {
                float addAcceleration = AcclerationSpeed * input.Acceleration * speedMult;
                newVelocity += Forward * addAcceleration;
            }

            // Maximum speed
            float velocity = newVelocity.Length();
            if (velocity > 0)
            {
                Vector2 newVelocityDir = newVelocity / velocity;
                velocity = MathUtil.Clamp(velocity, 0.0f, MaximumSpeed);
                newVelocity = newVelocityDir * velocity;
            }

            // Update position and velocity while completely ignoring the z axis
            Rigidbody.LinearVelocity = new Vector3(newVelocity, 0.0f);

            // Clear input after processing it
            input = InputState.None;
        }

        public void SendInput(InputState input)
        {
            this.input = input;
            input.Rotation = MathUtil.Clamp(input.Rotation, -1.0f, 1.0f);
            input.Acceleration = MathUtil.Clamp(input.Acceleration, -1.0f, 1.0f);
            input.Brake = (input.Brake < 0.5f) ? 0.0f : 1.0f;
        }
    }
}

