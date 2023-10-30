using System;
using Artemis.Core;
using Artemis.Plugins.Modules.Grid.ViewModels;
using Artemis.UI.Shared;

namespace Artemis.Plugins.Modules.Performance;

public class GridBootstrapper : PluginBootstrapper
{
    public override void OnPluginEnabled(Plugin plugin)
    {
        plugin.ConfigurationDialog = new PluginConfigurationDialog<GridConfigurationViewModel>();
    }
}