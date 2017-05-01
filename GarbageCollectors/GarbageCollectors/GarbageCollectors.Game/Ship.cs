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
        public const float ZDepth = 0.0f;

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
        public Vector2 Forward => Entity.Transform.WorldMatrix.Forward.XY();

        public AngleSingle RotationSpeed = new AngleSingle(200.0f, AngleType.Degree);
        public float AcclerationSpeed = 3.0f;

        private float accelerationInput;
        private float rotationInput;

        public override void Start()
        {
            // After input
            Priority = 2;

            Rigidbody = Entity.Get<RigidbodyComponent>();
        }

        public override void Update()
        {
            // Apply forces
            Vector2 currentVelocity = Velocity;
            
            Entity.Transform.Rotation = Entity.Transform.Rotation *
                                        Quaternion.RotationZ(RotationSpeed.Radians * rotationInput); 

            // Update position and velocity while completely ignoring the z axis
            Rigidbody.LinearVelocity = new Vector3(currentVelocity, 0.0f);
            Entity.Transform.Position = new Vector3(Position, ZDepth);

            accelerationInput = 0.0f;
            rotationInput = 0.0f;
        }

        public void Rotate(float rotation)
        {
            rotationInput = MathUtil.Clamp(rotationInput + rotation, -1.0f, 1.0f);
        }

        public void Accelerate(float acceleration)
        {
            accelerationInput = MathUtil.Clamp(accelerationInput + acceleration, -1.0f, 1.0f);
        }
    }
}
