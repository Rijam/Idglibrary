using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Idglibrary.Buffs
{
	public class RadiationThree: ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Radiation III");
			Description.SetDefault("You are suffering lethal radiation poisoning");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			BuffID.Sets.LongerExpertDebuff[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			IdgPlayer idgplayer = player.GetModPlayer<IdgPlayer>();
			idgplayer.radationlevel += 10;
		}

		//public override void Update(NPC npc, ref int buffIndex)
		//{
		//	npc.GetGlobalNPC<SGAWorld>(mod).eFlames = true;
		//}
	}
}
