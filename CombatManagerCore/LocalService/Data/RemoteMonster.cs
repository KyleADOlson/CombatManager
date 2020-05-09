using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatManager.LocalService.Data
{
    public class RemoteMonster
    {
        public string Name {get; set;}
        public string CR {get; set;}
        public string XP {get; set;}
        public string Race {get; set;}
        public string ClassName {get; set;}
        public string Alignment {get; set;}
        public string Size {get; set;}
        public string Type {get; set;}
        public string SubType {get; set;}
        public int Init {get; set;}
        public int? DualInit {get; set;}
        public string Senses {get; set;}
        public int AC {get; set;}
        public string ACMods {get; set;}
        public int HP {get; set;}
        public string HDText {get; set;}
        public RemoteDieRoll HD {get; set;}
        public int? Fort {get; set;}
        public int? Ref {get; set;}
        public int? Will {get; set;}
        public string SaveMods {get; set;}
        public string Resist {get; set;}
        public string DR {get; set;}
        public string SR {get; set;}
        public string Speed {get; set;}
        public string Melee {get; set;}
        public string Ranged {get; set;}
        public int Space {get; set;}
        public int Reach {get; set;}
        public string SpecialAttacks {get; set;}
        public string SpellLikeAbilities {get; set;}
        public int? Strength {get; set;}
        public int? Dexterity {get; set;}
        public int? Constitution {get; set;}
        public int? Intelligence {get; set;}
        public int? Wisdom {get; set;}
        public int? Charisma {get; set;}
        public int BaseAtk {get; set;}
        public int CMB {get; set;}
        public int CMD {get; set;}
        public String Feats {get; set;}
        public String Skills {get; set;}
        public String RacialMods {get; set;}
        public String Languages {get; set;}
        public String SQ {get; set;}
        public String Environment {get; set;}
        public String Organization {get; set;}
        public String Treasure {get; set;}
        public String DescriptionVisual {get; set;}
        public String Group {get; set;}
        public String Source {get; set;}
        public String IsTemplate {get; set;}
        public String SpecialAbilities {get; set;}
        public String Description {get; set;}
        public String FullText {get; set;}
        public String Gender {get; set;}
        public String Bloodline {get; set;}
        public String ProhibitedSchools {get; set;}
        public String BeforeCombat {get; set;}
        public String DuringCombat {get; set;}
        public String Morale {get; set;}
        public String Gear {get; set;}
        public String OtherGear {get; set;}
        public String Vulnerability {get; set;}
        public String Note {get; set;}
        public String CharacterFlag {get; set;}
        public String CompanionFlag {get; set;}
        public int? FlySpeed {get; set;}
        public int? ClimbSpeed {get; set;}
        public int? BurrowSpeed {get; set;}
        public int? SwimSpeed {get; set;}
        public int LandSpeed {get; set;}
        public String TemplatesApplied {get; set;}
        public String OffenseNote {get; set;}
        public String BaseStatistics {get; set;}
        public String SpellsPrepared {get; set;}
        public String SpellDomains {get; set;}
        public String Aura {get; set;}
        public String DefensiveAbilities {get; set;}
        public String Immune {get; set;}
        public String HPMods {get; set;}
        public String SpellsKnown {get; set;}
        public String Weaknesses {get; set;}
        public String SpeedMod {get; set;}
        public String MonsterSource {get; set;}
        public String ExtractsPrepared {get; set;}
        public String AgeCategory {get; set;}
        public bool DontUseRacialHD {get; set;}
        public String VariantParent {get; set;}
        public bool NPC {get; set;}
        public int? MR {get; set;}
        public String Mythic {get; set;}

        public int TouchAC {get; set;}
        public int FlatFootedAC {get; set;}
        public int NaturalArmor {get; set;}
        public int Shield {get; set;}
        public int Armor {get; set;}
        public int Dodge {get; set;}
        public int Deflection {get; set;}

        public List<RemoteActiveCondition> ActiveConditions { get; set; }


    }
}
