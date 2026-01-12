namespace PolariumClone.CustomData
{
    public class LevelData
    {
        public string Name { get; set; }
        public string PreviousLevelName { get; set; }
        public string NextLevelName { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int[] Board { get; set; }
    }
}
