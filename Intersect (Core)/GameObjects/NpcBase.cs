using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Intersect.Enums;
using Intersect.GameObjects.Conditions;
using Intersect.GameObjects.Events;
using Intersect.Models;
using Intersect.Utilities;

using Newtonsoft.Json;

namespace Intersect.GameObjects
{
    public partial class NpcBase : DatabaseObject<NpcBase>, IFolderable
    {
        [NotMapped]
        public ConditionLists AttackOnSightConditions = new ConditionLists();

        [NotMapped]
        public List<Drop> Drops = new List<Drop>();

        [NotMapped]
        public int[] MaxVital = new int[(int)Vital.VitalCount];

        [NotMapped]
        public ConditionLists PlayerCanAttackConditions = new ConditionLists();

        [NotMapped]
        public ConditionLists PlayerFriendConditions = new ConditionLists();

        [NotMapped]
        public int[] Stats = new int[(int)Enums.Stat.StatCount];

        [NotMapped]
        public int[] VitalRegen = new int[(int)Vital.VitalCount];

        [NotMapped]
        public List<SpellEffect> Immunities = new List<SpellEffect>();

        [JsonIgnore]
        [Column("Immunities")]
        public string ImmunitiesJson
        {
            get => JsonConvert.SerializeObject(Immunities);
            set
            {
                Immunities = JsonConvert.DeserializeObject<List<SpellEffect>>(value ?? "") ?? new List<SpellEffect>();
            }
        }

        [JsonConstructor]
        public NpcBase(Guid id) : base(id)
        {
            Name = "New Npc";
        }

        //Parameterless constructor for EF
        public NpcBase()
        {
            Name = "New Npc";
        }

        [Column("AggroList")]
        [JsonIgnore]
        public string JsonAggroList
        {
            get => JsonConvert.SerializeObject(AggroList);
            set => AggroList = JsonConvert.DeserializeObject<List<Guid>>(value);
        }

        [NotMapped]
        public List<Guid> AggroList { get; set; } = new List<Guid>();

        public bool AttackAllies { get; set; }

        [Column("AttackAnimation")]
        public Guid AttackAnimationId { get; set; }

        [NotMapped]
        [JsonIgnore]
        public AnimationBase AttackAnimation
        {
            get => AnimationBase.Get(AttackAnimationId);
            set => AttackAnimationId = value?.Id ?? Guid.Empty;
        }

        //Behavior
        public bool Aggressive { get; set; }

        public byte Movement { get; set; }

        public bool Swarm { get; set; }

        public byte FleeHealthPercentage { get; set; }

        public bool FocusHighestDamageDealer { get; set; } = true;

        public int ResetRadius { get; set; }

        //Conditions
        [Column("PlayerFriendConditions")]
        [JsonIgnore]
        public string PlayerFriendConditionsJson
        {
            get => PlayerFriendConditions.Data();
            set => PlayerFriendConditions.Load(value);
        }

        [Column("AttackOnSightConditions")]
        [JsonIgnore]
        public string AttackOnSightConditionsJson
        {
            get => AttackOnSightConditions.Data();
            set => AttackOnSightConditions.Load(value);
        }

        [Column("PlayerCanAttackConditions")]
        [JsonIgnore]
        public string PlayerCanAttackConditionsJson
        {
            get => PlayerCanAttackConditions.Data();
            set => PlayerCanAttackConditions.Load(value);
        }

        //Combat
        public int Damage { get; set; } = 1;

        public int DamageType { get; set; }

        public int CritChance { get; set; }

        public double CritMultiplier { get; set; } = 1.5;

        public double Tenacity { get; set; } = 0.0;

        public int AttackSpeedModifier { get; set; }

        public int AttackSpeedValue { get; set; }

        //Common Events
        [Column("OnDeathEvent")]
        public Guid OnDeathEventId { get; set; }

        [NotMapped]
        [JsonIgnore]
        public EventBase OnDeathEvent
        {
            get => EventBase.Get(OnDeathEventId);
            set => OnDeathEventId = value?.Id ?? Guid.Empty;
        }

        [Column("OnDeathPartyEvent")]
        public Guid OnDeathPartyEventId { get; set; }

        [NotMapped]
        [JsonIgnore]
        public EventBase OnDeathPartyEvent
        {
            get => EventBase.Get(OnDeathPartyEventId);
            set => OnDeathPartyEventId = value?.Id ?? Guid.Empty;
        }

        //Drops
        [Column("Drops")]
        [JsonIgnore]
        public string JsonDrops
        {
            get => JsonConvert.SerializeObject(Drops);
            set => Drops = JsonConvert.DeserializeObject<List<Drop>>(value);
        }

        /// <summary>
        /// If true this npc will drop individual loot for all of those who helped slay it.
        /// </summary>
        public bool IndividualizedLoot { get; set; }

        public long Experience { get; set; }

        public int Level { get; set; } = 1;

        //Vitals & Stats
        [Column("MaxVital")]
        [JsonIgnore]
        public string JsonMaxVital
        {
            get => DatabaseUtils.SaveIntArray(MaxVital, (int) Vital.VitalCount);
            set => DatabaseUtils.LoadIntArray(ref MaxVital, value, (int) Vital.VitalCount);
        }

        //NPC vs NPC Combat
        public bool NpcVsNpcEnabled { get; set; }

        public int Scaling { get; set; } = 100;

        public int ScalingStat { get; set; }

        public int SightRange { get; set; }

        //Basic Info
        public int SpawnDuration { get; set; }

        public int SpellFrequency { get; set; } = 2;

        //Spells
        [JsonIgnore]
        [Column("Spells")]
        public string CraftsJson
        {
            get => JsonConvert.SerializeObject(Spells, Formatting.None);
            protected set => Spells = JsonConvert.DeserializeObject<DbList<SpellBase>>(value);
        }

        [NotMapped]
        public DbList<SpellBase> Spells { get; set; } = new DbList<SpellBase>();

        public string Sprite { get; set; } = "";

        /// <summary>
        /// The database compatible version of <see cref="Color"/>
        /// </summary>
        [Column("Color")]
        [JsonIgnore]
        public string JsonColor
        {
            get => JsonConvert.SerializeObject(Color);
            set => Color = !string.IsNullOrWhiteSpace(value) ? JsonConvert.DeserializeObject<Color>(value) : Color.White;
        }

        /// <summary>
        /// Defines the ARGB color settings for this Npc.
        /// </summary>
        [NotMapped]
        public Color Color { get; set; } = new Color(255, 255, 255, 255);

        [Column("Stats")]
        [JsonIgnore]
        public string JsonStat
        {
            get => DatabaseUtils.SaveIntArray(Stats, (int) Enums.Stat.StatCount);
            set => DatabaseUtils.LoadIntArray(ref Stats, value, (int) Enums.Stat.StatCount);
        }

        //Vital Regen %
        [JsonIgnore]
        [Column("VitalRegen")]
        public string RegenJson
        {
            get => DatabaseUtils.SaveIntArray(VitalRegen, (int) Vital.VitalCount);
            set => VitalRegen = DatabaseUtils.LoadIntArray(value, (int) Vital.VitalCount);
        }

        /// <inheritdoc />
        public string Folder { get; set; } = "";

        public SpellBase GetRandomSpell(Random random)
        {
            if (Spells == null || Spells.Count == 0)
            {
                return null;
            }

            var spellIndex = random.Next(0, Spells.Count);
            var spellId = Spells[spellIndex];

            return SpellBase.Get(spellId);
        }
    }
}
