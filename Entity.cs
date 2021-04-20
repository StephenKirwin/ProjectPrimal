using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Primal
{
    public class Entity
    {
        public int entityID;
        public float x;
        public float y;
        float plannedX;
        float plannedY;
        float width = 1;
        float height = 2;
        float speedX;
        float speedY;
        int movementIQ;
        int searchDistance = 15;
        bool hasPlan = false;
        Animation animation;
        ParticleSystem particles = new ParticleSystem();
        int[,] localMap;
        List<Int2> path = new List<Int2>();
        Bar healthBar = new Bar();
        float health = 50;
        float maxHealth = 100;
        public bool die = false;

        public Entity(int entID, float xPos, float yPos, int moveIQ)
        {
            x = xPos;
            y = yPos;
            plannedX = x;
            plannedY = y;
            movementIQ = moveIQ;
            animation = new Animation(1, 1);
            entityID = entID;
        }

        protected Rectangle UniversalToScreen2(RenderDetails renderDetails)
        {
            float anchorY = y;
            int screenX = renderDetails.centreX - (int)(renderDetails.playerWidth * renderDetails.tileScale / 2) + (int)(x * renderDetails.tileScale) - (int)(renderDetails.playerX * renderDetails.tileScale);
            int screenY = renderDetails.centreY - (int)(renderDetails.playerHeight * renderDetails.tileScale / 2) + (int)(anchorY * renderDetails.tileScale) - (int)(renderDetails.playerY * renderDetails.tileScale);
            Rectangle rect = new Rectangle(screenX, screenY, (int)(renderDetails.tileScale), (int)(renderDetails.tileScale));
            return rect;
        }

        protected Rectangle UniversalToScreen(RenderDetails renderDetails, Vector2 offset, Vector2 size)
        {
            float anchorY = y;
            int screenX = renderDetails.centreX - (int)(renderDetails.playerWidth * renderDetails.tileScale / 2) + (int)((x + offset.X) * renderDetails.tileScale) - (int)(renderDetails.playerX * renderDetails.tileScale);
            int screenY = renderDetails.centreY - (int)(renderDetails.playerHeight * renderDetails.tileScale / 2) + (int)((anchorY + offset.Y) * renderDetails.tileScale) - (int)(renderDetails.playerY * renderDetails.tileScale);
            Rectangle rect = new Rectangle(screenX, screenY, (int)(renderDetails.tileScale * size.X), (int)(renderDetails.tileScale * size.Y));
            return rect;
        }

        public void Damage(float dmg)
        {
            health -= dmg;
        }

        public void Update(Map map, TileSet tileSet, ObjectSet objectSet, Random r, ParticleSet particleSet, Player player, EntitySet entitySet)
        {
            health -= 0.03125f;
            //set our speed
            speedX = entitySet.GetEntity(entityID).speed;
            speedY = entitySet.GetEntity(entityID).speed;
            //kill if neccessary
            if (health < 0)
            {
                die = true;
            }
            animation.Update(1);
            //if our lad has particles
            if (entitySet.GetEntity(entityID).hasParticles)
            {
                particles.Update(r, x, y, particleSet.GetParticle(0));
            }
            
            //HANDLE ACTION
            if (!hasPlan)
            {
                path.Clear();
                if (entitySet.GetEntity(entityID).attacksPlayer && !player.sleeping)
                {
                    DrawMap(map, tileSet, objectSet, player, -1);
                }
                else
                {
                    DrawMap(map, tileSet, objectSet, player, 1);
                }
                
                hasPlan = true;
            }
            else
            {
                if (path.Count > 0)
                {
                    MoveAgent((float)path.Last<Int2>().x, (float)path.Last<Int2>().y);
                    //Step Achieved
                    if (x == (float)path.Last<Int2>().x && y == (float)path.Last<Int2>().y)
                    {
                        hasPlan = false;
                        path.Clear();
                        if (x == plannedX && y == plannedY)//if we've reached the END GOAL
                        {//perform action
                            if (entitySet.GetEntity(entityID).attacksPlayer)
                            {
                                ActAttackPlayer(map);
                            }
                            else
                            {
                                ActHarvest(map, objectSet);
                            }
                        }

                    }
                }
                else
                {
                    hasPlan = false;
                }
            }
            
        }

        public void ActHarvest(Map map, ObjectSet objectSet)
        {
            map.objectData[(int)plannedX, (int)plannedY] = new Object(0, objectSet.getObject(0));
            health += 25;
        }

        public void ActAttackPlayer(Map map)
        {
            health += 25;
        }

        public void MoveAgent(float goalX, float goalY)
        {
            if (x < goalX)
            {
                x += speedX;
            }
            if (x > goalX)
            {
                x -= speedX;
            }
            if (y < goalY)
            {
                y += speedY;
            }
            if (y > goalY)
            {
                y -= speedY;
            }
        }

        public void DrawMap(Map map, TileSet tileSet, ObjectSet objectSet, Player player, int target)
        {
            //FIND OBSTACLES AND TARGETS
            localMap = new int[map.mapSize, map.mapSize];
            bool possibleTarget = false;
            for (int xi = (int)(x - 15); xi < x + 15; xi++)
            {
                for (int yi = (int)(y - 15); yi < y + 15; yi++)
                {
                    if (objectSet.getObject(map.objectData[xi, yi].id).blocks || tileSet.getTile(map.tileData[xi,yi].id).blocks)
                    {
                        localMap[xi, yi] = -1;
                    }
                    else
                    {
                        localMap[xi, yi] = -2;
                    }
                    //plot the targets as -3
                    if (target == -1)
                    {//HUNT THE PLAYER
                        if (xi == (int)player.x && yi == (int)player.y)
                        {
                            Console.WriteLine("Player FOUND");
                            localMap[xi, yi] = -3;
                            possibleTarget = true;
                        }
                    }
                    else
                    {
                        if (map.objectData[xi, yi].id == target)
                        {
                            localMap[xi, yi] = -3;
                            possibleTarget = true;
                        }
                    }
                }
            }
            //PLOT DISTANCES
            int searchVal = 1;
            List<Int2> nextSearch = new List<Int2>();
            List<Int2> currentSearch = new List<Int2>();
            currentSearch.Add(new Int2((int)x, (int)y));
            localMap[(int)x, (int)y] = 0;
            bool found = false;
            Int2 foundLoc = new Int2(0,0);
            while (searchVal < searchDistance && !found)
            {
                foreach(Int2 loc in currentSearch)
                {//check the location
                    foreach(Int2 surLoc in around(loc))
                    {//each of the surrounding locations
                        if (localMap[surLoc.x, surLoc.y] == -2)
                        {
                            localMap[surLoc.x, surLoc.y] = searchVal;
                            nextSearch.Add(surLoc);
                        }
                        if (localMap[surLoc.x, surLoc.y] == -3)
                        {
                            localMap[surLoc.x, surLoc.y] = searchVal;
                            found = true;
                            nextSearch.Add(surLoc);
                            foundLoc = surLoc;
                        }
                    }
                }
                searchVal += 1;
                currentSearch.Clear();
                currentSearch = nextSearch.ToList();
                nextSearch.Clear();
            }

            if (found)
            {
                plannedX = foundLoc.x;
                plannedY = foundLoc.y;
                TracePath(foundLoc);
            }
        }

        public void TracePath(Int2 foundLocation)
        {

            int depth = localMap[foundLocation.x, foundLocation.y] - 1;
            localMap[foundLocation.x, foundLocation.y] = 99;
            path.Add(foundLocation);
            for (int t = depth; t > 0; t--){
                //check each of the ones around
                bool tracedThisTurn = false;
                foreach(Int2 arLoc in around(foundLocation))
                {
                    if (localMap[arLoc.x, arLoc.y] == t && !tracedThisTurn)
                    {
                        localMap[arLoc.x, arLoc.y] = 99;
                        tracedThisTurn = true;
                        foundLocation = arLoc;
                        path.Add(foundLocation);
                    }
                }
            }

        }

        public Int2[] around(Int2 loc)
        {
            Int2[] surrounding = new Int2[4];
            surrounding[0] = new Int2(loc.x - 1, loc.y);
            surrounding[1] = new Int2(loc.x + 1, loc.y);
            surrounding[2] = new Int2(loc.x, loc.y + 1);
            surrounding[3] = new Int2(loc.x, loc.y - 1);
            return surrounding;
        }

        bool inBounds(Int2 bounds, int x, int y)
        {
            if (x >= 0 && y >= 0 && x < bounds.x && y < bounds.y)
            {
                return true;
            }
            return false;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture, Texture2D barTexture, SpriteFont font, RenderDetails renderDetails, ParticleSet particleSet, EntitySet entities)
        {
            EntityTemplate entityTemplate = entities.GetEntity(entityID);
            Rectangle sourceRect = animation.GetSnippet(entityTemplate.texture);
            spriteBatch.Draw(entityTemplate.texture, UniversalToScreen(renderDetails, new Vector2(entityTemplate.visOffx, entityTemplate.visOffy), new Vector2(entityTemplate.visWidth, entityTemplate.visHeight)), sourceRect, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, ((y + 1) / 250f));

            //Draw its path
            foreach (Int2 step in path)
            {
                int screenXA = renderDetails.centreX - (int)(renderDetails.playerWidth * renderDetails.tileScale / 2) + (int)(step.x * renderDetails.tileScale) - (int)(renderDetails.playerX * renderDetails.tileScale);
                int screenYA = renderDetails.centreY - (int)(renderDetails.playerHeight * renderDetails.tileScale / 2) + (int)(step.y * renderDetails.tileScale) - (int)(renderDetails.playerY * renderDetails.tileScale);
                spriteBatch.Draw(texture, new Rectangle(screenXA, screenYA, 32, 32), Color.Wheat);
            }

            //DRAW its Estimation
            for (int xi = (int)x - 14; xi < x + 14; xi++)
            {
                for (int yi = (int)y - 14; yi < y + 14; yi++)
                {
                    int screenXB = renderDetails.centreX - (int)(renderDetails.playerWidth * renderDetails.tileScale / 2) + (int)(xi * renderDetails.tileScale) - (int)(renderDetails.playerX * renderDetails.tileScale);
                    int screenYB = renderDetails.centreY - (int)(renderDetails.playerHeight * renderDetails.tileScale / 2) + (int)(yi * renderDetails.tileScale) - (int)(renderDetails.playerY * renderDetails.tileScale);
                    spriteBatch.DrawString(font, localMap[xi, yi].ToString(), new Vector2(screenXB, screenYB), Color.Black);
                }
            }

            //int screenX = renderDetails.centreX - (int)(renderDetails.playerWidth * renderDetails.tileScale / 2) + (int)(plannedX * renderDetails.tileScale) - (int)(renderDetails.playerX * renderDetails.tileScale);
            //int screenY = renderDetails.centreY - (int)(renderDetails.playerHeight * renderDetails.tileScale / 2) + (int)(plannedY * renderDetails.tileScale) - (int)(renderDetails.playerY * renderDetails.tileScale);
            //spriteBatch.Draw(texture, new Rectangle(screenX, screenY, 32, 32), Color.Black);

            Rectangle location = UniversalToScreen(renderDetails, new Vector2(0, 0.95f), new Vector2(1, 0.15f));
            healthBar.DrawUniversal(spriteBatch, barTexture, renderDetails, health, maxHealth, location, y);

            ParticleTemplate particleTemplate = particleSet.GetParticle(0);
            particles.Draw(spriteBatch, particleTemplate, renderDetails);
        }
    }
}
