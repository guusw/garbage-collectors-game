using System;
using System.Collections.Generic;
using System.Linq;
using SiliconStudio.Core;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Engine;

namespace GarbageCollectors
{
    [DataContract]
    public class BaseSpawner : StartupScript 
    {
        public Prefab Prefab { get; set; }

        [DataMemberIgnore]
        public IReadOnlyList<Entity> Instances => instances;
        
        public event Action<EntityComponent> InstanceRemoved;

        private readonly List<Entity> instances = new List<Entity>();

        public override void Start()
        {
            base.Start();
            SceneSystem.SceneInstance.EntityRemoved += SceneInstanceOnEntityRemoved;
        }

        public override void Cancel()
        {
            base.Cancel();
            SceneSystem.SceneInstance.EntityRemoved -= SceneInstanceOnEntityRemoved;
        }

        public Entity Spawn(Vector3 position)
        {
            if (Prefab == null)
                return null;

            Vector3 spawnPosition = Entity.Transform.Position;

            var newInstance = Prefab.Instantiate().First();
            SceneSystem.SceneInstance.RootScene.Entities.Add(newInstance);

            newInstance.Transform.Position = spawnPosition;

            var component = newInstance.Get<EntityComponent>();
            instances.Add(newInstance);

            return newInstance;
        }

        protected virtual void SceneInstanceOnEntityRemoved(object sender, Entity entity)
        {
            instances.Remove(entity);
        }
    }
}