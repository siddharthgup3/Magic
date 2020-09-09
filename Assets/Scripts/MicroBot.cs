using UnityEngine;
using System.Collections.Generic;

namespace Phoenix
{
    public class MicroBot
    {
        public MicroBot parentBot;
        public Transform botTransform;
        public List<MicroBot> childrenBots;
        public Color botColor;

        public bool isRendering;
        
        public MicroBot(MicroBot parentBot, Transform botTransform, Color botColor)
        {
            this.parentBot = parentBot;
            this.botTransform = botTransform;
            this.botColor = botColor;
            childrenBots = new List<MicroBot>();
            this.isRendering = true;
        }

        public MicroBot(MicroBot parentBot, Transform botTransform)
        {
            this.parentBot = parentBot;
            this.botTransform = botTransform;
            childrenBots = new List<MicroBot>();
            this.isRendering = false;
        }
    }
}