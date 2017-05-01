using SiliconStudio.Core;
using SiliconStudio.Core.Extensions;

namespace GarbageCollectors
{
    [DataContract]
    public class Player
    {
        [DataMemberIgnore]
        public int Index { get; private set; } = 0;

        [DataMemberIgnore]
        public int TeamIndex => Team?.Players.IndexOf(this) ?? -1;

        [DataMemberIgnore]
        public Team Team { get; private set; }

        [DataMemberIgnore]
        public Ship Ship { get; private set; }

        private GameManager gameManager;

        public Player(int index)
        {
            Index = index;
        }
        
        internal void AssignTeam(GameManager mgr, Team team)
        {
            Team?.RemovePlayer(this);
            gameManager = mgr;
            Team = team;
            Team?.AddPlayer(this);
        }

        public void AssignShip(Ship ship)
        {
            Ship = ship;
        }
    };
}