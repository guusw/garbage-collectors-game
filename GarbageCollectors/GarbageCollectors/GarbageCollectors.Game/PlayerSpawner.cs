using System.Collections.Generic;
using SiliconStudio.Core;
using SiliconStudio.Core.Annotations;
using SiliconStudio.Xenko.Engine;

namespace GarbageCollectors
{
    [DataContract]
    public class PlayerSpawner : BaseSpawner
    {
        private Dictionary<Entity, Player> playerMap = new Dictionary<Entity, Player>();

        public ShipController SpawnForPlayer(Player player)
        {
            var newEntity = Spawn(Entity.Transform.WorldMatrix.TranslationVector);

            // Create a ship
            Ship newShip = newEntity.Get<Ship>();
            player.AssignShip(newShip);
            newShip.SetOwner(player);

            // Create a controller
            var shipController = new ShipController();
            newEntity.Add(shipController);
            shipController.Player = player;

            return shipController;
        }

        protected override void SceneInstanceOnEntityRemoved(object sender, Entity entity)
        {
            base.SceneInstanceOnEntityRemoved(sender, entity);
            Player player;
            if (playerMap.TryGetValue(entity, out player))
            {
                player.AssignShip(null);
            }
        }
    }
}