// Copyright (c) 2011-2017 Silicon Studio Corp. All rights reserved. (https://www.siliconstudio.co.jp)
// See LICENSE.md for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SiliconStudio.Core;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Input;
using SiliconStudio.Xenko.Engine;

namespace GarbageCollectors
{
    public class SpawnScript : AsyncScript
    {
        private readonly List<Entity> instances = new List<Entity>();

        public Prefab Prefab { get; set; }

        [DataMemberIgnore]
        public IReadOnlyList<Entity> Instances => instances;

        public override async Task Execute()
        {
            while(Game.IsRunning)
            {
                if (Input.IsKeyPressed(Keys.Y))
                {
                    Spawn();
                }

                // Do stuff every new frame
                await Script.NextFrame();
            }
        }

        void Spawn()
        {
            if (Prefab == null)
                return;

            Vector3 spawnPosition = Entity.Transform.Position;

            var newInstance = Prefab.Instantiate().First();
            SceneSystem.SceneInstance.RootScene.Entities.Add(newInstance);

            newInstance.Transform.Position = spawnPosition;

            instances.Add(newInstance);
        }
    }
}
