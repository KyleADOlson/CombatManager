
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Herolab {

   
    [Serializable]
    [XmlRoot("document", IsNullable=false)]
    public class document {
        public Public @public { get; set; }
    }
    
    [Serializable]
    [XmlRoot("public", IsNullable=false)]
    public class Public 
    {
        public Program program { get; set; }
        public localization localization { get; set; }
        [XmlElement("character")]
        public Character[] character { get; set; }
    }
    
    [Serializable]
    [XmlRoot("program", IsNullable=false)]
    public class Program 
    {
        public Programinfo programinfo { get; set; }
        public Version version { get; set; }
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string url { get; set; }
    }
    
    [Serializable]
    [XmlRoot("programinfo", IsNullable=false)]
    public class Programinfo
    {
        [XmlText]
        public string Value { get; set; }
    }
   
    [Serializable]
    [XmlRoot("version", IsNullable=false)]
    public class Version 
    {
        [XmlAttribute]
        public string version { get; set; }
        [XmlAttribute]
        public string build { get; set; }
        [XmlAttribute]
        public string primary { get; set; }
        [XmlAttribute]
        public string secondary { get; set; }
        [XmlAttribute]
        public string tertiary { get; set; }
    }
    
    [Serializable]
    [XmlRoot("localization", IsNullable=false)]
    public class localization 
    {
        [XmlAttribute]
        public string language { get; set; }
        [XmlAttribute]
        public string units { get; set; }
    }
    
    [Serializable]
    [XmlRoot("character", IsNullable=false)]
    public class Character 
    {
        public Character() 
        {
            active = characterActive.no;
            nature = "normal";
            role = Role.pc;
            relationship = Relationship.ally;
            type = "Hero";
        }

        public Bookinfo bookinfo { get; set; }
        public Pathfindersociety pathfindersociety { get; set; }
        public Race race { get; set; }
        public Alignment alignment { get; set; }
        public Templates templates { get; set; }
        public Size size { get; set; }
        public Deity deity { get; set; }
        public CR challengerating { get; set; }
        public XPaward xpaward { get; set; }
        public Classes classes { get; set; }
        public Factions factions { get; set; }
        public Types types { get; set; }
        public Subtypes subtypes { get; set; }
        public Heropoints heropoints { get; set; }
        public Senses senses { get; set; }
        public Auras auras { get; set; }
        public Favoredclasses favoredclasses { get; set; }
        public Health health { get; set; }
        public XP xp { get; set; }
        public Money money { get; set; }
        public Personal personal { get; set; }
        public Languages languages { get; set; }
        public Attributes attributes { get; set; }
        public Saves saves { get; set; }
        public Defensive defensive { get; set; }
        public DR damagereduction { get; set; }
        public Immunities immunities { get; set; }
        public Resistances resistances { get; set; }
        public Weaknesses weaknesses { get; set; }
        public Armorclass armorclass { get; set; }
        public Penalties penalties { get; set; }
        public Maneuvers maneuvers { get; set; }
        public Initiative initiative { get; set; }
        public Movement movement { get; set; }
        public Encumbrance encumbrance { get; set; }
        public Skills skills { get; set; }
        public Skillabilities skillabilities { get; set; }
        public Feats feats { get; set; }
        public Traits traits { get; set; }
        public Flaws flaws { get; set; }
        public Skilltricks skilltricks { get; set; }
        public Animaltricks animaltricks { get; set; }
        public Attack attack { get; set; }
        public Melee melee { get; set; }
        public Ranged ranged { get; set; }
        public Defenses defenses { get; set; }
        public Magicitems magicitems { get; set; }
        public Gear gear { get; set; }
        public Spelllike spelllike { get; set; }
        public Trackedresources trackedresources { get; set; }
        public Otherspecials otherspecials { get; set; }
        public Spellsknown spellsknown { get; set; }
        public spellsprepared spellsmemorized { get; set; }
        public Spellbook spellbook { get; set; }
        public Spellclasses spellclasses { get; set; }
        public Journals journals { get; set; }
        public Images images { get; set; }
        public Validation validation { get; set; }
        public Settings settings { get; set; }
        public NPC npc { get; set; }
        public Minions minions { get; set; }
        [XmlAttribute, DefaultValue(characterActive.no)]
        public characterActive active { get; set; }
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string playername { get; set; }
        [XmlAttribute]
        public string nature { get; set; }
        [XmlAttribute, DefaultValue(Role.pc)]
        public Role role { get; set; }
        [XmlAttribute, DefaultValue(Relationship.ally)]
        public Relationship relationship { get; set; }
        [XmlAttribute, DefaultValue("Hero")]
        public string type { get; set; }
    }
    
    [Serializable]
    [XmlRoot("bookinfo", IsNullable=false)]
    public class Bookinfo 
    {
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string id { get; set; }
    }
    
    [Serializable]
    [XmlRoot("pathfindersociety", IsNullable=false)]
    public class Pathfindersociety 
    {
        [XmlAttribute]
        public string playernum { get; set; }
        [XmlAttribute]
        public string characternum { get; set; }
    }
    
    [Serializable]
    [XmlRoot("race", IsNullable=false)]
    public class Race 
    {
        [XmlAttribute]
        public string racetext { get; set; }
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string ethnicity { get; set; }
    }
    
    [Serializable]
    [XmlRoot("alignment", IsNullable=false)]
    public class Alignment 
    {
        [XmlAttribute]
        public string name { get; set; }
    }
    
    [Serializable]
    [XmlRoot("templates", IsNullable=false)]
    public class Templates 
    {
        [XmlAttribute]
        public string summary { get; set; }
    }
    
    [Serializable]
    [XmlRoot("size", IsNullable=false)]
    public class Size 
    {
        public Space space { get; set; }
        public Reach reach { get; set; }
        [XmlAttribute]
        public string name { get; set; }
    }
    
    [Serializable]
    [XmlRoot("space", IsNullable=false)]
    public class Space 
    {
        [XmlAttribute]
        public string text { get; set; }
        [XmlAttribute]
        public string value { get; set; }
    }
    
    [Serializable]
    [XmlRoot("reach", IsNullable=false)]
    public class Reach 
    {
        [XmlAttribute]
        public string text { get; set; }
        [XmlAttribute]
        public string value { get; set; }
    }
    
    [Serializable]
    [XmlRoot("deity", IsNullable=false)]
    public class Deity 
    {
        [XmlAttribute]
        public string name { get; set; }
    }
    
    [Serializable]
    [XmlRoot("challengerating", IsNullable=false)]
    public class CR 
    {
        [XmlAttribute]
        public string text { get; set; }
        [XmlAttribute]
        public string value { get; set; }
    }
    
    [Serializable]
    [XmlRoot("xpaward", IsNullable=false)]
    public class XPaward 
    {
        [XmlAttribute]
        public string text { get; set; }
        [XmlAttribute]
        public string value { get; set; }
    }
    
    [Serializable]
    [XmlRoot("classes", IsNullable=false)]
    public class Classes 
    {
        [XmlElement("class")]
        public Class[] @class { get; set; }
        [XmlAttribute]
        public string level { get; set; }
        [XmlAttribute]
        public string summary { get; set; }
        [XmlAttribute]
        public string summaryabbr { get; set; }
    }
    
    [Serializable]
    [XmlRoot("class", IsNullable=false)]
    public class Class 
    {
        public Arcanespellfailure arcanespellfailure { get; set; }
        [XmlAttribute]
        public string level { get; set; }
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string spells { get; set; }
        [XmlAttribute]
        public string casterlevel { get; set; }
        [XmlAttribute]
        public string concentrationcheck { get; set; }
        [XmlAttribute]
        public string overcomespellresistance { get; set; }
        [XmlAttribute]
        public string basespelldc { get; set; }
        [XmlAttribute]
        public string castersource { get; set; }
    }
    
    [Serializable]
    [XmlRoot("arcanespellfailure", IsNullable=false)]
    public class Arcanespellfailure 
    {
        [XmlAttribute]
        public string text { get; set; }
        [XmlAttribute]
        public string value { get; set; }
    }
    
    [Serializable]
    [XmlRoot("factions", IsNullable=false)]
    public class Factions 
    {
        [XmlElement("faction")]
        public Faction[] faction { get; set; }
    }
    
    [Serializable]
    [XmlRoot("faction", IsNullable=false)]
    public class Faction 
    {
        public Faction() {retired = FactionRetired.no;}
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string tpa { get; set; }
        [XmlAttribute]
        public string cpa { get; set; }
        [XmlAttribute, DefaultValue(FactionRetired.no)]
        public FactionRetired retired { get; set; }
    }
    
    [Serializable]
    [XmlType(AnonymousType=true)]
    public enum FactionRetired { yes,no,}
   
    [Serializable]
    [XmlRoot("types", IsNullable=false)]
    public class Types 
    {
        [XmlElement("type")]
        public Type[] type { get; set; }
    }
    
    [Serializable]
    [XmlRoot("type", IsNullable=false)]
    public class Type 
    {
        public Type() {active = TypeActive.no;}
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute, DefaultValue(TypeActive.no)]
        public TypeActive active { get; set; }
    }
    
    [Serializable]
    [XmlType(AnonymousType=true)]
    public enum TypeActive {yes,no,}
    
    [Serializable]
    [XmlRoot("subtypes", IsNullable=false)]
    public class Subtypes 
    {
        [XmlElement("subtype")]
        public Subtype[] subtype { get; set; }
    }
    
    [Serializable]
    [XmlRoot("subtype", IsNullable=false)]
    public class Subtype 
    {
        [XmlAttribute]
        public string name { get; set; }
    }
   
    [Serializable]
    [XmlRoot("heropoints", IsNullable=false)]
    public class Heropoints 
    {
        public Heropoints() {enabled = HeropointsEnabled.yes;}
        [XmlAttribute, DefaultValue(HeropointsEnabled.yes)]
        public HeropointsEnabled enabled { get; set; }
        [XmlAttribute]
        public string total { get; set; }
    }
    
    [Serializable]
    [XmlType(AnonymousType=true)]
    public enum HeropointsEnabled {yes,no,}
    
    [Serializable]
    [XmlRoot("senses", IsNullable=false)]
    public class Senses 
    {
        [XmlElement("special")]
        public Special[] special { get; set; }
    }
    
    [Serializable]
    [XmlRoot("special", IsNullable=false)]
    public class Special 
    {
        public Description description { get; set; }
        [XmlElement("specsource")]
        public Specsource[] specsource { get; set; }
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string shortname { get; set; }
        [XmlAttribute]
        public string sourcetext { get; set; }
        [XmlAttribute]
        public string type { get; set; }
    }
    
    [Serializable]
    [XmlRoot("description", IsNullable=false)]
    public class Description 
    {
        [XmlText]
        public string Value { get; set; }
    }
    
    [Serializable]
    [XmlRoot("specsource", IsNullable=false)]
    public class Specsource 
    {
        [XmlText]
        public string Value { get; set; }
    }
    
    [Serializable]
    [XmlRoot("auras", IsNullable=false)]
    public class Auras 
    {
        [XmlElement("special")]
        public Special[] special { get; set; }
    }
    
    [Serializable]
    [XmlRoot("favoredclasses", IsNullable=false)]
    public class Favoredclasses 
    {
        [XmlElement("favoredclass")]
        public Favoredclass[] favoredclass { get; set; }
    }
   
    [Serializable]
    [XmlRoot("favoredclass", IsNullable=false)]
    public class Favoredclass 
    {
        [XmlAttribute]
        public string name { get; set; }
    }
    
    [Serializable]
    [XmlRoot("health", IsNullable=false)]
    public class Health 
    {
        [XmlElement("special")]
        public Special[] special { get; set; }
        [XmlAttribute]
        public string currenthp { get; set; }
        [XmlAttribute]
        public string damage { get; set; }
        [XmlAttribute]
        public string hitdice { get; set; }
        [XmlAttribute]
        public string hitpoints { get; set; }
        [XmlAttribute]
        public string nonlethal { get; set; }
    }
    
    [Serializable]
    [XmlRoot("xp", IsNullable=false)]
    public class XP 
    {
        [XmlAttribute]
        public string total { get; set; }
    }
    
    [Serializable]
    [XmlRoot("money", IsNullable=false)]
    public class Money 
    {
        [XmlAttribute]
        public string cp { get; set; }
        [XmlAttribute]
        public string gp { get; set; }
        [XmlAttribute]
        public string pp { get; set; }
        [XmlAttribute]
        public string sp { get; set; }
        [XmlAttribute]
        public string total { get; set; }
        [XmlAttribute]
        public string valuables { get; set; }
    }
    
    [Serializable]
    [XmlRoot("personal", IsNullable=false)]
    public class Personal 
    {
        public Description description { get; set; }
        public Height charheight { get; set; }
        public Weight charweight { get; set; }
        [XmlAttribute]
        public string age { get; set; }
        [XmlAttribute]
        public string eyes { get; set; }
        [XmlAttribute]
        public Gender gender { get; set; }
        [XmlIgnore]
        public bool genderSpecified { get; set; }
        [XmlAttribute]
        public string hair { get; set; }
        [XmlAttribute]
        public string skin { get; set; }
    }
    
    [Serializable]
    [XmlRoot("charheight", IsNullable=false)]
    public class Height 
    {
        [XmlAttribute]
        public string text { get; set; }
        [XmlAttribute]
        public string value { get; set; }
    }
    
    [Serializable]
    [XmlRoot("charweight", IsNullable=false)]
    public class Weight 
    {
        [XmlAttribute]
        public string text { get; set; }
        [XmlAttribute]
        public string value { get; set; }
    }
    
    [Serializable]
    [XmlType(AnonymousType=true)]
    public enum Gender {Male,Female,}
    
    [Serializable]
    [XmlRoot("languages", IsNullable=false)]
    public class Languages 
    {
        [XmlElement("language")]
        public Language[] language { get; set; }
    }
    
    [Serializable]
    [XmlRoot("language", IsNullable=false)]
    public class Language 
    {
        public Language() {useradded = LanguageUseradded.yes;}
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute, DefaultValue(LanguageUseradded.yes)]
        public LanguageUseradded useradded { get; set; }
    }
    
    [Serializable]
    [XmlType(AnonymousType=true)]
    public enum LanguageUseradded {yes,no,}
    
    [Serializable]
    [XmlRoot("attributes", IsNullable=false)]
    public class Attributes 
    {
        [XmlElement("attribute")]
        public Attribute[] attribute { get; set; }
    }
    
    [Serializable]
    [XmlRoot("attribute", IsNullable=false)]
    public class Attribute 
    {
        public Attrvalue attrvalue { get; set; }
        public Attrbonus attrbonus { get; set; }
        public Situationalmodifiers situationalmodifiers { get; set; }
        [XmlAttribute]
        public string name { get; set; }
    }
    
    [Serializable]
    [XmlRoot("attrvalue", IsNullable=false)]
    public class Attrvalue 
    {
        [XmlAttribute]
        public string text { get; set; }
        [XmlAttribute]
        public string @base { get; set; }
        [XmlAttribute]
        public string modified { get; set; }
    }
    
    [Serializable]
    [XmlRoot("attrbonus", IsNullable=false)]
    public class Attrbonus 
    {
        [XmlAttribute]
        public string text { get; set; }
        [XmlAttribute]
        public string @base { get; set; }
        [XmlAttribute]
        public string modified { get; set; }
    }
    
    [Serializable]
    [XmlRoot("situationalmodifiers", IsNullable=false)]
    public class Situationalmodifiers 
    {
        [XmlElement("situationalmodifier")]
        public Situationalmodifier[] situationalmodifier { get; set; }
        [XmlAttribute]
        public string text { get; set; }
    }
    
    [Serializable]
    [XmlRoot("situationalmodifier", IsNullable=false)]
    public class Situationalmodifier
    {
        [XmlAttribute]
        public string text { get; set; }
        [XmlAttribute]
        public string source { get; set; }
    }
    
    [Serializable]
    [XmlRoot("saves", IsNullable=false)]
    public class Saves 
    {
        [XmlElement("save")]
        public Save[] save { get; set; }
        public Allsaves allsaves { get; set; }
    }
    
    [Serializable]
    [XmlRoot("save", IsNullable=false)]
    public class Save 
    {
        public Situationalmodifiers situationalmodifiers { get; set; }
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string abbr { get; set; }
        [XmlAttribute]
        public string save { get; set; }
        [XmlAttribute]
        public string @base { get; set; }
        [XmlAttribute]
        public string fromattr { get; set; }
        [XmlAttribute]
        public string frommisc { get; set; }
        [XmlAttribute]
        public string fromresist { get; set; }
    }
    
    [Serializable]
    [XmlRoot("allsaves", IsNullable=false)]
    public class Allsaves 
    {
        public Situationalmodifiers situationalmodifiers { get; set; }
        [XmlAttribute]
        public string save { get; set; }
        [XmlAttribute]
        public string @base { get; set; }
        [XmlAttribute]
        public string frommisc { get; set; }
        [XmlAttribute]
        public string fromresist { get; set; }
    }
    
    [Serializable]
    [XmlRoot("defensive", IsNullable=false)]
    public class Defensive 
    {
        [XmlElement("special")]
        public Special[] special { get; set; }
    }
    
    [Serializable]
    [XmlRoot("damagereduction", IsNullable=false)]
    public class DR 
    {
        [XmlElement("special")]
        public Special[] special { get; set; }
    }
    
    [Serializable]
    [XmlRoot("immunities", IsNullable=false)]
    public class Immunities 
    {
        [XmlElement("special")]
        public Special[] special { get; set; }
    }
    
    [Serializable]
    [XmlRoot("resistances", IsNullable=false)]
    public class Resistances 
    {
        [XmlElement("special")]
        public Special[] special { get; set; }
    }
    
    [Serializable]
    [XmlRoot("weaknesses", IsNullable=false)]
    public class Weaknesses 
    {
        [XmlElement("special")]
        public Special[] special { get; set; }
    }
    
    [Serializable]
    [XmlRoot("armorclass", IsNullable=false)]
    public class Armorclass 
    {
        public Situationalmodifiers situationalmodifiers { get; set; }
        [XmlAttribute]
        public string ac { get; set; }
        [XmlAttribute]
        public string flatfooted { get; set; }
        [XmlAttribute]
        public string touch { get; set; }
        [XmlAttribute]
        public string fromarmor { get; set; }
        [XmlAttribute]
        public string fromdeflect { get; set; }
        [XmlAttribute]
        public string fromdexterity { get; set; }
        [XmlAttribute]
        public string fromdodge { get; set; }
        [XmlAttribute]
        public string frommisc { get; set; }
        [XmlAttribute]
        public string fromnatural { get; set; }
        [XmlAttribute]
        public string fromshield { get; set; }
        [XmlAttribute]
        public string fromsize { get; set; }
    }
    
    [Serializable]
    [XmlRoot("penalties", IsNullable=false)]
    public class Penalties 
    {
        [XmlElement("penalty")]
        public Penalty[] penalty { get; set; }
    }
    
    [Serializable]
    [XmlRoot("penalty", IsNullable=false)]
    public class Penalty 
    {
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string text { get; set; }
        [XmlAttribute]
        public string value { get; set; }
    }
    
    [Serializable]
    [XmlRoot("maneuvers", IsNullable=false)]
    public class Maneuvers 
    {
        public Situationalmodifiers situationalmodifiers { get; set; }
        [XmlElement("maneuvertype")]
        public Maneuvertype[] maneuvertype { get; set; }
        [XmlAttribute]
        public string cmb { get; set; }
        [XmlAttribute]
        public string cmd { get; set; }
        [XmlAttribute]
        public string cmdflatfooted { get; set; }
    }
    
    [Serializable]
    [XmlRoot("maneuvertype", IsNullable=false)]
    public class Maneuvertype 
    {
        public Situationalmodifiers situationalmodifiers { get; set; }
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string bonus { get; set; }
        [XmlAttribute]
        public string cmb { get; set; }
        [XmlAttribute]
        public string cmd { get; set; }
    }
    
    [Serializable]
    [XmlRoot("initiative", IsNullable=false)]
    public class Initiative 
    {
        public Situationalmodifiers situationalmodifiers { get; set; }
        [XmlAttribute]
        public string total { get; set; }
        [XmlAttribute]
        public string attrtext { get; set; }
        [XmlAttribute]
        public string misctext { get; set; }
        [XmlAttribute]
        public string attrname { get; set; }
    }
    
    [Serializable]
    [XmlRoot("movement", IsNullable=false)]
    public class Movement 
    {
        public Speed speed { get; set; }
        public Basespeed basespeed { get; set; }
        [XmlElement("special")]
        public Special[] special { get; set; }
    }
    
    [Serializable]
    [XmlRoot("speed", IsNullable=false)]
    public class Speed 
    {
        [XmlAttribute]
        public string text { get; set; }
        [XmlAttribute]
        public string value { get; set; }
    }
    
    [Serializable]
    [XmlRoot("basespeed", IsNullable=false)]
    public class Basespeed 
    {
        [XmlAttribute]
        public string text { get; set; }
        [XmlAttribute]
        public string value { get; set; }
    }
    
    [Serializable]
    [XmlRoot("encumbrance", IsNullable=false)]
    public class Encumbrance 
    {
        [XmlAttribute]
        public string carried { get; set; }
        [XmlAttribute]
        public string encumstr { get; set; }
        [XmlAttribute]
        public string heavy { get; set; }
        [XmlAttribute]
        public string level { get; set; }
        [XmlAttribute]
        public string light { get; set; }
        [XmlAttribute]
        public string medium { get; set; }
    }
    
    [Serializable]
    [XmlRoot("skills", IsNullable=false)]
    public class Skills 
    {
        [XmlElement("skill")]
        public Skill[] skill { get; set; }
    }
    
    [Serializable]
    [XmlRoot("skill", IsNullable=false)]
    public class Skill 
    {
        public Skill() 
        {
            armorcheck = skillArmorcheck.no;
            classskill = Classskill.no;
            trainedonly = Trainedonly.no;
            usable = skillUsable.no;
        }


        public Description description { get; set; }
        public Situationalmodifiers situationalmodifiers { get; set; }
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string value { get; set; }
        [XmlAttribute]
        public string ranks { get; set; }
        [XmlAttribute]
        public string attrbonus { get; set; }
        [XmlAttribute]
        public string attrname { get; set; }
        [XmlAttribute]
        public SkillTools tools { get; set; }
        [XmlIgnore]
        public bool toolsSpecified { get; set; }
        [XmlAttribute, DefaultValue(skillArmorcheck.no)]
        public skillArmorcheck armorcheck { get; set; }
        [XmlAttribute, DefaultValue(Classskill.no)]
        public Classskill classskill { get; set; }
        [XmlAttribute, DefaultValue(Trainedonly.no)]
        public Trainedonly trainedonly { get; set; }
        [XmlAttribute, DefaultValue(skillUsable.no)]
        public skillUsable usable { get; set; }
    }
    
    [Serializable]
    [XmlType(AnonymousType=true)]
    public enum SkillTools {uses,needs,}
    
    [Serializable]
    [XmlType(AnonymousType=true)]
    public enum skillArmorcheck {yes,no,}
    
    [Serializable]
    [XmlType(AnonymousType=true)]
    public enum Classskill {yes,no,}
    
    [Serializable]
    [XmlType(AnonymousType=true)]
    public enum Trainedonly {yes,no,}
    
    [Serializable]
    [XmlType(AnonymousType=true)]
    public enum skillUsable {yes,no,}
    
    [Serializable]
    [XmlRoot("skillabilities", IsNullable=false)]
    public class Skillabilities 
    {
        [XmlElement("special")]
        public Special[] special { get; set; }
    }
    
    [Serializable]
    [XmlRoot("feats", IsNullable=false)]
    public class Feats 
    {
        [XmlElement("feat")]
        public Feat[] feat { get; set; }
    }
    
    [Serializable]
    [XmlRoot("feat", IsNullable=false)]
    public class Feat 
    {
        public Feat() 
        {
            profgroup = FeatProfgroup.no;
            useradded = FeatUseradded.yes;
        }

        public Description description { get; set; }
        [XmlElement("featcategory")]
        public Featcategory[] featcategory { get; set; }
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string categorytext { get; set; }
        [XmlAttribute, DefaultValue(FeatProfgroup.no)]
        public FeatProfgroup profgroup { get; set; }
        [XmlAttribute, DefaultValue(FeatUseradded.yes)]
        public FeatUseradded useradded { get; set; }
    }
    
    [Serializable]
    [XmlRoot("featcategory", IsNullable=false)]
    public class Featcategory 
    {
        [XmlText]
        public string Value { get; set; }
    }
    
    [Serializable]
    [XmlType(AnonymousType=true)]
    public enum FeatProfgroup {yes,no,}
    
    [Serializable]
    [XmlType(AnonymousType=true)]
    public enum FeatUseradded {yes,no,}
    
    [Serializable]
    [XmlRoot("traits", IsNullable=false)]
    public class Traits 
    {
        [XmlElement("trait")]
        public Trait[] trait { get; set; }
    }
    
    [Serializable]
    [XmlRoot("trait", IsNullable=false)]
    public class Trait 
    {
        public Trait() {useradded = TraitUseradded.yes;}
        public Description description { get; set; }
        [XmlElement("traitcategory")]
        public Traitcategory[] traitcategory { get; set; }
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string categorytext { get; set; }
        [XmlAttribute, DefaultValue(TraitUseradded.yes)]
        public TraitUseradded useradded { get; set; }
    }
    
    [Serializable]
    [XmlRoot("traitcategory", IsNullable=false)]
    public class Traitcategory 
    {
        [XmlText]
        public string Value { get; set; }
    }
    
    [Serializable]
    [XmlType(AnonymousType=true)]
    public enum TraitUseradded { yes,no,}
    
    [Serializable]
    [XmlRoot("flaws", IsNullable=false)]
    public class Flaws 
    {
        [XmlElement("flaw")]
        public Flaw[] flaw { get; set; }
    }
    
    [Serializable]
    [XmlRoot("flaw", IsNullable=false)]
    public class Flaw 
    {
        public Flaw() {useradded = FlawUseradded.yes;}
        public Description description { get; set; }
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute, DefaultValue(FlawUseradded.yes)]
        public FlawUseradded useradded { get; set; }
    }
    
    [Serializable]
    [XmlType(AnonymousType=true)]
    public enum FlawUseradded {yes,no,}
    
    [Serializable]
    [XmlRoot("skilltricks", IsNullable=false)]
    public class Skilltricks 
    {
        [XmlElement("skilltrick")]
        public Skilltrick[] skilltrick { get; set; }
    }
    
    [Serializable]
    [XmlRoot("skilltrick", IsNullable=false)]
    public class Skilltrick 
    {
        public Skilltrick() {useradded = SkilltrickUseradded.yes;}
        public Description description { get; set; }
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute, DefaultValue(SkilltrickUseradded.yes)]
        public SkilltrickUseradded useradded { get; set; }
    }
    
    [Serializable]
    [XmlType(AnonymousType=true)]
    public enum SkilltrickUseradded {yes,no,}
    
    [Serializable]
    [XmlRoot("animaltricks", IsNullable=false)]
    public class Animaltricks 
    {
        [XmlElement("animaltrick")]
        public Animaltrick[] animaltrick { get; set; }
    }
    
    [Serializable]
    [XmlRoot("animaltrick", IsNullable=false)]
    public class Animaltrick 
    {
        public Animaltrick() {useradded = AnimaltrickUseradded.yes;}
        public Description description { get; set; }
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute, DefaultValue(AnimaltrickUseradded.yes)]
        public AnimaltrickUseradded useradded { get; set; }
    }
    
    [Serializable]
    [XmlType(AnonymousType=true)]
    public enum AnimaltrickUseradded {yes,no,}
    
    [Serializable]
    [XmlRoot("attack", IsNullable=false)]
    public class Attack 
    {
        [XmlElement("special")]
        public Special[] special { get; set; }
        [XmlAttribute]
        public string attackbonus { get; set; }
        [XmlAttribute]
        public string baseattack { get; set; }
        [XmlAttribute]
        public string meleeattack { get; set; }
        [XmlAttribute]
        public string rangedattack { get; set; }
    }
    
    [Serializable]
    [XmlRoot("melee", IsNullable=false)]
    public class Melee 
    {
        [XmlElement("weapon")]
        public Weapon[] weapon { get; set; }
    }
    
    [Serializable]
    [XmlRoot("weapon", IsNullable=false)]
    public class Weapon 
    {
        public Weapon() 
        {
            equipped = WeaponEquipped.no;
            useradded = WeaponUseradded.yes;
            quantity = "1";
        }

        public Rangedattack rangedattack { get; set; }
        public Weaponweight weight { get; set; }
        public Cost cost { get; set; }
        public Description description { get; set; }
        public Itemslot itemslot { get; set; }
        [XmlElement("itempower")]
        public Itempower[] itempower { get; set; }
        [XmlElement("wepcategory")]
        public Weaponcategory[] wepcategory { get; set; }
        [XmlElement("weptype")]
        public Weapontype[] weptype { get; set; }
        public Situationalmodifiers situationalmodifiers { get; set; }
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string attack { get; set; }
        [XmlAttribute]
        public string damage { get; set; }
        [XmlAttribute]
        public string crit { get; set; }
        [XmlAttribute]
        public string categorytext { get; set; }
        [XmlAttribute]
        public string typetext { get; set; }
        [XmlAttribute]
        public string size { get; set; }
        [XmlAttribute]
        public string flurryattack { get; set; }
        [XmlAttribute, DefaultValue(WeaponEquipped.no)]
        public WeaponEquipped equipped { get; set; }
        [XmlAttribute, DefaultValue(WeaponUseradded.yes)]
        public WeaponUseradded useradded { get; set; }
        [XmlAttribute, DefaultValue("1")]
        public string quantity { get; set; }
    }
    
    [Serializable]
    [XmlRoot("rangedattack", IsNullable=false)]
    public class Rangedattack 
    {
        [XmlAttribute]
        public string attack { get; set; }
        [XmlAttribute]
        public string flurryattack { get; set; }
        [XmlAttribute]
        public string rangeinctext { get; set; }
        [XmlAttribute]
        public string rangeincvalue { get; set; }
    }
    
    [Serializable]
    [XmlRoot("weight", IsNullable=false)]
    public class Weaponweight 
    {
        [XmlAttribute]
        public string text { get; set; }
        [XmlAttribute]
        public string value { get; set; }
    }
    
    [Serializable]
    [XmlRoot("cost", IsNullable=false)]
    public class Cost 
    {
        [XmlAttribute]
        public string text { get; set; }
        [XmlAttribute]
        public string value { get; set; }
    }
    
    [Serializable]
    [XmlRoot("itemslot", IsNullable=false)]
    public class Itemslot 
    {
        [XmlText]
        public string Value { get; set; }
    }
    
    [Serializable]
    [XmlRoot("itempower", IsNullable=false)]
    public class Itempower 
    {
        public Description description { get; set; }
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string pricebonusvalue { get; set; }
        [XmlAttribute]
        public string pricebonustext { get; set; }
        [XmlAttribute]
        public string pricecashvalue { get; set; }
        [XmlAttribute]
        public string pricecashtext { get; set; }
    }
    
    [Serializable]
    [XmlRoot("wepcategory", IsNullable=false)]
    public class Weaponcategory 
    {
        [XmlText]
        public string Value { get; set; }
    }
    
    [Serializable]
    [XmlRoot("weptype", IsNullable=false)]
    public class Weapontype 
    {
        [XmlText]
        public string Value { get; set; }
    }
    
    [Serializable]
    [XmlType(AnonymousType=true)]
    public enum WeaponEquipped {@double,bothhands,mainhand,offhand,no,}
    
    [Serializable]
    [XmlType(AnonymousType=true)]
    public enum WeaponUseradded {yes,no,}
    
    [Serializable]
    [XmlRoot("ranged", IsNullable=false)]
    public class Ranged 
    {
        [XmlElement("weapon")]
        public Weapon[] weapon { get; set; }
    }
    
    [Serializable]
    [XmlRoot("defenses", IsNullable=false)]
    public class Defenses 
    {
        [XmlElement("armor")]
        public Armor[] armor { get; set; }
    }
    
    [Serializable]
    [XmlRoot("armor", IsNullable=false)]
    public class Armor 
    {
        public Armor() 
        {
            equipped = ArmorEquipped.no;
            natural = Naturalarmor.no;
            useradded = ArmorUseradded.yes;
            quantity = "1";
        }

        public Weaponweight weight { get; set; }
        public Cost cost { get; set; }
        public Description description { get; set; }
        public Itemslot itemslot { get; set; }
        [XmlElement("itempower")]
        public Itempower[] itempower { get; set; }
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string ac { get; set; }
        [XmlAttribute, DefaultValue(ArmorEquipped.no)]
        public ArmorEquipped equipped { get; set; }
        [XmlAttribute, DefaultValue(Naturalarmor.no)]
        public Naturalarmor natural { get; set; }
        [XmlAttribute, DefaultValue(ArmorUseradded.yes)]
        public ArmorUseradded useradded { get; set; }
        [XmlAttribute, DefaultValue("1")]
        public string quantity { get; set; }
    }
    
    [Serializable]
    [XmlType(AnonymousType=true)]
    public enum ArmorEquipped {yes,no,}
    
    [Serializable]
    [XmlType(AnonymousType=true)]
    public enum Naturalarmor {yes,no,}
    
    [Serializable]
    [XmlType(AnonymousType=true)]
    public enum ArmorUseradded {yes,no,}
    
    [Serializable]
    [XmlRoot("magicitems", IsNullable=false)]
    public class Magicitems 
    {
        [XmlElement("item")]
        public Item[] item { get; set; }
    }
    
    [Serializable]
    [XmlRoot("item", IsNullable=false)]
    public class Item 
    {
        public Item() 
        {
            useradded = ItemUseradded.yes;
            quantity = "1";
        }

        public Weaponweight weight { get; set; }
        public Cost cost { get; set; }
        public Description description { get; set; }
        public Itemslot itemslot { get; set; }
        [XmlElement("itempower")]
        public Itempower[] itempower { get; set; }
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute, DefaultValue(ItemUseradded.yes)]
        public ItemUseradded useradded { get; set; }
        [XmlAttribute, DefaultValue("1")]
        public string quantity { get; set; }
    }
    
    [Serializable]
    [XmlType(AnonymousType=true)]
    public enum ItemUseradded {yes,no,}
    
    [Serializable]
    [XmlRoot("gear", IsNullable=false)]
    public class Gear 
    {
        [XmlElement("item")]
        public Item[] item { get; set; }
    }
    
    [Serializable]
    [XmlRoot("spelllike", IsNullable=false)]
    public class Spelllike 
    {
        [XmlElement("special")]
        public Special[] special { get; set; }
    }
    
    [Serializable]
    [XmlRoot("trackedresources", IsNullable=false)]
    public class Trackedresources 
    {
        [XmlElement("trackedresource")]
        public Trackedresource[] trackedresource { get; set; }
    }
    
    [Serializable]
    [XmlRoot("trackedresource", IsNullable=false)]
    public class Trackedresource 
    {
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string text { get; set; }
        [XmlAttribute]
        public string used { get; set; }
        [XmlAttribute]
        public string left { get; set; }
        [XmlAttribute]
        public string min { get; set; }
        [XmlAttribute]
        public string max { get; set; }
    }
    
    [Serializable]
    [XmlRoot("otherspecials", IsNullable=false)]
    public class Otherspecials 
    {
        [XmlElement("special")]
        public Special[] special { get; set; }
    }
    
    [Serializable]
    [XmlRoot("spellsknown", IsNullable=false)]
    public class Spellsknown 
    {
        [XmlElement("spell")]
        public Spell[] spell { get; set; }
    }
    
    [Serializable]
    [XmlRoot("spell", IsNullable=false)]
    public class Spell 
    {
        public Spell() 
        {
            useradded = SpellUseradded.yes;
            spontaneous = spellSpontaneous.no;
            unlimited = SpellUnlimited.no;
        }

        public Description description { get; set; }
        [XmlElement("spellcomp")]
        public Spellcomponent[] spellcomp { get; set; }
        [XmlElement("spellschool")]
        public Spellschool[] spellschool { get; set; }
        [XmlElement("spellsubschool")]
        public Spellsubschool[] spellsubschool { get; set; }
        [XmlElement("spelldescript")]
        public Spelldescription[] spelldescript { get; set; }
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string level { get; set; }
        [XmlAttribute]
        public string area { get; set; }
        [XmlAttribute]
        public string casterlevel { get; set; }
        [XmlAttribute]
        public string castsleft { get; set; }
        [XmlAttribute]
        public string casttime { get; set; }
        [XmlAttribute]
        public string @class { get; set; }
        [XmlAttribute]
        public string componenttext { get; set; }
        [XmlAttribute]
        public string dc { get; set; }
        [XmlAttribute]
        public string descriptor { get; set; }
        [XmlAttribute]
        public string duration { get; set; }
        [XmlAttribute]
        public string effect { get; set; }
        [XmlAttribute]
        public string range { get; set; }
        [XmlAttribute]
        public string resist { get; set; }
        [XmlAttribute]
        public string save { get; set; }
        [XmlAttribute]
        public string schooltext { get; set; }
        [XmlAttribute]
        public string subschooltext { get; set; }
        [XmlAttribute]
        public string descriptortext { get; set; }
        [XmlAttribute]
        public string savetext { get; set; }
        [XmlAttribute]
        public string resisttext { get; set; }
        [XmlAttribute]
        public string target { get; set; }
        [XmlAttribute, DefaultValue(SpellUseradded.yes)]
        public SpellUseradded useradded { get; set; }
        [XmlAttribute, DefaultValue(spellSpontaneous.no)]
        public spellSpontaneous spontaneous { get; set; }
        [XmlAttribute, DefaultValue(SpellUnlimited.no)]
        public SpellUnlimited unlimited { get; set; }
    }
    
    [Serializable]
    [XmlRoot("spellcomp", IsNullable=false)]
    public class Spellcomponent 
    {
        [XmlText]
        public string Value { get; set; }
    }
    
    [Serializable]
    [XmlRoot("spellschool", IsNullable=false)]
    public class Spellschool 
    {
        [XmlText]
        public string Value { get; set; }
    }
    
    [Serializable]
    [XmlRoot("spellsubschool", IsNullable=false)]
    public class Spellsubschool 
    {
        [XmlText]
        public string Value { get; set; }
    }
    
    [Serializable]
    [XmlRoot("spelldescript", IsNullable=false)]
    public class Spelldescription 
    {
        [XmlText]
        public string Value { get; set; }
    }
    
    [Serializable]
    [XmlType(AnonymousType=true)]
    public enum SpellUseradded {yes,no,}
    
    [Serializable]
    [XmlType(AnonymousType=true)]
    public enum spellSpontaneous {yes,no,}
    
    [Serializable]
    [XmlType(AnonymousType=true)]
    public enum SpellUnlimited {yes,no,}
    
    [Serializable]
    [XmlRoot("spellsmemorized", IsNullable=false)]
    public class spellsprepared 
    {
        [XmlElement("spell")]
        public Spell[] spell { get; set; }
    }
    
    [Serializable]
    [XmlRoot("spellbook", IsNullable=false)]
    public class Spellbook 
    {
        [XmlElement("spell")]
        public Spell[] spell { get; set; }
    }
    
    [Serializable]
    [XmlRoot("spellclasses", IsNullable=false)]
    public class Spellclasses 
    {
        [XmlElement("spellclass")]
        public Spellclass[] spellclass { get; set; }
    }
    
    [Serializable]
    [XmlRoot("spellclass", IsNullable=false)]
    public class Spellclass 
    {
        [XmlElement("spelllevel")]
        public Spelllevel[] spelllevel { get; set; }
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string maxspelllevel { get; set; }
        [XmlAttribute]
        public string spells { get; set; }
    }
    
    [Serializable]
    [XmlRoot("spelllevel", IsNullable=false)]
    public class Spelllevel 
    {
        public Spelllevel() 
        {
            maxcasts = "0";
            used = "0";
            unlimited = SpelllevelUnlimited.no;
        }

        [XmlAttribute]
        public string level { get; set; }
        [XmlAttribute, DefaultValue("0")]
        public string maxcasts { get; set; }
        [XmlAttribute, DefaultValue("0")]
        public string used { get; set; }
        [XmlAttribute, DefaultValue(SpelllevelUnlimited.no)]
        public SpelllevelUnlimited unlimited { get; set; }
    }
    
    [Serializable]
    [XmlType(AnonymousType=true)]
    public enum SpelllevelUnlimited {yes,no,}
    
    [Serializable]
    [XmlRoot("journals", IsNullable=false)]
    public class Journals 
    {
        [XmlElement("journal")]
        public Journal[] journal { get; set; }
    }
    
    [Serializable]
    [XmlRoot("journal", IsNullable=false)]
    public class Journal 
    {
        public Description description { get; set; }
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string cp { get; set; }
        [XmlAttribute]
        public string gamedate { get; set; }
        [XmlAttribute]
        public string gp { get; set; }
        [XmlAttribute]
        public string pp { get; set; }
        [XmlAttribute]
        public string prestigeaward { get; set; }
        [XmlAttribute]
        public string prestigespend { get; set; }
        [XmlAttribute]
        public string realdate { get; set; }
        [XmlAttribute]
        public string sp { get; set; }
        [XmlAttribute]
        public string xp { get; set; }
    }
    
    [Serializable]
    [XmlRoot("images", IsNullable=false)]
    public class Images 
    {
        [XmlElement("image")]
        public Image[] image { get; set; }
    }
    
    [Serializable]
    [XmlRoot("image", IsNullable=false)]
    public class Image 
    {
        [XmlAttribute]
        public string filename { get; set; }
    }
    
    [Serializable]
    [XmlRoot("validation", IsNullable=false)]
    public class Validation 
    {
        public Report report { get; set; }
        [XmlElement("validmessage")]
        public Validatemessage[] validmessage { get; set; }
    }
    
    [Serializable]
    [XmlRoot("report", IsNullable=false)]
    public class Report 
    {
        [XmlText]
        public string Value { get; set; }
    }
    
    [Serializable]
    [XmlRoot("validmessage", IsNullable=false)]
    public class Validatemessage 
    {
        [XmlText]
        public string Value { get; set; }
    }
    
    [Serializable]
    [XmlRoot("settings", IsNullable=false)]
    public class Settings 
    {
        [XmlAttribute]
        public string summary { get; set; }
    }
    
    [Serializable]
    [XmlRoot("npc", IsNullable=false)]
    public class NPC 
    {
        public Description description { get; set; }
        public Basics basics { get; set; }
        public Tactics tactics { get; set; }
        public Ecology ecology { get; set; }
        public Additional additional { get; set; }
    }
    
    [Serializable]
    [XmlRoot("basics", IsNullable=false)]
    public class Basics 
    {
        [XmlElement("npcinfo")]
        public NPCinfo[] npcinfo { get; set; }
    }
    
    [Serializable]
    [XmlRoot("npcinfo", IsNullable=false)]
    public class NPCinfo 
    {
        [XmlAttribute]
        public string name { get; set; }
        [XmlText]
        public string[] Text { get; set; }
    }
    
    [Serializable]
    [XmlRoot("tactics", IsNullable=false)]
    public class Tactics 
    {
        [XmlElement("npcinfo")]
        public NPCinfo[] npcinfo { get; set; }
    }
    
    [Serializable]
    [XmlRoot("ecology", IsNullable=false)]
    public class Ecology 
    {
        [XmlElement("npcinfo")]
        public NPCinfo[] npcinfo { get; set; }
    }
    
    [Serializable]
    [XmlRoot("additional", IsNullable=false)]
    public class Additional 
    {
        [XmlElement("npcinfo")]
        public NPCinfo[] npcinfo { get; set; }
    }
    
    [Serializable]
    [XmlRoot("minions", IsNullable=false)]
    public class Minions 
    {
        [XmlElement("character")]
        public Character[] character { get; set; }
    }
    
    [Serializable]
    [XmlType(AnonymousType=true)]
    public enum characterActive {yes,no,}
    
    [Serializable]
    [XmlType(AnonymousType=true)]
    public enum Role {pc,npc,}
    
    [Serializable]
    [XmlType(AnonymousType=true)]
    public enum Relationship {ally,enemy,}
}
