using System;
using System.Collections.Generic;
using CombatManager;
using System.Text;

namespace CombatManagerDroid
{
    public static class RollResultHtmlCreator
    {
        public static string CreateHtml(List<object> results)
        {
            StringBuilder blocks = new StringBuilder();
            blocks.CreateHtmlHeader();

           

            
            blocks.AppendOpenTag("p");
            for (int i=0; i <results.Count; i++)
            {
                Object obj = results[results.Count - i - 1];


                if (obj is RollResult)
                {
                    RenderRollResult(blocks, (RollResult)obj);
                }
                else if (obj is CombatState.RequestedRoll)
                {
                    AppendRequestedRoll(blocks, (CombatState.RequestedRoll)obj);
                }


            }
            blocks.AppendCloseTag("p");


            blocks.CreateHtmlFooter();


            return blocks.ToString();

        }

        static void AppendRequestedRoll(StringBuilder blocks, CombatState.RequestedRoll r)
        {
            if (r.Result is RollResult)
            {
                RenderRollResult(blocks, (RollResult)r.Result);
            }
            else if (r.Result is CombatState.AttackSetResult)
            {
                RenderAttackSetResult(blocks, (CombatState.AttackSetResult)r.Result);
            }
            else if (r.Result is CombatState.AttackRollResult)
            {
                RenderAttackRollResult(blocks, (CombatState.AttackRollResult)r.Result);
            }
        }



        private static void RenderRollResult (StringBuilder resHtml, RollResult r, bool breakLine = true, int critRange = 20, string description = null)
        {

            if (description != null)
            {
                resHtml.AppendHtml(description);
                resHtml.AppendSpace();
            }

            bool first = true;
            foreach (DieResult dr in r.Rolls)
            {
                if (!first)
                {
                    if (dr.Result < 0)
                    {
                        resHtml.AppendHtml(" - ");
                    }
                    else
                    {
                        resHtml.AppendHtml(" + ");
                    }

                }

                resHtml.Append (DieHtml(dr.Die));

                string text;
                text = dr.Result.ToString();
                resHtml.AppendSpace();
                if (dr.Die == 20 && dr.Result >= critRange)
                {
                    resHtml.AppendOpenTagWithClass("span", "critical");
                    resHtml.AppendSpace();
                    resHtml.AppendHtml(text);
                    resHtml.AppendSpace();
                    resHtml.AppendCloseTag("span");
                    resHtml.AppendSpace();
                }
                else if (dr.Die == 20 && dr.Result == 1)
                {
                    resHtml.AppendOpenTagWithClass("span", "critfail");
                    resHtml.AppendSpace();
                    resHtml.AppendHtml(text);
                    resHtml.AppendSpace();
                    resHtml.AppendCloseTag("span");
                    resHtml.AppendSpace();
                }
                else
                {                    
                    resHtml.AppendHtml(text);
                }
                first = false;

            }
            if (r.Mod != 0)
            {
                resHtml.AppendHtml(r.Mod.PlusFormat());
            }
            resHtml.AppendHtml(" = ");
            resHtml.AppendOpenTagWithClass("sp", "bolded");
            resHtml.AppendHtml(r.Total.ToString());
            resHtml.AppendCloseTag("sp");
            if ( breakLine)
            {
                resHtml.AppendLineBreak();
            }
        }

        private static string DieHtml(int val)
        {
            string text = "(" + val + ")";
            return text;
        }

        private static void RenderAttackSetResult(StringBuilder resHtml, CombatState.AttackSetResult res)
        {
            if (res.Character != null)
            {
                AppendCharacterHeader(resHtml, res.Character);
            }

            foreach (CombatState.AttackRollResult ar in res.Results)
            {
                RenderAttackRollResult(resHtml, ar);
            }
        }

        private static void RenderAttackRollResult(StringBuilder resHtml, CombatState.AttackRollResult ar)
        {
            if (ar.Character != null)
            {
                AppendCharacterHeader(resHtml, ar.Character);
            }

            resHtml.AppendOpenTagWithClass("span", "weaponheader");
            if (ar.Attack.Weapon == null)
            {
                resHtml.AppendSmallIcon("sword");
            }
            else if (ar.Attack.Weapon.Ranged)
            {
                resHtml.AppendSmallIcon("bow");
            }
            else if (ar.Attack.Weapon.Natural)
            {
                resHtml.AppendSmallIcon("claw");
            }
            else
            {
                resHtml.AppendSmallIcon("sword");
            }

            resHtml.AppendHtml(ar.Name.Capitalize());
            resHtml.AppendLineBreak();
            resHtml.AppendCloseTag("span");

            foreach (CombatState.SingleAttackRoll res in ar.Rolls)
            {
                RenderRollResult(resHtml, res.Result, false, ar.Attack.CritRange);

                resHtml.AppendHtml(" Dmg: ");
                RenderRollResult(resHtml, res.Damage);

                foreach (CombatState.BonusDamage bd in res.BonusDamage)
                {
                    resHtml.Append(" + ");
                    RenderRollResult(resHtml, bd.Damage);
                    resHtml.Append(" " + bd.DamageType);
                }

                if (res.CritResult != null)
                {
                    resHtml.AppendHtml(" Crit: ");
                    RenderRollResult(resHtml, res.CritResult, res.CritResult.Rolls[0].Result == 1);

                    if (res.CritResult.Rolls[0].Result != 1)
                    {                    
                        resHtml.AppendHtml(" Dmg: ");
                    }
                    RenderRollResult(resHtml, res.CritDamage);
                }

            }

        }

        private static void AppendCharacterHeader(StringBuilder resHtml, Character ch)
        {
            resHtml.CreateHeader(ch.Name, null, "h2");
        }

    }
}

