// Copyright (c) 2011-2017 Silicon Studio Corp. All rights reserved. (https://www.siliconstudio.co.jp)
// See LICENSE.md for full license information.

using System;
using SiliconStudio.Core;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Engine;

namespace GarbageCollectors
{
    [DataContract]
    public class BackgroundLayer : SyncScript
    {
        public AngleSingle RotationSpeed { get; set; } = new AngleSingle();
        public AngleSingle CircularMovement { get; set; } = new AngleSingle();
        public float CircularMovementRadius { get; set; } = 0.0f;

        private Vector3 startingPosition;
        private float circularMovementPhase = 0.0f;

        public override void Start()
        {
            // Initialization of the script.
            Entity.Transform.UpdateWorldMatrix();
            startingPosition = Entity.Transform.WorldMatrix.TranslationVector;

            circularMovementPhase = (float)new Random().NextDouble() * MathUtil.TwoPi;
        }

        public override void Update()
        {
            float dt = (float)Game.UpdateTime.Elapsed.TotalSeconds;
            Entity.Transform.Rotation *= Quaternion.RotationZ(RotationSpeed.Radians * dt);
            
            Vector2 newPosition = new Vector2(
                (float)Math.Cos(circularMovementPhase),
                (float)Math.Sin(circularMovementPhase)) * CircularMovementRadius;
            Entity.Transform.Position = startingPosition + new Vector3(newPosition, 0.0f);

            circularMovementPhase += dt * CircularMovement.Radians;
        }
    }
}
