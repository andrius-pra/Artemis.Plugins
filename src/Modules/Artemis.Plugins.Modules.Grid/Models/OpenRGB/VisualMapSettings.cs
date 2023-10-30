namespace Artemis.Plugins.Modules.Grid.Models.OpenRGB
{
    public class VisualMapSettings
    {
        public required VisualMapCustomShape custom_shape { get; set; }
        public required int led_spacing { get; set; }
        public required bool reverse { get; set; }
        public required int shape { get; set; }
        public required int x { get; set; }
        public required int y { get; set; }
    }
}
