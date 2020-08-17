using HarmonyLib;
using System.Reflection;
using Verse;

namespace RW_ThePacifier
{
	[StaticConstructorOnStartup]
	class Main
    {
		static Main()
		{
			new Harmony("com.rimworld.mod.nyan.the_pacifier")
				.PatchAll(Assembly.GetExecutingAssembly());
		}
    }
}
