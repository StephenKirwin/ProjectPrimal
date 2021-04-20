using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Primal
{
    public class RenderDetails
    {
        public int leftX;
        public int rightX;
        public int upY;
        public int downY;
        public int screenX;
        public int screenY;
        public int tileScale;
        public float playerWidth;
        public float playerHeight;
        public float playerX;
        public float playerY;
        public int centreX;
        public int centreY;
        public int worldOffsetX;
        public int worldOffsetY;
        public float zoom;
        public LightSet lightSet;

        public RenderDetails(int sx, int sy, int ts, float pw, float ph, float px, float py, int cx, int cy, float z, LightSet ls)
        {
            screenX = sx;
            screenY = sy;
            tileScale = ts;
            playerWidth = pw;
            playerHeight = ph;
            playerX = px;
            playerY = py;
            centreX = cx;
            centreY = cy;
            zoom = z;
            tileScale = (int)(tileScale * zoom);
            worldOffsetX = centreX - (int)(playerWidth * tileScale / 2) - (int)(playerX * tileScale);
            worldOffsetY = centreY - (int)(playerHeight * tileScale / 2) - (int)(playerY * tileScale);
            lightSet = ls;
            leftX = (centreX / tileScale) + 1;
            rightX = (centreX / tileScale) + 1;
            upY = (centreY / tileScale) + 1;
            downY = (centreY / tileScale) + 5;
        }
    }

    public class Int2
    {
        public int x;
        public int y;

        public Int2(int newX, int newY)
        {
            x = newX;
            y = newY;
        }
    }

    public class Animation
    {
        public Texture2D Texture;
        int Rows;
        int Columns;
        public int currentFrame;
        int totalFrames;
        int frameUpdate = 0;



        public Animation(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            currentFrame = 0;
            totalFrames = Rows * Columns;
        }

        public void Update(int updateFreq)
        {
            frameUpdate++;
            if (frameUpdate == updateFreq)
            {
                frameUpdate = 0;
                currentFrame++;
                if (currentFrame == totalFrames)
                    currentFrame = 0;
            }
            
        }

        public Rectangle GetSnippet(Texture2D Texture)
        {
            int width = Texture.Width / Columns;
            int height = Texture.Height / Rows;
            int row = (int)((float)currentFrame / (float)Columns);
            int column = currentFrame % Columns;

            Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);

            return sourceRectangle;
        }
    }
}
