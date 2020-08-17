using UnityEngine;
using Verse;

namespace RW_ThePacifier
{
	public class SettingsController : Mod
	{
		public SettingsController(ModContentPack content) : base(content)
		{
			GetSettings<Settings>();
		}

		public override string SettingsCategory()
		{
			return "The Pacifier";
		}

		public override void DoSettingsWindowContents(Rect inRect)
		{
			Settings.DoWindowContents(inRect);
		}
	}
}
