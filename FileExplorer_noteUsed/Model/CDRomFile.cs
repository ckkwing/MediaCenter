using FileExplorer.Helper;
using NBEngineLib;
using System;
using System.IO;

namespace FileExplorer.Model
{
    class CDRomFile : FileBase
    {
        private IFileBackupItem CurrentFile { get; set; }

        public CDRomFile(IFileBackupItem file, IFolder folder)
            : base(string.Empty, folder)
        {
            if (file.IsNull() || folder.IsNull())
            {
                throw new ArgumentNullException();
            }
            this.CurrentFile = file;
            this.Parent = folder;

            this.Size = GetProperty<long>(CurrentFile, NB_FILEBACKUPITEM_PROPERTY_TYPE.NB_FILEBACKUPITEM_PROPERTY_ACTUALSIZE);
            this.Name = GetProperty<string>(CurrentFile, NB_FILEBACKUPITEM_PROPERTY_TYPE.NB_FILEBACKUPITEM_PROPERTY_WINNAME);
            this.Extension = Path.GetExtension(this.Name);
            this.FullPath = GetProperty<string>(CurrentFile, NB_FILEBACKUPITEM_PROPERTY_TYPE.NB_FILEBACKUPITEM_PROPERTY_PATH);
            this.LastModifyTime = GetProperty<DateTime>(CurrentFile, NB_FILEBACKUPITEM_PROPERTY_TYPE.NB_FILEBACKUPITEM_PROPERTY_MODIFIEDDATE);
        }

        private T GetProperty<T>(IFileBackupItem item, NB_FILEBACKUPITEM_PROPERTY_TYPE propName)
        {
            T result = default(T);
            if (item.IsNull())
                return result;
            try
            {
                result = item.GetProperty(propName);
            }
            catch (Exception ex)
            {
                LogHelper.Debug("Failed", ex);
            }
            return result;
        }

        private CDRomFile() { }
        public override object Clone()
        {
            CDRomFile file = new CDRomFile();
            CloneMembers(file);
            return file;
        }
    }
}
