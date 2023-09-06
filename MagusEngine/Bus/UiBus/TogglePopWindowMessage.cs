﻿namespace MagusEngine.Bus.UiBus
{
    public class TogglePopWindowMessage
    {
        public bool NoPopWindow { get; set; }

        public TogglePopWindowMessage(bool isFocused)
        {
            NoPopWindow = isFocused;
        }
    }
}
