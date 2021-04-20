using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Primal
{
    public class BlendedTextures
    {
        public Texture2D up;
        public Texture2D down;
        public Texture2D left;
        public Texture2D right;
        public List<int> overlaps;

        public bool overlapsID(int id)
        {
            bool overlapID = false;
            foreach (int i in overlaps)
            {
                if (i == id)
                {
                    overlapID = true;
                }
            }
            return overlapID;
        }

        public BlendedTextures(ContentManager content, XElement blendedTextureData)
        {
            up = content.Load<Texture2D>(blendedTextureData.Element("up").Value);
            down = content.Load<Texture2D>(blendedTextureData.Element("down").Value);
            left = content.Load<Texture2D>(blendedTextureData.Element("left").Value);
            right = content.Load<Texture2D>(blendedTextureData.Element("right").Value);

            XElement overlapData = blendedTextureData.Element("overlaps");
            overlaps = new List<int>();

            foreach (XElement overlapElement in overlapData.Elements("overlap"))
            {
                overlaps.Add(int.Parse(overlapElement.Value));
            }
        }
    }

    public class TileSet
    {
        TileTemplate[] tileData;
        int tileCount;

        public void LoadTileSet(ContentManager content, string fileLocation)
        {
            //Load the file
            XElement file = XElement.Load(fileLocation);

            tileCount = 0;
            foreach (XElement tile in file.Elements("tile"))
            {
                tileCount += 1;
            }
            //create the tile template set
            tileData = new TileTemplate[tileCount];
            //populate the tile set with the data from the file
            tileCount = 0;
            foreach (XElement tile in file.Elements("tile"))
            {
                bool blocks = bool.Parse(tile.Element("blocks").Value);
                Console.WriteLine(blocks);
                String name = tile.Element("name").Value;
                String texture = tile.Element("texture").Value;
                XElement blends = tile.Element("blendedTextures");
                BlendedTextures blendedTextures = new BlendedTextures(content, blends);
                tileData[tileCount] = new TileTemplate(content.Load<Texture2D>(texture), blocks, name, blendedTextures);
                tileCount += 1;
            }
        }

        public TileTemplate getTile(int id)
        {
            return tileData[id];
        }
    }

    public class TileTemplate
    {
        public Texture2D texture;
        public BlendedTextures blendedTextures;
        public bool blocks;
        public string name;
        public float xOffset;
        public float yOffset;
        public float width;
        public float height;


        public TileTemplate(Texture2D tex, bool blocking, string newname, BlendedTextures blends)
        {
            texture = tex;
            blocks = blocking;
            name = newname;
            blendedTextures = blends;
        }
    }

    public class ObjectSet
    {
        ObjectTemplate[] objectData;
        int objectCount;

        public void LoadObjectSet(ContentManager content, string fileLocation)
        {
            //Load the file
            XElement file = XElement.Load(fileLocation);

            objectCount = 0;
            foreach (XElement newObject in file.Elements("object"))
            {
                objectCount += 1;
            }
            //create the tile template set
            objectData = new ObjectTemplate[objectCount];
            //populate the tile set with the data from the file
            objectCount = 0;
            foreach (XElement newObject in file.Elements("object"))
            {
                bool blocks = bool.Parse(newObject.Element("blocks").Value);
                Console.WriteLine(blocks);
                String name = newObject.Element("name").Value;
                String texture = newObject.Element("texture").Value;
                float xOffset = float.Parse(newObject.Element("xoffset").Value);
                float yOffset = float.Parse(newObject.Element("yoffset").Value);
                float width = float.Parse(newObject.Element("width").Value);
                float height = float.Parse(newObject.Element("height").Value);
                bool breaks = bool.Parse(newObject.Element("breakable").Value);
                int givenItem = int.Parse(newObject.Element("gives").Value);
                bool hasParticles = bool.Parse(newObject.Element("particles").Value);
                int partType = int.Parse(newObject.Element("particleType").Value);
                float visWidth = float.Parse(newObject.Element("visWidth").Value);
                float visHeight = float.Parse(newObject.Element("visHeight").Value);
                float visOffX = float.Parse(newObject.Element("visOffx").Value);
                float visOffY = float.Parse(newObject.Element("visOffy").Value);
                bool isCraftingStation = bool.Parse(newObject.Element("isCraftingStation").Value);
                bool stores = bool.Parse(newObject.Element("stores").Value);
                int storageSpace = int.Parse(newObject.Element("storageSpace").Value);
                bool isBed = bool.Parse(newObject.Element("isBed").Value);
                objectData[objectCount] = new ObjectTemplate(content.Load<Texture2D>(texture), blocks, name, xOffset, yOffset, width, height, breaks, givenItem, hasParticles,partType, visWidth, visHeight, visOffX, visOffY, isCraftingStation, stores, storageSpace, isBed);
                objectCount += 1;
            }
        }

        public ObjectTemplate getObject(int id)
        {
            return objectData[id];
        }
    }

    public class ObjectTemplate
    {
        public Texture2D texture;
        public bool blocks;
        public string name;
        public float xOffset;
        public float yOffset;
        public float width;
        public float height;
        public bool breakable;
        public int gives;
        public bool particles;
        public int particleType;
        public float visWidth;
        public float visHeight;
        public float visOffx;
        public float visOffy;
        public bool craftingStation;
        public bool stores;
        public int storageSpace;
        public bool isBed;

        public ObjectTemplate(Texture2D tex, bool blocking, string newname, float xof, float yof, float w, float h, bool breaks, int givenItem, bool parts, int partType, float vw, float vh, float voffx, float voffy, bool isCraftingStation, bool storesState, int invSpace, bool isABed)
        {
            texture = tex;
            blocks = blocking;
            name = newname;
            xOffset = xof;
            yOffset = yof;
            width = w;
            height = h;
            breakable = breaks;
            gives = givenItem;
            particles = parts;
            particleType = partType;
            visWidth = vw;
            visHeight = vh;
            visOffx = voffx;
            visOffy = voffy;
            craftingStation = isCraftingStation;
            stores = storesState;
            storageSpace = invSpace;
            isBed = isABed;
        }
    }

    public class ItemSet
    {
        ItemTemplate[] itemData;
        int itemCount;

        public void LoadItemSet(ContentManager content, string fileLocation)
        {
            //Load the file
            XElement file = XElement.Load(fileLocation);

            itemCount = 0;
            foreach (XElement newItem in file.Elements("item"))
            {
                itemCount += 1;
            }
            //create the tile template set
            itemData = new ItemTemplate[itemCount];
            //populate the tile set with the data from the file
            itemCount = 0;
            foreach (XElement newItem in file.Elements("item"))
            {
                String name = newItem.Element("name").Value;
                String texture = newItem.Element("texture").Value;
                bool placeable = bool.Parse(newItem.Element("placeable").Value);
                int places = int.Parse(newItem.Element("places").Value);
                bool edible = bool.Parse(newItem.Element("edible").Value);
                float damage = float.Parse(newItem.Element("damage").Value);
                itemData[itemCount] = new ItemTemplate(content.Load<Texture2D>(texture), name, placeable, places, edible, damage);
                itemCount += 1;
            }
        }

        public ItemTemplate getItem(int id)
        {
            return itemData[id];
        }
    }

    public class ItemTemplate
    {
        public Texture2D texture;
        public string name;
        public bool placeable;
        public int places;
        public bool edible;
        public float damage;

        public ItemTemplate(Texture2D tex, string newname, bool canPlace, int placeID, bool canEat, float dmg)
        {
            texture = tex;
            name = newname;
            placeable = canPlace;
            places = placeID;
            edible = canEat;
            damage = dmg;
        }
    }

    public class ParticleSet
    {
        ParticleTemplate[] particleData;
        int particleCount;

        public void LoadParticleSet(ContentManager content, string fileLocation)
        {
            //Load the file
            XElement file = XElement.Load(fileLocation);

            particleCount = 0;
            foreach (XElement newParticle in file.Elements("particle"))
            {
                particleCount += 1;
            }
            //create the tile template set
            particleData = new ParticleTemplate[particleCount];
            //populate the tile set with the data from the file
            particleCount = 0;
            foreach (XElement newParticle in file.Elements("particle"))
            {
                String name = newParticle.Element("name").Value;
                String texture = newParticle.Element("texture").Value;
                float width = float.Parse(newParticle.Element("width").Value);
                float height = float.Parse(newParticle.Element("height").Value);
                float gravity = float.Parse(newParticle.Element("gravity").Value);
                float mingravity = float.Parse(newParticle.Element("mingravity").Value);
                float vz = float.Parse(newParticle.Element("vz").Value);
                int lifespan = int.Parse(newParticle.Element("lifespan").Value);
                float minvx = float.Parse(newParticle.Element("minvx").Value);
                float maxvx = float.Parse(newParticle.Element("maxvx").Value);
                float minvy = float.Parse(newParticle.Element("minvy").Value);
                float maxvy = float.Parse(newParticle.Element("maxvy").Value);
                int freq = int.Parse(newParticle.Element("emitfreq").Value);
                particleData[particleCount] = new ParticleTemplate(content.Load<Texture2D>(texture), name, width,height,gravity,lifespan,vz,minvx,maxvx,freq,mingravity, minvy, maxvy);
                particleCount += 1;
            }
        }

        public ParticleTemplate GetParticle(int id)
        {
            return particleData[id];
        }
    }

    public class ParticleTemplate
    {
        public Texture2D texture;
        public string name;
        public float width;
        public float height;
        public float gravity;
        public float mingravity;
        public int lifeSpan;
        public float vz;
        public float minVX;
        public float maxVX;
        public float minVY;
        public float maxVY;
        public int emitFreq;

        public ParticleTemplate(Texture2D tex, string newname, float thisWidth, float thisHeight, float thisGravity, int lifeLength, float ivz, float nminVX, float nmaxVX, int freq, float ming, float nminVY, float nmaxVY)
        {
            texture = tex;
            name = newname;
            width = thisWidth;
            height = thisHeight;
            gravity = thisGravity;
            lifeSpan = lifeLength;
            vz = ivz;
            minVX = nminVX;
            maxVX = nmaxVX;
            minVY = nminVY;
            maxVY = nmaxVY;
            emitFreq = freq;
            mingravity = ming;
        }
    }

    public class EntitySet
    {
        EntityTemplate[] entityData;
        int entityCount;

        public void LoadEntitySet(ContentManager content, string fileLocation)
        {
            //Load the file
            XElement file = XElement.Load(fileLocation);

            entityCount = 0;
            foreach (XElement newItem in file.Elements("entity"))
            {
                entityCount += 1;
            }
            //create the tile template set
            entityData = new EntityTemplate[entityCount];
            //populate the tile set with the data from the file
            entityCount = 0;
            foreach (XElement newEntity in file.Elements("entity"))
            {
                String name = newEntity.Element("name").Value;
                String texture = newEntity.Element("texture").Value;
                bool attacksPlayer = bool.Parse(newEntity.Element("attacksPlayer").Value);
                bool communicates = bool.Parse(newEntity.Element("communicates").Value);
                float moveSpeed = float.Parse(newEntity.Element("speed").Value);
                float visWidth = float.Parse(newEntity.Element("visWidth").Value);
                float visHeight = float.Parse(newEntity.Element("visHeight").Value);
                float visOffsetX = float.Parse(newEntity.Element("visOffX").Value);
                float visOffsetY = float.Parse(newEntity.Element("visOffY").Value);
                bool dropsItem = bool.Parse(newEntity.Element("drops").Value);
                int idDropped = int.Parse(newEntity.Element("dropsItem").Value);
                bool particles = bool.Parse(newEntity.Element("hasParticles").Value);
                entityData[entityCount] = new EntityTemplate(content.Load<Texture2D>(texture), name, attacksPlayer, moveSpeed, visWidth, visHeight, visOffsetX, visOffsetY, communicates, dropsItem, idDropped, particles);
                entityCount += 1;
            }
        }

        public EntityTemplate GetEntity(int id)
        {
            return entityData[id];
        }
    }

    public class EntityTemplate
    {
        public Texture2D texture;
        public string name;
        public bool attacksPlayer;
        public bool communicates;
        public float speed;
        public float visWidth;
        public float visHeight;
        public float visOffx;
        public float visOffy;
        public bool drops;
        public int dropID;
        public bool hasParticles;

        public EntityTemplate(Texture2D tex, string newname, bool attackPlayer, float moveSpeed, float vw, float vh, float vox, float voy, bool talks, bool dropsItem, int idDropped, bool particles)
        {
            texture = tex;
            name = newname;
            attacksPlayer = attackPlayer;
            speed = moveSpeed;
            visWidth = vw;
            visHeight = vh;
            visOffx = vox;
            visOffy = voy;
            communicates = talks;
            drops = dropsItem;
            dropID = idDropped;
            hasParticles = particles;
        }
    }

    public class RecipeSet
    {
        RecipeTemplate[] recipeData;
        public int recipeCount;

        public void LoadRecipeSet(ContentManager content, string fileLocation)
        {
            //Load the file
            XElement file = XElement.Load(fileLocation);

            recipeCount = 0;
            foreach (XElement newItem in file.Elements("recipe"))
            {
                recipeCount += 1;
            }
            //create the tile template set
            recipeData = new RecipeTemplate[recipeCount];
            //populate the tile set with the data from the file
            recipeCount = 0;
            foreach (XElement newItem in file.Elements("recipe"))
            {
                String name = newItem.Element("name").Value;
                int r1 = int.Parse(newItem.Element("req1").Value);
                int r2 = int.Parse(newItem.Element("req2").Value);
                int r3 = int.Parse(newItem.Element("req3").Value);
                int q1 = int.Parse(newItem.Element("qua1").Value);
                int q2 = int.Parse(newItem.Element("qua2").Value);
                int q3 = int.Parse(newItem.Element("qua3").Value);
                int yield = int.Parse(newItem.Element("yield").Value);
                int quay = int.Parse(newItem.Element("quay").Value);
                String craftsIn = newItem.Element("craftsIn").Value;
                recipeData[recipeCount] = new RecipeTemplate(name, r1, r2, r3, q1, q2, q3, yield, quay, craftsIn);
                recipeCount += 1;
            }
        }

        public List<RecipeTemplate> getRecipesOf(string craftsIn)
        {
            List<RecipeTemplate> recipes = new List<RecipeTemplate>();
            foreach(RecipeTemplate r in recipeData)
            {
                if (r.craftsIn == craftsIn)
                {
                    recipes.Add(r);
                }
            }
            return recipes;
        }

        public RecipeTemplate getRecipe(int id)
        {
            return recipeData[id];
        }
    }

    public class RecipeTemplate
    {
        public string name;
        public int[] reqs;
        public int[] quas;
        public int yield;
        public int quay;
        public string craftsIn;

        public RecipeTemplate(string newname, int r1, int r2, int r3, int q1, int q2, int q3, int prod, int qy, string craftsAt)
        {
            name = newname;
            reqs = new int[3];
            quas = new int[3];
            reqs[0] = r1;
            reqs[1] = r2;
            reqs[2] = r3;
            quas[0] = q1;
            quas[1] = q2;
            quas[2] = q3;
            yield = prod;
            quay = qy;
            craftsIn = craftsAt;
        }
    }
}
