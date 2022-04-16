using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Idglibrary.Buffs
{
	public class RadiationOne: ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Radiation I");
			Description.SetDefault("You are suffering minor radiation poisoning");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			BuffID.Sets.LongerExpertDebuff[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			IdgPlayer idgplayer = player.GetModPlayer<IdgPlayer>();
			idgplayer.radationlevel += 1;
		}

		//public override void Update(NPC npc, ref int buffIndex)
		//{
		//	npc.GetGlobalNPC<SGAWorld>(mod).eFlames = true;
		//}
	}

	public class LimboFading : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Limbo Fading");
			Description.SetDefault("Your existance is fading...");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			BuffID.Sets.LongerExpertDebuff[Type] = true;
		}

		public override string Texture => "Idglibrary/Buffs/SoulDrain";

		public override void ModifyBuffTip(ref string tip, ref int rare)
		{
			if (Main.LocalPlayer.HasBuff(BuffID.ChaosState))
				tip += "\nEffects amplied by Chaos Staff";
		}

		public override void Update(Player player, ref int buffIndex)
		{
			IdgPlayer idgplayer = player.GetModPlayer<IdgPlayer>();
			idgplayer.radationlevel += player.HasBuff(BuffID.ChaosState) ? 16 : 4;
			idgplayer.limbo = true;
		}


	}
	}
