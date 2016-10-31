using BackItUp.OnlineService;
using FileExplorer.Helper;
using System;
using System.IO;

namespace FileExplorer.Model
{
    public class CloudFile : FileBase
    {
        public ICloudFile CloudItem { get; private set; }

        public CloudFile(ICloudFile file, IFolder folder)
            : base(Path.Combine(folder.FullPath, file.CloudId), folder)
        {
            if (file.IsNull() || folder.IsNull())
            {
                throw new ArgumentNullException();
            }
            this.CloudItem = file;
            this.Parent = folder;

            this.Name = file.Name;
            this.Extension = Path.GetExtension(file.Name);
            this.LastModifyTime = file.ModificationTime;
            this.Size = file.Size;
            this.FullPath = Path.Combine(folder.FullPath, file.CloudId);
        }

        protected override string IconKey
        {
            get
            {
                return this.Name;
            }
        }

        public override string FolderPath
        {
            get
            {
                folderPath = folderPath.IsNullOrEmpty() && !Parent.IsNull() ? this.Parent.FolderPath : folderPath;
                return folderPath;
            }
        }

        public string GetCloudPath
        {
            get
            {
                string cloudPath = Path.Combine(FolderPath, this.Name);
                return cloudPath;
            }
        }

        private CloudFile() { }
        public override object Clone()
        {
            CloudFile file = new CloudFile();
            CloneMembers(file);
            return file;
        }
    }
}
