using SiliconStudio.Xenko.Engine;

namespace GarbageCollectors
{
    class GarbageCollectorsApp
    {
        static void Main(string[] args)
        {
            using (var game = new Game())
            {
                game.Run();
            }
        }
    }
}
