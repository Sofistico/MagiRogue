using MagiRogue.Data.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Entities
{
    public class Profession
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public AbilityName Ability { get; set; }
    }
}