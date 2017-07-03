using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Extension;

namespace FileExplorer.Model
{
    public class StaticLocalFolder : LocalFolder
    {
        public StaticLocalFolder(DirectoryInfo dirInfo, IFolder parent)
            : base(dirInfo, parent)
        {

        }

        public override void GetChildrenAsync(Action<IEnumerable<IFile>> callback)
        {
            IsCanceled = false;

            ///If all subitem got, don't need show loading
            if (!IsFolderLoaded || !IsFileLoaded)
            {
                IsLoading = true;
            }

            this.GetFoldersAsync((folders) =>
            {
                if (!callback.IsNull())
                {
                    callback(this.Items);
                }
            });
        }
    }
}
