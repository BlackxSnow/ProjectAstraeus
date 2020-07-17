using Items.Modules;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Items.Parts
{
    public class ItemPart
    {
        [Flags]
        public enum AttachmentTypeFlags
        {
            Input = 1 << 0,
            Output = 1 << 1,
            Primary = 1 << 2,
            Secondary = 1 << 3
        }
        public struct AttachmentPoint
        {
            //Normalized location data relative to part
            public Vector2 Position;
            public AttachmentTypeFlags AttachmentFlags;
            public ItemPart AttachedPart;
        }
        public enum StatTypeEnum
        {
            Additive,
            Multiplicative
        }
        public struct StatData<T>
        {
            //public string StatType_S;

            public bool Enabled;
            //TODO Check if this works
            [JsonConverter(typeof(StringEnumConverter))]
            public StatTypeEnum StatType;
            public T Value;

            //[OnDeserialized]
            //public void OnDeserialised(StreamingContext context)
            //{
            //    StatType = (StatTypeEnum)Enum.Parse(typeof(StatTypeEnum), StatType_S);
            //}
        }

        public class ModularStats : Item.ItemStats
        {
            //Deserialisation data
            public StatData<string> AttackSkill_S;

            //Object data
            public StatData<Weapon.DamageInfo> Damage;
            public StatData<float> Accuracy;
            public StatData<float> AttackSpeed;
            public StatData<ItemTypes.SubTypes> AttackSkill;
            public StatData<float> Block;

            [OnDeserialized]
            public void OnDeserialised(StreamingContext context)
            {
                AttackSkill = new StatData<ItemTypes.SubTypes>
                {
                    Enabled = AttackSkill_S.Enabled,
                    StatType = AttackSkill_S.StatType,
                    Value = (ItemTypes.SubTypes)Enum.Parse(typeof(ItemTypes.SubTypes), AttackSkill_S.Value)
                };
            }
        }
        //Deserialisation data
        public List<string> ValidGroups_S;

        //Object data
        public List<PartModule.ModuleGroups> ValidGroups;
        public AttachmentPoint[] AttachmentPoints;
        public ModularStats PartStats;

        //Runtime Data
        public PartModule[] InstalledModules;

        [OnDeserialized]
        public void OnDeserialised(StreamingContext context)
        {
            ValidGroups = Utility.Collections.DeserializeEnumCollection<PartModule.ModuleGroups>(ValidGroups_S);
            ValidGroups_S.Clear();
        }
    }
}
