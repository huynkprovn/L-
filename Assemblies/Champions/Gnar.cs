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
using System.Collections.Generic;
using System.Linq;
using Assemblies.Utilitys;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Assemblies.Champions
{
    internal class Gnar : Champion
    {
        private Spell eMega;
        private Spell qMega;

        public Gnar()
        {
            LoadMenu();
            LoadSpells();

            Game.OnGameUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Game.PrintChat("[Assemblies] - Gnar Loaded.");
        }

        private void LoadSpells()
        {
            Q = new Spell(SpellSlot.Q, 1100f);
            Q.SetSkillshot(0.066f, 60f, 1400f, false, SkillshotType.SkillshotLine);

            qMega = new Spell(SpellSlot.Q, 1100f);
            qMega.SetSkillshot(0.60f, 90f, 2100f, true, SkillshotType.SkillshotLine);

            W = new Spell(SpellSlot.W, 525f);
            W.SetSkillshot(0.25f, 80f, 1200f, false, SkillshotType.SkillshotLine);

            E = new Spell(SpellSlot.E, 475f);
            E.SetSkillshot(0.695f, 150f, 2000f, false, SkillshotType.SkillshotCircle);

            eMega = new Spell(SpellSlot.E, 475f);
            eMega.SetSkillshot(0.695f, 350f, 2000f, false, SkillshotType.SkillshotCircle);

            R = new Spell(SpellSlot.R, 1f);
            R.SetSkillshot(0.066f, 400f, 1400f, false, SkillshotType.SkillshotCircle);
        }

        private void LoadMenu()
        {
            Menu.AddSubMenu(new Menu("Combo Options", "combo"));
            Menu.SubMenu("combo").AddItem(new MenuItem("useQC", "Use _q in combo").SetValue(true));
            Menu.SubMenu("combo").AddItem(new MenuItem("useWC", "Use W in combo").SetValue(true));
            Menu.SubMenu("combo").AddItem(new MenuItem("useEC", "Use E in combo").SetValue(true));
            Menu.SubMenu("combo").AddItem(new MenuItem("useRC", "Use R in combo").SetValue(true));
            Menu.SubMenu("combo").AddItem(new MenuItem("minEnemies", "Enemies for R").SetValue(new Slider(2, 1, 5)));

            Menu.AddSubMenu(new Menu("Harass Options", "harass"));
            Menu.SubMenu("harass").AddItem(new MenuItem("useQH", "Use _q in harass").SetValue(true));
            Menu.SubMenu("harass").AddItem(new MenuItem("useWH", "Use W in harass").SetValue(true));
            Menu.SubMenu("harass").AddItem(new MenuItem("useEH", "Use E in harass").SetValue(false));

            Menu.AddSubMenu(new Menu("Laneclear Options", "laneclear"));
            Menu.SubMenu("laneclear").AddItem(new MenuItem("useQL", "Use _q in laneclear").SetValue(true));
            Menu.SubMenu("laneclear").AddItem(new MenuItem("useEL", "Use E in laneclear").SetValue(false));

            Menu.AddSubMenu(new Menu("Killsteal Options", "killsteal"));
            Menu.SubMenu("killsteal").AddItem(new MenuItem("useQK", "Use _q in killsteal").SetValue(true));
            Menu.SubMenu("killsteal").AddItem(new MenuItem("useWK", "Use W in killsteal").SetValue(true));

            Menu.AddSubMenu(new Menu("Flee Options", "flee"));
            Menu.SubMenu("flee").AddItem(new MenuItem("useEF", "Use E in flee").SetValue(true));

            Menu.AddSubMenu(new Menu("Drawing Options", "drawing"));
            Menu.SubMenu("drawing").AddItem(new MenuItem("drawQ", "Draw _q Range").SetValue(true));
            Menu.SubMenu("drawing").AddItem(new MenuItem("drawE", "Draw E Range").SetValue(true));
            Menu.SubMenu("drawing").AddItem(new MenuItem("drawR", "Draw R Range").SetValue(true));

            Menu.AddSubMenu(new Menu("Misc Options", "misc"));
            Menu.SubMenu("misc").AddItem(new MenuItem("unitHop", "Always bounce off units for flee").SetValue(true));
            Menu.SubMenu("misc")
                .AddItem(
                    new MenuItem("throwPos", "Position to throw enemies").SetValue(
                        new StringList(new[] { "Closest Wall", "Mouse Position", "Closest Turret", "Closest Ally" })));
            Menu.SubMenu("misc").AddItem(new MenuItem("alwaysR", "Always Ult if killable").SetValue(true));
        }

        private void OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

            if (Player.HasBuff("gnartransform"))
            {
                Q = qMega;
                //E = eMega;
                // Game.PrintChat("Big Gnar Mode rek ppl pls");
            }

            DoKillsteal(target);

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    DoCombo(target);
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    DoHarass(target);
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    DoLaneclear();
                    break;
                //case Orbwalker.Mode.Flee:
                //    UnitFlee();
                //    break;
            }
        }

        private void DoCombo(Obj_AI_Hero target)
        {
            //TODO le combo modes
            if (R.IsReady() && target.IsValidTarget(R.Width))
            {
                if (IsMenuEnabled(Menu, "useRC"))
                {
                    CastR(target);
                }
            }
            if (Q.IsReady() && target.IsValidTarget(Q.Range) &&
                Q.GetPrediction(target, true).Hitchance >= HitChance.Medium)
            {
                if (IsMenuEnabled(Menu, "useQC"))
                {
                    Q.Cast(target, true, true);
                }
            }
            if (qMega.IsReady() && target.IsValidTarget(qMega.Range) &&
                qMega.GetPrediction(target).Hitchance >= HitChance.Medium)
            {
                if (IsMenuEnabled(Menu, "useQC"))
                {
                    qMega.Cast(target, true);
                }
            }

            if (W.IsReady() && target.IsValidTarget(W.Range) && Player.Distance(target) < W.Range)
            {
                if (IsMenuEnabled(Menu, "useWC"))
                {
                    W.Cast(target, true);
                }
            }

            if (E.IsReady() && target.IsValidTarget(E.Range))
            {
                if (IsMenuEnabled(Menu, "useEC"))
                {
                    E.Cast(target, true);
                }
            }
        }

        private void DoHarass(Obj_AI_Hero target)
        {
            if (Q.IsReady() && target.IsValidTarget(Q.Range) &&
                Q.GetPrediction(target, true).Hitchance >= HitChance.Medium)
            {
                if (IsMenuEnabled(Menu, "useQH"))
                {
                    Q.Cast(target, true, true);
                }
            }
            if (qMega.IsReady() && target.IsValidTarget(qMega.Range) &&
                qMega.GetPrediction(target).Hitchance >= HitChance.Medium)
            {
                if (IsMenuEnabled(Menu, "useQH"))
                {
                    qMega.Cast(target, true);
                }
            }

            if (W.IsReady() && target.IsValidTarget(W.Range) && Player.Distance(target) < W.Range)
            {
                if (IsMenuEnabled(Menu, "useWH"))
                {
                    W.Cast(target, true);
                }
            }

            if (E.IsReady() && target.IsValidTarget(E.Range))
            {
                if (IsMenuEnabled(Menu, "useEH"))
                {
                    E.Cast(target, true);
                }
            }
        }

        private void DoLaneclear()
        {
            var allMinions = MinionManager.GetMinions(
                Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
            foreach (var minion in allMinions.Where(minion => minion.IsValidTarget(Q.Range)))
            {
                if (Q.IsKillable(minion) && minion.Distance(Player) <= Q.Range && Q.IsReady())
                {
                    if (IsMenuEnabled(Menu, "useQL"))
                    {
                        Q.Cast(minion, true);
                    }
                }
                if (qMega.IsKillable(minion) && minion.Distance(Player) <= qMega.Range && qMega.IsReady())
                {
                    if (IsMenuEnabled(Menu, "useQL"))
                    {
                        qMega.Cast(minion, true);
                    }
                }
            }
        }

        private void DoKillsteal(Obj_AI_Hero target)
        {
            if (Q.IsReady() && IsMenuEnabled(Menu, "useQK"))
            {
                if (Player.Distance(target) <= Q.Range && Q.IsKillable(target))
                {
                    Q.Cast(target, true);
                }
            }
            if (qMega.IsReady() && IsMenuEnabled(Menu, "useQK"))
            {
                if (Player.Distance(target) <= qMega.Range && qMega.IsKillable(target))
                {
                    qMega.Cast(target, true);
                }
            }
            if (W.IsReady() && IsMenuEnabled(Menu, "useWK"))
            {
                if (Player.Distance(target) <= W.Range && W.IsKillable(target))
                {
                    W.Cast(target, true);
                }
            }
        }

        private void CastR(Obj_AI_Hero target)
        {
            if (!R.IsReady())
            {
                return;
            }
            var mode = Menu.Item("throwPos").GetValue<StringList>().SelectedIndex;

            if (R.IsKillable(target) && IsMenuEnabled(Menu, "alwaysR"))
            {
                R.Cast(target, true);
            }

            switch (mode)
            {
                case 0: // wall.
                    foreach (var collisionTarget in
                        ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsValidTarget(R.Width)))
                    {
                        CastRToCollision(collisionTarget);
                    }
                    break;
                case 1:
                    //Mouse position
                    foreach (var collisionTarget in
                        ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsValidTarget(R.Width)))
                    {
                        if (UnitCheck(Game.CursorPos))
                        {
                            R.Cast(Game.CursorPos);
                        }
                    }
                    break;
                case 2:
                    //Closest Turret
                    foreach (var objAiTurret in
                        ObjectManager.Get<Obj_AI_Hero>()
                            .Where(hero => hero.IsValidTarget(R.Width))
                            .Select(
                                collisionTarget =>
                                    ObjectManager.Get<Obj_AI_Turret>()
                                        .First(
                                            tu =>
                                                tu.IsAlly && tu.Distance(collisionTarget) <= 975 + 425 && tu.Health > 0))
                            .Where(objAiTurret => objAiTurret.IsValid && UnitCheck(objAiTurret.Position)))
                    {
                        R.Cast(objAiTurret.Position);
                    }
                    break;
                case 3:
                    //Closest Ally
                    foreach (var collisionTarget in
                        ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsValidTarget(R.Width)))
                    {
                        //975 Turret Range
                        //425 Push distance (Idk if it is correct);
                        var ally =
                            ObjectManager.Get<Obj_AI_Hero>()
                                .First(tu => tu.IsAlly && tu.Distance(collisionTarget) <= 425 + 65 && tu.Health > 0);
                        if (ally.IsValid && UnitCheck(ally.Position))
                        {
                            R.Cast(ally.Position);
                        }
                    }
                    break;
            }
        }

        private bool UnitCheck(Vector3 endPosition)
        {
            var points = GRectangle(Player.Position.To2D(), endPosition.To2D(), R.Width);
            var polygon = new Polygon(points);
            var count =
                ObjectManager.Get<Obj_AI_Hero>()
                    .Where(hero => hero.IsValidTarget(R.Width))
                    .Count(collisionTarget => polygon.Contains(collisionTarget.Position.To2D()));
            if (count < Menu.Item("minEnemies").GetValue<Slider>().Value)
            {
                return false;
            }
            return true;
        }

        private void CastRToCollision(Obj_AI_Hero target)
        {
            var center = Player.Position;
            const int points = 36;
            const int radius = 300;
            const double slice = 2 * Math.PI / points;
            for (var i = 0; i < points; i++)
            {
                var angle = slice * i;
                var newX = (int) (center.X + radius * Math.Cos(angle));
                var newY = (int) (center.Y + radius * Math.Sin(angle));
                var position = new Vector3(newX, newY, 0);
                if (IsWall(position) && UnitCheck(position))
                {
                    R.Cast(position, true);
                }
            }
        }

        private void UnitFlee()
        {
            if (!E.IsReady() && !eMega.IsReady())
            {
                return;
            }

            var minions = MinionManager.GetMinions(
                Player.Position, E.Range, MinionTypes.All, MinionTeam.All, MinionOrderTypes.None);
            Obj_AI_Base bestMinion = null;

            foreach (var jumpableUnit in minions)
            {
                if (jumpableUnit.Distance(Game.CursorPos) <= 300 && Player.Distance(jumpableUnit) <= E.Range)
                {
                    bestMinion = jumpableUnit;
                }
            }

            if (bestMinion != null && bestMinion.IsValid)
            {
                E.Cast(bestMinion, true);
            }
        }

        private void OnDraw(EventArgs args) {}
        //Credits to Andreluis
        private List<Vector2> GRectangle(Vector2 startVector2, Vector2 endVector2, float radius)
        {
            var points = new List<Vector2>();

            var difference = endVector2 - startVector2;
            var to1Side = Vector2.Normalize(difference).Perpendicular() * radius;

            points.Add(startVector2 + to1Side);
            points.Add(startVector2 - to1Side);
            points.Add(endVector2 - to1Side);
            points.Add(endVector2 + to1Side);
            return points;
        }
    }
}