using System;

namespace PolariumClone.UI
{
    public class LevelSelectedEventArgs : EventArgs
    {
        public LevelSelectedEventArgs(string levelName)
        {
            LevelName = levelName;
        }

        public string LevelName { get; private set; }
    }
}
