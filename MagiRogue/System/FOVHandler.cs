using System;
using System.Linq;
using GoRogue;
using GoRogue.GameFramework;
using GoRogue.MapViews;

namespace MagiRogue.System
{
    /// <summary>
    /// The FOVHandler that handles what sees what and what to do with it, currently needs to be rewritten.
    /// </summary>
    public class FOVHandler
    {
        /// <summary>
        /// Possible states for the FOVVisibilityHandler to be in.
        /// </summary>
        public enum FovState
        {
            /// <summary>
            /// Enabled state -- FOVVisibilityHandler will actively set things as seen/unseen when appropriate.
            /// </summary>
            Enabled,
            /// <summary>
            /// Disabled state.  All items in the map will be set as seen, and the FOVVisibilityHandler
            /// will not set visibility of any items as FOV changes or as items are added/removed.
            /// </summary>
            DisabledResetVisibility,
            /// <summary>
            /// Disabled state.  No changes to the current visibility of terrain/entities will be made, and the FOVVisibilityHandler
            /// will not set visibility of any items as FOV changes or as items are added/removed.
            /// </summary>
            DisabledNoResetVisibility
        }
    }
}