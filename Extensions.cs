using HarmonyLib;
using Verse;

namespace RW_ThePacifier
{
	public static class Extensions
	{
		public static bool CanFlee(this Pawn pawn) => pawn.RaceProps.Humanlike && !pawn.IsColonist;

		public static void MakeDowned(this Pawn_HealthTracker self,
									 DamageInfo? dinfo = null,
									 Hediff hediff = null)
		{
			AccessTools.Method(typeof(Pawn_HealthTracker), "MakeDowned")?
				.Invoke(self, new object[] { dinfo, hediff });
		}

		public static void MakeUndowned(this Pawn_HealthTracker self)
		{
			AccessTools.Method(typeof(Pawn_HealthTracker), "MakeUndowned")?
				.Invoke(self, null);
		}

		public static bool ShouldBeDowned(this Traverse self)
		{
			return self.Method("ShouldBeDowned", new object[0]).GetValue<bool>();
		}

		public static bool ShouldBeDead(this Traverse self)
		{
			return self.Method("ShouldBeDead", new object[0]).GetValue<bool>();
		}
	}
}
