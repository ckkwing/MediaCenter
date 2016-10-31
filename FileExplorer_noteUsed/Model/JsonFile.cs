using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileExplorer.Helper;
using System.Windows.Media;
using System.IO;
using Data = BackItUp.Infrastructure.FileExplorerDatas;

namespace FileExplorer.Model
{
    public class JsonFile : FileBase
    {
        public JsonFile(Data.JsonFile file, IFolder folder)
            : base(string.Empty, folder)
        {
            if (file.IsNull() || folder.IsNull())
            {
                throw new ArgumentNullException();
            }
            this.Parent = folder;

            this.Name = file.Name;
            this.Extension = Path.GetExtension(file.Name);
            this.LastModifyTime = file.LastW;
            this.Size = file.Size;
            this.FullPath = Path.Combine(folder.FullPath, file.Name);
        }

        private JsonFile() { }
        public override object Clone()
        {
            JsonFile file = new JsonFile();
            CloneMembers(file);
            return file;
        }
    }
}
