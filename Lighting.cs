using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Primal
{
    public class LightSet
    {
        public List<Light> lights = new List<Light>();

        public List<Light> nNearest(float x, float y)
        {
            List<Light> nearestLights = new List<Light>();

            float min1 = 500;
            float min2 = 500;
            float min3 = 500;
            int i1 = -1;
            int i2 = -1;
            int i3 = -1;
            int i = 0;
            //Console.Write("Lighting count");
            //Console.WriteLine(lights.Count);
            foreach (Light light in lights)
            {
                float distance = (float)Math.Sqrt(Math.Pow(x - light.x, 2) + Math.Pow(y - light.y, 2));
                //Console.WriteLine(distance);
                if (distance < min1) { min3 = min2; i3 = i2; min2 = min1; i2 = i1; min1 = distance; i1 = i; }
                else if (distance < min2) { min3 = min2; i3 = i2; min2 = distance; i2 = i; }
                else if (distance < min3) { min3 = distance; i3 = i; }
                i++;
            }
            //Console.WriteLine(max1);
            //Console.WriteLine(max2);
            //Console.WriteLine(max3);

            nearestLights.Add(lights[i1]);
            nearestLights.Add(lights[i2]);
            nearestLights.Add(lights[i3]);

            return nearestLights;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture, RenderDetails renderDetails)
        {
            foreach (Light l in lights)
            {
                spriteBatch.Draw(texture, UniversalToScreen((float)l.x, (float)l.y, l.ld, l.ld, renderDetails), Color.White);
            }
        }

        Rectangle UniversalToScreen(float x, float y, float width, float height, RenderDetails renderDetails)
        {
            int screenX = renderDetails.worldOffsetX + (int)(x * renderDetails.tileScale);
            int screenY = renderDetails.worldOffsetY + (int)(y * renderDetails.tileScale);
            int offset = (int)(width * renderDetails.tileScale);
            offset = offset - renderDetails.tileScale;
            offset = offset / 2;
            Rectangle rect = new Rectangle(screenX - offset, screenY - offset, (int)(width * renderDetails.tileScale), (int)(height * renderDetails.tileScale));
            return rect;
        }
    }

    public class Light
    {
        public float x;
        public float y;
        public float li;
        public float ld;

        public Light(float xPos, float yPos, float intensity, float distance)
        {
            x = xPos;
            y = yPos;
            li = intensity;
            ld = distance;
        }
    }
}
