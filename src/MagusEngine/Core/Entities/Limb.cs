﻿using Arquimedes.Enumerators;
using MagusEngine.Core.Entities.Base;
using Newtonsoft.Json;

namespace MagusEngine.Core.Entities
{
    public sealed class Limb : BodyPart
    {
        [JsonProperty(nameof(ConnectedTo))]
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
        public Limb() : base()
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
            BodyPartFunction bodyPartFunction = BodyPartFunction.Limb) : base()
        {
            LimbType = limbType;
            Attached = true;
            BodyPartName = limbName;
            Orientation = orientation;
            ConnectedTo = connectedTo;
            BodyPartFunction = bodyPartFunction;
        }

        public Limb(string id, LimbType limbType,
            string limbName, BodyPartOrientation orientation, string connectedTo)
        {
            Id = id;
            LimbType = limbType;
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

            return new Item(actor.SadCell.AppearanceSingle.Appearance.Foreground,
                actor.SadCell.AppearanceSingle.Appearance.Background,
                limbName,
                253,
                actor.Position,
                size,
                materialId: Tissues.Find(i => i.Flags.Contains(TissueFlag.Structural)).MaterialId
                );
        }

        public override Limb Copy()
        {
            return new Limb()
            {
                Attached = Attached,
                Broken = Broken,
                ConnectedTo = ConnectedTo,
                Id = Id,
                BodyPartName = BodyPartName,
                Orientation = Orientation,
                LimbType = LimbType,
                BodyPartFunction = BodyPartFunction,
                RelativeVolume = RelativeVolume,
                Tissues = new(Tissues),
                Volume = Volume,
                Working = Working,
                Wounds = new(Wounds),
                Insides = new(Insides),
                Category = Category,
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