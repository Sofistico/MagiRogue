using MagiRogue.Data.Enumerators;
using MagiRogue.Data.Serialization;
using MagiRogue.Utils;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace MagiRogue.Entities
{
    public sealed class Limb : BodyPart
    {
        [JsonProperty("ConnectedTo")]
        private string? _connectedLimb;

        public LimbType LimbType { get; set; }

        [JsonIgnore]
        public string? ConnectedTo
        {
            get
            {
                return _connectedLimb;
            }

            set
            {
                _connectedLimb = value;
            }
        }

        public bool Broken { get; set; }

        public bool Attached { get; set; } = true;

        [JsonConstructor()]
        public Limb(string materialId) : base(materialId)
        {
        }

        /// <summary>
        /// This class creates a limb for a body.
        /// </summary>
        /// <param name="limbType">The type of the limb, if its a arm or a leg or etc...</param>
        /// <param name="limbName">The name of the limb</param>
        /// <param name="orientation">If it's in the center, left or right of the body</param>
        /// <param name="materialID">The id to define the material, if needeed look at the material definition json\n
        /// Defaults to "skin"</param>
        public Limb(LimbType limbType, string limbName,
            BodyPartOrientation orientation, string connectedTo,
            string materialID = "skin", BodyPartFunction bodyPartFunction = BodyPartFunction.Limb) : base(materialID)
        {
            LimbType = limbType;
            Attached = true;
            BodyPartName = limbName;
            Orientation = orientation;
            ConnectedTo = connectedTo;
            BodyPartFunction = bodyPartFunction;
        }

        public Limb(string id, LimbType limbType,
            string limbName, BodyPartOrientation orientation, string connectedTo,
            string materialID = "skin") : base(materialID)
        {
            Id = id;
            LimbType = limbType;
            //BodyPartWeight = limbWeight;
            // Defaults to true
            Attached = true;
            BodyPartName = limbName;
            Orientation = orientation;
            ConnectedTo = connectedTo;
        }

        public Item ReturnLimbAsItem(Actor actor)
        {
            string limbName = actor.Name + "'s " + BodyPartName;
            int size = Volume;
            //Attached = false;

            return new Item(actor.Appearance.Foreground,
                actor.Appearance.Background,
                limbName,
                253,
                actor.Position,
                size,
                materialId: MaterialId
                );
        }

        public override Limb Copy()
        {
            return new Limb(MaterialId)
            {
                Attached = this.Attached,
                Broken = this.Broken,
                ConnectedTo = this.ConnectedTo,
                Id = this.Id,
                BodyPartName = this.BodyPartName,
                Orientation = this.Orientation,
                LimbType = this.LimbType,
                BodyPartFunction = this.BodyPartFunction,
                RateOfHeal = this.RateOfHeal,
                RelativeVolume = this.RelativeVolume,
                CanHeal = this.CanHeal,
                Tissues = this.Tissues,
                Volume = this.Volume,
                Working = this.Working,
                Wounds = this.Wounds,
                Insides = this.Insides,
            };
        }

        public override void AddWound(Wound wound)
        {
            base.AddWound(wound);
            switch (wound.Severity)
            {
                case InjurySeverity.Broken:
                    Broken = true;
                    break;

                case InjurySeverity.Missing:
                    Attached = false;
                    break;

                case InjurySeverity.Pulped:
                    Broken = true;
                    break;

                default:
                    break;
            }
        }
    }
}