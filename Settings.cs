using UnityEngine;
using Verse;

namespace RW_ThePacifier
{
	public class Settings : ModSettings
	{
		public const string DESCRIPTION_MAX_INJURIES =
			"Pawns will die if they have more than this amount of injuries. " +
			"Setting to 0 or below will allow unlimited injuries.\n" +
			"Default: 1000";
		public const string DESCRIPTION_DESTROY_THRESHOLD =
			"The amount of severity needed for a " +
			"single injury to fully destroy/amputate a body part. " +
			"A value of 10,000 should be enough to " +
			"allow amputation via surgery, " +
			"while preventing serious injuries from " +
			"destroying body parts.\n" +
			"Default: 10000";

		public const string DESCRIPTION_PLAYER =
			"Pawns that belong to your faction.";
		public const string DESCRIPTION_ALLY =
			"Pawns that belong to an allied faction.";
		public const string DESCRIPTION_ENEMY =
			"Pawns that belong to a hostile faction.";
		public const string DESCRIPTION_WILD =
			"Pawns that do not belong to any factions.";

		public const string DESCRIPTION_NO_DEATH =
			"When enabled, pawns can no longer die. " +
			"Pawns that are supposedly dead are incapacitated until cured. " +
			"The `Finish Off` tool from the `Allow Tools` mod will " +
			"allow you to kill them.";
		public const string DESCRIPTION_DEATH_WAKE =
			"When enabled, incapacitated pawns have a random chance to " +
			"stand up regardless if they are unconcious. " +
			"Pawns that are not owned by you will flee the map. " +
			"Chance scales based on their current health. " +
			"Rolls everytime the health tracker is updated, roughly every 1 second.";
		public const string DESCRIPTION_STRONG_BODY =
			"When enabled, body parts can only be destroyed/amputated if the " +
			"injury's severity levels is greater than or equal to the " +
			"`" + DESTROY_THRESHOLD + "` value.";
		public const string DESCRIPTION_NO_SCAR =
			"When enabled, permanent injuries or scars will be replaced with " +
			"a curable counterpart with the same severity level. " +
			"Pawns who already have permanent injuries are affected. " +
			"This affects all kinds of pawns.";
		public const string DESCRIPTION_DEAD_BLOODLESS =
			"When enabled, blood filth will no longer be generated for pawns " +
			"who are supposed to be dead. " +
			"This prevents massive amounts of blood from being generated " +
			"(supposedly-dead pawns generate a lot of blood,) " +
			"while still allowing minor injuries to generate them. " +
			"This only applies to pawns who are still alive " +
			"but should be dead with their injuries.";
		public const string DESCRIPTION_NO_BLOOD =
			"When enabled, blood filth will no longer be generated completely. " +
			"Existing ones on the map will not be removed.";
		public const string DESCRIPTION_KILL_HIT =
			"When enabled, incapacitated pawns that are supposedly dead " +
			"will die if they are attacked at least once.";
		public const string DESCRIPTION_SELF_TEND =
			"When enabled, incapacitated pawns can self-tend " +
			"(attempts to self-tend roughly every second.) " +
			"This still requires them to be assigned to doctoring and allow self-tending, " +
			"and can be interrupted by disallowing them.";

		public const string MAX_INJURIES = "Max Injuries to Prevent Death";
		public const string DESTROY_THRESHOLD = "Body Part Destroy Threshold";

		public const string PLAYER = "Player";
		public const string ALLY = "Ally";
		public const string ENEMY = "Enemy";
		public const string WILD = "Wild";

		public const string NO_DEATH = "No Death";
		public const string DEATH_WAKE = "Random Chance for Incapacitated Pawns to Stand Up";
		public const string STRONG_BODY = "Indestructible Body Parts";
		public const string NO_SCAR = "Replace Scars with Curable Wounds";
		public const string DEAD_BLOODLESS = "Prevent Supposedly-Dead Pawns from Generating Blood Filth";
		public const string NO_BLOOD = "Disable Blood Filth Generation";
		public const string KILL_HIT = "Kill Supposedly-Dead Pawns on Hit";
		public const string SELF_TEND = "Self-Tend While Incapacitated";

		public static int MaxInjuries = 1000;
		public static string MaxInjuries_Buffer = null;

		public static float DestroyThreshold = 10000f;
		public static string DestroyThreshold_Buffer = null;

		public static bool Player_NoDeath = true;
		public static bool Player_DeathWake = false;
		public static bool Player_StrongBody = true;
		public static bool Player_NoScar = true;
		public static bool Player_DeadBloodless = true;
		public static bool Player_NoBlood = false;
		public static bool Player_KillHit = false;
		public static bool Player_SelfTend = false;

		public static bool Ally_NoDeath = true;
		public static bool Ally_DeathWake = true;
		public static bool Ally_StrongBody = true;
		public static bool Ally_NoScar = true;
		public static bool Ally_DeadBloodless = true;
		public static bool Ally_NoBlood = false;
		public static bool Ally_KillHit = false;
		public static bool Ally_SelfTend = true;

		public static bool Enemy_NoDeath = true;
		public static bool Enemy_DeathWake = true;
		public static bool Enemy_StrongBody = true;
		public static bool Enemy_NoScar = true;
		public static bool Enemy_DeadBloodless = true;
		public static bool Enemy_NoBlood = false;
		public static bool Enemy_KillHit = false;
		public static bool Enemy_SelfTend = true;

		public static bool Wild_NoDeath = false;
		public static bool Wild_DeathWake = false;
		public static bool Wild_StrongBody = false;
		public static bool Wild_NoScar = false;
		public static bool Wild_DeadBloodless = false;
		public static bool Wild_NoBlood = false;
		public static bool Wild_KillHit = false;
		public static bool Wild_SelfTend = false;

		public static void DoWindowContents(Rect inRect)
		{
			Listing_Standard gui = new Listing_Standard();

			gui.Begin(inRect);
			{
				if (DestroyThreshold_Buffer == null)
					DestroyThreshold_Buffer = DestroyThreshold.ToString();

				if (MaxInjuries_Buffer == null)
					MaxInjuries_Buffer = MaxInjuries.ToString();

				gui.Label(MAX_INJURIES, tooltip: DESCRIPTION_MAX_INJURIES);
				gui.IntEntry(ref MaxInjuries, ref MaxInjuries_Buffer);

				gui.Label(DESTROY_THRESHOLD, tooltip: DESCRIPTION_DESTROY_THRESHOLD);
				gui.TextFieldNumeric(ref DestroyThreshold, ref DestroyThreshold_Buffer);

				float height = gui.CurHeight;
				gui.ColumnWidth /= 2f;

				gui.Label(string.Empty);
				gui.Label(NO_DEATH, tooltip: DESCRIPTION_NO_DEATH);
				gui.Label(DEATH_WAKE, tooltip: DESCRIPTION_DEATH_WAKE);
				gui.Label(STRONG_BODY, tooltip: DESCRIPTION_STRONG_BODY);
				gui.Label(NO_SCAR, tooltip: DESCRIPTION_NO_SCAR);
				gui.Label(DEAD_BLOODLESS, tooltip: DESCRIPTION_DEAD_BLOODLESS);
				gui.Label(NO_BLOOD, tooltip: DESCRIPTION_NO_BLOOD);
				gui.Label(KILL_HIT, tooltip: DESCRIPTION_KILL_HIT);
				gui.Label(SELF_TEND, tooltip: DESCRIPTION_SELF_TEND);

				gui.NewColumn();
				gui.Gap(height);
				gui.ColumnWidth = 80f;

				Draw_Checkbox(
					gui,
					PLAYER,
					DESCRIPTION_PLAYER,
					ref Player_NoDeath,
					ref Player_DeathWake,
					ref Player_StrongBody,
					ref Player_NoScar,
					ref Player_DeadBloodless,
					ref Player_NoBlood,
					ref Player_KillHit,
					ref Player_SelfTend
				);

				gui.NewColumn();
				gui.Gap(height);

				Draw_Checkbox(
					gui,
					ALLY,
					DESCRIPTION_ALLY,
					ref Ally_NoDeath,
					ref Ally_DeathWake,
					ref Ally_StrongBody,
					ref Ally_NoScar,
					ref Ally_DeadBloodless,
					ref Ally_NoBlood,
					ref Ally_KillHit,
					ref Ally_SelfTend
				);

				gui.NewColumn();
				gui.Gap(height);

				Draw_Checkbox(
					gui,
					ENEMY,
					DESCRIPTION_ENEMY,
					ref Enemy_NoDeath,
					ref Enemy_DeathWake,
					ref Enemy_StrongBody,
					ref Enemy_NoScar,
					ref Enemy_DeadBloodless,
					ref Enemy_NoBlood,
					ref Enemy_KillHit,
					ref Enemy_SelfTend
				);

				gui.NewColumn();
				gui.Gap(height);

				Draw_Checkbox(
					gui,
					WILD,
					DESCRIPTION_WILD,
					ref Wild_NoDeath,
					ref Wild_DeathWake,
					ref Wild_StrongBody,
					ref Wild_NoScar,
					ref Wild_DeadBloodless,
					ref Wild_NoBlood,
					ref Wild_KillHit,
					ref Wild_SelfTend
				);
			}
			gui.End();
		}

		public static void Draw_Checkbox(Listing_Standard gui,
										 string LABEL,
										 string DESCRIPTION,
										 ref bool NoDeath,
										 ref bool DeathWake,
										 ref bool StrongBody,
										 ref bool NoScar,
										 ref bool DeadBloodless,
										 ref bool NoBlood,
										 ref bool KillHit,
										 ref bool SelfTend)
		{
			float width = gui.ColumnWidth;

			gui.Label(LABEL, tooltip: DESCRIPTION);

			gui.ColumnWidth = 30f;

			gui.CheckboxLabeled(string.Empty, ref NoDeath);
			gui.CheckboxLabeled(string.Empty, ref DeathWake);
			gui.CheckboxLabeled(string.Empty, ref StrongBody);
			gui.CheckboxLabeled(string.Empty, ref NoScar);
			gui.CheckboxLabeled(string.Empty, ref DeadBloodless);
			gui.CheckboxLabeled(string.Empty, ref NoBlood);
			gui.CheckboxLabeled(string.Empty, ref KillHit);
			gui.CheckboxLabeled(string.Empty, ref SelfTend);

			gui.ColumnWidth = width;
		}

		public override void ExposeData()
		{
			base.ExposeData();

			Scribe_Values.Look(ref MaxInjuries, "MaxInjuries", MaxInjuries, true);
			Scribe_Values.Look(ref DestroyThreshold, "DestroyThreshold", DestroyThreshold, true);

			Scribe_Values.Look(ref Player_NoDeath, "Player_NoDeath", Player_NoDeath, true);
			Scribe_Values.Look(ref Player_DeathWake, "Player_DeathWake", Player_DeathWake, true);
			Scribe_Values.Look(ref Player_StrongBody, "Player_StrongBody", Player_StrongBody, true);
			Scribe_Values.Look(ref Player_NoScar, "Player_NoScar", Player_NoScar, true);
			Scribe_Values.Look(ref Player_DeadBloodless, "Player_DeadBloodless", Player_DeadBloodless, true);
			Scribe_Values.Look(ref Player_NoBlood, "Player_NoBlood", Player_NoBlood, true);
			Scribe_Values.Look(ref Player_KillHit, "Player_KillHit", Player_KillHit, true);
			Scribe_Values.Look(ref Player_SelfTend, "Player_SelfTend", Player_SelfTend, true);

			Scribe_Values.Look(ref Ally_NoDeath, "Ally_NoDeath", Ally_NoDeath, true);
			Scribe_Values.Look(ref Ally_DeathWake, "Ally_DeathWake", Ally_DeathWake, true);
			Scribe_Values.Look(ref Ally_StrongBody, "Ally_StrongBody", Ally_StrongBody, true);
			Scribe_Values.Look(ref Ally_NoScar, "Ally_NoScar", Ally_NoScar, true);
			Scribe_Values.Look(ref Ally_DeadBloodless, "Ally_DeadBloodless", Ally_DeadBloodless, true);
			Scribe_Values.Look(ref Ally_NoBlood, "Ally_NoBlood", Ally_NoBlood, true);
			Scribe_Values.Look(ref Ally_KillHit, "Ally_KillHit", Ally_KillHit, true);
			Scribe_Values.Look(ref Ally_SelfTend, "Ally_SelfTend", Ally_SelfTend, true);

			Scribe_Values.Look(ref Enemy_NoDeath, "Enemy_NoDeath", Enemy_NoDeath, true);
			Scribe_Values.Look(ref Enemy_DeathWake, "Enemy_DeathWake", Enemy_DeathWake, true);
			Scribe_Values.Look(ref Enemy_StrongBody, "Enemy_StrongBody", Enemy_StrongBody, true);
			Scribe_Values.Look(ref Enemy_NoScar, "Enemy_NoScar", Enemy_NoScar, true);
			Scribe_Values.Look(ref Enemy_DeadBloodless, "Enemy_DeadBloodless", Enemy_DeadBloodless, true);
			Scribe_Values.Look(ref Enemy_NoBlood, "Enemy_NoBlood", Enemy_NoBlood, true);
			Scribe_Values.Look(ref Enemy_KillHit, "Enemy_KillHit", Enemy_KillHit, true);
			Scribe_Values.Look(ref Enemy_SelfTend, "Enemy_SelfTend", Enemy_SelfTend, true);

			Scribe_Values.Look(ref Wild_NoDeath, "Wild_NoDeath", Wild_NoDeath, true);
			Scribe_Values.Look(ref Wild_DeathWake, "Wild_DeathWake", Wild_DeathWake, true);
			Scribe_Values.Look(ref Wild_StrongBody, "Wild_StrongBody", Wild_StrongBody, true);
			Scribe_Values.Look(ref Wild_NoScar, "Wild_NoScar", Wild_NoScar, true);
			Scribe_Values.Look(ref Wild_DeadBloodless, "Wild_DeadBloodless", Wild_DeadBloodless, true);
			Scribe_Values.Look(ref Wild_NoBlood, "Wild_NoBlood", Wild_NoBlood, true);
			Scribe_Values.Look(ref Wild_KillHit, "Wild_KillHit", Wild_KillHit, true);
			Scribe_Values.Look(ref Wild_SelfTend, "Wild_SelfTend", Wild_SelfTend, true);
		}
	}
}
