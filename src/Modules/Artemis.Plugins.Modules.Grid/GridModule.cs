using System;
using System.Collections.Generic;
using Artemis.Core;
using Artemis.Core.Modules;


namespace Artemis.Plugins.Modules.Performance;

public class GridDataModel : DataModel
{
    public GridDataModel()
    {
    }
}


[PluginFeature(Name = "Grid", AlwaysEnabled = true)]
public class GridModule : Module<GridDataModel>
{
    public GridModule()
    {

    }

    public override List<IModuleActivationRequirement> ActivationRequirements => null;

    public override void Enable()
    {

    }

    public override void Disable()
    {
    }

    public override void Update(double deltaTime)
    {
    }

    private void UpdatePerformance()
    {

    }
}