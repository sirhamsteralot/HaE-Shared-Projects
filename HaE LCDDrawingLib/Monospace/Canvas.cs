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
        public class Canvas
	    {
            public int sizeX;
            public int sizeY;

            private StringBuilder canvas;

            public StringBuilder ToStringBuilder()
            {
                return canvas;
            }

            public Canvas(int sizeX, int sizeY)
            {
                this.sizeX = sizeX;
                this.sizeY = sizeY;

                canvas = new StringBuilder();

                for (int y = 0; y < sizeY; y++)
                {
                    canvas.Append(' ', sizeX);
                    canvas.Append('\n');
                }
            }

            public void Clear()
            {
                canvas.Clear();

                for (int y = 0; y < sizeY; y++)
                {
                    canvas.Append(' ', sizeX);
                    canvas.Append('\n');
                }
            }

            public void SetBackGround(Color color)
            {
                char pixel = MonospaceUtils.GetColorChar(color);

                for (int x = 0; x < sizeX; x++)
                {
                    for (int y = 0; y < sizeY; y++)
                    {
                        PaintPixel(pixel, x, y);
                    }
                }
            }

            public IEnumerator<bool> SetBackGround(Color color, int maxPerRun)
            {
                char pixel = MonospaceUtils.GetColorChar(color);
                int counter = 0;

                for (int x = 0; x < sizeX; x++)
                {
                    for (int y = 0; y < sizeY; y++)
                    {
                        if (counter++ >= maxPerRun)
                        {
                            counter = 0;
                            yield return true;
                        }

                        PaintPixel(pixel, x, y);
                    }
                }
            }

            public void PaintPixel(char pixel, int posX, int posY)
            {
                int i = CalculateStringPos(posX, posY);

                if (i < canvas.Length && i >= 0)
                    canvas[i] = pixel;
            }

            public void PaintPixel(Color color, int posX, int posY)
            {
                char pixel = MonospaceUtils.GetColorChar(color);

                PaintPixel(pixel, posX, posY);
            }

            public void PaintBitMap(bool[,] bitmap, char pixel, int posX, int posY)
            {
                int bitMapDimX = bitmap.GetLength(0);
                int bitMapDimY = bitmap.GetLength(1);

                int topLeftX = posX - bitMapDimX / 2;
                int topLeftY = posY - bitMapDimY / 2;

                for (int x = 0; x < bitMapDimX; x++)
                {
                    for (int y = 0; y < bitMapDimY; y++)
                    {
                        if (bitmap[x, y])
                            PaintPixel(pixel, topLeftX + x, topLeftY + y);
                    }
                }
            }

            public void PaintBitMap(bool[,] bitmap, Color color, int posX, int posY)
            {
                char pixel = MonospaceUtils.GetColorChar(color);

                PaintBitMap(bitmap, pixel, posX, posY);
            }

            public void MergeCanvas(Canvas other, Vector2I position)
            {
                Vector2I topLeft = new Vector2I(position.X - other.sizeX / 2, position.Y - other.sizeY / 2);

                for (int x = 0; x < other.sizeX; x++)
                {
                    for (int y = 0; y < other.sizeY; y++)
                    {
                        int i = other.CalculateStringPos(x, y);

                        if (other.canvas[i] != ' ')
                            PaintPixel(other.canvas[i], x + topLeft.X, y + topLeft.Y);
                    }
                }
            }

            public IEnumerator<bool> MergeCanvas(Canvas other, Vector2I position, int maxPerRun)
            {
                Vector2I topLeft = new Vector2I(position.X - other.sizeX / 2, position.Y - other.sizeY / 2);
                int counter = 0;

                for (int x = 0; x < other.sizeX; x++)
                {
                    for (int y = 0; y < other.sizeY; y++)
                    {
                        if (counter++ >= maxPerRun)
                        {
                            counter = 0;
                            yield return true;
                        }
                            

                        int i = other.CalculateStringPos(x, y);

                        if (other.canvas[i] != ' ')
                        {
                            int X = x + topLeft.X;

                            if (X >= sizeX || X < 0)
                                continue;

                            PaintPixel(other.canvas[i], X, y + topLeft.Y);
                        }
                            
                    }
                }
            }

            private int CalculateStringPos(int x, int y)
            {
                return (y * (sizeX + 1)) + x;
            }
        }
	}
}
