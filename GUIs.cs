using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Primal
{
    public class DiscussionGUI
    {
        public int width = 500;
        public int height = 300;

        public void Draw(SpriteBatch spriteBatch, Texture2D texture, int screenWidth, int screenHeight)
        {
            spriteBatch.Draw(texture, new Rectangle((screenWidth - width) / 2, (screenHeight - height) / 2, width, height), Color.Wheat);
        }
    }

    public class DeathGUI
    {
        public int width = 500;
        public int height = 300;

        public void Draw(SpriteBatch spriteBatch, Texture2D texture, SpriteFont font, int screenWidth, int screenHeight)
        {
            spriteBatch.Draw(texture, new Rectangle((screenWidth - width) / 2, (screenHeight - height) / 2, width, height), Color.Red);
            spriteBatch.DrawString(font, "YOU DIED BUCKO", new Vector2((screenWidth - width) / 2, (screenHeight - height) / 2), Color.White);
        }
    }

    class Bar
    {
        int width = 200;
        int height = 40;
        float x;
        float y;

        public Bar()
        {

        }
        public Bar(float xPos, float yPos)
        {
            x = xPos;
            y = yPos;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture, RenderDetails renderDetails, float val, float maxval)
        {
            float scale = val / maxval;
            int xPos = (int)(renderDetails.screenX * x);
            int yPos = (int)(renderDetails.screenY * y);
            spriteBatch.Draw(texture, new Rectangle(xPos, yPos, width, height), Color.White);
            spriteBatch.Draw(texture, new Rectangle(xPos, yPos, (int)(width * scale), height), Color.Red);
        }

        public void DrawUniversal(SpriteBatch spriteBatch, Texture2D texture, RenderDetails renderDetails, float val, float maxval, Rectangle location, float y)
        {
            float scale = val / maxval;
            spriteBatch.Draw(texture, location, null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, ((y + 1.1f) / 250f));
            spriteBatch.Draw(texture, new Rectangle(location.X, location.Y, (int)(location.Width * scale), location.Height), null, Color.Red, 0f, new Vector2(0, 0), SpriteEffects.None, ((y + 1.11f) / 250f));
        }
    }

    class HotbarGUI
    {
        int size = 6;
        int itemX = 64;
        public int focus = 0;

        public void Update(KeyboardState keys)
        {
            if (keys.IsKeyDown(Keys.D1))
            {
                focus = 0;
            }
            if (keys.IsKeyDown(Keys.D2))
            {
                focus = 1;
            }
            if (keys.IsKeyDown(Keys.D3))
            {
                focus = 2;
            }
            if (keys.IsKeyDown(Keys.D4))
            {
                focus = 3;
            }
            if (keys.IsKeyDown(Keys.D5))
            {
                focus = 4;
            }
            if (keys.IsKeyDown(Keys.D6))
            {
                focus = 5;
            }
        }

        public void Draw(SpriteBatch spritebatch, Texture2D texture, SpriteFont font, RenderDetails renderDetails, Inventory playerInventory, ItemSet items)
        {
            int anchorX = (renderDetails.screenX - (size * itemX)) / 2;
            int anchorY = renderDetails.screenY - itemX;
            for (int i = 0; i < size; i++)
            {
                int thisItem = 12 + i;
                ItemTemplate item = items.getItem(playerInventory.inventorySlots[thisItem].x);
                if (i == focus)
                {
                    spritebatch.Draw(texture, new Rectangle(anchorX + (i * itemX), anchorY, itemX, itemX), Color.Red);
                }
                else
                {
                    spritebatch.Draw(texture, new Rectangle(anchorX + (i * itemX), anchorY, itemX, itemX), Color.White);
                }
                spritebatch.Draw(item.texture, new Rectangle(anchorX + (i * itemX) + 8, anchorY + 8, 48, 48), Color.White);
                spritebatch.DrawString(font, playerInventory.inventorySlots[thisItem].y.ToString(), new Vector2(anchorX + (i * itemX), anchorY + 26), Color.Red);
            }
        }
    }

    class InventoryCraftingGUI
    {
        int sizeX = 6;
        int sizeY = 3;
        int itemX = 64;
        int focus = 0;
        int validRecipes = 0;
        public bool open = false;
        string craftType;
        bool storage;
        bool focusMain = true;
        bool oldFocusMain;
        public Int2 accessed = new Int2(0,0);
        

        public void Open(string crafting, bool isStorage, Int2 accessedObject)
        {
            craftType = crafting;
            storage = isStorage;
            open = true;
            accessed = accessedObject;
            //reset our focus to an impossible value
            focus = -1;
        }

        public void Update(KeyboardState keys, KeyboardState oldKeys, MouseState mouse, MouseState oldMouse, RenderDetails renderDetails, Inventory playerInventory, RecipeSet recipes, Object accessedObject)
        {
            if (keys.IsKeyDown(Keys.Escape) && oldKeys.IsKeyUp(Keys.Escape))
            {
                open = false;
            }
            if (keys.IsKeyDown(Keys.E) && oldKeys.IsKeyUp(Keys.E))
            {
                open = false;
            }

            //if it clicks
            if (mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released)
            {
                List<RecipeTemplate> currentRecipes = recipes.getRecipesOf(craftType);
                int anchorX = (renderDetails.screenX - (sizeX * itemX)) / 2;
                int anchorY = (renderDetails.screenY - (sizeY * itemX)) / 2;
                //handle the clicking location for crafting
                if (mouse.X > anchorX && mouse.X < anchorX + 425 &&mouse.Y > anchorY)
                {//clicking the main panel
                    oldFocusMain = focusMain;
                    focusMain = true;
                    //ADJUST THE MOUSE FOCUS
                    int xClicked = (int)Math.Floor((float)((mouse.X - anchorX) / itemX));
                    int yClicked = (int)Math.Floor((float)((mouse.Y - anchorY) / itemX));
                    int oldFocus = focus;
                    focus = (yClicked * sizeX) + xClicked;
                    if (oldFocus != -1)
                    {//not our first move, so now the mechanics begins
                        if (focus == oldFocus && oldFocusMain == focusMain)
                        {//deselection mode
                            focus = -1;
                        }
                        else
                        {//switch the inventory slots
                            Inventory current;
                            Inventory oldInv;
                            if (focusMain == true)
                            {
                                current = playerInventory;
                            }
                            else
                            {
                                current = accessedObject.inventory;
                            }
                            if (oldFocusMain == true)
                            {
                                oldInv = playerInventory;
                            }
                            else
                            {
                                oldInv = accessedObject.inventory;
                            }
                            SwapBetweenInvs(current, oldInv, focus, oldFocus);
                            focus = -1;
                        }
                    }
                }
                if (mouse.X > anchorX + 425 && mouse.Y > anchorY)//clicking the right panel
                {
                    oldFocusMain = focusMain;
                    focusMain = false;
                    if (!storage)//HANDLE CRAFTING INTERACTIONS
                    {
                        int yOffset = mouse.Y - anchorY;
                        yOffset = (int)Math.Floor((float)(yOffset / itemX));
                        PerformCraft(playerInventory, currentRecipes[yOffset]);
                    }
                    else
                    {
                        int xClicked = (int)Math.Floor((float)((mouse.X - (anchorX + 425)) / itemX));
                        int yClicked = (int)Math.Floor((float)((mouse.Y - (anchorY)) / itemX));
                        int oldFocus = focus;
                        focus = (yClicked * 5) + xClicked;
                        if (oldFocus != -1)
                        {//not our first move, so now the mechanics begins
                            if (focus == oldFocus && oldFocusMain == focusMain)
                            {//deselect focus
                                focus = -1;
                            }
                            else
                            {//switch the inventory Slots
                                Inventory current;
                                Inventory oldInv;
                                if (focusMain == true)
                                {
                                    current = playerInventory;
                                }
                                else
                                {
                                    current = accessedObject.inventory;
                                }
                                if (oldFocusMain == true)
                                {
                                    oldInv = playerInventory;
                                }
                                else
                                {
                                    oldInv = accessedObject.inventory;
                                }
                                SwapBetweenInvs(current, oldInv, focus, oldFocus);
                                focus = -1;
                            }
                        }
                    }
                }
            }
        }



        public void SwapBetweenInvs(Inventory invA, Inventory invB, int slotA, int slotB)
        {
            //if they're the same, we merge them...
            if (invA.inventorySlots[slotA].x == invB.inventorySlots[slotB].x)
            {
                //merge the quanitities into the new place and wipe the old
                invA.inventorySlots[slotA].y += invB.inventorySlots[slotB].y;
                invB.inventorySlots[slotB] = new Int2(0, 0);
            }
            else
            {
                Int2 buffer = invA.inventorySlots[slotA];
                invA.inventorySlots[slotA] = invB.inventorySlots[slotB];
                invB.inventorySlots[slotB] = buffer;
            }
        }

        public void PerformCraft(Inventory inventory, RecipeTemplate recipe)
        {
            bool r1 = recipe.reqs[0] == 0 || inventory.CountItem(recipe.reqs[0]) >= recipe.quas[0];
            bool r2 = recipe.reqs[1] == 0 || inventory.CountItem(recipe.reqs[1]) >= recipe.quas[1];
            bool r3 = recipe.reqs[2] == 0 || inventory.CountItem(recipe.reqs[2]) >= recipe.quas[2];
            if (r1 && r2 && r3)
            {
                //Remove the requirements
                if (recipe.reqs[0] != 0)
                {
                    //remove recipe.quas[0]
                    inventory.RemoveItem(recipe.reqs[0], recipe.quas[0]);
                }
                if (recipe.reqs[1] != 0)
                {
                    //remove recipe.quas[1]
                    inventory.RemoveItem(recipe.reqs[1], recipe.quas[1]);
                }
                if (recipe.reqs[2] != 0)
                {
                    //remove recipe.quas[2]
                    inventory.RemoveItem(recipe.reqs[2], recipe.quas[2]);
                }
                inventory.AddItem(new Int2(recipe.yield, recipe.quay));
            }
        }

        public void Draw(SpriteBatch spritebatch, Texture2D texture, SpriteFont font, RenderDetails renderDetails, Inventory playerInventory, ItemSet items, RecipeSet recipes, Object accessedObject)
        {
            if (open)
            {
                int anchorX = (renderDetails.screenX - (sizeX * itemX)) / 2;
                int anchorY = (renderDetails.screenY - (sizeY * itemX)) / 2;
                for (int x = 0; x < sizeX; x++)
                {
                    for (int y = 0; y < sizeY; y++)
                    {
                        if (focusMain && focus == (y * sizeX) + x)
                        {
                            spritebatch.Draw(texture, new Rectangle(anchorX + (x * itemX), anchorY + (y * itemX), itemX, itemX), Color.Yellow);
                        }
                        else
                        {
                            spritebatch.Draw(texture, new Rectangle(anchorX + (x * itemX), anchorY + (y * itemX), itemX, itemX), Color.White);
                        }
                        int thisItem = (y * 6) + x;
                        ItemTemplate item = items.getItem(playerInventory.inventorySlots[thisItem].x);
                        spritebatch.Draw(item.texture, new Rectangle(anchorX + (x * itemX) + 8, anchorY + (y * itemX) + 8, 48, 48), Color.White);
                        spritebatch.DrawString(font, playerInventory.inventorySlots[thisItem].y.ToString(), new Vector2(anchorX + (x * itemX), anchorY + 36 + (y * itemX)), Color.Red);
                    }
                }

                if (!storage)
                {
                    spritebatch.DrawString(font, "Crafting Options", new Vector2(anchorX + 425, anchorY - 32), Color.White);
                    List<RecipeTemplate> currentRecipes = recipes.getRecipesOf(craftType);
                    validRecipes = currentRecipes.Count;
                    for (int r = 0; r < validRecipes; r++)
                    {
                        RecipeTemplate recipe = currentRecipes[r];
                        spritebatch.Draw(texture, new Rectangle(anchorX + 425, anchorY + (r * itemX), itemX * 7, itemX), Color.Wheat);
                        spritebatch.Draw(items.getItem(recipe.yield).texture, new Rectangle(anchorX + 425 + 8, anchorY + (r * itemX) + 8, itemX - 16, itemX - 16), Color.White);
                        spritebatch.DrawString(font, recipe.quay.ToString(), new Vector2(anchorX + 425 + 4, anchorY + (r * itemX) + 32), Color.Red);
                        spritebatch.DrawString(font, recipe.name, new Vector2(anchorX + 425 + 10 + itemX, anchorY + 15 + (r * itemX)), Color.White);
                        for (int i = 0; i < 3; i++)
                        {
                            spritebatch.Draw(items.getItem(recipe.reqs[i]).texture, new Rectangle(anchorX + 425 + (itemX * (4 + i)) + 8, anchorY + (r * itemX) + 8, itemX - 16, itemX - 16), Color.White);
                            spritebatch.DrawString(font, recipe.quas[i].ToString(), new Vector2(anchorX + 425 + (itemX * (4 + i)) - 4, anchorY + (r * itemX) + 32), Color.Red);
                        }
                    }
                }
                else
                {
                    spritebatch.DrawString(font, "Storage Container", new Vector2(anchorX + 425, anchorY - 32), Color.White);
                    for (int y = 0; y < 5; y++)
                    {
                        for (int x = 0; x < 5; x++)
                        {
                            if ((y * 5) + x < accessedObject.inventory.inventorySize)
                            {
                                int accessedIndex = (y * 5) + x;
                                if (!focusMain && focus == accessedIndex)
                                {
                                    spritebatch.Draw(texture, new Rectangle(anchorX + 425 + (x * itemX), anchorY + (y * itemX), itemX, itemX), Color.Yellow);
                                }
                                else
                                {
                                    spritebatch.Draw(texture, new Rectangle(anchorX + 425 + (x * itemX), anchorY + (y * itemX), itemX, itemX), Color.Wheat);
                                }
                                ItemTemplate item = items.getItem(accessedObject.inventory.inventorySlots[accessedIndex].x);
                                spritebatch.Draw(item.texture, new Rectangle(anchorX + 425 + (x * itemX) + 8, anchorY + (y * itemX) + 8, 48, 48), Color.White);
                                spritebatch.DrawString(font, accessedObject.inventory.inventorySlots[accessedIndex].y.ToString(), new Vector2(anchorX + 425 + (x * itemX), anchorY + (y * itemX) + 36), Color.Red);
                            }
                        }
                    }
                }
            }
        }
    }

    class InventoryGUI
    {
        int sizeX = 6;
        int sizeY = 3;
        int itemX = 48;
        int focus = 0;
        public bool open = false;

        public void Update(KeyboardState keys, KeyboardState oldKeys, MouseState mouse)
        {
            if (keys.IsKeyDown(Keys.Escape) && oldKeys.IsKeyUp(Keys.Escape))
            {
                open = false;
            }
            if (keys.IsKeyDown(Keys.E) && oldKeys.IsKeyUp(Keys.E))
            {
                open = false;
            }
        }

        public void Draw(SpriteBatch spritebatch, Texture2D texture, SpriteFont font, RenderDetails renderDetails, Inventory playerInventory, ItemSet items)
        {
            if (open)
            {
                int anchorX = (renderDetails.screenX - (sizeX * itemX)) / 2;
                int anchorY = (renderDetails.screenY - (sizeY * itemX)) / 2;
                for (int x = 0; x < sizeX; x++)
                {
                    for (int y = 0; y < sizeY; y++)
                    {
                        spritebatch.Draw(texture, new Rectangle(anchorX + (x * itemX), anchorY + (y * itemX), itemX, itemX), Color.White);
                        int thisItem = (y * 6) + x;
                        ItemTemplate item = items.getItem(playerInventory.inventorySlots[thisItem].x);
                        spritebatch.Draw(item.texture, new Rectangle(anchorX + (x * itemX) + 8, anchorY + (y * itemX) + 8, 32, 32), Color.White);

                        spritebatch.DrawString(font, playerInventory.inventorySlots[thisItem].y.ToString(), new Vector2(anchorX + (x * itemX), anchorY + 26 + (y * itemX)), Color.Red);
                    }
                }
            }
        }
    }
}
