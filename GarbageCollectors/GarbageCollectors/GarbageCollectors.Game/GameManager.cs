// Copyright (c) 2011-2017 Silicon Studio Corp. All rights reserved. (https://www.siliconstudio.co.jp)
// See LICENSE.md for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
        GameOver
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

        public float GameTimer { get; private set; } = 0;
        public float TimeLimit = 60;
        public float TimeRemaining => Math.Max(0, TimeLimit - GameTimer);
        
        public override async Task Execute()
        {
            // no UI lol
            //Console.SetWindowSize(200,80);
            Console.Title = "Scoreboard";
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("PAD: (L)->Rotate (R)->Accelerate [LT]->Brake");
            Console.WriteLine("KEYB: [WASD/Arrows]->Rotate & Accelerate [SHIFT]->Brake [R]->Restart");
            Console.WriteLine("-------------------------------------------------------");

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
                        StartTick();
                        break;
                    case GameState.Running:
                        RunningTick();
                        break;
                }
                
                if (Input.IsKeyDown(Keys.R)) // Restart
                {
                    Reset();
                }

                // Do stuff every new frame
                await Script.NextFrame();
            }
        }

        private void StartTick()
        {
            SpawnPlayers();
            State = GameState.Running;
        }

        private int consoleFrame = 0;

        void RunningTick()
        {
            float dt = (float)Game.UpdateTime.Elapsed.TotalSeconds;
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

            objectSpawnerTimeout -= dt;

            GameTimer += dt;

            if (TimeRemaining <= 0)
            {
                State = GameState.GameOver;
                GameOverEnter();
            }

            if ((consoleFrame++) >= 10)
            {
                var sortedTeams = Teams.OrderByDescending(r => r.Points);
                int highestPoints = sortedTeams.First().Points;

                // Show game timer
                Console.SetCursorPosition(0, 3);
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
                int it = (int)TimeRemaining;
                char d = (it % 2) == 0 ? '~' : '-';
                Console.WriteLine(PadString("\t\t{0} Time Remaining: {1:0.##} {0}".ToFormat(d, TimeRemaining)));

                // Update score
                int i = 0;
                foreach(var team in sortedTeams)
                {

                    if (team.Points > 0 && team.Points == highestPoints)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    string s = " Team {0}>  {1}".ToFormat(team.Index + 1, team.Points);
                   
                    Console.WriteLine(PadString(s));
                    i++;
                }
                consoleFrame = 0;
            }
        }

        private string PadString(string inStr, int max = 32)
        {
            string padded = "";
            for (int j = 0; j < max; j++)
            {
                if (j < inStr.Length)
                    padded += inStr[j];
                else
                    padded += " ";
            }
            return padded;
        }

        private void GameOverEnter()
        {
            Console.SetCursorPosition(0, 3);
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Red;
            int it = (int)TimeRemaining;
            char d = (it % 2) == 0 ? '~' : '-';
            Console.WriteLine(PadString("\t\t- Game over -"));
        }

        void SpawnPlayers()
        {
            foreach (var player in Players)
            {
                player.Team.Spawner.SpawnForPlayer(player);
            }
        }

        void Reset()
        {
            GameTimer = 0;

            foreach (var player in Players)
                player.AssignShip(null);

            foreach (var team in Teams)
            {
                team.Reset();
                team.Spawner.Reset();
            }

            objectSpawner.Reset();

            State = GameState.Starting;
        }

        private Vector3 SelectSpawnPosition()
        {
            int spawnJitter = random.Next(-1, 1);
            var actualSpawner = objectSpawnPoints[Math.Abs(spawnIndex + spawnJitter) % objectSpawnPoints.Count];
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