// This file is part of LeagueSharp.Common.
// 
// LeagueSharp.Common is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// LeagueSharp.Common is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with LeagueSharp.Common.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Assemblies.Utilitys
{
    internal class AntiRengar
    {
        private readonly Spell gapcloseSpell;
        private readonly Obj_AI_Hero player = ObjectManager.Player;
        private Menu menu;
        private Obj_AI_Hero rengarObject;

        public AntiRengar()
        {
            gapcloseSpell = GetSpell();

            GameObject.OnCreate += OnCreateObj;
        }

        public void AddToMenu(ref Menu attachMenu)
        {
            menu = attachMenu;
            menu.AddSubMenu(new Menu("Anti Rengar", "antiRengar"));
            menu.SubMenu("antiRengar").AddItem(new MenuItem("enabled", "Enabled").SetValue(true));

            Game.PrintChat("[Assemblies] - AntiRengar Loaded!");
        }

        public bool IsCompitableChampion()
        {
            return player.ChampionName == "Vayne" || player.ChampionName == "Tristana";
        }

        private Spell GetSpell()
        {
            switch (player.ChampionName)
            {
                case "Vayne":
                    return new Spell(SpellSlot.E);
                case "Tristana":
                    return new Spell(SpellSlot.R, 550);
                case "Draven":
                    return new Spell(SpellSlot.E, 1100);
            }
            return null;
        }

        private void GapcloserRengar()
        {
            if (rengarObject.ChampionName == "Rengar")
            {
                if (rengarObject.IsValidTarget(1000) && gapcloseSpell.IsReady() &&
                    rengarObject.Distance(player) <= gapcloseSpell.Range)
                {
                    gapcloseSpell.Cast(rengarObject, true);
                    Utility.DelayAction.Add(50, GapcloserRengar);
                }
            }
        }

        private void OnCreateObj(GameObject obj, EventArgs args)
        {
            if (obj.Name == "Rengar_LeapSound.troy" && obj.IsEnemy)
            {
                foreach (var enemy in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(hero => hero.IsValidTarget(1500) && hero.ChampionName == "Rengar"))
                {
                    rengarObject = enemy;
                }
            }
            if (rengarObject != null && Vector3.DistanceSquared(player.Position, rengarObject.Position) < 1000 * 1000 &&
                menu.Item("enabled").GetValue<bool>())
            {
                GapcloserRengar();
            }
        }
    }
}