using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Concurrency;
using System.Text.Json;
using System.Threading.Tasks;
using Artemis.Core;
using Artemis.Core.Services;
using Artemis.Plugins.Modules.Grid.Models;
using Artemis.Plugins.Modules.Grid.Models.OpenRGB;
using Artemis.UI.Shared.Services;
using Serilog;

namespace Artemis.Plugins.Modules.Grid.Services
{
    public interface IOpenRgbVisualMapService : IPluginService, IDisposable
    {
        Task ExportSettings();
    }

    internal class OpenRgbVisualMapService : IOpenRgbVisualMapService
    {
        private readonly PluginSettings _settings;
        private readonly IDeviceService _deviceService;
        private readonly IWindowService _windowService;

        public OpenRgbVisualMapService(PluginSettings settings, IDeviceService deviceService, IWindowService windowService)
        {
            _settings = settings;
            _deviceService = deviceService;
            _windowService = windowService;
        }

        public async Task ExportSettings()
        {
            string? result = await _windowService.CreateSaveFileDialog()
                .HavingFilter(f => f.WithExtension("json").WithName("JSON"))
                .WithDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @$"OpenRGB\plugins\settings\virtual-controllers"))
                .WithInitialFileName("rgb-master.json")
                .ShowAsync();

            if (result != null)
            {
                VisualMapConfig settings = new VisualMapConfig
                {
                    ctrl_zones = GetControllerZones(),
                    grid_settings = GetGridSettings()
                };

                string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNameCaseInsensitive = false,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                json = json.Replace("\\u0026", "&");
                await File.WriteAllTextAsync(result, json);
            }

        }

        private IList<VisualMapCtrlZone> GetControllerZones()
        {
            List<VisualMapCtrlZone> zones = new List<VisualMapCtrlZone>();

            foreach (ArtemisDevice device in _deviceService.Devices)
            {
                if (TryGetControllerZone(device, out VisualMapCtrlZone zone))
                {
                    zones.Add(zone);
                }
            }

            return zones;
        }

        private bool TryGetControllerZone(ArtemisDevice device, out VisualMapCtrlZone? zone)
        {
            string type = device.RgbDevice.DeviceInfo.GetType().FullName;

            if (type != "RGB.NET.Devices.OpenRGB.OpenRGBDeviceInfo" || !device.Identifier.StartsWith("HyperX Alloy Origins 65 (HP)"))
            {
                zone = null;
                return false;
            }
            zone = new VisualMapCtrlZone
            {
                controller = GetController(device),
                custom_zone_name = "",
                zone_idx = 0,
                settings = GetVisualMapSettings(device)
            };

            return true;
        }

        private VisualMapSettings GetVisualMapSettings(ArtemisDevice device)
        {
            return new VisualMapSettings
            {
                x = (int)Math.Truncate(device.X * 15 / device.Rectangle.Width),
                y = (int)Math.Truncate(device.Y * 5 / device.Rectangle.Height),
                shape = 2,
                reverse = false,
                led_spacing = 1,
                custom_shape = new VisualMapCustomShape
                {
                    h = 5,
                    w = 15,
                    led_positions = new List<VisualMapLedPosition>
                    {
                        new() {
                            led_num = 1,
                            x = 0,
                            y = 0
                        },
                        new() {
                            led_num = 2,
                            x = 1,
                            y = 0
                        },
                        new() {
                            led_num = 3,
                            x = 2,
                            y = 0
                        },
                        new() {
                            led_num = 4,
                            x = 3,
                            y = 0
                        },
                        new() {
                            led_num = 5,
                            x = 4,
                            y = 0
                        },
                        new() {
                            led_num = 6,
                            x = 5,
                            y = 0
                        },
                        new() {
                            led_num = 7,
                            x = 6,
                            y = 0
                        },
                        new() {
                            led_num = 8,
                            x = 7,
                            y = 0
                        },
                        new() {
                            led_num = 9,
                            x = 8,
                            y = 0
                        },
                        new() {
                            led_num = 10,
                            x = 9,
                            y = 0
                        },
                        new() {
                            led_num = 11,
                            x = 10,
                            y = 0
                        },
                        new() {
                            led_num = 12,
                            x = 11,
                            y = 0
                        },
                        new() {
                            led_num = 13,
                            x = 12,
                            y = 0
                        },
                        new() {
                            led_num = 15,
                            x = 13,
                            y = 0
                        },
                        new() {
                            led_num = 69,
                            x = 14,
                            y = 0
                        },
                        new() {
                            led_num = 16,
                            x = 0,
                            y = 1
                        },
                        new() {
                            led_num = 17,
                            x = 1,
                            y = 1
                        },
                        new() {
                            led_num = 18,
                            x = 2,
                            y = 1
                        },
                        new() {
                            led_num = 19,
                            x = 3,
                            y = 1
                        },
                        new() {
                            led_num = 20,
                            x = 4,
                            y = 1
                        },
                        new() {
                            led_num = 21,
                            x = 5,
                            y = 1
                        },
                        new() {
                            led_num = 22,
                            x = 6,
                            y = 1
                        },
                        new() {
                            led_num = 23,
                            x = 7,
                            y = 1
                        },
                        new() {
                            led_num = 24,
                            x = 8,
                            y = 1
                        },
                        new() {
                            led_num = 25,
                            x = 9,
                            y = 1
                        },
                        new() {
                            led_num = 26,
                            x = 10,
                            y = 1
                        },
                        new() {
                            led_num = 27,
                            x = 11,
                            y = 1
                        },
                        new() {
                            led_num = 28,
                            x = 12,
                            y = 1
                        },
                        new() {
                            led_num = 29,
                            x = 13,
                            y = 1
                        },
                        new() {
                            led_num = 70,
                            x = 14,
                            y = 1
                        },
                        new() {
                            led_num = 30,
                            x = 0,
                            y = 2
                        },
                        new() {
                            led_num = 31,
                            x = 1,
                            y = 2
                        },
                        new() {
                            led_num = 32,
                            x = 2,
                            y = 2
                        },
                        new() {
                            led_num = 33,
                            x = 3,
                            y = 2
                        },
                        new() {
                            led_num = 34,
                            x = 4,
                            y = 2
                        },
                        new() {
                            led_num = 35,
                            x = 5,
                            y = 2
                        },
                        new() {
                            led_num = 36,
                            x = 6,
                            y = 2
                        },
                        new() {
                            led_num = 37,
                            x = 7,
                            y = 2
                        },
                        new() {
                            led_num = 38,
                            x = 8,
                            y = 2
                        },
                        new() {
                            led_num = 39,
                            x = 9,
                            y = 2
                        },
                        new() {
                            led_num = 40,
                            x = 10,
                            y = 2
                        },
                        new() {
                            led_num = 41,
                            x = 11,
                            y = 2
                        },
                        new() {
                            led_num = 43,
                            x = 13,
                            y = 2
                        },
                        new() {
                            led_num = 71,
                            x = 14,
                            y = 2
                        },
                        new() {
                            led_num = 44,
                            x = 0,
                            y = 3
                        },
                        new() {
                            led_num = 46,
                            x = 2,
                            y = 3
                        },
                        new() {
                            led_num = 47,
                            x = 3,
                            y = 3
                        },
                        new() {
                            led_num = 48,
                            x = 4,
                            y = 3
                        },
                        new() {
                            led_num = 49,
                            x = 5,
                            y = 3
                        },
                        new() {
                            led_num = 50,
                            x = 6,
                            y = 3
                        },
                        new() {
                            led_num = 51,
                            x = 7,
                            y = 3
                        },
                        new() {
                            led_num = 52,
                            x = 8,
                            y = 3
                        },
                        new() {
                            led_num = 53,
                            x = 9,
                            y = 3
                        },
                        new() {
                            led_num = 54,
                            x = 10,
                            y = 3
                        },
                        new() {
                            led_num = 55,
                            x = 11,
                            y = 3
                        },
                        new() {
                            led_num = 57,
                            x = 12,
                            y = 3
                        },
                        new() {
                            led_num = 73,
                            x = 13,
                            y = 3
                        },
                        new() {
                            led_num = 72,
                            x = 14,
                            y = 3
                        },
                        new() {
                            led_num = 58,
                            x = 0,
                            y = 4
                        },
                        new() {
                            led_num = 59,
                            x = 1,
                            y = 4
                        },
                        new() {
                            led_num = 60,
                            x = 2,
                            y = 4
                        },
                        new() {
                            led_num = 61,
                            x = 3,
                            y = 4
                        },
                        new() {
                            led_num = 62,
                            x = 6,
                            y = 4
                        },
                        new() {
                            led_num = 63,
                            x = 9,
                            y = 4
                        },
                        new() {
                            led_num = 64,
                            x = 10,
                            y = 4
                        },
                        new() {
                            led_num = 68,
                            x = 11,
                            y = 4
                        },
                        new() {
                            led_num = 74,
                            x = 12,
                            y = 4
                        },
                        new() {
                            led_num = 75,
                            x = 13,
                            y = 4
                        },
                        new() {
                            led_num = 76,
                            x = 14,
                            y = 4
                        }
                    }
                }
            };
        }


        private VisualMapController GetController(ArtemisDevice device)
        {

            object openRgbDevice = GetPropValue(device.RgbDevice.DeviceInfo, "OpenRGBDevice");
            string rgbDeviceLocation = (string)GetPropValue(openRgbDevice, "Location");
            string name = (string)GetPropValue(openRgbDevice, "Name");

            return new VisualMapController
            {
                location = rgbDeviceLocation,
                name = name,
                serial = "",
                vendor = "HyperX"
            };
        }

        private static object GetPropValue<T>(T src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

        private VisualMapGridSettings GetGridSettings()
        {
            int rows = _settings.GetSetting("Rows", 12).Value;
            int columns = _settings.GetSetting("Columns", 6).Value;

            return new VisualMapGridSettings
            {
                auto_load = false,
                auto_register = false,
                grid_size = 1,
                h = 5 * rows,
                show_bounds = true,
                show_grid = true,
                unregister_members = true,
                w = 15 * columns
            };
        }

        public void Dispose()
        {

        }
    }
}
