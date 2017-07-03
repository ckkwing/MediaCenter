using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileExplorer.Model;
using System.Collections.ObjectModel;
using Utilities.Extension;
using System.IO;

namespace FileExplorer.Factory
{
    class StaticLocalExplorerFactory : LocalExplorerFactory
    {
        IList<string> folderPaths = new List<string>();
        public StaticLocalExplorerFactory(IList<string> folderPaths)
        {
            this.folderPaths = folderPaths;
        }

        public override void GetRootFoldersAsync(Action<IEnumerable<IFolder>> callback)
        {
            ObservableCollection<IFolder> roots = new ObservableCollection<IFolder>();

            LocalRootFolder pcFolder = new LocalRootFolder();
            ComposeRoutes(pcFolder);
            roots.Add(pcFolder);

            if (!callback.IsNull())
            {
                callback(roots);
            }
        }

        private void ComposeRoutes(LocalRootFolder pcRoot)
        {
            if (pcRoot.IsNull())
                return;
            
            IFolder parent = pcRoot;
            IList<IFolder> createdFolders = new List<IFolder>();
            foreach (string folderPath in folderPaths)
            {
                //FindParent(folderPath, ref parents);
                parent = pcRoot;
                string[] splitedFolders = folderPath.Split(new string[] { PATH_SPLITER }, StringSplitOptions.RemoveEmptyEntries);
                string path = string.Empty;
                for(int i =0; i<splitedFolders.Length;i++)
                {
                    path += splitedFolders[i] + PATH_SPLITER;
                    DirectoryInfo directory = new DirectoryInfo(path);
                    if (directory.IsNull())
                        break;
                    StaticLocalFolder currentFolder = createdFolders.FirstOrDefault(item => (0 == string.Compare(path, item.FullPath, true))) as StaticLocalFolder;
                    if (currentFolder.IsNull())
                    {
                        currentFolder = new StaticLocalFolder(directory, parent);
                        currentFolder.Folders.Clear();
                        createdFolders.Add(currentFolder);
                        parent.Folders.Add(currentFolder);
                    }
                    
                    parent = currentFolder;
                }
            }
        }
    }
}
