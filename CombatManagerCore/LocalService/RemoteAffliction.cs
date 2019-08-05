using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatManager.LocalService
{
    public class RemoteAffliction
    {

        public String Name{get; set; }
        public String Type{get; set; }
        public String Cause{get; set; }
        public String SaveType{get; set; }
        public int Save{get; set; }
        public RemoteDieRoll Onset{get; set; }
        public String OnsetUnit{get; set; }
        public bool Immediate{get; set; }
        public int Frequency{get; set; }
        public String FrequencyUnit{get; set; }
        public int Limit{get; set; }
        public String LimitUnit{get; set; }
        public String SpecialEffectName{get; set; }
        public RemoteDieRoll SpecialEffectTime {get; set; }
        public String SpecialEffectUnit{get; set; }
        public String OtherEffect{get; set; }
        public bool Once{get; set; }
        public RemoteDieRoll DamageDie {get; set; }
        public String DamageType{get; set; }
        public bool IsDamageDrain{get; set; }
        public RemoteDieRoll SecondaryDamageDie {get; set; }
        public String SecondaryDamageType{get; set; }
        public bool IsSecondaryDamageDrain{get; set; }
        public String DamageExtra{get; set; }
        public String Cure{get; set; }
        public String Details{get; set; }
        public String Cost{get; set; }

    
        
    }
}
