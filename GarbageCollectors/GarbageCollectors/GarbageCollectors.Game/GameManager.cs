// Copyright (c) 2011-2017 Silicon Studio Corp. All rights reserved. (https://www.siliconstudio.co.jp)
// See LICENSE.md for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconStudio.Core;
using SiliconStudio.Xenko.Input;
using SiliconStudio.Xenko.Engine;

namespace GarbageCollectors
{
    public class GameManager : AsyncScript
    {
        public List<SpawnScript> ObjectSpawners { get; private set; } = new List<SpawnScript>();
        public List<Team> Teams { get; private set; } = new List<Team>();

        [DataMemberIgnore]
        public List<Player> Players { get; private set; } = new List<Player>();

        public override async Task Execute()
        {
            // Count players
            for(int i = 0; i < Teams.Count; i++)
            {
                Teams[i].AssignTeamIndex(this, i);
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

            while (Game.IsRunning)
            {
                // Do stuff every new frame
                await Script.NextFrame();
            }
        }

        IEnumerable<Player> EnumeratePlayers()
        {
            yield return new Player(0);

            for(int i = 0; i < Input.GamePadCount; i++)
                yield return new Player(i+1);
        }
    }
}
