using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
	partial class Program
	{
        public class Text : IComplexElement, IRuntimeUpdatable
        {
            public Vector2I Position { get { return position; } set { position = value; } }
            public Color Color { get { return color; } set { color = value;} }
            public Canvas Canvas { get { return canvas; }}

            public bool Updated { get; set; }

            private Vector2I position;
            private Color color;
            private Canvas canvas;

            private string text;

            private int size;
            private Vector2I leftCenter;

            private int charSizeStepX;

            public Text(Vector2I position, string text, Color color, int size)
            {
                this.position = position;
                this.color = color;
                this.size = size;
                this.text = text.ToUpper();
            }

            public void UpdateValue(string text)
            {
                this.text = text.ToUpper();
                Updated = true;
            }

            public IEnumerator<bool> Generate()
            {
                charSizeStepX = FontLibrary.CHARWIDTH * size + 1;

                int sizeX = (text.Length * charSizeStepX) + 2;
                int sizeY = (FontLibrary.CHARHEIGHT * size + 1) + 1;

                leftCenter = new Vector2I(FontLibrary.CHARWIDTH / 2 + 1, (FontLibrary.CHARHEIGHT * size) / 2 + 1);

                canvas = new Canvas(sizeX, sizeY);



                char pixel = MonospaceUtils.GetColorChar(color);
                canvas.Clear();

                for (int x = 0; x < text.Length; x++)
                {
                    yield return true;

                    var charPos = leftCenter;
                    charPos.X += x * charSizeStepX;

                    var character = new ResizableCharacter(charPos, text[x], Color.White, size);
                    canvas.MergeCanvas(character.Draw(), character.Position);
                }
            }

            public Canvas Draw()
            {
                return canvas;
            }
        }
    }
}
