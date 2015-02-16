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

using Assemblies.Utilitys;
using LeagueSharp;
using LeagueSharp.Common;

namespace Assemblies.Champions
{
    internal class Champion : ChampionUtils
    {
        public static Menu TargetSelectorMenu;
        public AntiRengar AntiRengar;
        protected Spell E;
        protected Menu Menu;
        //public MenuWrapper menuWrapper;
        protected Orbwalking.Orbwalker Orbwalker;
        public Obj_AI_Hero Player = ObjectManager.Player;
        protected Spell Q;
        protected Spell R;
        protected Spell W;

        public Champion()
        {
            AddBasicMenu();
            if (AntiRengar.IsCompitableChampion())
            {
                AntiRengar = new AntiRengar();
            }
        }

        private void AddBasicMenu()
        {
            //menuWrapper = new MenuWrapper("Assemblies - " + player.ChampionName);
            Menu = new Menu("Assemblies - " + Player.ChampionName, "Assemblies - " + Player.ChampionName, true);

            TargetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(TargetSelectorMenu);
            Menu.AddSubMenu(TargetSelectorMenu);

            //Orbwalker submenu
            var orbwalkingMenu = new Menu("Orbwalker", "orbwalker");
            Orbwalker = new Orbwalking.Orbwalker(orbwalkingMenu);
            Menu.AddSubMenu(orbwalkingMenu);

            Menu.AddToMainMenu();
        }
    }
}