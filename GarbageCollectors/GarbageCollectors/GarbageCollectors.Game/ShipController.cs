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
    public class ShipController : SyncScript
    {
        public Ship Ship { get; set; }
        public Player Player { get; set; }

        public override void Start()
        {
            // Input should go first
            Priority = 0;
        }

        public override void Update()
        {
            if (Player == null || Player.Manager == null)
                return;

            if (Player.Manager.State == GameState.GameOver)
                return;

            InputState input = InputState.None;
            ReadInputState(Player.Index, ref input);
        }

        void ReadInputState(int playerIndex, ref InputState state)
        {
            //if (playerIndex == 0)
            //{
            //    ReadKeyboardInputState(ref state);
            //}
            //else
            {
                ReadControllerInputState(playerIndex, ref state);
            }

            Ship?.SendInput(state);
        }

        void ReadKeyboardInputState(ref InputState state)
        {
            if (Input.IsKeyDown(Keys.A) || Input.IsKeyDown(Keys.Left))
                state.Rotation += 1.0f;
            if (Input.IsKeyDown(Keys.D) || Input.IsKeyDown(Keys.Right))
                state.Rotation -= 1.0f;
            if (Input.IsKeyDown(Keys.S) || Input.IsKeyDown(Keys.Down))
                state.Acceleration -= 1.0f;
            if (Input.IsKeyDown(Keys.W) || Input.IsKeyDown(Keys.Up))
                state.Acceleration += 1.0f;
            if (Input.IsKeyDown(Keys.LeftShift) || Input.IsKeyDown(Keys.RightShift))
                state.Brake = 1.0f;
        }

        void ReadControllerInputState(int gamePadIndex, ref InputState state)
        {
            var gamePadState = Input.GetGamePad(gamePadIndex);
            state.Rotation -= gamePadState.LeftThumb.X;
            state.Acceleration += gamePadState.RightThumb.Y;
            state.Brake = gamePadState.LeftTrigger;
        }
    }
}
