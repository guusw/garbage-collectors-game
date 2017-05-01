namespace GarbageCollectors
{
    public struct InputState
    {
        public static readonly InputState None = new InputState
        {
            Rotation = 0.0f,
            Acceleration = 0.0f,
            Brake = 0.0f
        };

        public float Rotation;
        public float Acceleration;
        public float Brake;
    }
}