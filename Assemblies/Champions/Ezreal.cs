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
using System.Drawing;
using LeagueSharp;
using LeagueSharp.Common;

namespace Assemblies.Champions
{
    internal class Ezreal : Champion
    {
        public Ezreal()
        {
            if (Player.ChampionName != "Ezreal")
            {
                return;
            }
            LoadMenu();
            LoadSpells();

            Drawing.OnDraw += OnDraw;
            Game.OnGameUpdate += OnUpdate;
            Game.PrintChat("[Assemblies] - Ezreal Loaded.");
        }

        private void LoadSpells()
        {
            Q = new Spell(SpellSlot.Q, 1200);
            Q.SetSkillshot(0.25f, 60f, 2000f, true, SkillshotType.SkillshotLine);

            W = new Spell(SpellSlot.W, 1050);
            W.SetSkillshot(0.25f, 80f, 2000f, false, SkillshotType.SkillshotLine);

            R = new Spell(SpellSlot.R, 3000);
            R.SetSkillshot(1f, 160f, 2000f, false, SkillshotType.SkillshotLine);
        }

        private void LoadMenu()
        {
            Menu.AddSubMenu(new Menu("Combo Options", "combo"));
            Menu.SubMenu("combo").AddItem(new MenuItem("useQC", "Use Q in combo").SetValue(true));
            Menu.SubMenu("combo").AddItem(new MenuItem("useWC", "Use W in combo").SetValue(true));
            Menu.SubMenu("combo").AddItem(new MenuItem("useRC", "Use R in combo").SetValue(true));

            Menu.AddSubMenu(new Menu("Harass Options", "harass"));
            Menu.SubMenu("harass").AddItem(new MenuItem("useQH", "Use Q in harass").SetValue(true));
            Menu.SubMenu("harass").AddItem(new MenuItem("useWH", "Use W in harass").SetValue(false));

            Menu.AddSubMenu(new Menu("Laneclear Options", "laneclear"));
            Menu.SubMenu("laneclear").AddItem(new MenuItem("useQLC", "Use Q in laneclear").SetValue(true));
            Menu.SubMenu("laneclear").AddItem(new MenuItem("AutoQLC", "Auto Q to farm").SetValue(false));
            Menu.SubMenu("laneclear").AddItem(new MenuItem("useQLCH", "Harass while laneclearing").SetValue(false));


            Menu.AddSubMenu(new Menu("Lasthit Options", "lastHit"));
            Menu.SubMenu("lastHit").AddItem(new MenuItem("lastHitq", "Use Q Last Hit").SetValue(false));
            Menu.SubMenu("lastHit").AddItem(new MenuItem("autoLastHit", "Auto Last Hit Q").SetValue(false));

            Menu.AddSubMenu(new Menu("Killsteal Options", "killsteal"));
            Menu.SubMenu("killsteal").AddItem(new MenuItem("useQK", "Use Q for killsteal").SetValue(true));

            Menu.AddSubMenu(new Menu("Hitchance Options", "hitchance"));
            Menu.SubMenu("hitchance")
                .AddItem(
                    new MenuItem("hitchanceSetting", "Hitchance").SetValue(
                        new StringList(new[] { "Low", "Medium", "High", "Very High" })));

            Menu.AddSubMenu(new Menu("Drawing Options", "drawing"));
            Menu.SubMenu("drawing").AddItem(new MenuItem("drawQ", "Draw Q").SetValue(false));
            Menu.SubMenu("drawing").AddItem(new MenuItem("drawW", "Draw W").SetValue(false));
            Menu.SubMenu("drawing").AddItem(new MenuItem("drawR", "Draw R").SetValue(false));

            Menu.AddSubMenu(new Menu("Misc Options", "misc"));
            Menu.SubMenu("misc").AddItem(new MenuItem("usePackets", "Use packet Casting").SetValue(true));
            Menu.SubMenu("misc").AddItem(new MenuItem("useNE", "No R if Closer than range").SetValue(false));
            Menu.SubMenu("misc").AddItem(new MenuItem("NERange", "No R Range").SetValue(new Slider(450, 450, 1400)));
        }

        private void OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            if (Menu.Item("useQK").GetValue<bool>())
            {
                if (Q.IsKillable(TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical)))
                {
                    CastQ();
                }
            }
            LaneClear();
            LastHit();
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    if (Menu.Item("useQC").GetValue<bool>())
                    {
                        CastQ();
                    }
                    if (Menu.Item("useWC").GetValue<bool>())
                    {
                        CastW();
                    }
                    if (Menu.Item("useRC").GetValue<bool>())
                    {
                        var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
                        if (GetUnitsInPath(Player, target, R))
                        {
                            var prediction = R.GetPrediction(target, true);
                            if (target.IsValidTarget(R.Range) && R.IsReady() && prediction.Hitchance >= HitChance.High)
                            {
                                SendSimplePing(target.Position);
                                R.Cast(target, GetPackets(), true);
                            }
                        }
                    }
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    if (Menu.Item("useQH").GetValue<bool>())
                    {
                        CastQ();
                    }
                    if (Menu.Item("useWH").GetValue<bool>())
                    {
                        CastW();
                    }
                    break;
            }
        }

        private void LastHit()
        {
            //TODO - get minions around you
            //Check if minion is killable with Q && isInRange
            //Also check if orbwalking mode == lasthit
            var autoQ = Menu.Item("autoLastHit").GetValue<bool>();
            var lastHitNormal = Menu.Item("lastHitq").GetValue<bool>();

            foreach (var minion in
                MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly))
            {
                if (autoQ && Q.IsReady() && Q.IsKillable(minion))
                {
                    Q.Cast(minion.Position, GetPackets());
                }
                if (lastHitNormal && Q.IsReady() && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit &&
                    Q.IsKillable(minion))
                {
                    Q.Cast(minion.Position, GetPackets());
                }
            }
        }

        private void LaneClear()
        {
            var minionforQ = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
            var useQ = Menu.Item("useQLC").GetValue<bool>();
            var useAutoQ = Menu.Item("AutoQLC").GetValue<bool>();
            var qPosition = Q.GetLineFarmLocation(minionforQ);
            if (useQ && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && Q.IsReady() &&
                qPosition.MinionsHit >= 1)
            {
                Q.Cast(qPosition.Position, GetPackets());
            }
            if (useAutoQ && Q.IsReady() && qPosition.MinionsHit >= 1)
            {
                Q.Cast(qPosition.Position, GetPackets());
            }
            if (Menu.Item("useQLCH").GetValue<bool>() && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                CastQ();
            }
        }

        private HitChance GetHitchance()
        {
            switch (Menu.Item("hitchanceSetting").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
                case 3:
                    return HitChance.VeryHigh;
                default:
                    return HitChance.High;
            }
        }

        private bool GetPackets()
        {
            return Menu.Item("usePackets").GetValue<bool>();
        }

        private void OnDraw(EventArgs args)
        {
            if (Menu.Item("drawQ").GetValue<bool>())
            {
                Utility.DrawCircle(Player.Position, Q.Range, Color.Cyan);
            }
            if (Menu.Item("drawW").GetValue<bool>())
            {
                Utility.DrawCircle(Player.Position, W.Range, Color.Crimson);
            }
            if (Menu.Item("drawR").GetValue<bool>())
            {
                Utility.DrawCircle(Player.Position, R.Range, Color.Purple, 5, 30, true);
            }
        }

        private void CastQ()
        {
            var qTarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (!Q.IsReady() || qTarget == null || Player.Distance(qTarget) > 1200)
            {
                return;
            }

            if (qTarget.IsValidTarget(Q.Range) && qTarget.IsVisible && !qTarget.IsDead &&
                Q.GetPrediction(qTarget).Hitchance >= GetHitchance())
            {
                Q.Cast(qTarget, GetPackets());
            }
        }

        private void CastW()
        {
            var wTarget = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
            if (!W.IsReady() || wTarget == null)
            {
                return;
            }
            if (wTarget.IsValidTarget(W.Range) || W.GetPrediction(wTarget).Hitchance >= GetHitchance())
            {
                W.Cast(wTarget, GetPackets());
            }
        }
    }
}