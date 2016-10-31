using FileExplorer.Factory;
using FileExplorer.Helper;
using FileExplorer.Model;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Utilities.Extension;

namespace FileExplorer.ViewModel
{
    [Export(typeof(StaticFileExplorerViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class StaticFileExplorerViewModel : FileExplorerViewModel
    {
        #region Command
        //public ICommand OnLoadedCommand { get; private set; }

        #endregion

        public StaticFileExplorerViewModel()
        {
        }

        public void LoadExplorerByFolderPaths(IList<string> paths)
        {
            this.InitialExplorer(new StaticLocalExplorerFactory(paths));
        }
    }
}
