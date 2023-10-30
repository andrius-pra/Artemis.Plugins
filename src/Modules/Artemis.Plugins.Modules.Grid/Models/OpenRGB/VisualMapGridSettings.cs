namespace Artemis.Plugins.Modules.Grid.Models.OpenRGB
{
    public class VisualMapGridSettings
    {
        public required bool auto_load { get; set; }
        public required bool auto_register { get; set; }
        public required int grid_size { get; set; }
        public required int h { get; set; }
        public required bool show_bounds { get; set; }
        public required bool show_grid { get; set; }
        public required bool unregister_members { get; set; }
        public required int w { get; set; }
    }
}
