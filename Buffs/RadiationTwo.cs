using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Idglibrary.Buffs
{
	public class RadiationTwo: ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Radiation II");
			Description.SetDefault("You are suffering moderate radiation poisoning");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			BuffID.Sets.LongerExpertDebuff[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			IdgPlayer idgplayer = player.GetModPlayer<IdgPlayer>();
			idgplayer.radationlevel += 3;
		}

		//public override void Update(NPC npc, ref int buffIndex)
		//{
		//	npc.GetGlobalNPC<SGAWorld>(mod).eFlames = true;
		//}
	}
}
