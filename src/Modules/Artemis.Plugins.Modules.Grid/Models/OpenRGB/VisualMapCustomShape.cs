using System.Collections.Generic;

namespace Artemis.Plugins.Modules.Grid.Models.OpenRGB
{
    public class VisualMapCustomShape
    {
        public required int h { get; set; }
        public required List<VisualMapLedPosition> led_positions { get; set; }
        public required int w { get; set; }
    }
}
