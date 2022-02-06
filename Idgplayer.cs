using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameInput;
using Terraria.Graphics.Shaders;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Idglibrary
{
    public class IdgPlayer: ModPlayer
    {
    public int radationlevel=0;
    public int souldrainlevel=0;
    public float radationRecover=0.01f;
	public float radationAmmount=0f;
	public float souldrainAmmount=0f;
	public string bossmodstring="";
		public bool limbo = false;
		public int noimmunity = 0;
		public int NullUcosmet = 0;
		public int NullU = 0;
		public int hotbarcurse = -1;
		public int itemcurse = -1;
		public int Damnation = 0;
		public bool resistLimbo = false;
		public float radresist = 1f;
	public Mod bossmod = null;

		public static void AddRadiationDebuff(Player player,int time,int level=0,bool quiet=false)
		{
		Idglib themod=(Idglib.Instance);
		string[] buffs={"RadiationOne","RadiationTwo","RadiationThree"};
		player.AddBuff(themod.BuffType(buffs[(int)MathHelper.Clamp(level,0,2)]),time,quiet);
		}

		public override void ResetEffects()
		{
			radationlevel=0;
			souldrainlevel=0;
			radationRecover=0.01f;
			bossmodstring = "";
			bossmod = null;
			limbo = false;
			radresist = 1f;
			resistLimbo = false;
			Damnation = Math.Max(Damnation - 1, 0);
		}

		public override TagCompound Save()
        {
            TagCompound IDGSave = new TagCompound();
            IDGSave["idgradation"]=radationAmmount;
            return IDGSave;
        }

        public override void Load(TagCompound IDGSave)
        {
        	radationAmmount=IDGSave.GetFloat("idgradation");
        }

		public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			radationAmmount=0;

			/*if (MoneyMismanagement==true && damageSource.SourceOtherIndex == 8)
			{
				damageSource = PlayerDeathReason.ByCustomReason(" couldn't stop spending money");
			}*/
			return true;
		}

		public override void PreUpdateBuffs()
		{
			if (!player.HasBuff(mod.BuffType("HotBarCurse")))
				hotbarcurse = -1;
			if (!player.HasBuff(mod.BuffType("ItemCurse")))
				itemcurse = -1;
			if (noimmunity > 0)
			{
				for (int loc = 0; loc < player.buffImmune.Length; loc += 1)
				{
					if (Main.debuff[loc])
					player.buffImmune[loc] = false;
				}
			}
				noimmunity = Math.Max(noimmunity - 1, 0);
		}

		public override void PostUpdateEquips()
		{

			if (NullU > 0)
			{
				FuncNullU(NullU, true);
			}

			NullUcosmet = NullU;
				NullU = 0;


			if (radationlevel > 0)
			{
				radationAmmount += ((0.01f * ((float)radationlevel))* (1f+(Idglib.GetSGAmodNightmareMode()*2f)) / (limbo && !resistLimbo ? 1f : radresist));
				if (player.statLife < 2) 
				{
					float rad = radationAmmount;
					player.KillMe(PlayerDeathReason.ByCustomReason(player.name + (limbo ? " faded away" : "'s flesh dissolved away")), 111111, 0, false);
					radationAmmount = rad*0.75f;
				}

			}
			else
			{

				radationAmmount = MathHelper.Clamp(radationAmmount - (radationRecover * (Main.expertMode ? 1f : 5f)), 0f, 10000f);

			}
			if (souldrainlevel > 0)
			{
				souldrainAmmount += ((float)souldrainlevel) * 0.05f;
			}
			else { souldrainAmmount = Math.Max(souldrainAmmount - 0.1f, 0f); }

			player.statManaMax2 = (int)MathHelper.Clamp(player.statManaMax2 - ((int)souldrainAmmount), 0, 10000);
			player.statLifeMax2 -= (int)radationAmmount;

			if (Idglib.GetSGAmodNightmareMode()>0)
			{
				player.statLifeMax2 *= 2 + Idglib.GetSGAmodNightmareMode();
			}


		}

		public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
		{
			if (Damnation > 0)
			{
				healValue = (int)(healValue / 3);
			}
		}

		public override void NaturalLifeRegen(ref float regen)
		{
			if (Damnation > 0)
			{
				regen *= 0.1f;
			}
			else
			{
				if (Idglib.GetSGAmodNightmareMode() > 0)
				{
					regen /= (2 + Idglib.GetSGAmodNightmareMode());
				}
			}
		}

		public override void UpdateBadLifeRegen()
		{
			if (Damnation > 0 && player.lifeRegen>0)
			{
				player.lifeRegen/=5;
			}
			if (Idglib.GetSGAmodNightmareMode()>0)
			{
				if (player.lifeRegen < 0)
				player.lifeRegen *= 2+Idglib.GetSGAmodNightmareMode();
			}
		}

		public void GetHit(Entity npc, ref int damage, ref bool crit)
		{
			if (Idglib.GetSGAmodNightmareMode()>0)
			{
				damage *= 2 + Idglib.GetSGAmodNightmareMode();
			}

		}

		public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
		{
			GetHit(proj as Entity, ref damage, ref crit);
		}

		public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
		{
			GetHit(npc as Entity, ref damage, ref crit);
		}

		public override void FrameEffects()
		{
			NullU = NullUcosmet;
			FuncNullU(NullU,false,true);
			NullU=0;
			NullUcosmet = 0;
		}

		public override void PostUpdateBuffs()
		{
			if (NullU > 0)
			{
				FuncNullU(NullU,false);
			}
		}

		private void FuncNullU(int level,bool runit=false, bool runit2 = false)
		{
			if (level > 3)
			{
				if (!runit2)
				player.ResetEffects();
				player.head = -1;
				player.body = -1;
				player.legs = -1;
				player.handon = -1;
				player.handoff = -1;
				player.back = -1;
				player.front = -1;
				player.shoe = -1;
				player.waist = -1;
				player.shield = -1;
				player.neck = -1;
				player.face = -1;
				player.balloon = -1;
				return;
			}

			if (level > 2)
			{
				int wingtype = player.wings;
				sbyte backtype = player.back;
				int wingtime = player.wingTimeMax;
				if (!runit2)
				player.ResetEffects();
				player.head = -1;
				player.body = -1;
				player.legs = -1;
				player.handon = -1;
				player.handoff = -1;
				player.back = -1;
				player.front = -1;
				player.shoe = -1;
				player.waist = -1;
				player.shield = -1;
				player.neck = -1;
				player.face = -1;
				player.balloon = -1;

				if (runit)
				{

					for (int x = 3; x < 8 + player.extraAccessorySlots; x++)
					{
						if ((int)player.armor[x].wingSlot > 0)
						{
							/*if (player.armor[x].modItem == null)
								player.VanillaUpdateEquip(player.armor[x]);
							else
								player.armor[x].modItem.UpdateAccessory(player, true);*/

							player.wingsLogic = (int)player.armor[x].wingSlot;

							break;
						}
					}

				}

				player.wings = wingtype;
				player.back = backtype;
				player.wingTimeMax = wingtime;

				return;
			}

			if (level > 0)
			{
				int wingtype = player.wings;
				sbyte backtype = player.back;
				int wingtime = player.wingTimeMax;

				if (!runit2)
					player.ResetEffects();

				if (runit)
				{
					for (int x = 0; x < 3; x++)
					{
						if (player.armor[x].modItem == null)
							player.VanillaUpdateEquip(player.armor[x]);
						else
							player.armor[x].modItem.UpdateEquip(player);

						ItemLoader.UpdateEquip(player.armor[x], player);
					}
					ItemLoader.UpdateArmorSet(player, player.armor[0], player.armor[1], player.armor[2]);

					for (int x = 3; x < 8 + player.extraAccessorySlots; x++)
					{
						if ((int)player.armor[x].wingSlot > 0)
						{
							/*if (player.armor[x].modItem == null)
								player.VanillaUpdateEquip(player.armor[x]);
							else
								player.armor[x].modItem.UpdateAccessory(player,true);*/

							player.wingsLogic = (int)player.armor[x].wingSlot;



							break;
						}
					}

				}

				player.handon = -1;
				player.handoff = -1;
				player.back = -1;
				player.front = -1;
				player.shoe = -1;
				player.waist = -1;
				player.shield = -1;
				player.neck = -1;
				player.face = -1;
				player.balloon = -1;

				player.wings = wingtype;
				player.back = backtype;
				player.wingTimeMax = wingtime;

				return;
			}

		}



		public override void DrawEffects(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
		{
			int ammount2 = limbo ? 5 : 1;
			int ammount=(radationlevel*4+1)+(radationAmmount>0f ? 2 : 0)* ammount2;
			if (ammount>0)
			{
			int q = 0;
			for (q = 0; q < ammount; q=q+1)
				{
					if (Main.rand.Next(0,100)<1){
						float size = 0.15f + (float)Math.Pow((double)radationAmmount / 80.00, 0.5);
						if (limbo)
						size = 0.15f + (float)Math.Pow((double)radationAmmount / 160.00, 0.5);
						Vector2 randomcircle=new Vector2(Main.rand.Next(-8000,8000),Main.rand.Next(-8000,8000)); randomcircle.Normalize();
					int dust = Dust.NewDust(new Vector2(drawInfo.position.X,drawInfo.position.Y)+randomcircle*8f, player.width + 4, player.height + 4, DustID.AncientLight, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 30, default(Color), size);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].color=Main.hslToRgb((float)(Main.GlobalTime*2)%1f, 0.5f, 0.35f);
						if (limbo)
						Main.dust[dust].shader = GameShaders.Armor.GetShaderFromItemId(ItemID.ShadowDye);
							Main.playerDrawDust.Add(dust);
				}}
				//r *= 0.1f;
				//g *= 0.2f;
				//b *= 0.7f;
				//fullBright = true;
			}
		}

	}

}