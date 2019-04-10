using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;
using VRage.Game.GUI.TextPanel;

namespace IngameScript
{
    partial class Program
    {
        public class TextSurfaceDrawing
        {
            public IMyTextSurface textSurface;
            public Vector2 SurfaceSize => textSurface.SurfaceSize;
            public int SpriteCount => sprites.Count;


            private Dictionary<long, MySprite> sprites = new Dictionary<long, MySprite>();

            public TextSurfaceDrawing(IMyTextSurface textSurface)
            {
                this.textSurface = textSurface;
            }

            public void Draw()
            {
                using (MySpriteDrawFrame frame = textSurface.DrawFrame())
                {
                    frame.AddRange(sprites.Values);
                }
            }

            public void SetSquare(Vector2 position, Vector2 size, Color color, long id)
            {
                var SquareSprite = new MySprite(SpriteType.TEXTURE, "Square", position, size, color);
                sprites[id] = SquareSprite;
            }

            public void SetSprite(MySprite sprite, long id)
            {
                sprites[id] = sprite;
            } 
        }
    }
}
