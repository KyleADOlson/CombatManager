using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatManager.LocalService
{
    public class RemoteCharacter
    {
        public Guid ID {get; set;}
        public string Name {get; set;}
        public int HP {get; set;}
        public int MaxHP {get; set;}
        public int NonlethalDamage {get; set;}
        public int TemporaryHP {get; set;}
        public String Notes {get; set;}
        public bool IsMonster {get; set;}
        public bool IsReadying {get; set;}
        public bool IsDelaying {get; set;}
        public uint? Color {get; set;}
        public bool IsActive {get; set;}
        public bool IsIdle {get; set;}
        public RemoteInitiativeCount InitiativeCount {get; set;}
        public int InitiativeRolled {get; set;}
        public Guid? InitiativeLeader {get; set;}
        public List<Guid> InitiativeFollowers {get; set;}
        public RemoteMonster Monster {get; set;}


    }
}
