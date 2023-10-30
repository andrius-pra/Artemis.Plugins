using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artemis.Plugins.Modules.Grid.Models.OpenRGB
{
    public class VisualMapController
    {
        public required string location { get; set; }
        public required string name { get; set; }
        public required string serial { get; set; }
        public required string vendor { get; set; }
    }
}
