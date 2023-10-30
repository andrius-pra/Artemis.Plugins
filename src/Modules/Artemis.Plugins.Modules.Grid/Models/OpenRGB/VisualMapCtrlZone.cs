namespace Artemis.Plugins.Modules.Grid.Models.OpenRGB
{
    public class VisualMapCtrlZone
    {
        public required VisualMapController controller { get; set; }
        public required string custom_zone_name { get; set; }
        public required VisualMapSettings settings { get; set; }
        public required int zone_idx { get; set; }
    }
}
