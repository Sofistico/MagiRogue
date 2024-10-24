﻿using Arquimedes.Enumerators;
using MagusEngine.Serialization;
using MagusEngine.Systems;
using MagusEngine.Systems.Physics;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MagusEngine.Core.Entities.Base
{
    public sealed class Tissue
    {
        private Material? material;

        public string Id { get; set; }

        /// <summary>
        /// The name of the tissue
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The template of the material, should be get only
        /// </summary>
        public Material Material
        {
            get
            {
                material ??= DataManager.QueryMaterial(MaterialId);
                return material;
            }
        }

        /// <summary>
        /// The id of the material
        /// </summary>
        public string MaterialId { get; set; }

        /// <summary>
        /// The relative thickness of the tissue.A higher thickness is harder to penetrate, but
        /// raising a tissue's relative thickness decreases the thickness of all other tissues.
        /// </summary>
        public int RelativeThickness { get; set; }

        public List<TissueFlag> Flags { get; set; } = new();

        public double Volume { get; set; }
        public double HealingRate { get; set; }
        public double BleedingRate { get; set; }
        public int PainReceptor { get; set; }
        public TissueShape Shape { get; set; } = TissueShape.Layer;

        [JsonConstructor()]
        public Tissue(string name, string materialId, int thickness)
        {
            Name = name;
            MaterialId = materialId;
            RelativeThickness = thickness;
        }
    }
}