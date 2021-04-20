using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimplexNoise;

namespace Primal
{
    public class WorldGeneration
    {
        public int width = 250;
        public int height = 250;
        float scale = 0.0175f;
        int radialFactor = 12;

        public int[,] GenerateObjectMap(int[,] map, Random r)
        {
            int[,] objectMap = new int[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (map[x,y] == 1)
                    {
                        if (r.Next(4) == 0)
                        {
                            int objID = 1 + r.Next(5);
                            if (objID != 4)
                            {
                                objectMap[x, y] = objID;
                            }
                        }
                    }
                    if (map[x, y] == 2)
                    {
                        if (r.Next(14) == 1)
                        {
                            objectMap[x, y] = 8;
                        }
                    }
                    if (map[x,y] == 7)
                    {
                        if (r.Next(3) == 0)
                        {
                            objectMap[x, y] = 2;
                        }
                    }
                    if (map[x, y] == 6)
                    {
                        if (r.Next(3) == 0)
                        {
                            objectMap[x, y] = 11;
                        }
                    }
                }
            }
            objectMap[125, 125] = 4;
            objectMap[128, 124] = 10;
            objectMap[124, 122] = 10;
            objectMap[122, 125] = 10;
            return objectMap;
        }

        public int[,] GenerateMap()
        {
            float[,] noiseValues = SimplexNoise.Noise.Calc2D(width, height, scale);
            Console.WriteLine(noiseValues.GetLength(0));
            Console.WriteLine(noiseValues.GetLength(1));
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    noiseValues[x, y] = noiseValues[x, y] / 255;
                }
            }

            float[,] circle = GenerateCircle();
            int[,] map = new int[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float thisTile = noiseValues[x, y] * circle[x, y];
                    if (thisTile > 0.25)
                    {
                        map[x, y] = 1;
                    }
                    else
                    {
                        map[x, y] = 0;
                    }
                }
            }

            map = AddSand(map);
            map = AddShallows(map);
            map = AddStone(map);
            map = AddRivers(map);
            map = AddRiverBed(map);
            map = AddVillage(map);


            return map;
        }

        int[,] AddStone(int[,] map)
        {
            float stoneScale = 0.045f;
            float[,] riverNoiseValues = SimplexNoise.Noise.Calc2D(width, height, stoneScale);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    riverNoiseValues[x, y] = riverNoiseValues[x, y] / 255;
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (riverNoiseValues[x, y] > 0.9f  && map[x, y] == 1)
                    {
                        map[x, y] = 7;
                    }
                }
            }

            return map;
        }

        int[,] AddRivers(int [,] map)
        {
            float riverScale = 0.005f;
            float[,] riverNoiseValues = SimplexNoise.Noise.Calc2D(width, height, riverScale);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    riverNoiseValues[x, y] = riverNoiseValues[x, y] / 255;
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (riverNoiseValues[x, y] > 0.55f && riverNoiseValues[x, y] < 0.6f && map[x, y] != 0 && map[x, y] != 3)
                    {
                        map[x, y] = 5;
                    }
                }
            }

            return map;
        }

        int[,] AddVillage(int[,] map)
        {
            for (int x = 100; x < width - 100; x++)
            {
                for (int y = 100; y < height - 100; y++)
                {
                    float distance = (float)Math.Sqrt(Math.Pow(125 - x, 2) + Math.Pow(125 - y, 2));
                    if (distance < 5)
                    {
                        map[x, y] = 4;
                    }
                }
            }
            return map;
        }

        int[,] AddRiverBed(int[,] map)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (map[x, y] == 1)
                    {
                        if (map[x + 1, y] == 5)
                        {
                            map[x, y] = 6;
                        }
                        if (map[x - 1, y] == 5)
                        {
                            map[x, y] = 6;
                        }
                        if (map[x, y + 1] == 5)
                        {
                            map[x, y] = 6;
                        }
                        if (map[x, y - 1] == 5)
                        {
                            map[x, y] = 6;
                        }
                    }
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (map[x, y] == 1)
                    {
                        if (map[x + 2, y] == 5)
                        {
                            map[x, y] = 6;
                        }
                        if (map[x - 2, y] == 5)
                        {
                            map[x, y] = 6;
                        }
                        if (map[x, y + 2] == 5)
                        {
                            map[x, y] = 6;
                        }
                        if (map[x, y - 2] == 5)
                        {
                            map[x, y] = 6;
                        }
                    }
                }
            }

            return map;
        }

        int[,] AddShallows(int[,] map)
        {
            for (int x = 10; x < width - 10; x++)
            {
                for (int y = 10; y < height - 10; y++)
                {
                    if (map[x, y] == 0)
                    {
                        if (map[x + 1, y] == 2)
                        {
                            map[x, y] = 3;
                        }
                        if (map[x - 1, y] == 2)
                        {
                            map[x, y] = 3;
                        }
                        if (map[x, y + 1] == 2)
                        {
                            map[x, y] = 3;
                        }
                        if (map[x, y - 1] == 2)
                        {
                            map[x, y] = 3;
                        }
                    }
                }
            }

            for (int x = 10; x < width - 10; x++)
            {
                for (int y = 10; y < height - 10; y++)
                {
                    if (map[x, y] == 0)
                    {
                        if (map[x + 2, y] == 2)
                        {
                            map[x, y] = 3;
                        }
                        if (map[x - 2, y] == 2)
                        {
                            map[x, y] = 3;
                        }
                        if (map[x, y + 2] == 2)
                        {
                            map[x, y] = 3;
                        }
                        if (map[x, y - 2] == 2)
                        {
                            map[x, y] = 3;
                        }
                    }
                }
            }
            for (int x = 10; x < width - 10; x++)
            {
                for (int y = 10; y < height - 10; y++)
                {
                    if (map[x, y] == 0)
                    {
                        if (map[x + 3, y] == 2)
                        {
                            map[x, y] = 3;
                        }
                        if (map[x - 3, y] == 2)
                        {
                            map[x, y] = 3;
                        }
                        if (map[x, y + 3] == 2)
                        {
                            map[x, y] = 3;
                        }
                        if (map[x, y - 3] == 2)
                        {
                            map[x, y] = 3;
                        }
                    }
                }
            }

            for (int x = 10; x < width - 10; x++)
            {
                for (int y = 10; y < height - 10; y++)
                {
                    if (map[x, y] == 0)
                    {
                        if (map[x + 4, y] == 2)
                        {
                            map[x, y] = 3;
                        }
                        if (map[x - 4, y] == 2)
                        {
                            map[x, y] = 3;
                        }
                        if (map[x, y + 4] == 2)
                        {
                            map[x, y] = 3;
                        }
                        if (map[x, y - 4] == 2)
                        {
                            map[x, y] = 3;
                        }
                    }
                }
            }

            return map;
        }

        int[,] AddSand(int[,] map)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (map[x,y] == 1)
                    {
                        if (map[x + 1,y] == 0)
                        {
                            map[x, y] = 2;
                        }
                        if (map[x - 1, y] == 0)
                        {
                            map[x, y] = 2;
                        }
                        if (map[x, y + 1] == 0)
                        {
                            map[x, y] = 2;
                        }
                        if (map[x, y - 1] == 0)
                        {
                            map[x, y] = 2;
                        }
                    }
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (map[x, y] == 1)
                    {
                        if (map[x + 2, y] == 0)
                        {
                            map[x, y] = 2;
                        }
                        if (map[x - 2, y] == 0)
                        {
                            map[x, y] = 2;
                        }
                        if (map[x, y + 2] == 0)
                        {
                            map[x, y] = 2;
                        }
                        if (map[x, y - 2] == 0)
                        {
                            map[x, y] = 2;
                        }
                    }
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (map[x, y] == 1)
                    {
                        if (map[x + 3, y] == 0)
                        {
                            map[x, y] = 2;
                        }
                        if (map[x - 3, y] == 0)
                        {
                            map[x, y] = 2;
                        }
                        if (map[x, y + 3] == 0)
                        {
                            map[x, y] = 2;
                        }
                        if (map[x, y - 3] == 0)
                        {
                            map[x, y] = 2;
                        }
                    }
                }
            }

            return map;
        }

        public float[,] GenerateCircle()
        {
            float[,] circleValues = new float[width,height];
            int centreX = width / 2;
            int centreY = height / 2;
            int cutoff = width / radialFactor;
            int radius = ((width - (cutoff * 2)) / 2);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x >= cutoff && x < width - cutoff && y >= cutoff && y < height - cutoff)
                    {
                        circleValues[x, y] = ((float)Math.Sqrt(Math.Pow((double)x - (double)centreX, (double)2) + Math.Pow((double)y - (double)centreY, (double)2)) / radius) * 255;
                        if (circleValues[x, y] > 255)
                        {
                            circleValues[x, y] = 255;
                        }
                    }
                    else
                    {
                        circleValues[x, y] = 255;
                    }
                    //invert the value
                    circleValues[x, y] = 255 - circleValues[x, y];
                    circleValues[x, y] = circleValues[x, y] / 255;
                }
            }

            return circleValues;
        }

    }
}
