using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Xml.Linq;
using System.Drawing;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Color = Microsoft.Xna.Framework.Color;
using System.Collections.Generic;
using System;

namespace Primal
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D tile1;
        Texture2D tile2;
        Texture2D npc;
        Player player;
        int tileScale = 64;
        int width = 1280;
        int height = 720;
        int centreX;
        int centreY;
        WorldGeneration generator = new WorldGeneration();
        Map map;
        List<Entity> entityData;
        DiscussionGUI discGUI = new DiscussionGUI();
        DeathGUI deathGUI = new DeathGUI();
        KeyboardState oldkeys;
        MouseState oldMouse;
        Random randomGenerator;
        bool inGUI = false;
        float minZoom = 1;
        float zoom = 1;
        float maxZoom = 3f;
        float zoomSpeed = 0.03125f;
        TileSet tileSet = new TileSet();
        ObjectSet objectSet = new ObjectSet();
        ItemSet itemSet = new ItemSet();
        ParticleSet particleSet = new ParticleSet();
        RecipeSet recipeSet = new RecipeSet();
        EntitySet entitySet = new EntitySet();
        Effect lightingShader;
        float time = 0f;
        float ambientLight;
        HotbarGUI hotbar;
        InventoryCraftingGUI inventory;
        SpriteFont font;
        Bar hungerBar = new Bar(0.8f,0.7f);
        Bar healthBar = new Bar(0.8f, 0.8f);
        Bar sleepBar = new Bar(0.8f, 0.9f);
        LightSet lightSet = new LightSet();
        RenderTarget2D darkness;
        Texture2D alphaMask;
        Texture2D lightMask;
        Color skyColor;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            randomGenerator = new Random();
            player = new Player(Content);
            // TODO: Add your initialization logic here
            tileSet.LoadTileSet(Content, "TileData.xml");
            objectSet.LoadObjectSet(Content, "ObjectData.xml");
            itemSet.LoadItemSet(Content, "ItemData.xml");
            particleSet.LoadParticleSet(Content, "ParticleData.xml");
            recipeSet.LoadRecipeSet(Content, "RecipeData.xml");
            entitySet.LoadEntitySet(Content, "EntityData.xml");
            map = new Map(generator, randomGenerator, objectSet);
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            graphics.ApplyChanges();
            this.IsMouseVisible = true;
            centreX = width / 2;
            centreY = height / 2;            
            entityData = new List<Entity>();
            entityData.Add(new Entity(0, 120f, 120f, 0));
            entityData.Add(new Entity(1, 130f, 130f, 0));
            entityData.Add(new Entity(2, 135f, 135f, 0));
            hotbar = new HotbarGUI();
            inventory = new InventoryCraftingGUI();
            darkness = new RenderTarget2D(GraphicsDevice, width, height);
            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            tile1 = Content.Load<Texture2D>("tile");
            tile2 = Content.Load<Texture2D>("tile2");
            npc = Content.Load<Texture2D>("npc");
            font = Content.Load<SpriteFont>("DefaultFont");
            lightingShader = Content.Load<Effect>("BasicLighting");
            alphaMask = Content.Load<Texture2D>("alphamask");
            lightMask = Content.Load<Texture2D>("light");
        }
        
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        
        public float modifyTime(float time)
        {
            if (time > 6.283f)
            {
                time = 0.0f;
            }
            else
            {
                if (player.sleeping)
                {
                    time += 0.01f;
                }
                else
                {
                    time += 0.001f;
                }
            }
            return time;
        }

        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            KeyboardState keys = Keyboard.GetState();
            MouseState mouseInput = Mouse.GetState();
            bool hasClicked = false;

            if (time >= (3.1415f / 2) && time <= (3.1415f * 1.5f))
            {
                player.sleeping = false;
            }

            time = modifyTime(time);
            ambientLight = (float)Math.Cos(time) / 1.3f;
            float sunriseFactor = 0;
            if (time > 1.37f && time < 1.77f)
            {
                float sunriseOffset = time - 1.37f;
                sunriseOffset /= 0.4f;
                sunriseOffset *= 3.1415f;
                sunriseFactor = (float)Math.Sin(sunriseOffset);
            }
            if (time > 4.51f && time < 4.91f)
            {
                float sunriseOffset = time - 4.51f;
                sunriseOffset /= 0.4f;
                sunriseOffset *= 3.1415f;
                sunriseFactor = (float)Math.Sin(sunriseOffset);
            }
            int r = (int)(sunriseFactor * 50);
            int g = (int)(sunriseFactor * 25);
            int b = 0;
            int alpha = (int)(ambientLight * 255);
            skyColor = new Color(r, g, b, alpha);

            map.Update(player, randomGenerator, objectSet, particleSet);
            RenderDetails renderDetails = new RenderDetails(width, height, tileScale, player.width, player.height, player.x, player.y, centreX, centreY, zoom, lightSet);

            if (!inGUI)
            {
                player.Update(keys, map, tileSet, objectSet);
                if (zoom > minZoom)
                {
                    zoom -= zoomSpeed;
                }
                
                if (inventory.open == false)
                {
                    if (keys.IsKeyDown(Keys.E) && oldkeys.IsKeyUp(Keys.E))
                    {
                        inventory.Open("inv", false, new Int2(0, 0));
                    }
                }
                else
                {
                    inventory.Update(keys, oldkeys, mouseInput, oldMouse, renderDetails, player.inventory, recipeSet, map.objectData[inventory.accessed.x, inventory.accessed.y]);
                }

                //get the mouse details
                int mouseX = mouseInput.X;
                int mouseY = mouseInput.Y;
                Vector2 universalPos = ScreenToUniversal(mouseX, mouseY);
                int uniX = (int)universalPos.X;
                int uniY = (int)universalPos.Y;

                float clickDistance = (float)Math.Sqrt(Math.Pow(player.x - uniX, 2) + Math.Pow(player.y - uniY, 2));

                //Handle Left mouse input
                if (mouseInput.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released && clickDistance < player.reach)
                {//break/harvest

                    if (uniX >= 0 && uniX <= map.mapSize - 1 && uniY >= 0 && uniY <= map.mapSize - 1)
                    {//if its within the bounds
                        int entityTouched = touchingEntity(universalPos, entityData);
                        if (entityTouched != -1)
                        {//if touching one, interact
                            //attack the entity
                            float damage = itemSet.getItem(player.inventory.inventorySlots[12 + hotbar.focus].x).damage;
                            entityData[entityTouched].Damage(damage);
                        }
                        else
                        {
                            if (objectSet.getObject(map.objectData[uniX, uniY].id).breakable)//if its a breakable object
                            {//object here!
                                //harvest the object
                                int brokenid = objectSet.getObject(map.objectData[uniX, uniY].id).gives;
                                map.SummonItem(randomGenerator, uniX, uniY, particleSet.GetParticle(3), brokenid);
                                //clear the objects inventory
                                foreach (Int2 item in map.objectData[uniX, uniY].inventory.inventorySlots)
                                {
                                    for (int i = 0; i < item.y; i++)
                                    {
                                        if(item.x != 0)
                                        {
                                            map.SummonItem(randomGenerator, uniX, uniY, particleSet.GetParticle(3), item.x);
                                        }
                                    }
                                }
                                //reset the object
                                map.objectData[uniX, uniY] = new Object(0, objectSet.getObject(0));
                            }
                            else
                            {//oh, we must be touching the tile then... or ourself ;)
                            }
                        }
                    }
                }

                //Handle Right mouse input
                if (mouseInput.RightButton == ButtonState.Pressed && oldMouse.RightButton == ButtonState.Released && !hasClicked && clickDistance < player.reach)
                {//break/harvest
                    if (uniX >= 0 && uniX <= map.mapSize - 1 && uniY >= 0 && uniY <= map.mapSize - 1)
                    {//if its within the bounds
                        int entityTouched = touchingEntity(universalPos, entityData);
                        if (entityTouched != -1)
                        {//if touching one, interact
                            if (entitySet.GetEntity(entityData[entityTouched].entityID).communicates)
                            {
                                inGUI = true;
                            }
                        }
                        else
                        {
                            if (map.objectData[uniX, uniY].id == 0 && itemSet.getItem(player.inventory.inventorySlots[12 + hotbar.focus].x).placeable)//if its an empty object slot, and the object in hand is placeable
                            {//place object here
                                //Place the object
                                int placeId = itemSet.getItem(player.inventory.inventorySlots[12 + hotbar.focus].x).places;
                                map.objectData[uniX, uniY] = new Object(placeId, objectSet.getObject(placeId));
                                player.inventory.DecrementSlot(12 + hotbar.focus);
                                hasClicked = true;
                            }
                            //enter a crafting menu if its a crafting ting
                            if (objectSet.getObject(map.objectData[uniX, uniY].id).craftingStation && !hasClicked)
                            {
                                //open a crafting menu with parameter name of block
                                inventory.Open(objectSet.getObject(map.objectData[uniX, uniY].id).name, false, new Int2(0, 0));
                                hasClicked = true;
                            }
                            //enter a storage menu if its a storage ting
                            if (objectSet.getObject(map.objectData[uniX, uniY].id).stores && !hasClicked)
                            {
                                //open a crafting menu with parameter name of block
                                inventory.Open(objectSet.getObject(map.objectData[uniX, uniY].id).name, true, new Int2(uniX, uniY));
                                hasClicked = true;
                            }
                            //enter a storage menu if its a storage ting
                            if (objectSet.getObject(map.objectData[uniX, uniY].id).isBed && !hasClicked)
                            {
                                //open a crafting menu with parameter name of block
                                player.sleeping = true;
                                hasClicked = true;
                            }
                            if (itemSet.getItem(player.inventory.inventorySlots[12 + hotbar.focus].x).edible && !hasClicked)
                            {
                                //eat the food
                                player.inventory.DecrementSlot(12 + hotbar.focus);
                                player.Heal(5);
                            }
                        }
                    }

                }

                foreach (Entity e in entityData)
                {
                    e.Update(map, tileSet, objectSet, randomGenerator, particleSet, player, entitySet);
                    if (e.die && entitySet.GetEntity(e.entityID).drops)
                    {
                        map.SummonItem(randomGenerator, e.x, e.y, particleSet.GetParticle(3), entitySet.GetEntity(e.entityID).dropID);
                    }
                }
                entityData.RemoveAll(e => e.die);
            }
            else
            {//handle the GUI
                if (keys.IsKeyDown(Keys.Escape))
                {
                    inGUI = false;
                }
                if (zoom < maxZoom)
                {
                    zoom += zoomSpeed;
                }
            }
            hotbar.Update(keys);
            
            oldkeys = keys;
            oldMouse = mouseInput;

            base.Update(gameTime);
        }

        protected int touchingEntity(Vector2 pos, List<Entity> entities)
        {
            int entity = -1;
            int entCount = 0;
            foreach (Entity e in entities)
            {
                if (pos.X >= e.x && pos.X <= e.x + 1 && pos.Y >= e.y && pos.Y <= e.y + 1)
                {
                    entity = entCount;
                }
                entCount++;
            }
            return entity;
        }

        protected Vector2 ScreenToUniversal(int screenX, int screenY)
        {
            float uniX = (screenX - centreX + (player.width * tileScale / 2) + (player.x * tileScale)) / tileScale;
            float uniY = (screenY - centreY + (player.height * tileScale / 2) + (player.y * tileScale)) / tileScale;
            return new Vector2(uniX, uniY);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);


            // TODO: Add your drawing code here
            RenderDetails renderDetails = new RenderDetails(width, height, tileScale, player.width, player.height, player.x, player.y, centreX, centreY, zoom, lightSet);
            lightSet.lights = map.FindLights((int)player.x, (int)player.y, renderDetails);
            GraphicsDevice.SetRenderTarget(darkness);
            GraphicsDevice.Clear(skyColor);

            var blend = new BlendState
            {
                AlphaBlendFunction = BlendFunction.ReverseSubtract,
                AlphaSourceBlend = Blend.One,
                AlphaDestinationBlend = Blend.One,
                ColorBlendFunction = BlendFunction.Add,
                ColorSourceBlend = Blend.One,
                ColorDestinationBlend = Blend.One,
            };
            spriteBatch.Begin(blendState: blend);
            lightSet.Draw(spriteBatch, lightMask, renderDetails);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //DO OUR ACTUAL RENDER
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);

            //DRAW OUR WHOLE GAME
            map.Draw(spriteBatch, renderDetails, lightingShader, tileSet, objectSet, ambientLight, GraphicsDevice, particleSet, itemSet);
            foreach (Entity e in entityData)
            {
                e.Draw(spriteBatch, npc, tile1, font, renderDetails, particleSet, entitySet);
            }
            player.Draw(spriteBatch, renderDetails);
            spriteBatch.End();

            
            spriteBatch.Begin();
            spriteBatch.Draw(darkness, Vector2.Zero, Color.White);
            if (inGUI && zoom == maxZoom)
            {
                discGUI.Draw(spriteBatch, tile1, width, height);
            }
            else
            {
                if (player.IsDead())
                {
                    deathGUI.Draw(spriteBatch, tile1, font, width, height);
                }
                hotbar.Draw(spriteBatch, tile1, font, renderDetails, player.inventory, itemSet);
                inventory.Draw(spriteBatch, tile1, font, renderDetails, player.inventory, itemSet, recipeSet, map.objectData[inventory.accessed.x, inventory.accessed.y]);
                healthBar.Draw(spriteBatch, tile1, renderDetails, player.health, player.maxHealth);
                hungerBar.Draw(spriteBatch, tile1, renderDetails, player.hunger, player.maxHunger);
                sleepBar.Draw(spriteBatch, tile1, renderDetails, player.sleep, player.maxSleep);
            }

            //foreach (Entity e in entityData)
            //{
            //    e.Draw(spriteBatch, tile1, tile1, font, renderDetails, particleSet, entitySet);
            //}

            //DRAW CLOCK
            float perMin = 6.28f / (24 * 60);
            int hour = (int)Math.Floor(time / (perMin * 60));
            int min = (int)Math.Floor((time % (perMin * 60)) / perMin);
            string timeString = hour.ToString() + ":" + min.ToString();
            spriteBatch.DrawString(font, timeString, new Vector2(0, 0), Color.Red);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
