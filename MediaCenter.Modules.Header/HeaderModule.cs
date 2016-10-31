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

namespace MediaCenter.Modules.Header
{
    //[ModuleExport(typeof(HeaderModule), InitializationMode = InitializationMode.WhenAvailable)]
    public class HeaderModule //: IModule
    {
        //[Import]
        //public IRegionManager regionManager;

        //public void Initialize()
        //{
        //    this.regionManager.RegisterViewWithRegion(RegionNames.HeaderRegion, typeof(HeaderView));
        //}
    }
}
