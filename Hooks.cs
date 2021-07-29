using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RW_ThePacifier
{
	/*[HarmonyPatch(typeof(Pawn_JobTracker), "TryFindAndStartJob")]
	public class Patch_Pawn_JobTracker_TryFindAndStartJob
	{
		[HarmonyPriority(Priority.Last)]
		[HarmonyPrefix]
		public static bool Patch(Pawn ___pawn)
		{
			if (!___pawn.health.Downed)
				return true;

			bool SelfTend =
				___pawn.Faction == null ?
					Settings.Wild_SelfTend :
				___pawn.Faction.IsPlayer ?
					Settings.Player_SelfTend :
				___pawn.Faction.HostileTo(Faction.OfPlayerSilentFail) ?
					Settings.Enemy_SelfTend :
					Settings.Ally_SelfTend;

			if (!SelfTend)
				return true;

			if (!___pawn.playerSettings.selfTend)
				return true;

			if (___pawn.WorkTypeIsDisabled(WorkTypeDefOf.Doctor))
				return true;

			if (!___pawn.health.HasHediffsNeedingTend())
				return true;

			if (___pawn.CurJobDef != JobDefOf.TendPatient)
			{
				Job job = JobMaker.MakeJob(JobDefOf.TendPatient, ___pawn);
				job.endAfterTendedOnce = true;
				___pawn.jobs.StartJob(job);
			}

			return false;
		}
	}*/

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
					injury.Severity >= Settings.DestroyThreshold)
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

			if (___pawn.Dead)
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
					__instance.MakeDowned(new DamageInfo?(dinfo));

			Pawn pawn;

			if (dinfo.Def.additionalHediffs != null &&
				(dinfo.Def.applyAdditionalHediffsIfHuntingForFood ||
				(pawn = dinfo.Instigator as Pawn) == null ||
				pawn.CurJob == null ||
				pawn.CurJob.def != JobDefOf.PredatorHunt))
			{
				List<DamageDefAdditionalHediff> additionalHediffs = dinfo.Def.additionalHediffs;

				for (int i = 0; i < additionalHediffs.Count; i++)
				{
					DamageDefAdditionalHediff damageDefAdditionalHediff = additionalHediffs[i];

					if (damageDefAdditionalHediff.hediff != null)
					{
						float num = (damageDefAdditionalHediff.severityFixed <= 0f)
							? (totalDamageDealt * damageDefAdditionalHediff.severityPerDamageDealt)
							: damageDefAdditionalHediff.severityFixed;

						if (damageDefAdditionalHediff.victimSeverityScalingByInvBodySize)
							num *= 1f / ___pawn.BodySize;

						if (damageDefAdditionalHediff.victimSeverityScaling != null)
							num *= ___pawn.GetStatValue(damageDefAdditionalHediff.victimSeverityScaling, true);
						
						if (num >= 0f)
						{
							Hediff hediff = HediffMaker.MakeHediff(damageDefAdditionalHediff.hediff, ___pawn, null);
							hediff.Severity = num;
							__instance.AddHediff(hediff, null, new DamageInfo?(dinfo), null);
						}
					}
				}
			}

			/*if (dinfo.Def.additionalHediffs != null)
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
					}*/

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

			if (NoBlood)
				return false;

			Traverse traverse = Traverse.Create(___pawn.health);

			if (!traverse.ShouldBeDead())
				return true;

			bool DeadBloodless =
				___pawn.Faction == null ?
					Settings.Wild_DeadBloodless :
				___pawn.Faction.IsPlayer ?
					Settings.Player_DeadBloodless :
				___pawn.Faction.HostileTo(Faction.OfPlayerSilentFail) ?
					Settings.Enemy_DeadBloodless :
					Settings.Ally_DeadBloodless;

			return !DeadBloodless;
		}
	}

	[HarmonyPatch(typeof(Pawn_HealthTracker), "CheckForStateChange")]
	public class Patch_Pawn_HealthTracker_CheckForStateChange
	{
		public static void Patch_NoScar
			(Pawn_HealthTracker instance,
			Pawn pawn)
		{
			bool NoScar =
				pawn.Faction == null ?
					Settings.Wild_NoScar :
				pawn.Faction.IsPlayer ?
					Settings.Player_NoScar :
				pawn.Faction.HostileTo(Faction.OfPlayerSilentFail) ?
					Settings.Enemy_NoScar :
					Settings.Ally_NoScar;

			if (!NoScar)
				return;

			List<Hediff> list = instance.hediffSet.hediffs;
			int i = list.Count - 1;

			while (i >= 0)
			{
				Hediff hediff1 = list[i];

				if (hediff1.IsPermanent())
				{
					list.Remove(hediff1);

					Hediff newHediff = HediffMaker.MakeHediff(
						hediff1.def,
						pawn,
						hediff1.Part
					);
					newHediff.Severity = hediff1.Severity;

					list.Add(newHediff);
				}

				i--;
			}

			return;
		}

		public static bool Patch_NoDeath_DeathWake
			(Pawn_HealthTracker instance,
			Pawn pawn,
			DamageInfo? dinfo,
			Hediff hediff)
		{
			bool NoDeath =
				pawn.Faction == null ?
					Settings.Wild_NoDeath :
				pawn.Faction.IsPlayer ?
					Settings.Player_NoDeath :
				pawn.Faction.HostileTo(Faction.OfPlayerSilentFail) ?
					Settings.Enemy_NoDeath :
					Settings.Ally_NoDeath;

			if (!NoDeath)
				return true;

			Traverse traverse = Traverse.Create(instance);

			if (instance.Dead)
				return false;

			if (!instance.Downed)
			{
				if (traverse.ShouldBeDead() || traverse.ShouldBeDowned())
				{
					instance.forceIncap = false;

					if (pawn.mindState.duty == null ||
						pawn.mindState.duty.def != DutyDefOf.ExitMapBest)
						instance.MakeDowned(dinfo, hediff);

					return false;
				}

				if (!instance.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
				{
					if (pawn.carryTracker != null &&
						pawn.carryTracker.CarriedThing != null &&
						pawn.jobs != null &&
						pawn.CurJob != null)
						pawn.jobs.EndCurrentJob(JobCondition.InterruptForced, true, true);

					if (pawn.equipment != null &&
						pawn.equipment.Primary != null)
					{
						if (pawn.kindDef.destroyGearOnDrop)
						{
							pawn.equipment.DestroyEquipment(pawn.equipment.Primary);
							return false;
						}

						if (pawn.InContainerEnclosed)
						{
							pawn.equipment.TryTransferEquipmentToContainer(
								pawn.equipment.Primary,
								pawn.holdingOwner
							);
							return false;
						}

						if (pawn.SpawnedOrAnyParentSpawned)
						{
							pawn.equipment.TryDropEquipment(
								pawn.equipment.Primary,
								out ThingWithComps _,
								pawn.PositionHeld,
								true
							);
							return false;
						}

						if (!pawn.IsCaravanMember())
						{
							pawn.equipment.DestroyEquipment(pawn.equipment.Primary);
							return false;
						}

						ThingWithComps primary = pawn.equipment.Primary;
						pawn.equipment.Remove(primary);

						if (!pawn.inventory.innerContainer.TryAdd(primary, true))
						{
							primary.Destroy(DestroyMode.Vanish);
							return false;
						}
					}
				}
			}
			else if (!traverse.ShouldBeDowned())
			{
				instance.MakeUndowned();
				return false;
			}
			else if (pawn.CarriedBy == null)
			{
				bool DeathWake =
					pawn.Faction == null ?
						Settings.Wild_DeathWake :
					pawn.Faction.IsPlayer ?
						Settings.Player_DeathWake :
					pawn.Faction.HostileTo(Faction.OfPlayerSilentFail) ?
						Settings.Enemy_DeathWake :
						Settings.Ally_DeathWake;

				if (!DeathWake ||
					Rand.Value >= instance.summaryHealth.SummaryHealthPercent / 4)
					return false;

				instance.MakeUndowned();

				if (!pawn.Faction.IsPlayer)
					pawn.mindState.duty = new PawnDuty(DutyDefOf.ExitMapBest)
					{
						locomotion = LocomotionUrgency.Sprint
					};
			}

			return false;
		}

		public static void Patch_SelfTend(Pawn pawn)
		{
			if (!pawn.Spawned)
				return;

			if (!pawn.health.Downed)
				return;

			bool SelfTend =
				pawn.Faction == null ?
					Settings.Wild_SelfTend :
				pawn.Faction.IsPlayer ?
					Settings.Player_SelfTend :
				pawn.Faction.HostileTo(Faction.OfPlayerSilentFail) ?
					Settings.Enemy_SelfTend :
					Settings.Ally_SelfTend;

			if (!SelfTend)
				return;

			if (!pawn.playerSettings.selfTend)
				return;

			if (pawn.WorkTypeIsDisabled(WorkTypeDefOf.Doctor))
				return;

			if (!pawn.health.HasHediffsNeedingTend())
				return;

			if (pawn.CurJobDef != JobDefOf.TendPatient)
			{
				Job job = JobMaker.MakeJob(JobDefOf.TendPatient, pawn);
				job.endAfterTendedOnce = true;
				pawn.jobs.StartJob(job);
			}
		}

		[HarmonyPriority(Priority.Last)]
		[HarmonyPrefix]
		public static bool Patch
			(Pawn_HealthTracker __instance,
			Pawn ___pawn,
			DamageInfo? dinfo,
			Hediff hediff)
		{
			Patch_NoScar(__instance, ___pawn);
			bool result = Patch_NoDeath_DeathWake(__instance, ___pawn, dinfo, hediff);
			Patch_SelfTend(___pawn);

			return result;
		}
	}
}
