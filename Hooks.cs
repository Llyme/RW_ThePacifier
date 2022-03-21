using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RW_ThePacifier
{
	[HarmonyPatch(typeof(HediffSet), "GetPartHealth")]
	public class Patch_HediffSet_GetPartHealth
	{
		[HarmonyPriority(Priority.Last)]
		[HarmonyPostfix]
		public static void Patch(HediffSet __instance, ref float __result, BodyPartRecord part)
		{
			bool StrongBody =
				__instance.pawn.Faction == null ?
					Settings.Wild_StrongBody :
				__instance.pawn.Faction.IsPlayer ?
					Settings.Player_StrongBody :
				__instance.pawn.Faction.HostileTo(Faction.OfPlayerSilentFail) ?
					Settings.Enemy_StrongBody :
					Settings.Ally_StrongBody;

			if (!StrongBody)
				return;

			foreach (Hediff hediff in __instance.hediffs)
				if (hediff.Part == part &&
					hediff is Hediff_Injury injury &&
					injury.Severity >= 10000)
					return; // Most likely being amputated via operation bill.

			if (part.def.destroyableByDamage)
				__result = Mathf.Max(__result, 1f);
		}
	}

	[HarmonyPatch(typeof(Pawn_HealthTracker), "PostApplyDamage")]
	public class Patch_Pawn_HealthTracker_PostApplyDamage
	{
		[HarmonyPriority(Priority.Last)]
		[HarmonyPrefix]
		public static bool Patch(Pawn_HealthTracker __instance,
								 Pawn ___pawn,
								 DamageInfo dinfo,
								 float totalDamageDealt)
		{
			bool NoDeath =
				___pawn.Faction == null ?
					Settings.Wild_NoDeath :
				___pawn.Faction.IsPlayer ?
					Settings.Player_NoDeath :
				___pawn.Faction.HostileTo(Faction.OfPlayerSilentFail) ?
					Settings.Enemy_NoDeath :
					Settings.Ally_NoDeath;

			if (!NoDeath || ___pawn.Destroyed)
				return true;

			Traverse traverse = Traverse.Create(__instance);

			if (traverse.ShouldBeDead())
				if (__instance.Downed)
				{
					bool KillHit =
						___pawn.Faction == null ?
							Settings.Wild_KillHit :
						___pawn.Faction.IsPlayer ?
							Settings.Player_KillHit :
						___pawn.Faction.HostileTo(Faction.OfPlayerSilentFail) ?
							Settings.Enemy_KillHit :
							Settings.Ally_KillHit;

					if (KillHit)
					{
						___pawn.Kill(new DamageInfo?(dinfo));
						return false;
					}
				}
				else
					__instance.SetDowned(new DamageInfo?(dinfo));

			if (dinfo.Def.additionalHediffs != null)
				foreach (DamageDefAdditionalHediff dmg in dinfo.Def.additionalHediffs)
					if (dmg.hediff != null)
					{
						float num = totalDamageDealt * dmg.severityPerDamageDealt;

						if (dmg.victimSeverityScaling != null)
							num *= ___pawn.GetStatValue(dmg.victimSeverityScaling, true);

						if (num >= 0f)
						{
							Hediff hediff = HediffMaker.MakeHediff(dmg.hediff, ___pawn, null);
							hediff.Severity = num;

							__instance.AddHediff(hediff, null, new DamageInfo?(dinfo), null);
						}
					}

			return false;
		}
	}

	[HarmonyPatch(typeof(Pawn_HealthTracker), "DropBloodFilth")]
	public class Patch_Pawn_HealthTracker_DropBloodFilth
	{
		[HarmonyPriority(Priority.Last)]
		[HarmonyPrefix]
		public static bool Patch(Pawn ___pawn)
		{
			bool NoBlood =
				___pawn.Faction == null ?
					Settings.Wild_NoBlood :
				___pawn.Faction.IsPlayer ?
					Settings.Player_NoBlood :
				___pawn.Faction.HostileTo(Faction.OfPlayerSilentFail) ?
					Settings.Enemy_NoBlood :
					Settings.Ally_NoBlood;

			return !NoBlood;
		}
	}

	[HarmonyPatch(typeof(Pawn_HealthTracker), "CheckForStateChange")]
	public class Patch_Pawn_HealthTracker_CheckForStateChange
	{
		[HarmonyPriority(Priority.Last)]
		[HarmonyPrefix]
		public static bool Patch(Pawn_HealthTracker __instance,
								 Pawn ___pawn,
								 DamageInfo? dinfo,
								 Hediff hediff)
		{
			if (__instance.Dead)
				return true;

			bool NoScar =
				___pawn.Faction == null ?
					Settings.Wild_NoScar :
				___pawn.Faction.IsPlayer ?
					Settings.Player_NoScar :
				___pawn.Faction.HostileTo(Faction.OfPlayerSilentFail) ?
					Settings.Enemy_NoScar :
					Settings.Ally_NoScar;

			bool NoDeath =
				___pawn.Faction == null ?
					Settings.Wild_NoDeath :
				___pawn.Faction.IsPlayer ?
					Settings.Player_NoDeath :
				___pawn.Faction.HostileTo(Faction.OfPlayerSilentFail) ?
					Settings.Enemy_NoDeath :
					Settings.Ally_NoDeath;

			if (NoScar)
			{
				List<Hediff> list = __instance.hediffSet.hediffs;
				int i = list.Count - 1;

				while (i >= 0)
				{
					Hediff hediff1 = list[i];

					if (hediff1.IsPermanent())
					{
						list.Remove(hediff1);

						Hediff newHediff = HediffMaker.MakeHediff(
							hediff1.def,
							___pawn,
							hediff1.Part
						);
						newHediff.Severity = hediff1.Severity;

						list.Add(newHediff);
					}

					i--;
				}
			}

			if (!NoDeath)
				return true;

			Traverse traverse = Traverse.Create(__instance);

			if (!__instance.Downed)
			{
				if (traverse.ShouldBeDead() || traverse.ShouldBeDowned())
				{
					__instance.forceIncap = false;

					if (___pawn.mindState.duty == null ||
						___pawn.mindState.duty.def != DutyDefOf.ExitMapBest)
						__instance.SetDowned(dinfo, hediff);

					return false;
				}

				if (!__instance.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
				{
					if (___pawn.carryTracker != null &&
						___pawn.carryTracker.CarriedThing != null &&
						___pawn.jobs != null &&
						___pawn.CurJob != null)
						___pawn.jobs.EndCurrentJob(JobCondition.InterruptForced, true);

					if (___pawn.equipment != null && ___pawn.equipment.Primary != null)
						if (___pawn.kindDef.destroyGearOnDrop)
							___pawn.equipment.DestroyEquipment(___pawn.equipment.Primary);
						else if (___pawn.InContainerEnclosed)
							___pawn.equipment.TryTransferEquipmentToContainer(
								___pawn.equipment.Primary,
								___pawn.holdingOwner
							);
						else if (___pawn.SpawnedOrAnyParentSpawned)
							___pawn.equipment.TryDropEquipment(
								___pawn.equipment.Primary,
								out var _,
								___pawn.PositionHeld,
								true
							);
						else
							___pawn.equipment.DestroyEquipment(___pawn.equipment.Primary);
				}
			}
			else if (!traverse.ShouldBeDowned())
			{
				__instance.SetUndowned();
			}
			else if (___pawn.CarriedBy == null)
			{
				bool DeathWake =
					___pawn.Faction == null ?
						Settings.Wild_DeathWake :
					___pawn.Faction.IsPlayer ?
						Settings.Player_DeathWake :
					___pawn.Faction.HostileTo(Faction.OfPlayerSilentFail) ?
						Settings.Enemy_DeathWake :
						Settings.Ally_DeathWake;

				if (!DeathWake ||
					Rand.Value >= __instance.summaryHealth.SummaryHealthPercent / 4)
					return false;

				__instance.SetUndowned();

				if (!___pawn.Faction.IsPlayer)
					___pawn.mindState.duty = new PawnDuty(DutyDefOf.ExitMapBest)
					{
						locomotion = LocomotionUrgency.Sprint
					};
			}

			return false;
		}
	}
}
