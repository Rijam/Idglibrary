using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.GameContent.Generation;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;
using Terraria.ObjectData;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.Utilities;
using Terraria.Audio;
using Terraria.Chat;

//This is a libary of functions I (IDGCaptainRussia) wrote for my own mod and am now bringing it everywhere.

namespace Idglibrary
{

    /*public class InflictingTiles : GlobalTile
    {
        int maxstack = 0;

        public void Random(int i, int j, int type)
        {
            if (maxstack > 10 || Main.tile[i, j].type != TileID.Cobweb)
                return;

            Point16[] cardsorg = { new Point16(1, 0), new Point16(0, -1), new Point16(-1, 0), new Point16(0, 1) };
            Random rnd = new Random();
            Point16[] cards = cardsorg.OrderBy(x => rnd.Next()).ToArray();
            maxstack += 1;
            foreach (Point16 card in cards)
            {
                Point16 newloc = new Point16(i + card.X, j + card.Y);
                if (WorldGen.InWorld(newloc.X, newloc.Y) && Main.tile[newloc.X, newloc.Y].active())
                {
                    Main.tile[newloc.X, newloc.Y].type = TileID.Cobweb;
                    Main.tile[newloc.X, newloc.Y].active(true);
                    Main.tile[newloc.X, newloc.Y].wall = 0;
                    if (Main.rand.Next(0, 2) == 0)
                        Random(newloc.X, newloc.Y, TileID.Cobweb);
                    //Main.NewText("Change");

                }

            }

        }
        public override void RandomUpdate(int i, int j, int type)
        {
            maxstack = 0;
            Random(i, j, type);
            if (Main.tile[i, j].type == TileID.Cobweb)
            {
                Main.tile[i,j].type = TileID.Cobweb;
                Main.tile[i, j].active(false);
                Main.tile[i, j].wall = 0;
                Main.tile[i, j].liquidType(1);
                Main.tile[i, j].liquid = 255;
            }
        }



    }*/

    public class IdgProjectiles : GlobalProjectile
    {
        public override bool InstancePerEntity
        {
            get
            {
                return true;
            }
        }
        public List<int> buffid = new List<int>();
        public List<int> bufftime = new List<int>();

        public void IdgAddBuff(int buff, int time)
        {
            buffid.Insert(buffid.Count, buff);
            bufftime.Insert(bufftime.Count, time);
        }

        public override void OnHitNPC(Projectile projectile, NPC npc, int damage, float knockback, bool crit)
        {
            if ((projectile.hostile != npc.friendly) || (!projectile.hostile && !npc.friendly))
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    for (int num172 = 0; num172 < buffid.Count; num172 += 1)
                    {
                        npc.AddBuff(buffid[num172], bufftime[num172], true);
                    }
                }
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    //Idglib.Chat("(client) get server of this npc test",255,255,255);
                    ModPacket packet = Idglib.Instance.GetPacket();
                    packet.Write((byte)MessageType.IdgMessage);
                    packet.Write(3);
                    packet.Write(npc.whoAmI);
                    packet.Write(projectile.whoAmI);
                    packet.Send();
                }
            }
        }

        public override void OnHitPlayer(Projectile projectile, Player player, int damage, bool crit)
        {
            if (projectile.hostile == true)
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    for (int num172 = 0; num172 < buffid.Count; num172 += 1)
                    {
                        player.AddBuff(buffid[num172], bufftime[num172], true);
                    }
                }
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    //Idglib.Chat("(client) get server of this npc test",255,255,255);
                    ModPacket packet = Idglib.Instance.GetPacket();
                    packet.Write((byte)MessageType.IdgMessage);
                    packet.Write(2);
                    packet.Write(player.whoAmI);
                    packet.Write(projectile.whoAmI);
                    packet.Send();
                }
            }
        }

        public enum MessageType : byte
        {
            IdgMessage
        }

    }

    public class IdgNPCs : GlobalNPC
    {

        public override bool InstancePerEntity
        {
            get
            {
                return true;
            }
        }
        public List<int> buffid = new List<int>();
        public List<int> bufftime = new List<int>();

        public void IdgAddBuff(int buff, int time)
        {
            buffid.Insert(buffid.Count, buff);
            bufftime.Insert(bufftime.Count, time);
        }

        public override void SetDefaults(NPC npc)
        {
            if (Idglib.GetSGAmodNightmareMode() > 0)
            {
                if (!npc.friendly)
                {
                    npc.life += (int)(npc.life * (Idglib.GetSGAmodNightmareMode() * 0.2));
                    npc.lifeMax += (int)(npc.lifeMax * (Idglib.GetSGAmodNightmareMode() * 0.2));

                }

            }
        }


        /*          for (int num172 = 0; num172 < buffid.Count; num172 += 1){
            player.AddBuff(buffid[num172], bufftime[num172], true);
            Idglib.Chat("test",255,255,255);
            if (Main.netMode==2){
        ModPacket packet = Idglib.Instance.GetPacket();
        packet.Write((byte)MessageType.IdgMessage);
        packet.Write(0);
        packet.Write(player.whoAmI);
        packet.Write(buffid[num172]);
        packet.Write(bufftime[num172]);
        packet.Send();
            }
        }*/

        public override void OnHitPlayer(NPC npc, Player player, int damage, bool crit)
        {
            if (Main.netMode == 0) {
                for (int num172 = 0; num172 < buffid.Count; num172 += 1) {
                    player.AddBuff(buffid[num172], bufftime[num172], true);
                } }
            if (Main.netMode == 1) {
                //Idglib.Chat("(client) get server of this npc test",255,255,255);
                ModPacket packet = Idglib.Instance.GetPacket();
                packet.Write((byte)MessageType.IdgMessage);
                packet.Write(1);
                packet.Write(player.whoAmI);
                packet.Write(npc.whoAmI);
                packet.Send();
            }
        }

        public override void OnKill(NPC npc)
        {
            /*if (npc.type==NPCID.SkeletronHead){
            Idglib.EasyShop(NPCID.Merchant,ItemID.FlurryBoots);
            Idglib.EasyShop(NPCID.Merchant,ItemID.SweetheartNecklace,true);
            Idglib.Chat("Shot item debug test "+IdgWorld.shopitem.Count+" type "+IdgWorld.shopitem[IdgWorld.shopitem.Count-1],255,255,255);
            }*/
        }

        public enum MessageType : byte
        {
            IdgMessage
        }

    }



    public class IdgWorld : ModSystem
    {
        #region vars
        public static List<int> shopitem = new List<int>();
        public static List<int> shopitemnpcid = new List<int>();
        public static List<int> nonsavedshopitem = new List<int>();
        public static List<int> nonsavedshopitemnpcid = new List<int>();
        #endregion


        public override void OnWorldLoad()
        {
            //nothing yet
            IdgWorld.nonsavedshopitem = new List<int>();
            IdgWorld.nonsavedshopitemnpcid = new List<int>();
            Idglib.EasyShop(NPCID.Merchant, 17);
        }

        public override void SaveWorldData(TagCompound tag)
        {
            TagCompound IDGSave = new TagCompound();
            IDGSave["idgsavecomplete"] = true;
            //return IDGSave;
        }

        public override void LoadWorldData(TagCompound IDGSave)
        {
            //nothing atm
        }

        public static int EZShopInternal(int npcid, int item, bool nosave = false)
        {
            //List<int> shop=IdgWorld.shopitem;
            if (nosave == false) {
                IdgWorld.shopitemnpcid.Insert(IdgWorld.shopitemnpcid.Count, npcid);
                IdgWorld.shopitem.Insert(IdgWorld.shopitem.Count, item);
            } else {
                IdgWorld.nonsavedshopitemnpcid.Insert(IdgWorld.nonsavedshopitemnpcid.Count, npcid);
                IdgWorld.nonsavedshopitem.Insert(IdgWorld.nonsavedshopitem.Count, item);
            }
            if (nosave)
                return IdgWorld.shopitem.Count;
            else
                return -1;
        }

        public override void PreSaveAndQuit()
        {
            Idglib.nightmaremode = 0;
        }

    }










    public interface IHopperInterface
    {
        bool HopperInputItem(Item item, Point tilelocation, int movementCount, ref bool testOnly);

        bool HopperExportItem(ref Item item, Point tilelocation, int movementCount, ref bool testOnly);
    }













    public class Idglib : Mod
    {
        public static Idglib Instance;
        public static Dictionary<int, string> AbsentItemDisc;
        public static Vector2 skeletronarmpos;
        public static Vector2 skeletronarmjointpos;
        public static Color? coloroverride = null;
        public static int nightmaremode = 0;
        public Idglib()
        {

            /*Properties = new ModProperties()
            {
                Autoload = true,
                AutoloadGores = true,
                AutoloadSounds = true
            };*/
        }
        public static int GetSGAmodNightmareMode()
        {
            if (ModLoader.TryGetMod("SGAmod", out Mod sgamod) == false)
                return 0;
            else
                return SGAmodNightmareMode;

        }
        public static int SGAmodNightmareMode
        {
            get
            {
                return Idglib.nightmaremode;
            }
        }

        public override void Load()
        {
            Instance = this;
            AbsentItemDisc = new Dictionary<int, string>();
            //godmode thing
            On.Terraria.Player.KillMe += Player_KillMe;
        }

        private void Player_KillMe(On.Terraria.Player.orig_KillMe orig, Player self, PlayerDeathReason damageSource, double dmg, int hitDirection, bool pvp = false)
        {
            // 'orig' is a delegate that lets you call back into the original method.
            // 'self' is the 'this' parameter that would have been passed to the original method.

            if (self.HasItem(ModContent.ItemType<TrueGodMode>()))
                return;

            orig(self, damageSource,dmg,hitDirection,pvp);

        }



        public override void Unload()
        {
            Instance = null;
            IdgWorld.shopitem = null;
            IdgWorld.shopitemnpcid = null;
            IdgWorld.nonsavedshopitem = null;
            IdgWorld.nonsavedshopitemnpcid = null;
            AbsentItemDisc = null;
            nightmaremode = 0;
        }

        // Completely removed
        // Use `public class UpdateMusic : ModSceneEffect` or something.
        /*public override void UpdateMusic(ref int music, ref SceneEffectPriority priority) 
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Player local = Main.LocalPlayer;
                for (int i = 0; i < local.inventory.Length; i += 1)
                {
                    if (!local.inventory[i].IsAir && local.inventory[i].favorited && local.inventory[i].ModItem != null && local.inventory[i].ModItem is MagicMusicBox magic)
                    {
                        if (magic.myMusic > -1)
                        {
                            music = magic.myMusic;
                            priority = SceneEffectPriority.BossHigh;
                            break;
                        }
                    }

                }

            }
        }*/

        public static int RaycastDown(int x, int y)
        {
            x = (int)MathHelper.Clamp(x, 0, Main.maxTilesX);
            y = (int)MathHelper.Clamp(y, 0, Main.maxTilesY);
             while (!((Main.tile[x, y] != null && y < Main.maxTilesY - 5 && (Main.tile[x, y].HasUnactuatedTile && (Main.tileSolid[(int)Main.tile[x, y].TileType] || Main.tileSolidTop[(int)Main.tile[x, y].TileType] && (int)Main.tile[x, y].TileFrameY == 0)))))
            {
                y++;
            }
            return y;
        }

        public static string ColorText(Color color, string text)
        {
            return "[c/" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2") + ":" + text + "]";
        }

        //Credit given where Credit due! Over here! https://stackoverflow.com/questions/4243042/c-sharp-point-in-polygon
        public static bool IsInPolygon(Point[] poly, Point p)
        {
            Point p1, p2;
            bool inside = false;

            if (poly.Length < 3)
            {
                return inside;
            }

            var oldPoint = new Point(
                poly[poly.Length - 1].X, poly[poly.Length - 1].Y);

            for (int i = 0; i < poly.Length; i++)
            {
                var newPoint = new Point(poly[i].X, poly[i].Y);

                if (newPoint.X > oldPoint.X)
                {
                    p1 = oldPoint;
                    p2 = newPoint;
                }
                else
                {
                    p1 = newPoint;
                    p2 = oldPoint;
                }

                if ((newPoint.X < p.X) == (p.X <= oldPoint.X)
                    && (p.Y - (long)p1.Y) * (p2.X - p1.X)
                    < (p2.Y - (long)p1.Y) * (p.X - p1.X))
                {
                    inside = !inside;
                }

                oldPoint = newPoint;
            }

            return inside;
        }

        public static string IconText(int type, int stacksize = 1)
        {
            if (stacksize > 0)
                return "[i/s" + stacksize + ":" + type + "]";
            else
                return "[i:" + type + "]";

        }

        public static void DrawTether(string Tex, Vector2 Start, Vector2 End, float Alpha = 1f, float scaleX = 1f, float scaleY = 1f, Color coloroverride = default(Color))
        {
            DrawTether((Texture2D)ModContent.Request<Texture2D>(Tex), Start, End, Alpha = 1f, scaleX, scaleY, coloroverride);
        }

        public static void DrawTether(Texture2D Tex, Vector2 Start, Vector2 End, float Alpha = 1f, float scaleX = 1f, float scaleY = 1f, Color coloroverride = default(Color))
        {

            Texture2D texture = Tex;

            Vector2 position = Start;
            Vector2 mountedCenter = End;
            Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?();
            float num1 = (float)(texture.Height * (scaleY));
            Vector2 origin = new Vector2((float)texture.Width * 0.5f, (float)texture.Height * 0.5f);
            Vector2 vector2_4 = mountedCenter - position;
            float keepgoing = vector2_4.Length();
            Vector2 vector2t = vector2_4;
            vector2t.Normalize();
            position -= vector2t * (num1 * 0.5f);
            int countup = 0;

            float rotation = (float)Math.Atan2((double)vector2_4.Y, (double)vector2_4.X) - 1.57f;
            bool flag = true;
            if (float.IsNaN(position.X) && float.IsNaN(position.Y))
                flag = false;
            if (float.IsNaN(vector2_4.X) && float.IsNaN(vector2_4.Y))
                flag = false;
            while (flag)
            {
                if (keepgoing <= -1)
                {
                    flag = false;
                }
                else
                {
                    Vector2 vector2_1 = vector2_4;
                    vector2_1.Normalize();
                    position += vector2_1 * num1;
                    keepgoing -= num1;
                    vector2_4 = mountedCenter - position;
                    Microsoft.Xna.Framework.Color color2 = Lighting.GetColor((int)position.X / 16, (int)((double)position.Y / 16.0));
                    color2 = new Color(color2.R, color2.G, color2.B);
                    //color2 = npc.GetAlpha(color2);
                    if (coloroverride != default(Color))
                        color2 = coloroverride;
                    Main.spriteBatch.Draw(texture, position - Main.screenPosition, new Rectangle(0, 0, texture.Width, (int)Math.Min(texture.Height, texture.Height + keepgoing)), color2 * (Alpha), rotation, origin, new Vector2(scaleX, scaleY), SpriteEffects.None, 0.0f);
                }
            }
            Idglib.coloroverride = null;

        }


        //contributed by Putan (Direwolf) on TMODLoader's Discord (Putan#1022)
        //Thanks man, I was always curious how Terraria did Skeletron arms...
        public static void DrawSkeletronLikeArms(SpriteBatch spriteBatch, string texString, Vector2 selfPos, Vector2 centerPos, float selfPad = 0f, float centerPad = 0f, float direction = 0f)
        {
            DrawSkeletronLikeArms(spriteBatch, (Texture2D)ModContent.Request<Texture2D>(texString), selfPos, centerPos, selfPad, centerPad, direction);
        }

        public static void DrawSkeletronLikeArms(SpriteBatch spriteBatch, Texture2D tex, Vector2 selfPos, Vector2 centerPos, float selfPad = 0f, float centerPad = 0f, float direction = 0f)
        {
            //with all float params = 0f, the arm will originate below the selfPos
            //Pos parameters should be Entity.Center
            //Pad parameters are actually just y offsets
            //direction determines in what direction the elbow bends and by how much (-1 to 1 are preferred)
            if (tex == null) tex = (Texture2D)Terraria.GameContent.TextureAssets.BoneArm;
            Vector2 drawPos = selfPos;
            drawPos += new Vector2(-5f * direction, selfPad);
            centerPos.Y += -tex.Height / 2 + centerPad;
            float permrot = 0f;
            for (int i = 0; i < 2; i++)
            {
                float x = centerPos.X - drawPos.X;
                float y = centerPos.Y - drawPos.Y;
                float magnitude;
                if (i == 0) //first arm piece starting at selfPos
                {
                    x += -(100 + tex.Height) * direction;
                    y += 100 + tex.Width;
                    magnitude = (float)Math.Sqrt(x * x + y * y);
                    magnitude = (tex.Height / 2) / magnitude;
                    drawPos.X += x * magnitude;
                    drawPos.Y += y * magnitude;
                }
                else //second arm piece
                {
                    x += -(30 + tex.Width / 2) * direction;
                    y += 30 + tex.Height / 2;
                    magnitude = (float)Math.Sqrt(x * x + y * y);
                    magnitude = (tex.Height / 2) / magnitude;
                    drawPos.X += x * magnitude;
                    drawPos.Y += y * magnitude;
                }
                float rotation = (float)Math.Atan2(y, x) - 1.57f;
                permrot = rotation;
                Color color = Lighting.GetColor((int)drawPos.X / 16, (int)(drawPos.Y / 16f));
                if (Idglib.coloroverride != null)
                    color = (Color)Idglib.coloroverride;
                spriteBatch.Draw(tex, new Vector2(drawPos.X - Main.screenPosition.X, drawPos.Y - Main.screenPosition.Y), tex.Bounds, color, rotation, tex.Bounds.Size() / 2, 1f, SpriteEffects.None, 0f);

                if (i == 0)
                {
                    //padding for the second arm piece
                    skeletronarmjointpos = (new Vector2(drawPos.X, drawPos.Y)) + (((rotation + MathHelper.ToRadians(90)).ToRotationVector2()) * tex.Height / 2);
                    drawPos.X += x * magnitude * 1.1f;
                    drawPos.Y += y * magnitude * 1.1f;
                }
                else if (Main.instance.IsActive) //not sure what this part does
                {
                    drawPos.X += x * magnitude - 16f;
                    drawPos.Y += y * magnitude - 6f;
                }

            }
            Idglib.skeletronarmpos = new Vector2(drawPos.X, drawPos.Y) + (permrot.ToRotationVector2() * tex.Height / 2);
        }
        //additional info: with default arguments, the "center" of where the arm is "attached" is the bottom of what ever it is, down about 40-60f (this is how skeletron is), use a negative centerPad to counteract that


        //universal network-supported message broadcaster (see Sharkvern's message for how to use)
        public static void Chat(string message, byte color1, byte color2, byte color3)
        {
            if (Main.netMode != 2)
            {
                string text = message;
                Main.NewText(text, color1, color3, color3);
            }
            else
            {
                NetworkText text = NetworkText.FromLiteral(message);
                ChatHelper.BroadcastChatMessage(text, new Color(color1, color2, color3));
            }
        }


        //gets the closest target (0 for enemy, 1 for player), also supports optional argument for view-blocking (true by default), and for filtering either friendly or non-friendly (also true)---this last one is broken for so some reason, so it's been disabled
        //this was copied & edited from the source code for Cholo bullets, sue me
        public static int FindClosestTarget(int targettype, Vector2 loc, Vector2 size, bool block = true, bool friendlycheck = true, bool chasecheck = false, Projectile projectile = null)
        {
            int num;
            float num170 = 1000000;
            int num171 = 0;


            if (targettype == 0) {
                for (int num172 = 0; num172 < Main.maxNPCs; num172 = num + 1)
                {
                    float num173 = Main.npc[num172].position.X + (float)(Main.npc[num172].width / 2);
                    float num174 = Main.npc[num172].position.Y + (float)(Main.npc[num172].height / 2);
                    float num175 = Math.Abs(loc.X + (float)(size.X / 2) - num173) + Math.Abs(loc.Y + (float)(size.Y / 2) - num174);
                    if (Main.npc[num172].active && Main.npc[num172].type!=NPCID.OldMan && (!Main.npc[num172].friendly && !Main.npc[num172].townNPC))
                    {
                        if (num175 < num170 && ((Main.npc[num172].CanBeChasedBy()) && (Collision.CanHit(new Vector2(loc.X, loc.Y), 1, 1, Main.npc[num172].position, Main.npc[num172].width, Main.npc[num172].height) || block == false)))
                        {
                            num170 = num175;
                            num171 = num172;
                        }
                    }
                    num = num172;
                }
            }

            if (targettype == 1) {
                for (int num172 = 0; num172 < Main.maxPlayers; num172 = num + 1)
                {
                    float num173 = Main.player[num172].position.X + (float)(Main.player[num172].width / 2);
                    float num174 = Main.player[num172].position.Y + (float)(Main.player[num172].height / 2);
                    float num175 = Math.Abs(loc.X + (float)(size.X / 2) - num173) + Math.Abs(loc.Y + (float)(size.Y / 2) - num174);
                    if (num175 < num170 && (Collision.CanHit(new Vector2(loc.X, loc.Y), 1, 1, Main.player[num172].position, Main.player[num172].width, Main.player[num172].height) || block == false))
                    {
                        num170 = num175;
                        num171 = num172;
                    }
                    num = num172;
                }

            }

            return num171;

        }


        //Legecy Function, does nothing now
        public static int EasyShop(int npcid, int item)
        {
            return -1;
        }

        //new item, but is able to spawn this item from the client end on all other clients.

        public static void NewItemClient(int x, int y, int width, int height, int item)
        {
            if (Main.netMode != 1) {
                Item.NewItem(null, x, y, width, height, item); //Passing null for IEntitySource will work, but it may break other functionality
            } else {
                ModPacket packet = Idglib.Instance.GetPacket();
                packet.Write((byte)MessageType.IdgMessage);
                packet.Write(4);
                packet.Write(x);
                packet.Write(y);
                packet.Write(width);
                packet.Write(height);
                packet.Write(item);
                packet.Send();
            }
        }

        //universal network-supported sound maker (client only, might not work in network games, no idea)
        public static void PlaySound(int type, Vector2 here, int style)
        {
            if (Main.netMode != 2)
            {
                SoundEngine.PlaySound(type, (int)here.X, (int)here.Y, style);
            }
        }



        //returns an angle to look at something
        public static float LookAt(Vector2 here, Vector2 there)
        {

            float rotation = (float)Math.Atan2(here.Y - there.Y, here.X - there.X);
            return rotation;

        }



        //This was heavily edited from Joostmod

        public static List<Projectile> Shattershots(Vector2 here, Vector2 there, Vector2 widthheight, int type, int damage, float Speed, float spread, int count, bool centershot, float globalangularoffset, bool tilecollidez, int timeleft) {
            //if (Main.netMode!=1){
            List<Projectile> returns = new List<Projectile>();
            Vector2 vector8 = new Vector2(here.X + (0), here.Y + (0));
            float rotation = (float)Math.Atan2(vector8.Y - (there.Y + (widthheight.X * 0.5f)), vector8.X - (there.X + (widthheight.Y * 0.5f)));
            spread = spread * (0.0174f);
            float baseSpeed = (float)Math.Sqrt((float)((Math.Cos(rotation) * Speed) * -1) * (float)((Math.Cos(rotation) * Speed) * -1) + (float)((Math.Sin(rotation) * Speed) * -1) * (float)((Math.Sin(rotation) * Speed) * -1));
            double startAngle = Math.Atan2((float)((Math.Cos(rotation) * Speed) * -1), (float)((Math.Sin(rotation) * Speed) * -1));
            double deltaAngle = spread / count;
            double offsetAngle;
            int i;
            for (i = 0; i < count; i++)
            {
                offsetAngle = (startAngle + globalangularoffset) + deltaAngle * i;
                double offsetAngle2 = (startAngle + globalangularoffset) - (deltaAngle * i);
                if (centershot == true || i > 0) {
                    int proj = Projectile.NewProjectile(null, vector8.X, vector8.Y, baseSpeed * (float)Math.Sin(offsetAngle), (int)(baseSpeed * (float)Math.Cos(offsetAngle)), type, damage, Speed, 0);
                    Main.projectile[proj].friendly = false;
                    Main.projectile[proj].hostile = true;
                    Main.projectile[proj].tileCollide = tilecollidez;
                    Main.projectile[proj].timeLeft = timeleft;
                    Main.projectile[proj].netUpdate = true;
                    returns.Insert(returns.Count, Main.projectile[proj]);
                }
                if (i > 0) {
                    int proj2 = Projectile.NewProjectile(null, vector8.X, vector8.Y, baseSpeed * (float)Math.Sin(offsetAngle2), (int)(baseSpeed * (float)Math.Cos(offsetAngle2)), type, damage, Speed, 0);
                    Main.projectile[proj2].friendly = false;
                    Main.projectile[proj2].hostile = true;
                    Main.projectile[proj2].tileCollide = tilecollidez;
                    Main.projectile[proj2].timeLeft = timeleft;
                    Main.projectile[proj2].netUpdate = true;
                    returns.Insert(returns.Count, Main.projectile[proj2]);
                }
            }


            return returns;
        }
        //end of function

        //Line to Line Intersection (http://csharphelper.com/blog/2014/08/determine-where-two-lines-intersect-in-c/)

        // Find the point of intersection between
        // the lines p1 --> p2 and p3 --> p4.
        public static void FindLineLineIntersection(
            Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4,
            out bool lines_intersect, out bool segments_intersect,
            out Vector2 intersection,
            out Vector2 close_p1, out Vector2 close_p2)
        {
            // Get the segments' parameters.
            float dx12 = p2.X - p1.X;
            float dy12 = p2.Y - p1.Y;
            float dx34 = p4.X - p3.X;
            float dy34 = p4.Y - p3.Y;

            // Solve for t1 and t2
            float denominator = (dy12 * dx34 - dx12 * dy34);

            float t1 =
                ((p1.X - p3.X) * dy34 + (p3.Y - p1.Y) * dx34)
                    / denominator;
            if (float.IsInfinity(t1))
            {
                // The lines are parallel (or close enough to it).
                lines_intersect = false;
                segments_intersect = false;
                intersection = new Vector2(float.NaN, float.NaN);
                close_p1 = new Vector2(float.NaN, float.NaN);
                close_p2 = new Vector2(float.NaN, float.NaN);
                return;
            }
            lines_intersect = true;

            float t2 =
                ((p3.X - p1.X) * dy12 + (p1.Y - p3.Y) * dx12)
                    / -denominator;

            // Find the point of intersection.
            intersection = new Vector2(p1.X + dx12 * t1, p1.Y + dy12 * t1);

            // The segments intersect if t1 and t2 are between 0 and 1.
            segments_intersect =
                ((t1 >= 0) && (t1 <= 1) &&
                 (t2 >= 0) && (t2 <= 1));

            // Find the closest points on the segments.
            if (t1 < 0)
            {
                t1 = 0;
            }
            else if (t1 > 1)
            {
                t1 = 1;
            }

            if (t2 < 0)
            {
                t2 = 0;
            }
            else if (t2 > 1)
            {
                t2 = 1;
            }

            close_p1 = new Vector2(p1.X + dx12 * t1, p1.Y + dy12 * t1);
            close_p2 = new Vector2(p3.X + dx34 * t2, p3.Y + dy34 * t2);
        }

        //Strangely enough, had to use webarchive to find this one:
        //Distance to Line Segment https://web.archive.org/web/20150124032821/http://csharphelper.com/blog/2014/08/find-the-shortest-distance-between-a-point-and-a-line-segment-in-c/

        // Calculate the distance between
        // point pt and the segment p1 --> p2.
        public static double FindDistanceToSegment(
            Vector2 pt, Vector2 p1, Vector2 p2, out Vector2 closest)
        {
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            if ((dx == 0) && (dy == 0))
            {
                // It's a point not a line segment.
                closest = p1;
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }

            // Calculate the t that minimizes the distance.
            float t = ((pt.X - p1.X) * dx + (pt.Y - p1.Y) * dy) /
                (dx * dx + dy * dy);

            // See if this represents one of the segment's
            // end points or a point in the middle.
            if (t < 0)
            {
                closest = new Vector2(p1.X, p1.Y);
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
            }
            else if (t > 1)
            {
                closest = new Vector2(p2.X, p2.Y);
                dx = pt.X - p2.X;
                dy = pt.Y - p2.Y;
            }
            else
            {
                closest = new Vector2(p1.X + t * dx, p1.Y + t * dy);
                dx = pt.X - closest.X;
                dy = pt.Y - closest.Y;
            }

            return Math.Sqrt(dx * dx + dy * dy);
        }

        // Find the points of intersection. (http://csharphelper.com/blog/2014/09/determine-where-a-line-intersects-a-circle-in-c/)
        public static int FindLineCircleIntersections(
            float cx, float cy, float radius,
            Vector2 point1, Vector2 point2,
            out Vector2 intersection1, out Vector2 intersection2)
        {
            float dx, dy, A, B, C, det, t;

            dx = point2.X - point1.X;
            dy = point2.Y - point1.Y;

            A = dx * dx + dy * dy;
            B = 2 * (dx * (point1.X - cx) + dy * (point1.Y - cy));
            C = (point1.X - cx) * (point1.X - cx) +
                (point1.Y - cy) * (point1.Y - cy) -
                radius * radius;

            det = B * B - 4 * A * C;
            if ((A <= 0.0000001) || (det < 0))
            {
                // No real solutions.
                intersection1 = new Vector2(float.NaN, float.NaN);
                intersection2 = new Vector2(float.NaN, float.NaN);
                return 0;
            }
            else if (det == 0)
            {
                // One solution.
                t = -B / (2 * A);
                intersection1 =
                    new Vector2(point1.X + t * dx, point1.Y + t * dy);
                intersection2 = new Vector2(float.NaN, float.NaN);
                return 1;
            }
            else
            {
                // Two solutions.
                t = (float)((-B + Math.Sqrt(det)) / (2 * A));
                intersection1 =
                    new Vector2(point1.X + t * dx, point1.Y + t * dy);
                t = (float)((-B - Math.Sqrt(det)) / (2 * A));
                intersection2 =
                    new Vector2(point1.X + t * dx, point1.Y + t * dy);
                return 2;
            }
        }

        //diddo http://csharphelper.com/blog/2014/09/determine-where-two-circles-intersect-in-c/
        public static int FindCircleCircleIntersections(
    Vector2 c0, float radius0,
    Vector2 c1, float radius1,
    out Vector2 intersection1, out Vector2 intersection2)
        {
            // Find the distance between the centers.
            float dx = c0.X - c1.X;
            float dy = c0.Y - c1.Y;
            double dist = Math.Sqrt(dx * dx + dy * dy);

            // See how many solutions there are.
            if (dist > radius0 + radius1)
            {
                // No solutions, the circles are too far apart.
                intersection1 = new Vector2(float.NaN, float.NaN);
                intersection2 = new Vector2(float.NaN, float.NaN);
                return 0;
            }
            else if (dist < Math.Abs(radius0 - radius1))
            {
                // No solutions, one circle contains the other.
                intersection1 = new Vector2(float.NaN, float.NaN);
                intersection2 = new Vector2(float.NaN, float.NaN);
                return 0;
            }
            else if ((dist == 0) && (radius0 == radius1))
            {
                // No solutions, the circles coincide.
                intersection1 = new Vector2(float.NaN, float.NaN);
                intersection2 = new Vector2(float.NaN, float.NaN);
                return 0;
            }
            else
            {
                // Find a and h.
                double a = (radius0 * radius0 -
                    radius1 * radius1 + dist * dist) / (2 * dist);
                double h = Math.Sqrt(radius0 * radius0 - a * a);

                // Find P2.
                double cx2 = c0.X + a * (c1.X - c0.X) / dist;
                double cy2 = c0.Y + a * (c1.Y - c0.Y) / dist;

                // Get the points P3.
                intersection1 = new Vector2(
                    (float)(cx2 + h * (c1.Y - c0.Y) / dist),
                    (float)(cy2 - h * (c1.X - c0.X) / dist));
                intersection2 = new Vector2(
                    (float)(cx2 - h * (c1.Y - c0.Y) / dist),
                    (float)(cy2 + h * (c1.X - c0.X) / dist));

                // See if we have 1 or 2 solutions.
                if (dist == radius0 + radius1) return 1;
                return 2;
            }
        }


        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            MessageType type = (MessageType)reader.ReadByte();

            if (type == MessageType.IdgMessage) {
                int type2 = reader.ReadInt32();

                if (type2 == 0 && Main.netMode == NetmodeID.MultiplayerClient) {
                    //Idglib.Chat("(client) got info, adding buff",255,255,255);
                    int ply = reader.ReadInt32();
                    int buff = reader.ReadInt32();
                    int time = reader.ReadInt32();
                    Main.player[ply].AddBuff(buff, time, true);
                }

                if (type2 == 1 && Main.netMode == 2) {
                    //Idglib.Chat("(server) Someone got hit, send info to server",255,255,255);
                    int playerid = reader.ReadInt32();
                    int npcid = reader.ReadInt32();
                    IdgNPCs npcx = Main.npc[npcid].GetGlobalNPC<IdgNPCs>();

                    for (int num172 = 0; num172 < npcx.buffid.Count; num172 += 1) {
                        //Idglib.Chat("(server) sending buff info to clients",255,255,255);
                        if (Main.netMode == NetmodeID.Server) {
                            ModPacket packet = Idglib.Instance.GetPacket();
                            packet.Write((byte)MessageType.IdgMessage);
                            packet.Write(0);
                            packet.Write(playerid);
                            packet.Write(npcx.buffid[num172]);
                            packet.Write(npcx.bufftime[num172]);
                            packet.Send();
                        }
                    }

                }

                if (type2 == 2 && Main.netMode == NetmodeID.Server) {
                    //Idglib.Chat("(server) Someone got hit, send info to server",255,255,255);
                    int playerid = reader.ReadInt32();
                    int projectileid = reader.ReadInt32();
                    if (projectileid >= 0 && projectileid < Main.maxProjectiles)
                    {
                        IdgProjectiles npcx = Main.projectile[projectileid].GetGlobalProjectile<IdgProjectiles>();

                        for (int num172 = 0; num172 < npcx.buffid.Count; num172 += 1)
                        {
                            //Idglib.Chat("(server) sending buff info to clients",255,255,255);
                            if (Main.netMode == NetmodeID.Server)
                            {
                                ModPacket packet = Idglib.Instance.GetPacket();
                                packet.Write((byte)MessageType.IdgMessage);
                                packet.Write(0);
                                packet.Write(playerid);
                                packet.Write(npcx.buffid[num172]);
                                packet.Write(npcx.bufftime[num172]);
                                packet.Send();
                            }
                        }
                    }
                }

                if (type2 == 3 && Main.netMode == NetmodeID.Server) {
                    //Idglib.Chat("(server) Someone got hit, send info to server",255,255,255);
                    int npcid = reader.ReadInt32();
                    int projectileid = reader.ReadInt32();
                    if (projectileid >= 0 && projectileid < Main.maxProjectiles)
                    {
                        IdgProjectiles npcx = Main.projectile[projectileid].GetGlobalProjectile<IdgProjectiles>();

                        for (int num172 = 0; num172 < npcx.buffid.Count; num172 += 1)
                        {
                            //Idglib.Chat("(server) sending buff info to clients",255,255,255);
                            if (Main.netMode == NetmodeID.Server)
                            {
                                ModPacket packet = Idglib.Instance.GetPacket();
                                packet.Write((byte)MessageType.IdgMessage);
                                packet.Write(-1);
                                packet.Write(npcid);
                                packet.Write(npcx.buffid[num172]);
                                packet.Write(npcx.bufftime[num172]);
                                packet.Send();
                            }
                        }
                    }
                }


                if (type2 == 4 && Main.netMode == NetmodeID.Server) {// Client Item (server)
                    int x = reader.ReadInt32();
                    int y = reader.ReadInt32();
                    int width = reader.ReadInt32();
                    int height = reader.ReadInt32();
                    int item = reader.ReadInt32();
                    //possible server intervention here
                    ModPacket packet = Idglib.Instance.GetPacket();
                    packet.Write((byte)MessageType.IdgMessage);
                    packet.Write(-3);
                    packet.Write(x);
                    packet.Write(y);
                    packet.Write(width);
                    packet.Write(height);
                    packet.Write(item);
                    packet.Send();
                }


                if (type2 == -1 && Main.netMode == NetmodeID.MultiplayerClient) {// add buff (client)
                    Idglib.Instance.Logger.Debug("add buff (client)");
                    //Idglib.Chat("(client) got info, adding buff",255,255,255);
                    int npc = reader.ReadInt32();
                    int buff = reader.ReadInt32();
                    int time = reader.ReadInt32();
                    Main.npc[npc].AddBuff(buff, time, true);
                }

                if (type2 == -2 && Main.netMode == NetmodeID.MultiplayerClient) {// EZShop (client)
                    Idglib.Instance.Logger.Debug("EZShop (client)");
                    int npc = reader.ReadInt32();
                    int item = reader.ReadInt32();
                    IdgWorld.EZShopInternal(npc, item, true);
                }

                if (type2 == -3 && Main.netMode == NetmodeID.MultiplayerClient) {// CLient Item (client)
                    Idglib.Instance.Logger.Debug("CLient Item (client)");
                    Item.NewItem(null, reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32()); //Passing null for IEntitySource will work, but it may break other functionality.
                }

                if (type2 == 10 && Main.netMode == NetmodeID.Server) {//Sync Projectile pt1 (server)
                    Idglib.Instance.Logger.Debug("Sync Projectile pt1 (server)");
                    //int projectileid = reader.ReadInt32();
                    //IdgProjectiles npcx = Main.projectile[projectileid].GetGlobalProjectile<IdgProjectiles>();
                }

                if (type2 == -10 && Main.netMode == NetmodeID.MultiplayerClient) {//Sync Projectile pt2 (client)
                    Idglib.Instance.Logger.Debug("Sync Projectile pt2 (client)");
                    int projectileid = reader.ReadInt32();
                    int atype = reader.ReadInt32();
                    //IdgProjectiles npcx = Main.projectile[projectileid].GetGlobalProjectile<IdgProjectiles>();

                        int damage = reader.ReadInt32();
                        bool friendly = reader.ReadBoolean();
                        bool hostile = reader.ReadBoolean();
                        bool usesLocalNPCImmunity = reader.ReadBoolean();
                        bool magic = reader.ReadBoolean();
                        bool thrown = reader.ReadBoolean();
                        bool minion = reader.ReadBoolean();
                        bool melee = reader.ReadBoolean();
                        bool ranged = reader.ReadBoolean();
                        int penetrate = reader.ReadInt32();
                        int localNPCHitCooldown = reader.ReadInt16();
                        float scale = (float)(reader.ReadInt32() / 100f);
                        Vector2 position = new Vector2((float)reader.ReadDouble(), (float)reader.ReadDouble());
                        Vector2 velocity = new Vector2((float)reader.ReadDouble(), (float)reader.ReadDouble());
                        int timeLeft = reader.ReadInt32();

                    if (Main.projectile[atype].type == atype)
                    {
                        Main.projectile[projectileid].damage = damage;
                        Main.projectile[projectileid].friendly = friendly;
                        Main.projectile[projectileid].hostile = hostile;
                        Main.projectile[projectileid].usesLocalNPCImmunity = usesLocalNPCImmunity;
                        Main.projectile[projectileid].DamageType = DamageClass.Magic;
                        Main.projectile[projectileid].DamageType = DamageClass.Throwing;
                        Main.projectile[projectileid].minion = minion; //DamageClass.Summon ?
                        Main.projectile[projectileid].DamageType = DamageClass.Melee;
                        Main.projectile[projectileid].DamageType = DamageClass.Ranged;
                        Main.projectile[projectileid].penetrate = penetrate;
                        Main.projectile[projectileid].localNPCHitCooldown = localNPCHitCooldown;
                        Main.projectile[projectileid].scale = scale;
                        Main.projectile[projectileid].position = position;
                        Main.projectile[projectileid].velocity = velocity;
                        Main.projectile[projectileid].timeLeft = timeLeft;
                    }
                }



            }

        }

        public enum MessageType : byte
        {
            IdgMessage
        }

    }

    public static class IdgExtensions
    {
        //These 2 helper methods came from here! http://csharphelper.com/blog/2014/12/draw-a-bezier-curve-by-hand-in-c/
        public static float BezierX(float t,
    float x0, float x1, float x2, float x3)
        {
            return (float)(
                x0 * Math.Pow((1 - t), 3) +
                x1 * 3 * t * Math.Pow((1 - t), 2) +
                x2 * 3 * Math.Pow(t, 2) * (1 - t) +
                x3 * Math.Pow(t, 3)
            );
        }
        public static float BezierY(float t,
            float y0, float y1, float y2, float y3)
        {
            return (float)(
                y0 * Math.Pow((1 - t), 3) +
                y1 * 3 * t * Math.Pow((1 - t), 2) +
                y2 * 3 * Math.Pow(t, 2) * (1 - t) +
                y3 * Math.Pow(t, 3)
            );
        }
        public static Vector2 BezierCurve(this Vector2 vector, Vector2 start, Vector2 controlPoint1, Vector2 controlPoint2, Vector2 end, float percent)
        {
            return new Vector2(
                BezierX(percent, start.X, controlPoint1.X, controlPoint2.X, end.X),
                BezierY(percent, start.Y, controlPoint1.Y, controlPoint2.Y, end.Y));
        }

    }

        public static class IdgNPC
    {

        public static bool bossAlive
        {
            get
            {

                for (int i = 0; i < Main.maxNPCs; i += 1)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && npc.boss)
                    {
                        return true;
                    }

                }
                return false;
            }
        }

        public static void AddBuffBypass(int npcplyid, int type, int time, bool silent = false)
        {
            bool preval = Main.npc[npcplyid].buffImmune[type];
            Main.npc[npcplyid].buffImmune[type] = false;
            Main.npc[npcplyid].AddBuff(type, time, silent);
            Main.npc[npcplyid].buffImmune[type] = preval;
        }

        public static void AddOnHitBuff(int npcid, int buff, int time)
        {
            if (Main.netMode != 1) {
                Main.npc[npcid].GetGlobalNPC<IdgNPCs>().IdgAddBuff(buff, time);
            } }


        //this returns a list of NPC data. (advanced use)
        public static List<NPC> FindNPCs(int type) {
            List<NPC> returns = new List<NPC>();
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].type == type) {
                    returns.Insert(returns.Count, Main.npc[i]);
                }
            }

            return returns;
        }

        //this returns a list of NPC data. (advanced use)
        public static List<NPC> FindNPCsMultitype(int[] npctypes) {
            List<NPC> returns = new List<NPC>();

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (npctypes.Any(x => x == Main.npc[i].type) && Main.npc[i].active == true) {
                    returns.Insert(returns.Count, Main.npc[i]);
                }
            }

            return returns;
        }

        public enum MessageType : byte
        {
            IdgMessage
        }




    }


    public class IdgItems : GlobalItem
    {

        public bool IsHotbarCurse(Item item, Player player)
        {
            IdgPlayer idgplayer = player.GetModPlayer<IdgPlayer>();
            if (idgplayer.hotbarcurse > -1)
            {
                for (int i = 0; i < 10; i += 1)
                {
                    if (player.inventory[i] == item && idgplayer.hotbarcurse == i)
                    {
                        return true;
                    }
                }
            }
                if (idgplayer.itemcurse > -1)
                {
                        if (!item.IsAir && idgplayer.itemcurse == item.type)
                        {
                            return true;
                        }
                }
            return false;
        }

        public override void HoldItem(Item item, Player player)
        {
            if (IsHotbarCurse(item, player))
            {
                player.controlUseItem = false;
                player.releaseUseItem = false;
            }
        }

        //Snipit from Bluemagic, could have wrote it myself, just wanted to make this faster
        public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor,
    Color itemColor, Vector2 origin, float scale)
        {
            if (!Main.gameMenu)
            {
                IdgPlayer idgplayer = Main.LocalPlayer.GetModPlayer<IdgPlayer>();
                if (idgplayer.hotbarcurse > -1 || idgplayer.itemcurse > -1)
                {
                    if (IsHotbarCurse(item, Main.LocalPlayer))
                    {
                        Texture2D texture = (Texture2D)Terraria.GameContent.TextureAssets.Cd;
                        Vector2 slotSize = new Vector2(52f, 52f);
                        position -= slotSize * Main.inventoryScale / 2f - frame.Size() * scale / 2f;
                        Vector2 drawPos = position + slotSize * Main.inventoryScale / 2f;
                        Vector2 textureOrigin = new Vector2(texture.Width / 2, texture.Height / 2);
                        spriteBatch.Draw(texture, drawPos, null, Color.White, 0f, textureOrigin, Main.inventoryScale, SpriteEffects.None, 0f);
                    }
                }
            }
        }

        public override bool CanUseItem(Item item, Player player)
        {
            IdgPlayer idgplayer = player.GetModPlayer<IdgPlayer>();
            if (IsHotbarCurse(item, player))
                return false;

            if (idgplayer.bossmod != null)
            {
                if (item.ModItem != null)
                {
                    if (item.ModItem.Mod == idgplayer.bossmod)
                        return true;
                    else
                        return false;

                }
            }
            return true;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.ModItem != null) {
                if (((item.ModItem).GetType()).BaseType != null) {
                    string itemclass = ((item.ModItem).GetType().BaseType).Name;
                    //Main.NewText(itemclass,100,100,100);F
                    if (itemclass == "IdgDebugItem") {

                        Color c = Main.hslToRgb((float)(Main.GlobalTimeWrappedHourly / 2) % 1f, 0.5f, 0.35f);
                        string potion = "[i:" + ItemID.RedPotion + "]";
                        tooltips.Add(new TooltipLine(Mod, "IDG Debug Item", Idglib.IconText(ItemID.RedPotion) + Idglib.ColorText(c, "This is a Debug Item") + Idglib.IconText(ItemID.RedPotion)));

                    } }
                if (Idglib.AbsentItemDisc.ContainsKey(item.type)) { Color c = Main.hslToRgb(0.6f, 0f, 0.5f);
                    string text;
                    bool check = Idglib.AbsentItemDisc.TryGetValue(item.type, out text);
                    if (check && !Main.LocalPlayer.HasItem(item.type))
                        tooltips.Add(new TooltipLine(Mod, "Absent Item Disc", Idglib.ColorText(c, text)));
                }

            }
        }

    }

    public static class IdgProjectile
    {

        public static void Sync(int projectileid)
        {
            if (Main.netMode == NetmodeID.Server) {

                ModPacket packet = Idglib.Instance.GetPacket();
                packet.Write((byte)MessageType.IdgMessage);
                packet.Write(-10);
                packet.Write(projectileid);
                packet.Write(Main.projectile[projectileid].type);
                packet.Write(Main.projectile[projectileid].damage);
                packet.Write(Main.projectile[projectileid].friendly);
                packet.Write(Main.projectile[projectileid].hostile);
                packet.Write(Main.projectile[projectileid].usesLocalNPCImmunity);
                packet.Write(Main.projectile[projectileid].CountsAsClass(DamageClass.Magic));
                packet.Write(Main.projectile[projectileid].CountsAsClass(DamageClass.Throwing));
                packet.Write(Main.projectile[projectileid].minion); //CountsAsClass(DamageClass.Summon) ?
                packet.Write(Main.projectile[projectileid].CountsAsClass(DamageClass.Melee));
                packet.Write(Main.projectile[projectileid].CountsAsClass(DamageClass.Ranged));
                packet.Write(Main.projectile[projectileid].penetrate);
                packet.Write((short)Main.projectile[projectileid].localNPCHitCooldown);
                packet.Write((int)Main.projectile[projectileid].scale * 100);
                packet.Write((double)Main.projectile[projectileid].position.X);
                packet.Write((double)Main.projectile[projectileid].position.Y);
                packet.Write((double)Main.projectile[projectileid].velocity.X);
                packet.Write((double)Main.projectile[projectileid].velocity.Y);
                packet.Write(Main.projectile[projectileid].timeLeft);
                packet.Send();


                /*ModPacket packet = Idglib.Instance.GetPacket();
                packet.Write((byte)MessageType.IdgMessage);
                packet.Write(10);
                packet.Write(projectileid);
                packet.Send();*/
            }
        }

        public static void AddOnHitBuff(int projectileid, int buff, int time)
        {
            if (Main.netMode != 1) {
                Main.projectile[projectileid].GetGlobalProjectile<IdgProjectiles>().IdgAddBuff(buff, time);
            } }

        public enum MessageType : byte
        {
            IdgMessage
        }

    }

    //Came from here: https://gamedev.stackexchange.com/questions/13371/generating-island-terrains-with-simplex-noise-c-xna
    //Not my code, slightly tweaked to be an instance-able class and added directional gradients via wikipedia snippets
    public class NoiseGenerator
    {
        public int Seed { get; private set; }
        public int Octaves { get; set; }

        public double Amplitude { get; set; }

        public double Persistence { get; set; }

        public double Frequency { get; set; }

        public bool UseGradient { get; set; }

        public int InterpolateType { get; set; }

        public NoiseGenerator(Int32 seed)
        {
            this.Seed = seed;
            Octaves = 8;
            Amplitude = 1;
            Frequency = 0.015;
            Persistence = 0.65;
            UseGradient = false;
            InterpolateType = 3;
        }

        Vector2 randomGradient(int ix, int iy)
        {
            // Random float. No precomputed gradients mean this works for any number of grid coordinates. (https://en.wikipedia.org/wiki/Perlin_noise)
            double random = (2920.00+Seed) * Math.Sin(ix * 21942.00 + iy * 171324.00 + 8912.00) * Math.Cos(ix * 23157.00 * iy * 217832.00 + 9758.00);
            return new Vector2((float)Math.Cos(random), (float)Math.Sin(random));
        }

        // Computes the dot product of the distance and gradient vectors. (https://en.wikipedia.org/wiki/Perlin_noise)
        float DotGridGradient(int ix, int iy, float x, float y)
        {
            // Get gradient from integer coordinates
            Vector2 gradient = randomGradient(ix, iy);

            // Compute the distance vector
            float dx = x - (float)ix;
            float dy = y - (float)iy;

            // Compute the dot-product
            return (dx * gradient.X + dy * gradient.Y);
        }

        public double Noise(int x, int y)
        {
            //returns -1 to 1
            double total = 0.0;
            double freq = this.Frequency, amp = this.Amplitude;
            for (int i = 0; i < Octaves; ++i)//FBM fractalify
            {
                total = total + this.InterpolateNoise(x * freq, y * freq) * amp;
                freq *= 2;
                amp *= this.Persistence;
            }
            if (total < -2.4) total = -2.4;
            else if (total > 2.4) total = 2.4;

            return (total / 2.4);
        }

        public double NoiseGeneration(int x, int y)
        {
            int n = x + y * 57;
            n = (n << 13) ^ n;

            return (1.0 - ((n * (n * n * 15731 + 789221) + this.Seed) & 0x7fffffff) / 1073741824.0);
        }

        private double InterpolateSin(double x, double y, double a)
        {
            double value = (1 - Math.Cos(a * Math.PI)) * 0.5;
            return x * (1 - value) + y * value;
        }

        float Interpolate(float a0, float a1, float w)
        {
            /* // You may want clamping by inserting:
             * if (0.0 > w) return a0;
             * if (1.0 < w) return a1;
             */
            if (InterpolateType == 0)
            return (a1 - a0) * w + a0;

            // Use this cubic interpolation [[Smoothstep]] instead, for a smooth appearance:
            if (InterpolateType == 1)
                return ((a1 - a0) * (3.0f - w * 2.0f) * w * w + a0);

            // Use [[Smootherstep]] for an even smoother result with a second derivative equal to zero on boundaries:
            if (InterpolateType == 2)
                return (a1 - a0) * ((w * (w * 6.0f - 15.0f) + 10.0f) * w * w * w) + a0;

            return (float)InterpolateSin(a0,a1,w);

        }
     

        private double InterpolateNoise(double x, double y)//Formally called "Smooth"
        {
            float n0, n1,n2,n3,n4, ix0, ix1;

            if (UseGradient)
            {
                int x0 = (int)x;
                int x1 = x0 + 1;
                int y0 = (int)y;
                int y1 = y0 + 1;

                float xx = (float)x;
                float yy = (float)y;

                float sx = xx - (float)x0;
                float sy = yy - (float)y0;

                n0 = DotGridGradient(x0, y0, xx, yy);
                n1 = DotGridGradient(x1, y0, xx, yy);
                ix0 = Interpolate(n0, n1, sx);

                n0 = DotGridGradient(x0, y1, xx, yy);
                n1 = DotGridGradient(x1, y1, xx, yy);
                ix1 = Interpolate(n0, n1, sx);

                return Interpolate(ix0, ix1, sy);

            }
            else
            {
                n1 = (float)this.NoiseGeneration((int)x, (int)y);
                n2 = (float)this.NoiseGeneration((int)x + 1, (int)y);
                n3 = (float)this.NoiseGeneration((int)x, (int)y + 1);
                n4 = (float)this.NoiseGeneration((int)x + 1, (int)y + 1);

                float i1 = (float)this.Interpolate(n1, n2, (float)(x - (int)x));
                float i2 = (float)this.Interpolate(n3, n4, (float)(x - (int)x));

                return this.Interpolate(i1, i2, (float)(y - (int)y));
            }
        }
    }

    //http://codingha.us/2018/12/17/xorshift-fast-csharp-random-number-generator/
    //God I look like such a noob having trouble understanding this, thankfully the license is quite tame
    //shame the guy only wrote 3 articles on his site, his stuff is quite good!

    /*===============================[ XorShiftPlus ]==============================
      ==-------------[ (c) 2018 R. Wildenhaus - Licensed under MIT ]-------------==
      ============================================================================= */


            /// <summary>
            ///   Generates pseudorandom primitive types with a 64-bit implementation
            ///   of the XorShift algorithm.
            /// </summary>
        public class XorShiftRandom
        {

            #region Data Members

            // Constants
            private const double DOUBLE_UNIT = 1.0 / (int.MaxValue + 1.0);

            // State Fields
            private ulong x_;
            private ulong y_;

            // Buffer for optimized bit generation.
            private ulong buffer_;
            private ulong bufferMask_;

            #endregion

            #region Constructor

            /// <summary>
            ///   Constructs a new  generator using two
            ///   random Guid hash codes as a seed.
            /// </summary>
            public XorShiftRandom()
            {
                x_ = (ulong)Guid.NewGuid().GetHashCode();
                y_ = (ulong)Guid.NewGuid().GetHashCode();
            }

            /// <summary>
            ///   Constructs a new  generator
            ///   with the supplied seed.
            /// </summary>
            /// <param name="seed">
            ///   The seed value.
            /// </param>
            public XorShiftRandom(ulong seed)
            {
                x_ = seed << 3; x_ = seed >> 3;
            }

            #endregion

            #region Public Methods

            /// <summary>
            ///   Generates a pseudorandom boolean.
            /// </summary>
            /// <returns>
            ///   A pseudorandom boolean.
            /// </returns>
            public bool NextBoolean()
            {
                bool _;
                if (bufferMask_ > 0)
                {
                    _ = (buffer_ & bufferMask_) == 0;
                    bufferMask_ >>= 1;
                    return _;
                }

                ulong temp_x, temp_y;
                temp_x = y_;
                x_ ^= x_ << 23; temp_y = x_ ^ y_ ^ (x_ >> 17) ^ (y_ >> 26);

                buffer_ = temp_y + y_;
                x_ = temp_x;
                y_ = temp_y;

                bufferMask_ = 0x8000000000000000;
                return (buffer_ & 0xF000000000000000) == 0;
            }

            /// <summary>
            ///   Generates a pseudorandom byte.
            /// </summary>
            /// <returns>
            ///   A pseudorandom byte.
            /// </returns>
            public byte NextByte()
            {
                if (bufferMask_ >= 8)
                {
                    byte _ = (byte)buffer_;
                    buffer_ >>= 8;
                    bufferMask_ >>= 8;
                    return _;
                }

                ulong temp_x, temp_y;
                temp_x = y_;
                x_ ^= x_ << 23; temp_y = x_ ^ y_ ^ (x_ >> 17) ^ (y_ >> 26);

                buffer_ = temp_y + y_;
                x_ = temp_x;
                y_ = temp_y;

                bufferMask_ = 0x8000000000000;
                return (byte)(buffer_ >>= 8);
            }

            /// <summary>
            ///   Generates a pseudorandom 16-bit signed integer.
            /// </summary>
            /// <returns>
            ///   A pseudorandom 16-bit signed integer.
            /// </returns>
            public short NextInt16()
            {
                short _;
                ulong temp_x, temp_y;

                temp_x = y_;
                x_ ^= x_ << 23; temp_y = x_ ^ y_ ^ (x_ >> 17) ^ (y_ >> 26);

                _ = (short)(temp_y + y_);

                x_ = temp_x;
                y_ = temp_y;

                return _;
            }

            /// <summary>
            ///   Generates a pseudorandom 16-bit unsigned integer.
            /// </summary>
            /// <returns>
            ///   A pseudorandom 16-bit unsigned integer.
            /// </returns>
            public ushort NextUInt16()
            {
                ushort _;
                ulong temp_x, temp_y;

                temp_x = y_;
                x_ ^= x_ << 23; temp_y = x_ ^ y_ ^ (x_ >> 17) ^ (y_ >> 26);

                _ = (ushort)(temp_y + y_);

                x_ = temp_x;
                y_ = temp_y;

                return _;
            }

            /// <summary>
            ///   Generates a pseudorandom 32-bit signed integer.
            /// </summary>
            /// <returns>
            ///   A pseudorandom 32-bit signed integer.
            /// </returns>
            public int NextInt32()
            {
                int _;
                ulong temp_x, temp_y;

                temp_x = y_;
                x_ ^= x_ << 23; temp_y = x_ ^ y_ ^ (x_ >> 17) ^ (y_ >> 26);

                _ = (int)(temp_y + y_);

                x_ = temp_x;
                y_ = temp_y;

                return _;
            }

            /// <summary>
            ///   Generates a pseudorandom 32-bit unsigned integer.
            /// </summary>
            /// <returns>
            ///   A pseudorandom 32-bit unsigned integer.
            /// </returns>
            public uint NextUInt32()
            {
                uint _;
                ulong temp_x, temp_y;

                temp_x = y_;
                x_ ^= x_ << 23; temp_y = x_ ^ y_ ^ (x_ >> 17) ^ (y_ >> 26);

                _ = (uint)(temp_y + y_);

                x_ = temp_x;
                y_ = temp_y;

                return _;
            }

            /// <summary>
            ///   Generates a pseudorandom 64-bit signed integer.
            /// </summary>
            /// <returns>
            ///   A pseudorandom 64-bit signed integer.
            /// </returns>
            public long NextInt64()
            {
                long _;
                ulong temp_x, temp_y;

                temp_x = y_;
                x_ ^= x_ << 23; temp_y = x_ ^ y_ ^ (x_ >> 17) ^ (y_ >> 26);

                _ = (long)(temp_y + y_);

                x_ = temp_x;
                y_ = temp_y;

                return _;
            }

            /// <summary>
            ///   Generates a pseudorandom 64-bit unsigned integer.
            /// </summary>
            /// <returns>
            ///   A pseudorandom 64-bit unsigned integer.
            /// </returns>
            public ulong NextUInt64()
            {
                ulong _;
                ulong temp_x, temp_y;

                temp_x = y_;
                x_ ^= x_ << 23; temp_y = x_ ^ y_ ^ (x_ >> 17) ^ (y_ >> 26);

                _ = (ulong)(temp_y + y_);

                x_ = temp_x;
                y_ = temp_y;

                return _;
            }

            /// <summary>
            ///   Generates a pseudorandom double between
            ///   0 and 1 non-inclusive.
            /// </summary>
            /// <returns>
            ///   A pseudorandom double.
            /// </returns>
            public double NextDouble()
            {
                double _;
                ulong temp_x, temp_y, temp_z;

                temp_x = y_;
                x_ ^= x_ << 23; temp_y = x_ ^ y_ ^ (x_ >> 17) ^ (y_ >> 26);

                temp_z = temp_y + y_;
                _ = DOUBLE_UNIT * (0x7FFFFFFF & temp_z);

                x_ = temp_x;
                y_ = temp_y;

                return _;
            }

            /// <summary>
            ///   Generates a pseudorandom decimal between
            ///   0 and 1 non-inclusive.
            /// </summary>
            /// <returns>
            ///   A pseudorandom decimal.
            /// </returns>
            public decimal NextDecimal()
            {
                decimal _;
                int l, m, h;
                ulong temp_x, temp_y, temp_z;

                temp_x = y_;
                x_ ^= x_ << 23; temp_y = x_ ^ y_ ^ (x_ >> 17) ^ (y_ >> 26);

                temp_z = temp_y + y_;

                h = (int)(temp_z & 0x1FFFFFFF);
                m = (int)(temp_z >> 16);
                l = (int)(temp_z >> 32);

                _ = new decimal(l, m, h, false, 28);

                x_ = temp_x;
                y_ = temp_y;

                return _;
            }

        /// <summary>
        ///   Fills the supplied buffer with pseudorandom bytes.
        /// </summary>
        /// <param name="buffer">
        ///   The buffer to fill.
        /// </param>

        //Commented out just so I could build the mod. I have <AllowUnsafeBlocks>true</AllowUnsafeBlocks> but it won't build in game
        /*public unsafe void NextBytes(byte[] buffer)
        {
            // Localize state for stack execution
            ulong x = x_, y = y_, temp_x, temp_y, z;

            fixed (byte* pBuffer = buffer)
            {
                ulong* pIndex = (ulong*)pBuffer;
                ulong* pEnd = (ulong*)(pBuffer + buffer.Length);

                // Fill array in 8-byte chunks
                while (pIndex <= pEnd - 1)
                {
                    temp_x = y;
                    x ^= x << 23; temp_y = x ^ y ^ (x >> 17) ^ (y >> 26);

                    *(pIndex++) = temp_y + y;

                    x = temp_x;
                    y = temp_y;
                }

                // Fill remaining bytes individually to prevent overflow
                if (pIndex < pEnd)
                {
                    temp_x = y;
                    x ^= x << 23; temp_y = x ^ y ^ (x >> 17) ^ (y >> 26);
                    z = temp_y + y;

                    byte* pByte = (byte*)pIndex;
                    while (pByte < pEnd) *(pByte++) = (byte)(z >>= 8);
                }
            }

            // Store modified state in fields.
            x_ = x;
            y_ = y;
        }*/

        #endregion

    }

    //Vector2 but without the constant need to cast the floats, LOL
    //Also me, failing to realize Point exists
    /*public struct TileVector
    {
        public int X;
        public int Y;

        public TileVector(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }*/

    public static class IDGWorldGen
    {

        //wasn't aware WorldGen already had a method for this
        public static bool InsideMap(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Main.maxTilesX && y < Main.maxTilesY;
        }

        //these first are derived from here https://github.com/SebLague/Procedural-Cave-Generation/blob/master/Episode%2008/MapGenerator.cs
        //SebLague-Procedural Cave Generation-Ep7
        //Slightly Altered to work with Terraria

        //A Tad lazy, I could have written my own here lol
        public static List<Point> DrawCircle(Point c, int r)
        {
            List<Point> alltiles = new List<Point>();
            for (int x = -r; x <= r; x++)
            {
                for (int y = -r; y <= r; y++)
                {
                    if (x * x + y * y <= r * r)
                    {
                        int drawX = c.X + x;
                        int drawY = c.Y + y;
                        if (InsideMap(drawX, drawY))
                        {
                            alltiles.Add(new Point(drawX, drawY));
                        }
                    }
                }
            }
            return alltiles;
        }

        //The math for this one is just, confusing me way too much, and it's too useful, sorry but I have to; also from the above source
        public static List<Point> GetLine(Point from, Point to)
        {
            List<Point> line = new List<Point>();

            int x = from.X;
            int y = from.Y;

            int dx = to.X - from.X;
            int dy = to.Y - from.Y;

            bool inverted = false;
            int step = Math.Sign(dx);
            int gradientStep = Math.Sign(dy);

            int longest = Math.Abs(dx);
            int shortest = Math.Abs(dy);

            if (longest < shortest)
            {
                inverted = true;
                longest = Math.Abs(dy);
                shortest = Math.Abs(dx);

                step = Math.Sign(dy);
                gradientStep = Math.Sign(dx);
            }

            int gradientAccumulation = longest / 2;
            for (int i = 0; i < longest; i++)
            {
                line.Add(new Point(x, y));

                if (inverted)
                {
                    y += step;
                }
                else
                {
                    x += step;
                }

                gradientAccumulation += shortest;
                if (gradientAccumulation >= longest)
                {
                    if (inverted)
                    {
                        x += gradientStep;
                    }
                    else
                    {
                        y += gradientStep;
                    }
                    gradientAccumulation -= longest;
                }
            }

            return line;
        }

        public static void PlaceMulti(Vector2 placementspot2, int type, int size, int wall = -1, bool replacetile = true)
        {
            PlaceMulti(placementspot2.ToPoint(), type, size, wall, replacetile);
        }

            public static void PlaceMulti(Point placementspot2, int type, int size, int wall = -1, bool replacetile = true)
        {
            for (int x2 = -size + 1; x2 < size; x2++)
            {
                for (int y2 = -size + 1; y2 < size; y2++)
                {
                    Point placementspot = new Point(placementspot2.X + x2, placementspot2.Y+y2);
                    if (InsideMap(placementspot.X, placementspot.Y))
                    {
                        Tile tstart = Framing.GetTileSafely(placementspot.X, placementspot.Y);
                        if (wall > -1 && y2 > -size + 1 && y2 < size - 1 && x2 < size - 1 && x2 > -size + 1)
                            tstart.WallType = (ushort)wall;
                        if (type > -1000 && (!tstart.HasTile || replacetile))
                        {
                            if (type > -1)
                            {
                                tstart.TileType = (ushort)type;
                                _ = tstart.HasTile;
                                tstart.LiquidType = 0;
                            }
                            else
                            {
                                _ = tstart.HasTile;
                            }
                        }
                    }
                }
            }

        }

        public static void TileRunner(int i, int j, double strength, int steps, int type, bool addTile = false, float speedX = 0f, float speedY = 0f, bool noYChange = false, bool overRide = true,UnifiedRandom rand=null)
        {
            if (rand == null)
                rand = WorldGen.genRand;

            double num = strength;
            float num2 = (float)steps;
            Vector2 vector;
            vector.X = (float)i;
            vector.Y = (float)j;
            Vector2 vector2;
            vector2.X = (float)WorldGen.genRand.Next(-10, 11) * 0.1f;
            vector2.Y = (float)WorldGen.genRand.Next(-10, 11) * 0.1f;
            if (speedX != 0f || speedY != 0f)
            {
                vector2.X = speedX;
                vector2.Y = speedY;
            }
            bool flag = type == 368;
            bool flag2 = type == 367;
            while (num > 0.0 && num2 > 0f)
            {
                if (vector.Y < 0f && num2 > 0f && type == 59)
                {
                    num2 = 0f;
                }
                num = strength * (double)(num2 / (float)steps);
                num2 -= 1f;
                int num3 = (int)((double)vector.X - num * 0.5);
                int num4 = (int)((double)vector.X + num * 0.5);
                int num5 = (int)((double)vector.Y - num * 0.5);
                int num6 = (int)((double)vector.Y + num * 0.5);
                if (num3 < 1)
                {
                    num3 = 1;
                }
                if (num4 > Main.maxTilesX - 1)
                {
                    num4 = Main.maxTilesX - 1;
                }
                if (num5 < 1)
                {
                    num5 = 1;
                }
                if (num6 > Main.maxTilesY - 1)
                {
                    num6 = Main.maxTilesY - 1;
                }
                for (int k = num3; k < num4; k++)
                {
                    for (int l = num5; l < num6; l++)
                    {
                        if ((double)(Math.Abs((float)k - vector.X) + Math.Abs((float)l - vector.Y)) < strength * 0.5 * (1.0 + (double)WorldGen.genRand.Next(-10, 11) * 0.015))
                        {
                            if (type < 0)
                            {
                                if (!addTile)
                                {
                                    _ = Main.tile[k, l].HasTile;
                                    Main.tile[k, l].Get<LiquidData>().LiquidType = 0;
                                }
                                else
                                {
                                    Main.tile[k, l].Get<LiquidData>().LiquidType = 255;
                                    if (type == -2)
                                    {
                                        Main.tile[k, l].Get<LiquidData>().LiquidType = 0;
                                    }
                                    _ = Main.tile[k, l].HasTile;
                                }
                            }
                            else
                            {
                                if (flag && (double)(Math.Abs((float)k - vector.X) + Math.Abs((float)l - vector.Y)) < strength * 0.3 * (1.0 + (double)WorldGen.genRand.Next(-10, 11) * 0.01))
                                {
                                    WorldGen.PlaceWall(k, l, 180, true);
                                }
                                if (flag2 && (double)(Math.Abs((float)k - vector.X) + Math.Abs((float)l - vector.Y)) < strength * 0.3 * (1.0 + (double)WorldGen.genRand.Next(-10, 11) * 0.01))
                                {
                                    WorldGen.PlaceWall(k, l, 178, true);
                                }
                                if (overRide || !Main.tile[k, l].HasTile)
                                {
                                    Tile tile = Main.tile[k, l];
                                    bool flag3 = Main.tileStone[type] && tile.TileType != 1;
                                    if (!TileID.Sets.CanBeClearedDuringGeneration[(int)tile.TileType])
                                    {
                                        flag3 = true;
                                    }
                                    ushort type2 = tile.TileType;
                                    if (type2 <= 147)
                                    {
                                        if (type2 <= 45)
                                        {
                                            if (type2 != 1)
                                            {
                                                if (type2 == 45)
                                                {
                                                    goto IL_5AB;
                                                }
                                            }
                                            else if (type == 59 && (double)l < Main.worldSurface + (double)WorldGen.genRand.Next(-50, 50))
                                            {
                                                flag3 = true;
                                            }
                                        }
                                        else if (type2 != 53)
                                        {
                                            if (type2 == 147)
                                            {
                                                goto IL_5AB;
                                            }
                                            if (type2 == 196)
                                            {
                                                goto IL_5AB;
                                            }
                                        }
                                        else
                                        {
                                            /*if (type == 40)
                                            {
                                                flag3 = true;
                                            }
                                            if ((double)l < Main.worldSurface && type != 59)
                                            {
                                                flag3 = true;
                                            }*/
                                        }
                                    }
                                    else if (type2 == 367 || type2 == 368)
                                    {
                                        if (type == 59)
                                        {
                                            flag3 = true;
                                        }
                                    }
                                    else if (type2 == 396 || type2 == 397)
                                    {
                                        flag3 = !TileID.Sets.Ore[type];
                                    }
                                IL_59B:
                                    if (!flag3)
                                    {
                                        tile.TileType = (ushort)type;
                                        goto IL_5B0;
                                    }
                                    goto IL_5B0;
                                IL_5AB:
                                    flag3 = true;
                                    goto IL_59B;
                                }
                            IL_5B0:
                                if (addTile)
                                {
                                    _ = Main.tile[k, l].HasTile;
                                    Main.tile[k, l].Get<LiquidData>().LiquidType = 0;
                                    //Main.tile[k, l].lava(false);
                                }
                                if (noYChange && (double)l < Main.worldSurface && type != 59)
                                {
                                    Main.tile[k, l].WallType = 2;
                                }
                                if (type == 59 && l > WorldGen.waterLine && Main.tile[k, l].LiquidType > 0)
                                {
                                    //Main.tile[k, l].lava(false);
                                    Main.tile[k, l].Get<LiquidData>().LiquidType = 0;
                                }
                            }
                        }
                    }
                }
                vector += vector2;
                if (num > 50.0)
                {
                    vector += vector2;
                    num2 -= 1f;
                    vector2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                    vector2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                    if (num > 100.0)
                    {
                        vector += vector2;
                        num2 -= 1f;
                        vector2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                        vector2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                        if (num > 150.0)
                        {
                            vector += vector2;
                            num2 -= 1f;
                            vector2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                            vector2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                            if (num > 200.0)
                            {
                                vector += vector2;
                                num2 -= 1f;
                                vector2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                vector2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                if (num > 250.0)
                                {
                                    vector += vector2;
                                    num2 -= 1f;
                                    vector2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                    vector2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                    if (num > 300.0)
                                    {
                                        vector += vector2;
                                        num2 -= 1f;
                                        vector2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                        vector2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                        if (num > 400.0)
                                        {
                                            vector += vector2;
                                            num2 -= 1f;
                                            vector2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                            vector2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                            if (num > 500.0)
                                            {
                                                vector += vector2;
                                                num2 -= 1f;
                                                vector2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                                vector2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                                if (num > 600.0)
                                                {
                                                    vector += vector2;
                                                    num2 -= 1f;
                                                    vector2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                                    vector2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                                    if (num > 700.0)
                                                    {
                                                        vector += vector2;
                                                        num2 -= 1f;
                                                        vector2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                                        vector2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                                        if (num > 800.0)
                                                        {
                                                            vector += vector2;
                                                            num2 -= 1f;
                                                            vector2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                                            vector2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                                            if (num > 900.0)
                                                            {
                                                                vector += vector2;
                                                                num2 -= 1f;
                                                                vector2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                                                vector2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                vector2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                if (vector2.X > 1f)
                {
                    vector2.X = 1f;
                }
                if (vector2.X < -1f)
                {
                    vector2.X = -1f;
                }
                if (!noYChange)
                {
                    vector2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                    if (vector2.Y > 1f)
                    {
                        vector2.Y = 1f;
                    }
                    if (vector2.Y < -1f)
                    {
                        vector2.Y = -1f;
                    }
                }
                else if (type != 59 && num < 3.0)
                {
                    if (vector2.Y > 1f)
                    {
                        vector2.Y = 1f;
                    }
                    if (vector2.Y < -1f)
                    {
                        vector2.Y = -1f;
                    }
                }
                if (type == 59 && !noYChange)
                {
                    if ((double)vector2.Y > 0.5)
                    {
                        vector2.Y = 0.5f;
                    }
                    if ((double)vector2.Y < -0.5)
                    {
                        vector2.Y = -0.5f;
                    }
                    if ((double)vector.Y < Main.rockLayer + 100.0)
                    {
                        vector2.Y = 1f;
                    }
                    if (vector.Y > (float)(Main.maxTilesY - 300))
                    {
                        vector2.Y = -1f;
                    }
                }
            }
        }





    }

}