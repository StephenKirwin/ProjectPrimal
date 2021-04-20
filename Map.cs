using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Color = Microsoft.Xna.Framework.Color;

namespace Primal
{
    public class Map
    {
        public Tile[,] tileData;
        public Object[,] objectData;
        public int mapSize = 250;
        public ItemParticleSystem mapItems;

        public List<Light> FindLights(int playerX, int playerY, RenderDetails renderDetails)
        {
            List<Light> allLights = new List<Light>();

            for (int x = playerX - (renderDetails.leftX + 20); x < playerX + (renderDetails.rightX + 20); x++)
            {
                for (int y = playerY - (renderDetails.upY + 20); y < playerY + (renderDetails.downY + 20); y++)
                {
                    if (objectData[x,y].id == 4)
                    {
                        allLights.Add(new Light(x, y, 0.35f, 20));
                    }
                }
            }

            return allLights;
        }
        

        public void SummonItem(Random random, float x, float y, ParticleTemplate particleTemplate, int id)
        {
            mapItems.SummonItem(random, x, y, particleTemplate, id);
        }

        public Map(WorldGeneration worldGenerator, Random randomGenerator, ObjectSet objectSet)
        {
            int[,] tileMap = worldGenerator.GenerateMap();
            int[,] objectMap = worldGenerator.GenerateObjectMap(tileMap, randomGenerator);
            mapItems = new ItemParticleSystem();

            tileData = new Tile[mapSize, mapSize];
            objectData = new Object[mapSize, mapSize];

            for (int x = 0; x < worldGenerator.width; x++)
            {
                for (int y = 0; y < worldGenerator.height; y++)
                {
                    tileData[x, y] = new Tile(tileMap[x, y]);
                    objectData[x, y] = new Object(objectMap[x, y], objectSet.getObject(objectMap[x, y]));
                }
            }
        }

        bool Collides(RectangleF moving, RectangleF obstacle)
        {
            if (moving.IntersectsWith(obstacle))
            {
                return true;
            }
            return false;
        }

        public bool EntityCollides(RectangleF entity, ObjectSet objectSet, TileSet tileSet)
        {
            int entityColX = (int)entity.X;
            int entityColY = (int)entity.Y;
            if (entityColX < 2)
            {
                entityColX = 2;
            }
            if (entityColY < 2)
            {
                entityColY = 2;
            }
            if (entityColX + 3 >= mapSize - 1)
            {
                entityColX = mapSize - 3;
            }
            if (entityColY + 3 >= mapSize - 1)
            {
                entityColY = mapSize - 3;
            }
            for (int x = entityColX - 2; x < entityColX + 3; x++)
            {
                for (int y = entityColY - 2; y < entityColY + 3; y++)
                {
                    //tileMap
                    RectangleF obstacle = new RectangleF(x, y, 1, 1);
                    //Console.WriteLine(tileSet.getTile(objectData[x, y].id).blocks);
                    if (tileSet.getTile(tileData[x, y].id).blocks && Collides(entity, obstacle))
                    {
                        return true;
                    }
                    obstacle = new RectangleF(x + objectSet.getObject(objectData[x, y].id).xOffset, y + objectSet.getObject(objectData[x, y].id).yOffset, objectSet.getObject(objectData[x, y].id).width, objectSet.getObject(objectData[x, y].id).height);
                    if (objectSet.getObject(objectData[x, y].id).blocks && Collides(entity, obstacle))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void Update(Player player, Random r, ObjectSet objectSet, ParticleSet particleSet)
        {
            mapItems.Update();
            for (int x = (int)player.x - 30; x < player.x + 31; x++)
            {
                for (int y = (int)player.y - 20; y < player.y + 20; y++)
                {
                    if (objectSet.getObject(objectData[x, y].id).particles)
                    {
                        objectData[x, y].Update(r, x, y, particleSet.GetParticle(objectSet.getObject(objectData[x,y].id).particleType));
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, RenderDetails renderDetails, Effect lightingShader, TileSet tileSet, ObjectSet objectSet, float ambientLight, GraphicsDevice graphicsDevice, ParticleSet particleSet, ItemSet itemSet)
        {
            for (int x = (int)renderDetails.playerX - renderDetails.leftX; x < renderDetails.playerX + renderDetails.rightX; x++)
            {
                for (int y = (int)renderDetails.playerY - renderDetails.upY; y < renderDetails.playerY + renderDetails.downY; y++)
                {
                    
                    tileData[x, y].Draw(spriteBatch, tileSet, x, y, renderDetails, tileData[x, y - 1].id, tileData[x, y + 1].id, tileData[x - 1, y].id, tileData[x + 1, y].id);
                    
                }
            }
            for (int x = (int)renderDetails.playerX - renderDetails.leftX; x < renderDetails.playerX + renderDetails.rightX; x++)
            {
                for (int y = (int)renderDetails.playerY - renderDetails.upY; y < renderDetails.playerY + renderDetails.downY; y++)
                {
                    
                    objectData[x, y].Draw(spriteBatch, objectSet, x, y, renderDetails);
                    
                }
            }
            for (int x = (int)renderDetails.playerX - (renderDetails.leftX + 10); x < renderDetails.playerX + (renderDetails.rightX + 10); x++)
            {
                for (int y = (int)renderDetails.playerY - (renderDetails.upY + 10); y < renderDetails.playerY + (renderDetails.downY + 10); y++)
                {
                    
                    ParticleTemplate particleTemplate = particleSet.GetParticle(objectSet.getObject(objectData[x, y].id).particleType);
                    objectData[x, y].DrawParticle(spriteBatch, objectSet, renderDetails, particleTemplate);
                    
                }
            }
            mapItems.Draw(spriteBatch, itemSet, renderDetails);
        }
    }

    public class Tile
    {
        public int id;

        public Tile(int tileID)
        {
            id = tileID;
        }

        protected Rectangle UniversalToScreen(int x, int y, RenderDetails renderDetails)
        {
            int screenX = renderDetails.centreX - (int)(renderDetails.playerWidth * renderDetails.tileScale / 2) + (int)(x * renderDetails.tileScale) - (int)(renderDetails.playerX * renderDetails.tileScale);
            int screenY = renderDetails.centreY - (int)(renderDetails.playerHeight * renderDetails.tileScale / 2) + (int)(y * renderDetails.tileScale) - (int)(renderDetails.playerY * renderDetails.tileScale);
            Rectangle rect = new Rectangle(screenX, screenY, renderDetails.tileScale, renderDetails.tileScale);
            return rect;
        }

        public void Draw(SpriteBatch spriteBatch, TileSet tileSet, int xPos, int yPos, RenderDetails renderDetails, int up, int down, int left, int right)
        {
            TileTemplate thisTemplate = tileSet.getTile(id);
            spriteBatch.Draw(thisTemplate.texture, UniversalToScreen(xPos, yPos, renderDetails), null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, (yPos / 250f));
            if (tileSet.getTile(up).blendedTextures.overlapsID(id))
            {
                spriteBatch.Draw(tileSet.getTile(up).blendedTextures.up, UniversalToScreen(xPos, yPos, renderDetails), null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, ((yPos + 0.01f) / 250f));
            }
            if (tileSet.getTile(down).blendedTextures.overlapsID(id))
            {
                spriteBatch.Draw(tileSet.getTile(down).blendedTextures.down, UniversalToScreen(xPos, yPos, renderDetails), null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, ((yPos + 0.05f) / 250f));
            }
            if (tileSet.getTile(left).blendedTextures.overlapsID(id))
            {
                spriteBatch.Draw(tileSet.getTile(left).blendedTextures.left, UniversalToScreen(xPos, yPos, renderDetails), null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, ((yPos + 0.03f) / 250f));
            }
            if (tileSet.getTile(right).blendedTextures.overlapsID(id))
            {
                spriteBatch.Draw(tileSet.getTile(right).blendedTextures.right, UniversalToScreen(xPos, yPos, renderDetails), null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, ((yPos + 0.03f) / 250f));
            }
        }
    }

    public class Object
    {
        public int id;
        ParticleSystem particles;
        public Inventory inventory;

        public Object(int objID, ObjectTemplate objectTemplate)
        {
            id = objID;
            particles = new ParticleSystem();
            inventory = new Inventory(objectTemplate.storageSpace, new Int2(0, 0));
        }

        Rectangle UniversalToScreen(float x, float y, float width, float height, RenderDetails renderDetails)
        {
            int screenX = renderDetails.worldOffsetX + (int)(x * renderDetails.tileScale);
            int screenY = renderDetails.worldOffsetY + (int)(y * renderDetails.tileScale);
            Rectangle rect = new Rectangle(screenX, screenY, (int)(width * renderDetails.tileScale), (int)(height * renderDetails.tileScale));
            return rect;
        }

        public void Update(Random r, float x, float y, ParticleTemplate particleTemplate)
        {
            particles.Update(r, x, y, particleTemplate);
        }

        public void Draw(SpriteBatch spriteBatch, ObjectSet objectSet, float xPos, float yPos, RenderDetails renderDetails)
        {
            if (id != 0)
            {
                ObjectTemplate thisObject = objectSet.getObject(id);
                spriteBatch.Draw(thisObject.texture, UniversalToScreen((float)xPos + thisObject.visOffx, (float)yPos + thisObject.visOffy, thisObject.visWidth, thisObject.visHeight, renderDetails), null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, ((yPos + 0.1f) / 250f));
            }
        }

        public void DrawParticle(SpriteBatch spriteBatch, ObjectSet objectSet, RenderDetails renderDetails, ParticleTemplate particleTemplate)
        {
            if (id != 0)
            {
                ObjectTemplate thisObject = objectSet.getObject(id);
                particles.Draw(spriteBatch, particleTemplate, renderDetails);
            }
        }
    }
}
