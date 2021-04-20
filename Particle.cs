using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Primal
{
    public class ParticleSystem
    {
        List<Particle> particles;
        int emitCounter = 0;
        int emitFreq = 1;

        public ParticleSystem()
        {
            particles = new List<Particle>();
        }

        public void Update(Random Random, float X, float Y, ParticleTemplate particleModel)
        {
            emitCounter += 1;
            if (emitCounter > particleModel.emitFreq)
            {
                emitCounter = 0;
                float VX = particleModel.minVX + ((float)Random.NextDouble() * (particleModel.maxVX - particleModel.minVX));
                float VY = particleModel.minVX + ((float)Random.NextDouble() * (particleModel.maxVY - particleModel.minVY));
                float gravity = particleModel.mingravity + ((float)Random.NextDouble() * (particleModel.gravity - particleModel.mingravity));
                particles.Add(new Particle(particleModel.width, particleModel.height, (float)X + 0.5f, (float)Y + 0.5f, 0, VX, VY, particleModel.vz, gravity, particleModel.lifeSpan));
            }
            particles.RemoveAll(p => p.die);
            foreach (Particle p in particles)
            {
                p.Update();
            }
        }

        public void Draw(SpriteBatch spriteBatch, ParticleTemplate particleTemplate, RenderDetails renderDetails)
        {
            foreach (Particle p in particles)
            {
                p.Draw(spriteBatch, particleTemplate, renderDetails);
            }
        }
    }

    public class Particle
    {
        float Width;
        float Height;
        float X;
        float Y;
        float Z;
        float vX;
        float vY;
        float vZ;
        float gravity;
        int age;
        int lifeSpan;
        public bool die = false;

        public Particle(float pw, float ph, float iX, float iY, float iZ, float ivX, float ivY, float ivZ, float iGravity, int maxLife)
        {
            Width = pw;
            Height = ph;
            X = iX;
            Y = iY;
            Z = iZ;
            vX = ivX;
            vY = ivY;
            vZ = ivZ;
            gravity = iGravity;
            age = 0;
            lifeSpan = maxLife;
        }

        void Land()
        {
            gravity = 0;
            vX = 0;
            vY = 0;
            vZ = 0;
            Z = 0;
        }

        public void Update()
        {
            vZ += gravity;
            X += vX;
            Y += vY;
            Z += vZ;
            age += 1;
            if (Z < 0)
            {
                Land();
            }
            if (age >= lifeSpan)
            {//I have decided that i want to die now
                die = true;
            }
        }

        protected Rectangle UniversalToScreen(RenderDetails renderDetails)
        {
            int screenX = renderDetails.centreX - (int)(renderDetails.playerWidth * renderDetails.tileScale / 2) + (int)(X * renderDetails.tileScale) - (int)(renderDetails.playerX * renderDetails.tileScale);
            int screenY = renderDetails.centreY - (int)(renderDetails.playerHeight * renderDetails.tileScale / 2) + (int)((Y - Z) * renderDetails.tileScale) - (int)(renderDetails.playerY * renderDetails.tileScale);
            Rectangle rect = new Rectangle(screenX, screenY, (int)(Width * renderDetails.tileScale), (int)(Height * renderDetails.tileScale));
            return rect;
        }

        public void Draw(SpriteBatch spriteBatch, ParticleTemplate particleTemplate, RenderDetails renderDetails)
        {
            spriteBatch.Draw(particleTemplate.texture, UniversalToScreen(renderDetails), null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, ((Y + Z) / 250f));
            //spriteBatch.Draw(particleTemplate.texture, UniversalToScreen(renderDetails), Color.White);
        }
    }

    public class ItemParticleSystem
    {
        List<ItemParticle> particles;

        public ItemParticleSystem()
        {
            particles = new List<ItemParticle>();
        }

        public void SummonItem(Random Random, float X, float Y, ParticleTemplate particleModel, int id)
        {
            float VX = particleModel.minVX + ((float)Random.NextDouble() * (particleModel.maxVX - particleModel.minVX));
            float VY = particleModel.minVX + ((float)Random.NextDouble() * (particleModel.maxVY - particleModel.minVY));
            float gravity = particleModel.mingravity + ((float)Random.NextDouble() * (particleModel.gravity - particleModel.mingravity));
            particles.Add(new ItemParticle(id, particleModel.width, particleModel.height, (float)X + 0.5f, (float)Y + 0.5f, 0, VX, VY, particleModel.vz, gravity, particleModel.lifeSpan));
        }

        public void Update()
        {
            particles.RemoveAll(p => p.die);
            foreach (ItemParticle p in particles)
            {
                p.Update();
            }
        }

        public void Collect(Inventory inventory, float x, float y)
        {
            foreach (ItemParticle p in particles)
            {
                float dx = x - p.X;
                float dy = y - p.Y;
                float distance = (float)Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
                if (distance < 1f)
                {
                    //IF INRANGE, AND IF INVENTORY HAS SPACE
                    p.collected = true;
                    inventory.AddItem(new Int2(p.id, 1));
                }
            }
            //KILL ALL THE COLLECTED
            particles.RemoveAll(p => p.collected);
        }

        public void Draw(SpriteBatch spriteBatch, ItemSet itemSet, RenderDetails renderDetails)
        {
            foreach (ItemParticle p in particles)
            {
                p.Draw(spriteBatch, itemSet.getItem(p.id), renderDetails);
            }
        }
    }

    public class ItemParticle
    {
        public int id;
        float Width;
        float Height;
        public float X;
        public float Y;
        float Z;
        float vX;
        float vY;
        float vZ;
        float gravity;
        int age;
        int lifeSpan;
        public bool die = false;
        public bool collected = false;

        public ItemParticle(int newID, float pw, float ph, float iX, float iY, float iZ, float ivX, float ivY, float ivZ, float iGravity, int maxLife)
        {
            id = newID;
            Width = pw;
            Height = ph;
            X = iX;
            Y = iY;
            Z = iZ;
            vX = ivX;
            vY = ivY;
            vZ = ivZ;
            gravity = iGravity;
            age = 0;
            lifeSpan = maxLife;
        }

        void Land()
        {
            gravity = 0;
            vX = 0;
            vY = 0;
            vZ = 0;
            Z = 0;
        }

        public void Update()
        {
            vZ += gravity;
            X += vX;
            Y += vY;
            Z += vZ;
            age += 1;
            if (Z < 0)
            {
                Land();
            }
            if (age >= lifeSpan)
            {//I have decided that i want to die now
                die = true;
            }
        }

        protected Rectangle UniversalToScreen(RenderDetails renderDetails)
        {
            int screenX = renderDetails.centreX - (int)(renderDetails.playerWidth * renderDetails.tileScale / 2) + (int)(X * renderDetails.tileScale) - (int)(renderDetails.playerX * renderDetails.tileScale);
            int screenY = renderDetails.centreY - (int)(renderDetails.playerHeight * renderDetails.tileScale / 2) + (int)((Y - Z) * renderDetails.tileScale) - (int)(renderDetails.playerY * renderDetails.tileScale);
            Rectangle rect = new Rectangle(screenX, screenY, (int)(Width * renderDetails.tileScale), (int)(Height * renderDetails.tileScale));
            return rect;
        }

        public void Draw(SpriteBatch spriteBatch, ItemTemplate itemTemplate, RenderDetails renderDetails)
        {
            spriteBatch.Draw(itemTemplate.texture, UniversalToScreen(renderDetails), null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, (((Y + 0.5f) + Z) / 250f));
            //spriteBatch.Draw(particleTemplate.texture, UniversalToScreen(renderDetails), Color.White);
        }
    }
}
