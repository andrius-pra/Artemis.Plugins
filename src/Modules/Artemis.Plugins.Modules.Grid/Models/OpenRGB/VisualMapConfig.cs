using System.Collections.Generic;

namespace Artemis.Plugins.Modules.Grid.Models.OpenRGB
{
    public class VisualMapConfig
    {
        public required IList<VisualMapCtrlZone> ctrl_zones { get; set; }
        public required VisualMapGridSettings grid_settings { get; set; }
    }
}
