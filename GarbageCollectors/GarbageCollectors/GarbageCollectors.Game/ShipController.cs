// Copyright (c) 2011-2017 Silicon Studio Corp. All rights reserved. (https://www.siliconstudio.co.jp)
// See LICENSE.md for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Input;
using SiliconStudio.Xenko.Engine;

namespace GarbageCollectors
{
    struct InputState
    {
        public float Rotation;
        public float Acceleration;
    }
    public class ShipController : SyncScript
    {
        public Ship Ship { get; set; }
        public int PlayerIndex { get; set; } = 0;

        public override void Start()
        {
            // Input should go first
            Priority = 0;
        }

        public override void Update()
        {
            InputState input = new InputState
            {
                Acceleration = 0.0f,
                Rotation = 0.0f,
            };

            ReadInputState(PlayerIndex, ref input);
        }

        void ReadInputState(int playerIndex, ref InputState state)
        {
            if (playerIndex == 0)
            {
                ReadKeyboardInputState(ref state);
            }
            else
            {
                ReadControllerInputState(playerIndex - 1, ref state);
            }

            if (Ship == null)
                return;

            Ship.Rotate(state.Rotation);
            Ship.Accelerate(state.Acceleration);
        }

        void ReadKeyboardInputState(ref InputState state)
        {
            if (Input.IsKeyDown(Keys.A))
                state.Rotation -= 1.0f;
            if (Input.IsKeyDown(Keys.D))
                state.Rotation += 1.0f;
        }

        void ReadControllerInputState(int gamePadIndex, ref InputState state)
        {
            var gamePadState = Input.GetGamePad(gamePadIndex);
            state.Rotation += gamePadState.LeftThumb.X;
            state.Acceleration += gamePadState.RightThumb.Y;
        }
    }
}
