using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Primal
{
    public class Inventory
    {
        public int inventorySize;
        public Int2[] inventorySlots;
        Int2 preferredRegion;
        bool hasRegion = false;

        public Inventory(int invSize, Int2 preffered)
        {
            inventorySize = invSize;
            inventorySlots = new Int2[invSize];
            hasRegion = true;
            preferredRegion = preffered;
            ResetInventory();
        }

        public void ResetInventory()
        {
            for (int i = 0; i < inventorySize; i++)
            {
                inventorySlots[i] = new Int2(0, 0);
            }
        }

        public int CountItem(int id)
        {//count the number of item with id
            int count = 0;
            for (int i = 0; i < inventorySize; i++)
            {
                if (inventorySlots[i].x == id)
                {
                    count += inventorySlots[i].y;
                }
            }
            return count;
        }

        public void RemoveItem(int id, int quantity)
        {
            int toRemove = quantity;
            for (int i = 0; i < inventorySize; i++)
            {
                if (inventorySlots[i].x == id && toRemove > 0)
                {
                    if (toRemove > inventorySlots[i].y)
                    {//remove the y value,and deduct from toRemove
                        toRemove -= inventorySlots[i].y;
                        inventorySlots[i].y = 0;
                    }
                    else
                    {//remove the toRemove and set it to 0
                        inventorySlots[i].y -= toRemove;
                        toRemove = 0;
                    }
                    //if we've emptied the slot? blank it
                    if (inventorySlots[i].y == 0)
                    {
                        inventorySlots[i].x = 0;
                    }
                }
            }
        }

        public void SwapSlot(int slotA, int slotB)
        {
            Int2 buffer = inventorySlots[slotA];
            inventorySlots[slotA] = inventorySlots[slotB];
            inventorySlots[slotB] = buffer;
        }

        public int FindBestSlot(int id)
        {
            int bestSlot = -1;
            bool placed = false;
            bool withCorrectItem = false;
            bool inPreffered = false;

            //search the whole area
            for (int i = 0; i < inventorySize; i++)
            {
                bool blankSlot = inventorySlots[i].x == 0;
                bool correctSlot = inventorySlots[i].x == id;
                bool preffered = i > preferredRegion.x && i <= preferredRegion.y;
                if (!withCorrectItem)
                {
                    if (correctSlot)
                    {//if this is the correct slot, we place here
                        withCorrectItem = true;
                        bestSlot = i;
                    }
                    else
                    {//if its not the correct slot, we should check if its blank
                        if (blankSlot)
                        {//okay its blank, we can use it
                            if (!inPreffered)
                            {//okay its not already in a prefered slot, so if this one is preffered... USE IT
                                if (preffered)
                                {
                                    inPreffered = true;
                                    bestSlot = i;
                                }
                                else
                                {//okay its not a prefered slot... so has it even been placed yet
                                    if (!placed)
                                    {
                                        placed = true;
                                        bestSlot = i;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    //its with the right item, we got it right
                }
            }

            return bestSlot;
        }

        public void DecrementSlot(int slotID)
        {
            inventorySlots[slotID].y -= 1;

            if (inventorySlots[slotID].y == 0)
            {//if that was the last one? empty the slot
                inventorySlots[slotID] = new Int2(0, 0);
            }
        }

        public bool AddItem(Int2 item)
        {//return true if it is added successfully
            int bestSlot = FindBestSlot(item.x);

            if (bestSlot == -1)
            {//no empty or matching slot found
                return false;
            }
            inventorySlots[bestSlot].x = item.x;
            inventorySlots[bestSlot].y += item.y;
            return true;
        }
    }
}
