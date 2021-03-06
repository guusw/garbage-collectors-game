using System.Collections.Generic;
using SiliconStudio.Core;
using SiliconStudio.Core.Mathematics;

namespace GarbageCollectors
{
    [DataContract]
    public class Team
    {
        public Color4 Color = Color4.White;
        public int Index { get; private set; }

        [DataMemberIgnore]
        public IReadOnlyList<Player> Players => players;

        public PlayerSpawner Spawner;

        public CollectionArea CollectionArea;

        public int Points { get; private set; } = 0;

        private GameManager gameManager;
        private List<Player> players = new List<Player>();


        public bool HasPlayer(Player player)
        {
            return players.Contains(player);
        }

        internal void AssignTeamIndex(GameManager mgr, int index)
        {
            gameManager = mgr;
            Index = index;
        }

        internal void RemovePlayer(Player player)
        {
            players.Remove(player);
        }

        internal void AddPlayer(Player player)
        {
            players.Add(player);
        }

        internal void OnGarbageCollected(Garbage garbage)
        {
            Points++;
        }

        /// <summary>
        /// Reset game state (points)
        /// </summary>
        internal void Reset()
        {
            Points = 0;
        }
    }
}