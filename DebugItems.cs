using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;
using Terraria.World.Generation;
using static Terraria.ModLoader.ModContent;

namespace Idglibrary
{


        public class IDGSettings : ModConfig
        {
            public static IDGSettings Instance;
            // You MUST specify a ConfigScope.
            public override ConfigScope Mode => ConfigScope.ServerSide;
            [Label("Items")]
            [Tooltip("Enables/Disables all items added by this mod")]
            [ReloadRequired]
            [DefaultValue(true)]
            public bool Items { get; set; }


        }


    public class IdgDebugItem: ModItem
    {

        public override bool Autoload(ref string name)
        {
            if (!GetInstance<IDGSettings>().Items || this.GetType()==typeof(IdgDebugItem))
                return false;

            return base.Autoload(ref name);
        }

        public override string Texture
        {
            get { return "Terraria/Item_" + 0; }
        }

                public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Debug Item base");
            Tooltip.SetDefault("IDGlib's debug item, it does nothing, seriously. It's only used as a parent for all other debug items");
        }

    }

    public class MagicMusicBox : ModItem
    {

        public int myMusic = -1;
        public string musicString = "";
        public int knownID = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Universal Music Box");
            Tooltip.SetDefault("Instantly records and stores the current playing song, even if they don't have a music box\nFunctions while favorited in your inventory\nDoesn't record vanilla tracks\nAdding new mods may cause the music box to reset");
        }

        public override bool CloneNewInstances => true;

        public override TagCompound Save()
        {
            TagCompound tag = new TagCompound();
            tag["myMusic"] = myMusic;
            tag["musicString"] = musicString;
            tag["knownID"] = item.type;
            return tag;
        }

        public override void Load(TagCompound tag)
        {
            knownID = tag.GetInt("knownID");
            if (knownID == item.type)
            {
                myMusic = tag.GetInt("myMusic");
                musicString = tag.GetString("musicString");
            }

        }

        public override void SetDefaults()
        {
            item.width = 14;
            item.height = 14;
            item.maxStack = 1;
            item.rare = 8;
            item.value = Item.buyPrice(platinum: 3);
            item.useStyle = 2;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useTurn = true;
            item.UseSound = SoundID.Item9;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            if (musicString != null && musicString != "")
                return Main.hslToRgb(((float)myMusic / MathHelper.Pi) % 1f, 0.8f, 0.75f);

            return Main.hslToRgb((Main.GlobalTime * 0.316f) % 1f, 0.65f, 0.75f);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (musicString != null && musicString != "")
                tooltips.Add(new TooltipLine(mod, "SGAmodMagicMusicBox", Idglib.ColorText(Main.hslToRgb((-Main.GlobalTime / 3f) % 1f, 0.8f, 0.8f), musicString)));
        }

        public override bool CanUseItem(Player player)
        {
            return Main.curMusic > 41;
        }

        public override bool UseItem(Player player)
        {

            FieldInfo sounds = typeof(SoundLoader).GetField("sounds", BindingFlags.Static | BindingFlags.NonPublic);
            IDictionary<SoundType, IDictionary<string, int>> sounds2 = (IDictionary<SoundType, IDictionary<string, int>>)sounds.GetValue(null);

            IDictionary<string, int> sounds3 = sounds2[SoundType.Music];

            musicString = sounds3.FirstOrDefault(x => x.Value == Main.curMusic).Key;
            myMusic = Main.curMusic;
            return true;
        }
        public override string Texture
        {
            get { return "Terraria/Item_" + ItemID.MusicBox; }
        }

    }

    public class TrueGodMode : IdgDebugItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("True Godmode");
            Tooltip.SetDefault("While in your inventory, prevents even Player.KillMe() Deaths");
        }
        public override string Texture
        {
            get { return "Terraria/Item_" + ItemID.RedPotion; }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Main.DiscoColor;
        }
        public override void SetDefaults()
        {
            item.maxStack = 1;
            item.width = 32;
            item.height = 32;
            item.value = 0;
            item.rare = ItemRarityID.Expert;
            item.UseSound = SoundID.Item2;
        }
    }

    public class DebugPotion1: IdgDebugItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("IDG's Radation I Potion");
            Tooltip.SetDefault("Gives the player radiation poisoning; consumption not recommended for the smart.");
        }
        public override string Texture
        {
            get { return "Terraria/Item_" + ItemID.RedPotion; }
        }
        public override void SetDefaults()
        {
            item.maxStack = 999;
            item.consumable = true;
            item.width = 32;
            item.height = 32;
            item.useTime = 2;
            item.useAnimation = 2;
            item.useStyle = 4;
            item.noMelee = true; //so the item's animation doesn't do damage
            item.value = 0;
            item.rare = 9;
            item.UseSound = SoundID.Item2;
        }

        public override bool UseItem(Player player)
        {
            IdgPlayer idgplayer = player.GetModPlayer(mod,typeof(IdgPlayer).Name) as IdgPlayer;
            //bdplayer.DyeStrengthBoost+=2;
            Main.PlaySound(13, (int)player.position.X, (int)player.position.Y, 0);
            IdgPlayer.AddRadiationDebuff(player,600,0);
            return true;
        }
    }
        public class DebugPotion2: IdgDebugItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("IDG's Radation II Potion");
            Tooltip.SetDefault("Gives the player radiation poisoning; recommended for those wanting to melt their skin off.");
        }
        public override string Texture
        {
            get { return "Terraria/Item_" + ItemID.RedPotion; }
        }
        public override void SetDefaults()
        {
            item.maxStack = 999;
            item.consumable = true;
            item.width = 32;
            item.height = 32;
            item.useTime = 2;
            item.useAnimation = 2;
            item.useStyle = 4;
            item.noMelee = true; //so the item's animation doesn't do damage
            item.value = 0;
            item.rare = 9;
            item.UseSound = SoundID.Item2;
        }

        public override bool UseItem(Player player)
        {
            IdgPlayer idgplayer = player.GetModPlayer(mod,typeof(IdgPlayer).Name) as IdgPlayer;
            //bdplayer.DyeStrengthBoost+=2;
            Main.PlaySound(13, (int)player.position.X, (int)player.position.Y, 0);
            IdgPlayer.AddRadiationDebuff(player,600,1);

            return true;
        }
    }

        public class DebugPotion3: IdgDebugItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("IDG's Radation III Potion");
            Tooltip.SetDefault("Gives the player radiation poisoning; recommended for ghouls only");
        }
        public override string Texture
        {
            get { return "Terraria/Item_" + ItemID.RedPotion; }
        }
        public override void SetDefaults()
        {
            item.maxStack = 999;
            item.consumable = true;
            item.width = 32;
            item.height = 32;
            item.useTime = 2;
            item.useAnimation = 2;
            item.useStyle = 4;
            item.noMelee = true; //so the item's animation doesn't do damage
            item.value = 0;
            item.rare = 9;
            item.UseSound = SoundID.Item2;
        }

        public override bool UseItem(Player player)
        {
            IdgPlayer idgplayer = player.GetModPlayer(mod,typeof(IdgPlayer).Name) as IdgPlayer;
            //bdplayer.DyeStrengthBoost+=2;
            Main.PlaySound(13, (int)player.position.X, (int)player.position.Y, 0);
            IdgPlayer.AddRadiationDebuff(player,600,2);

            return true;
        }
    }

    public class DebugPotion4 : IdgDebugItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("IDG's 'Boss Fight Purity' Potion");
            Tooltip.SetDefault("Prevents use of any items not present from a boss's mod provided the boss is from a mod\nis removed from players when no boss is alive");
        }
        public override string Texture
        {
            get { return "Terraria/Item_" + ItemID.RedPotion; }
        }
        public override void SetDefaults()
        {
            item.maxStack = 999;
            item.consumable = true;
            item.width = 32;
            item.height = 32;
            item.useTime = 2;
            item.useAnimation = 2;
            item.useStyle = 4;
            item.noMelee = true; //so the item's animation doesn't do damage
            item.value = 0;
            item.rare = 9;
            item.UseSound = SoundID.Item2;
        }

        public override bool UseItem(Player player)
        {
            IdgPlayer idgplayer = player.GetModPlayer(mod, typeof(IdgPlayer).Name) as IdgPlayer;
            //bdplayer.DyeStrengthBoost+=2;
            Main.PlaySound(13, (int)player.position.X, (int)player.position.Y, 0);
            player.AddBuff(mod.BuffType("BossFightPurity"), 600);

            return true;
        }
    }
    public class DebugPotion5: IdgDebugItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("IDG's 'Curse of Red' Potion");
            Tooltip.SetDefault("Will utterly butch players");
        }
        public override string Texture
        {
            get { return "Terraria/Item_" + ItemID.RedPotion; }
        }
        public override void SetDefaults()
        {
            item.maxStack = 999;
            item.consumable = true;
            item.width = 32;
            item.height = 32;
            item.useTime = 2;
            item.useAnimation = 2;
            item.useStyle = 4;
            item.noMelee = true; //so the item's animation doesn't do damage
            item.value = 0;
            item.rare = 9;
            item.UseSound = SoundID.Item2;
        }

        public override bool UseItem(Player player)
        {
            IdgPlayer idgplayer = player.GetModPlayer(mod,typeof(IdgPlayer).Name) as IdgPlayer;
            //bdplayer.DyeStrengthBoost+=2;
            Main.PlaySound(13, (int)player.position.X, (int)player.position.Y, 0);
            player.AddBuff(mod.BuffType("CurseOfRed"), 600);

            return true;
        }
    }    
    public class DebugPotion6: IdgDebugItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("IDG's SoulDrain I Potion");
            Tooltip.SetDefault("Sucks the source of a player's magic from them; a lichtime favorite!");
        }
        public override string Texture
        {
            get { return "Terraria/Item_" + ItemID.RedPotion; }
        }
        public override void SetDefaults()
        {
            item.maxStack = 999;
            item.consumable = true;
            item.width = 32;
            item.height = 32;
            item.useTime = 2;
            item.useAnimation = 2;
            item.useStyle = 4;
            item.noMelee = true; //so the item's animation doesn't do damage
            item.value = 0;
            item.rare = 9;
            item.UseSound = SoundID.Item2;
        }

        public override bool UseItem(Player player)
        {
            IdgPlayer idgplayer = player.GetModPlayer(mod,typeof(IdgPlayer).Name) as IdgPlayer;
            //bdplayer.DyeStrengthBoost+=2;
            Main.PlaySound(13, (int)player.position.X, (int)player.position.Y, 0);
            player.AddBuff(mod.BuffType("SoulDrain"), 600);

            return true;
        }
    }
    public class DebugPotion7 : IdgDebugItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("IDG's N0LL U Potion");
            Tooltip.SetDefault("N0LL U");
        }
        public override string Texture
        {
            get { return "Terraria/Item_" + ItemID.RedPotion; }
        }
        public override void SetDefaults()
        {
            item.maxStack = 999;
            item.consumable = true;
            item.width = 32;
            item.height = 32;
            item.useTime = 2;
            item.useAnimation = 2;
            item.useStyle = 4;
            item.noMelee = true; //so the item's animation doesn't do damage
            item.value = 0;
            item.rare = 9;
            item.UseSound = SoundID.Item2;
        }

        public override bool UseItem(Player player)
        {
            IdgPlayer idgplayer = player.GetModPlayer(mod, typeof(IdgPlayer).Name) as IdgPlayer;
            //bdplayer.DyeStrengthBoost+=2;
            Main.PlaySound(13, (int)player.position.X, (int)player.position.Y, 0);
            player.AddBuff(mod.BuffType("NullExceptionDebuff"), 300);

            return true;
        }
    }
    public class DebugWeapon1 : IdgDebugItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("IDG's True Butcher");
            Tooltip.SetDefault("Not even DoG will survive...\nRight Click to despawn enemies and make them directly drop loot");
        }
        public override void SetDefaults()
        {
            item.damage = 1;
            item.melee = true;
            item.width = 40;
            item.height = 40;
            item.useTime = 10;
            item.useAnimation = 10;
            item.useStyle = 1;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 0, 0, 0);
            item.rare = 9;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
        }

        public override string Texture
        {
            get { return "Terraria/Item_" + ItemID.TerraBlade; }
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool UseItem(Player player)
        {
        for (int x = 0; x < Main.maxNPCs; x++){
        NPC thisnpc=Main.npc[x];
        if (thisnpc!=null && thisnpc.active) 
        {
                    if (thisnpc.friendly == false)
                    {

                        if (player.altFunctionUse == 2)
                        {
                            if (thisnpc.GetType().GetMethod("NPCLoot") != null)
                            {
                                if (thisnpc.modNPC != null)
                                    thisnpc.modNPC.NPCLoot();
                                else
                                    thisnpc.NPCLoot();
                            }
                            thisnpc.active = false;
                        }
                        else
                        {

                            thisnpc.life = 1;
                            thisnpc.StrikeNPCNoInteraction(5, 20f, player.direction, false, false, false);

                        }
                    }
        }}
        return true;
    }

    }

    public class DebugWeapon2 : IdgDebugItem
    {
        int mode=0;
        bool altfired=false;
        int modid=0;

        string[] modesstring={"Modded Inventory Data","Modded NPC Data","ALL Modded Buffs (prints ALL values!)","ALL Modded NPCs (prints ALL values!)"};
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("IDG's Modded Class Scanner");
            Tooltip.SetDefault("Displays modded names of classes to chat\nAlt Fire to switch modes\nHold shift while using to display only 1 mod and cycle through them");
        }
        public override void SetDefaults()
        {
            item.melee = true;
            item.width = 40;
            item.height = 40;
            item.useTime = 10;
            item.useAnimation = 10;
            item.useStyle = 2;
            item.melee = false;
            item.value = Item.sellPrice(0, 0, 0, 0);
            item.rare = 9;
            item.UseSound = SoundID.Item1;
            item.autoReuse = false;
        }

                public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(mod,"IDG Debug Mode info","Use to see "+modesstring[mode]));
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
            mode+=1;
            if (mode>modesstring.Length-1)
            mode=0;
            altfired=true;
            Main.PlaySound(21, (int)player.position.X, (int)player.position.Y, 17);
            Main.NewText("Now Displaying: "+modesstring[mode],200,200,200);
            return false;
            }else{altfired=false;return true;}

        }

        private Color newcolor(float value){

        return Main.hslToRgb((float)((Main.GlobalTime/15)+value)%1f, 0.75f, 0.35f);

        }

        public override string Texture
        {
            get { return "Terraria/Item_" + ItemID.LunarTabletFragment; }
        }

        public override bool UseItem(Player player)
        {
            float morecolorwhendifferentmod=0f;
            string lastmod="";
            int filtertomod=-1;
            Color c = newcolor(morecolorwhendifferentmod);

                if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift) && mode!=0 && mode!=1){
                modid+=1; if (modid>ModLoader.Mods.Length-1){modid=0;};
                filtertomod=modid;
                }

            if (mode==0){
            for (int i = 0; i < 58; i++)
            {
                Item item = player.inventory[i];
                if (item.modItem!=null){
                string currentmode=(item.modItem.mod).GetType().Name;
                if (lastmod!=(item.modItem.mod).GetType().Name){morecolorwhendifferentmod+=0.27f; c=newcolor(morecolorwhendifferentmod);}
                Main.NewText("[i:"+item.type+"]Class"+Idglib.ColorText(c,item.modItem.GetType().Name)+"Type ID"+Idglib.ColorText(c,(item.type).ToString())+"Mod"+Idglib.ColorText(c,(item.modItem.mod).GetType().Name),200,250,250);
                lastmod=currentmode;
            }}
            }

            if (mode==1){
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.modNPC!=null){
                string currentmode=(npc.modNPC.mod).GetType().Name;
                if (lastmod!=(npc.modNPC.mod).GetType().Name){morecolorwhendifferentmod+=0.27f; c=newcolor(morecolorwhendifferentmod);}
                Main.NewText(Idglib.ColorText(c,npc.FullName)+"Class"+Idglib.ColorText(c,npc.modNPC.GetType().Name)+"Mod"+Idglib.ColorText(c,(npc.modNPC.mod).GetType().Name),200,250,250);
                lastmod=currentmode;
            }}
            }

            if (mode==2){
            for (int i = 0; i < BuffLoader.BuffCount; i++)
            {
                ModBuff modbuff = BuffLoader.GetBuff(i);
                if (modbuff!=null && (filtertomod < 0 || modbuff.mod==ModLoader.GetMod(filtertomod))){
                string currentmode=(modbuff.mod).GetType().Name;
                if (lastmod!=(modbuff.mod).GetType().Name){morecolorwhendifferentmod+=0.27f; c=newcolor(morecolorwhendifferentmod);}
                Main.NewText(Idglib.ColorText(c,modbuff.DisplayName.GetDefault())+"Class"+Idglib.ColorText(c,modbuff.GetType().Name)+"Mod"+Idglib.ColorText(c,currentmode),200,250,250);
                lastmod=currentmode;
            }}
            }

            if (mode==3){
            for (int i = 0; i < NPCLoader.NPCCount; i++)
            {
                ModNPC modnpc = NPCLoader.GetNPC(i);
                if (modnpc!=null && (filtertomod < 0 || modnpc.mod==ModLoader.GetMod(filtertomod))){
                string currentmode=(modnpc.mod).GetType().Name;
                if (lastmod!=(modnpc.mod).GetType().Name){morecolorwhendifferentmod+=0.27f; c=newcolor(morecolorwhendifferentmod);}
                Main.NewText(Idglib.ColorText(c,modnpc.DisplayName.GetDefault())+"Class"+Idglib.ColorText(c,modnpc.GetType().Name)+"Mod"+Idglib.ColorText(c,currentmode),200,250,250);
                lastmod=currentmode;
            }}
            }
            if (filtertomod>0)
            Main.NewText("filtered to: "+ModLoader.GetMod(modid).DisplayName,255,255,255);
        return true;
    }

    }

    public class DebugWeapon3 : IdgDebugItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("IDG's MushSlapper");
            Tooltip.SetDefault("Launches Skeletron-like Mushrooms");
        }
        public override void SetDefaults()
        {
            item.damage = 15;
            item.melee = true;
            item.width = 40;
            item.height = 40;
            item.useTime = 10;
            item.useAnimation = 10;
            item.useStyle = 1;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 0, 0, 0);
            item.rare = 9;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.useTurn = true;
            item.shoot = mod.ProjectileType("DebugWeapon3Projectile");
        }

        public override string Texture
        {
            get { return "Terraria/Item_" + 5; }
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
        Vector2 thespeed=new Vector2(Main.mouseX+Main.screenPosition.X,Main.mouseY+Main.screenPosition.Y)-player.Center;thespeed.Normalize();
        thespeed=thespeed.RotatedByRandom(MathHelper.ToRadians(5));
        speedX=thespeed.X*15f;speedY=thespeed.Y*15f;

        return true;
        }

    }

        public class DebugWeapon3Projectile : ModProjectile
    {

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            //projectile.aiStyle = 1;
            projectile.friendly = true;
            projectile.timeLeft = 600;
            projectile.tileCollide=false;
        }

                public override string Texture
        {
            get { return "Terraria/Item_" + 5; }
        }
public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
{
Vector2 drawPos = projectile.Center - Main.screenPosition;

Texture2D tex=Main.itemTexture[5];

Idglib.DrawSkeletronLikeArms(spriteBatch,tex,projectile.Center,Main.player[projectile.owner].Center,0f,0f,(projectile.velocity.X > 0f ? 1f : -1f));

spriteBatch.Draw(Main.itemTexture[5], drawPos, null, lightColor, Main.rand.Next(-360,360)/100, new Vector2(16,16),new Vector2(2f,2f), SpriteEffects.None, 0f);
return false;
}

    }

}