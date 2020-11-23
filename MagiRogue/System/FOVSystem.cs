using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoRogue;
using GoRogue.MapViews;
using MagiRogue.Entities;
using Microsoft.Xna.Framework;
using SadConsole;

namespace MagiRogue.System
{
    public class FOVSystem
    {
        private readonly Actor actorView;
        private readonly Distance distanceMeasureView;
        private FOV fieldOfView;
        private Map map; // a field to hold information of the map that is being worked

        /// <summary>
        /// The constructor for the FOVSystem
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="distanceMeasure"></param>
        public FOVSystem(Actor actor, Distance distanceMeasure)
        {
            actorView = actor;
            distanceMeasureView = distanceMeasure;
        }

        /// <summary>
        /// Initializes the Field Of View map, that this component should be based on
        /// </summary>
        /// <param name="fieldOfViewMap"></param>
        public void Initialize([NotNull] IMapView<bool> fieldOfViewMap)
        {
            fieldOfView = new FOV(fieldOfViewMap);
        }

        /// <summary>
        /// Resets the field of view component
        /// </summary>
        public void Reset() => fieldOfView = null;

        /// <summary>
        /// Calculates the FieldOfView component
        /// </summary>
        public void Calculate()
        {
            if (fieldOfView == null)
                return;

            fieldOfView.Calculate(actorView.Position, actorView.ViewRadius, distanceMeasureView);
        }

        /// <summary>
        /// Returns true if the position is in the field of view of this actor
        /// If the FieldOfView component is not yet initialized, it will return false
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool PositionInFieldOfView(Point position)
        {
            if (fieldOfView == null) return false;
            return fieldOfView.BooleanFOV[position];
        }

        /// <summary>
        /// Returns all positions that are in the field of view of this actor
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Coord> PositionInFieldOfView()
        {
            if (fieldOfView == null) yield break;

            for (int y = 0; y < fieldOfView.BooleanFOV.Width; y++)
            {
                for (int x = 0; x < fieldOfView.BooleanFOV.Height; x++)
                {
                    yield return new Coord(x, y);
                }
            }
        }
    }
}