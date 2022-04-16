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
using Terraria.WorldBuilding;
using static Terraria.ModLoader.ModContent;
using Terraria.Audio;
using Terraria.GameContent;

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

        public override bool IsLoadingEnabled(Mod mod)
        {
            if (!GetInstance<IDGSettings>().Items || this.GetType()==typeof(IdgDebugItem))
                return false;

            return base.IsLoadingEnabled(mod);
        }

        public override string Texture
        {
            get { return "Terraria/Images/Item_" + 0; }
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

        //public override bool CloneNewInstances => true; commenting this out will probably break everything

        public override void SaveData(TagCompound tag)
        {
            //TagCompound tag = new TagCompound();
            tag["myMusic"] = myMusic;
            tag["musicString"] = musicString;
            tag["knownID"] = Item.type;
            //return tag;
        }

        public override void LoadData(TagCompound tag)
        {
            knownID = tag.GetInt("knownID");
            if (knownID == Item.type)
            {
                myMusic = tag.GetInt("myMusic");
                musicString = tag.GetString("musicString");
            }

        }

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 14;
            Item.maxStack = 1;
            Item.rare = 8;
            Item.value = Item.buyPrice(platinum: 3);
            Item.useStyle = 2;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item9;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            if (musicString != null && musicString != "")
                return Main.hslToRgb(((float)myMusic / MathHelper.Pi) % 1f, 0.8f, 0.75f);

            return Main.hslToRgb((Main.GlobalTimeWrappedHourly * 0.316f) % 1f, 0.65f, 0.75f);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (musicString != null && musicString != "")
                tooltips.Add(new TooltipLine(Mod, "SGAmodMagicMusicBox", Idglib.ColorText(Main.hslToRgb((-Main.GlobalTimeWrappedHourly / 3f) % 1f, 0.8f, 0.8f), musicString)));
        }

        public override bool CanUseItem(Player player)
        {
            return Main.curMusic > 41;
        }

        public override bool? UseItem(Player player)
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
            get { return "Terraria/Images/Item_" + ItemID.MusicBox; }
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
            get { return "Terraria/Images/Item_" + ItemID.RedPotion; }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Main.DiscoColor;
        }
        public override void SetDefaults()
        {
            Item.maxStack = 1;
            Item.width = 32;
            Item.height = 32;
            Item.value = 0;
            Item.rare = ItemRarityID.Expert;
            Item.UseSound = SoundID.Item2;
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
            get { return "Terraria/Images/Item_" + ItemID.RedPotion; }
        }
        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.useStyle = 4;
            Item.noMelee = true; //so the item's animation doesn't do damage
            Item.value = 0;
            Item.rare = 9;
            Item.UseSound = SoundID.Item2;
        }

        public override bool? UseItem(Player player)
        {
            IdgPlayer idgplayer = player.GetModPlayer<IdgPlayer>();
            //bdplayer.DyeStrengthBoost+=2;
            SoundEngine.PlaySound(13, (int)player.position.X, (int)player.position.Y, 0);
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
            get { return "Terraria/Images/Item_" + ItemID.RedPotion; }
        }
        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.useStyle = 4;
            Item.noMelee = true; //so the item's animation doesn't do damage
            Item.value = 0;
            Item.rare = 9;
            Item.UseSound = SoundID.Item2;
        }

        public override bool? UseItem(Player player)
        {
            IdgPlayer idgplayer = player.GetModPlayer<IdgPlayer>();
            //bdplayer.DyeStrengthBoost+=2;
            SoundEngine.PlaySound(13, (int)player.position.X, (int)player.position.Y, 0);
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
            get { return "Terraria/Images/Item_" + ItemID.RedPotion; }
        }
        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.useStyle = 4;
            Item.noMelee = true; //so the item's animation doesn't do damage
            Item.value = 0;
            Item.rare = 9;
            Item.UseSound = SoundID.Item2;
        }

        public override bool? UseItem(Player player)
        {
            IdgPlayer idgplayer = player.GetModPlayer<IdgPlayer>();
            //bdplayer.DyeStrengthBoost+=2;
            SoundEngine.PlaySound(13, (int)player.position.X, (int)player.position.Y, 0);
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
            get { return "Terraria/Images/Item_" + ItemID.RedPotion; }
        }
        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.useStyle = 4;
            Item.noMelee = true; //so the item's animation doesn't do damage
            Item.value = 0;
            Item.rare = 9;
            Item.UseSound = SoundID.Item2;
        }

        public override bool? UseItem(Player player)
        {
            IdgPlayer idgplayer = player.GetModPlayer<IdgPlayer>();
            //bdplayer.DyeStrengthBoost+=2;
            SoundEngine.PlaySound(13, (int)player.position.X, (int)player.position.Y, 0);
            player.AddBuff(ModContent.BuffType<Buffs.BossFightPurity>(), 600);

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
            get { return "Terraria/Images/Item_" + ItemID.RedPotion; }
        }
        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.useStyle = 4;
            Item.noMelee = true; //so the item's animation doesn't do damage
            Item.value = 0;
            Item.rare = 9;
            Item.UseSound = SoundID.Item2;
        }

        public override bool? UseItem(Player player)
        {
            IdgPlayer idgplayer = player.GetModPlayer<IdgPlayer>();
            //bdplayer.DyeStrengthBoost+=2;
            SoundEngine.PlaySound(13, (int)player.position.X, (int)player.position.Y, 0);
            player.AddBuff(ModContent.BuffType<Buffs.CurseOfRed>(), 600);

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
            get { return "Terraria/Images/Item_" + ItemID.RedPotion; }
        }
        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.useStyle = 4;
            Item.noMelee = true; //so the item's animation doesn't do damage
            Item.value = 0;
            Item.rare = 9;
            Item.UseSound = SoundID.Item2;
        }

        public override bool? UseItem(Player player)
        {
            IdgPlayer idgplayer = player.GetModPlayer<IdgPlayer>();
            //bdplayer.DyeStrengthBoost+=2;
            SoundEngine.PlaySound(13, (int)player.position.X, (int)player.position.Y, 0);
            player.AddBuff(ModContent.BuffType<Buffs.SoulDrain>(), 600);

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
            get { return "Terraria/Images/Item_" + ItemID.RedPotion; }
        }
        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.useStyle = 4;
            Item.noMelee = true; //so the item's animation doesn't do damage
            Item.value = 0;
            Item.rare = 9;
            Item.UseSound = SoundID.Item2;
        }

        public override bool? UseItem(Player player)
        {
            IdgPlayer idgplayer = player.GetModPlayer<IdgPlayer>();
            //bdplayer.DyeStrengthBoost+=2;
            SoundEngine.PlaySound(13, (int)player.position.X, (int)player.position.Y, 0);
            player.AddBuff(ModContent.BuffType<Buffs.NullExceptionDebuff>(), 300);

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
            Item.damage = 1;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = 1;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 0, 0, 0);
            Item.rare = 9;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        public override string Texture
        {
            get { return "Terraria/Images/Item_" + ItemID.TerraBlade; }
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool? UseItem(Player player)
        {
        for (int x = 0; x < Main.maxNPCs; x++){
        NPC thisnpc=Main.npc[x];
        if (thisnpc!=null && thisnpc.active) 
        {
                    if (thisnpc.friendly == false)
                    {

                        if (player.altFunctionUse == 2)
                        {
                            if (thisnpc.GetType().GetMethod("OnKill") != null)
                            {
                                if (thisnpc.ModNPC != null)
                                    thisnpc.ModNPC.OnKill(); //OnKill is probably the wrong hook
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
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = 2;
            // item.melee = false /* tModPorter - this is redundant, for more info see https://github.com/tModLoader/tModLoader/wiki/Update-Migration-Guide#damage-classes */ ;
            Item.value = Item.sellPrice(0, 0, 0, 0);
            Item.rare = 9;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = false;
        }

                public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod,"IDG Debug Mode info","Use to see "+modesstring[mode]));
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
            mode+=1;
            if (mode>modesstring.Length-1)
            mode=0;
            altfired=true;
            SoundEngine.PlaySound(21, (int)player.position.X, (int)player.position.Y, 17);
            Main.NewText("Now Displaying: "+modesstring[mode],200,200,200);
            return false;
            }else{altfired=false;return true;}

        }

        private Color newcolor(float value){

        return Main.hslToRgb((float)((Main.GlobalTimeWrappedHourly /15)+value)%1f, 0.75f, 0.35f);

        }

        public override string Texture
        {
            get { return "Terraria/Images/Item_" + ItemID.LunarTabletFragment; }
        }

        public override bool? UseItem(Player player)
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
                if (item.ModItem!=null){
                string currentmode=(item.ModItem.Mod).GetType().Name;
                if (lastmod!=(item.ModItem.Mod).GetType().Name){morecolorwhendifferentmod+=0.27f; c=newcolor(morecolorwhendifferentmod);}
                Main.NewText("[i:"+item.type+"]Class"+Idglib.ColorText(c,item.ModItem.GetType().Name)+"Type ID"+Idglib.ColorText(c,(item.type).ToString())+"Mod"+Idglib.ColorText(c,(item.ModItem.Mod).GetType().Name),200,250,250);
                lastmod=currentmode;
            }}
            }

            if (mode==1){
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.ModNPC != null){
                string currentmode=(npc.ModNPC.Mod).GetType().Name;
                if (lastmod!=(npc.ModNPC.Mod).GetType().Name){morecolorwhendifferentmod+=0.27f; c=newcolor(morecolorwhendifferentmod);}
                Main.NewText(Idglib.ColorText(c,npc.FullName)+"Class"+Idglib.ColorText(c,npc.ModNPC.GetType().Name)+"Mod"+Idglib.ColorText(c,(npc.ModNPC.Mod).GetType().Name),200,250,250);
                lastmod=currentmode;
            }}
            }

            if (mode==2){
            for (int i = 0; i < BuffLoader.BuffCount; i++)
            {
                ModBuff modbuff = BuffLoader.GetBuff(i);
                if (modbuff!=null && (filtertomod < 0 || modbuff.Mod==ModLoader.GetMod(filtertomod.ToString()))){
                string currentmode=(modbuff.Mod).GetType().Name;
                if (lastmod!=(modbuff.Mod).GetType().Name){morecolorwhendifferentmod+=0.27f; c=newcolor(morecolorwhendifferentmod);}
                Main.NewText(Idglib.ColorText(c,modbuff.DisplayName.GetDefault())+"Class"+Idglib.ColorText(c,modbuff.GetType().Name)+"Mod"+Idglib.ColorText(c,currentmode),200,250,250);
                lastmod=currentmode;
            }}
            }

            if (mode==3){
            for (int i = 0; i < NPCLoader.NPCCount; i++)
            {
                ModNPC modnpc = NPCLoader.GetNPC(i);
                if (modnpc!=null && (filtertomod < 0 || modnpc.Mod==ModLoader.GetMod(filtertomod.ToString()))){
                string currentmode=(modnpc.Mod).GetType().Name;
                if (lastmod!=(modnpc.Mod).GetType().Name){morecolorwhendifferentmod+=0.27f; c=newcolor(morecolorwhendifferentmod);}
                Main.NewText(Idglib.ColorText(c,modnpc.DisplayName.GetDefault())+"Class"+Idglib.ColorText(c,modnpc.GetType().Name)+"Mod"+Idglib.ColorText(c,currentmode),200,250,250);
                lastmod=currentmode;
            }}
            }
            if (filtertomod>0)
            Main.NewText("filtered to: "+ModLoader.GetMod(modid.ToString()).DisplayName,255,255,255);
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
            Item.damage = 15;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = 1;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 0, 0, 0);
            Item.rare = 9;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.shoot = Mod.Find<ModProjectile>("DebugWeapon3Projectile").Type;
        }

        public override string Texture
        {
            get { return "Terraria/Images/Item_" + 5; }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockBack)
        {
            Vector2 thespeed=new Vector2(Main.mouseX+Main.screenPosition.X,Main.mouseY+Main.screenPosition.Y)-player.Center;thespeed.Normalize();
            thespeed=thespeed.RotatedByRandom(MathHelper.ToRadians(5));
            velocity.X= thespeed.X*15f; velocity.Y= thespeed.Y*15f;

            return true;
        }

    }

    public class DebugWeapon3Projectile : ModProjectile
    {

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            //projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.timeLeft = 600;
            Projectile.tileCollide=false;
        }

        public override string Texture
        {
            get { return "Terraria/Images/Item_" + 5; }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            Texture2D tex= TextureAssets.Item[5].Value;

            //spriteBatch was removed from PreDraw
            //Idglib.DrawSkeletronLikeArms(spriteBatch,tex,Projectile.Center,Main.player[Projectile.owner].Center,0f,0f,(Projectile.velocity.X > 0f ? 1f : -1f));

            //spriteBatch.Draw(TextureAssets.Item[5].Value, drawPos, null, lightColor, Main.rand.Next(-360,360)/100, new Vector2(16,16),new Vector2(2f,2f), SpriteEffects.None, 0f);
            return false;
        }

    }

}