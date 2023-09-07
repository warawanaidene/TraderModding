using Aki.Reflection.Patching;
using EFT.InventoryLogic;
using System.Reflection;
using System.Linq;
using EFT.UI.WeaponModding;
using UnityEngine;
using EFT.UI;
using EFT.UI.Screens;

namespace TraderModding
{
    public class ModsHidePatch : ModulePatch
    {
        // This patch returns false on the method that populates mods in the slot view container. pretty
        // spaghetti but it works

        protected override MethodBase GetTargetMethod()
        {
            return typeof(DropDownMenu).GetMethod("method_0", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        [PatchPrefix]
        static bool Prefix(Item item, ref RectTransform container)
        {
            if (Globals.isTraderModding && Globals.traderMods.Length > 0)
            {
                if (!Globals.traderMods.Any(mod => mod == item.TemplateId))
                {
                    return false;
                }
            }
            return true;
        }
    }

    public class ScreenChangePatch : ModulePatch
    {
        // A patch that stops trader modding logic if player goes to inventory, menu or hideout.

        // TODO eventually add a way for player to be able to go to character tab in
        // TaskBarMenu without clearing trader modding.

        protected override MethodBase GetTargetMethod()
        {
            return typeof(MenuTaskBar).GetMethod("OnScreenChanged", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPrefix]
        static void Prefix(EEftScreenType eftScreenType)
        {
            if (eftScreenType == EEftScreenType.Inventory || eftScreenType == EEftScreenType.MainMenu || eftScreenType == EEftScreenType.Hideout)
            {
                TraderModdingUtils.EndTraderModding();
            }
        }
    }

}
