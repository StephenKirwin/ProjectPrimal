using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Primal
{
    public class Player
    {
        public float x = 123;
        public float y = 125;
        public float width = 1;
        public float height = 2;
        public float boundOffset = 1;
        public Inventory inventory = new Inventory(18, new Int2(11, 17));
        public Texture2D texture;
        public Animation animation;
        public float health = 50;
        public float maxHealth = 100;
        public float hunger = 50;
        public float maxHunger = 100;
        public float sleep = 50;
        public float maxSleep = 100;
        public float reach = 5;
        public bool sleeping = false;
        public Color skinColour;

        public Player(ContentManager content)
        {
            texture = content.Load<Texture2D>("player");
            animation = new Animation(1, 3);
            skinColour = new Color(232,196,175);
            skinColour = new Color(72, 57, 49);
        }

        public void handleNeeds()
        {
            hunger -= 0.03125f;
            sleep -= 0.03125f;
            if (hunger <= 0)
            {
                health -= 0.03125f;
            }
            if (hunger >= 50)
            {
                Heal(0.03125f);
            }
            if (sleep <= 0)
            {
                health -= 0.03125f;
            }
        }

        public void Update(KeyboardState keys, Map map, TileSet tileSet, ObjectSet objectSet)
        {
            animation.Update(20);
            //handleNeeds();
            if (!sleeping)
            {
                map.mapItems.Collect(inventory, x, y);

                float oldY = y;
                if (keys.IsKeyDown(Keys.W))
                {
                    y -= 0.0625f;
                }
                if (keys.IsKeyDown(Keys.S))
                {
                    y += 0.0625f;
                }
                //check if collision has occured on Y
                if (map.EntityCollides(new RectangleF(x, y, width, height - boundOffset), objectSet, tileSet))
                {
                    y = oldY;
                }
                float oldX = x;
                if (keys.IsKeyDown(Keys.A))
                {
                    x -= 0.0625f;
                }
                if (keys.IsKeyDown(Keys.D))
                {
                    x += 0.0625f;
                }
                //check if collision has occured on X
                if (map.EntityCollides(new RectangleF(x, y, width, height - boundOffset), objectSet, tileSet))
                {
                    x = oldX;
                }
            }
            else
            {
                if (keys.IsKeyDown(Keys.LeftShift))
                {
                    sleeping = false;
                }
            }
        }

        public void Eat(float value)
        {
            hunger += value;
            if (hunger > maxHunger)
            {
                hunger = maxHunger;
            }
        }

        public void Heal(float value)
        {
            health += value;
            if (health > maxHealth)
            {
                health = maxHealth;
            }
        }

        public bool IsDead()
        {
            bool dead = false;
            if (health <= 0)
            {
                dead = true;
            }
            return dead;
        }

        public void Draw(SpriteBatch spriteBatch, RenderDetails renderDetails)
        {
            if (!sleeping)
            {
                spriteBatch.Draw(texture, new Rectangle(renderDetails.centreX - (int)((renderDetails.tileScale * width) / 2), renderDetails.centreY - (int)((renderDetails.tileScale * (height)) / 1), (int)(width * renderDetails.tileScale), (int)(height * renderDetails.tileScale)), animation.GetSnippet(texture), skinColour, 0f, new Vector2(0, 0), SpriteEffects.None, ((y + 1) / 250f));
            }
        }
    }
}
