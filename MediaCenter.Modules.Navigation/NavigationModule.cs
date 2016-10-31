using MediaCenter.Infrastructure;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaCenter.Modules.Navigation
{
    //[ModuleExport(typeof(NavigationModule), InitializationMode = InitializationMode.WhenAvailable)]
    public class NavigationModule //: IModule
    {
        //[Import]
        //public IRegionManager regionManager;

        //public void Initialize()
        //{
        //   this.regionManager.RegisterViewWithRegion(RegionNames.NavigationRegion, typeof(NavigationView));
        //}
    }
}
