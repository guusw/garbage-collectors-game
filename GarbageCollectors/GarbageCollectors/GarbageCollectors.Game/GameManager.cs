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

namespace GarbageCollectors
{
    public enum GameState
    {
        Starting,
        Running,
    }

    public class GameManager : AsyncScript
    {
        public List<Team> Teams { get; private set; } = new List<Team>();

        [DataMemberIgnore]
        public List<Player> Players { get; private set; } = new List<Player>();

        public GameState State { get; private set; } = GameState.Starting;
        
        public Prefab ObjectPrefab { get; set; }

        public Entity SpawnerRoot { get; set; }

        public Vector2 ObjectSpawnTimoutRange;
        public int MaximumInstances = 10;

        private BaseSpawner objectSpawner;
        private List<ObjectSpawnPoint> objectSpawnPoints  = new List<ObjectSpawnPoint>();
        private Random random = new Random();

        private float objectSpawnerTimeout = 0.0f;
        private int spawnIndex = 0;

        public override async Task Execute()
        {
            // Count players
            for (int i = 0; i < Teams.Count; i++)
            {
                Teams[i].AssignTeamIndex(this, i);
                Teams[i].CollectionArea.AssignTeam(this, Teams[i]);
            }

            if (Teams.Count == 0)
                return;

            // Assign players to teams
            var players = EnumeratePlayers();
            int assignTeam = 0;
            foreach (var player in players)
            {
                player.AssignTeam(this, Teams[assignTeam++]);
                Players.Add(player);
                assignTeam %= Teams.Count;

                player.Team.Spawner.SpawnForPlayer(player);
            }

            // Find spawn points
            foreach (var c in SpawnerRoot.Transform.Children)
            {
                var spawnPoint = c.Entity.Get<ObjectSpawnPoint>();
                if (spawnPoint != null)
                {
                    objectSpawnPoints.Add(spawnPoint);
                }
            }

            // Create object spawner
            var spawnerEntity = new Entity();
            spawnerEntity.Add(objectSpawner = new BaseSpawner());
            SceneSystem.SceneInstance.RootScene.Entities.Add(spawnerEntity);
            objectSpawner.Prefab = ObjectPrefab;

            while (Game.IsRunning)
            {
                switch (State)
                {
                    case GameState.Starting:
                        State = GameState.Running;
                        break;
                    case GameState.Running:
                        RunningTick();
                        break;
                }

                // Do stuff every new frame
                await Script.NextFrame();
            }
        }

        void RunningTick()
        {
            if (objectSpawner.Instances.Count < MaximumInstances)
            {
                if (objectSpawnerTimeout <= 0.0f)
                {
                    objectSpawnerTimeout = (float)random.NextDouble() *
                                           (ObjectSpawnTimoutRange.Y - ObjectSpawnTimoutRange.X) +
                                           ObjectSpawnTimoutRange.X;
                    objectSpawner.Spawn(SelectSpawnPosition());
                }
            }
            objectSpawnerTimeout -= (float)Game.UpdateTime.Elapsed.TotalSeconds;
        }

        private Vector3 SelectSpawnPosition()
        {
            var actualSpawner = objectSpawnPoints[spawnIndex % objectSpawnPoints.Count];
            spawnIndex++;
            return actualSpawner.Position;
        }

        private  IEnumerable<Player> EnumeratePlayers()
        {
            yield return new Player(0);

            for (int i = 0; i < Input.GamePadCount; i++)
                yield return new Player(i + 1);
        }
    }
}