using FileExplorer.Model;
using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FileExplorer.Helper
{
    public class JsonParser
    {
        const string tagHeader = "<Header>";
        const string tagCopyright = "<Copyright_Descriptor>";
        const string tagDateDescriptor = "<Data_Descriptor>";
        const string tagLastWriteTime = "LastW";
        const string tagSize = "Size";
        const string tagAttr = "Attr";
        const string tagData = "Data";
        const string tagStream = "Stream";

        public JsonHeader Header { get; private set; }

        /// <summary>
        /// init root node
        /// </summary>
        /// <param name="source"></param>
        /// <param name="isShowCheckBox"></param>
        /// <returns></returns>
        public IList<JsonFolder> GetJsonItems(string filePath)
        {
            IList<JsonFolder> result = new List<JsonFolder>();
            if (filePath.IsNullOrEmpty() || !File.Exists(filePath))
                return result;

            result = ParseJsonData(filePath);
            return result;
        }

        /// <summary>
        /// when init, fill data to memory
        /// </summary>
        /// <param name="jsonPath"></param>
        IList<JsonFolder> ParseJsonData(string jsonPath)
        {
            IList<JsonFolder> result = new List<JsonFolder>();
            try
            {
                using (StreamReader streamReader = new StreamReader(new FileStream(jsonPath, FileMode.Open,
                                                                                   FileAccess.Read, FileShare.Read)))
                {
                    //first line is head info.
                    string headerStr = streamReader.ReadLine();
                    Header = ParseHeader(headerStr);
                    if (!Header.IsNull())
                    {
                        string rootPath = Header.RootPath;

                        //check version
                        double minorVersion = 0;
                        double.TryParse(Header.Header.Minor, out minorVersion);
                        //Debug.Assert(minorVersion >= 1.7, "Json File Minor version is less than 1.7, this is old file, maybe fail!");
                    }

                    StringBuilder sb = new StringBuilder();
                    while (!streamReader.EndOfStream)
                    {
                        //skip blank line
                        string line = streamReader.ReadLine();
                        if (line.IsNullOrEmpty())
                        {
                            line = sb.ToString();
                            if (line.Contains(tagCopyright) ||
                                line.Contains(tagDateDescriptor))
                            {
                                sb.Clear();
                                continue;
                            }

                            JsonFolder folder = GetFolders(line);
                            if (!folder.IsNull())
                            {
                                result.Add(folder);
                            }
                            sb.Clear();
                        }
                        else
                        {
                            sb.AppendLine(line);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Debug("Failed", ex);
            }
            return result;
        }

        /// <summary>
        /// get folder from json
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        JsonFolder GetFolders(string content)
        {
            JsonFolder result = null;
            if (string.IsNullOrEmpty(content))
                return result;
            JsonReader jsonReader = GetJsonReader(content);
            if (jsonReader.IsNull())
                return result;

            try
            {
                while (jsonReader.Read())
                {
                    if (jsonReader.Token == JsonToken.ObjectStart)
                    {
                        result = new JsonFolder();
                        continue;
                    }

                    if (!jsonReader.Value.IsNull())
                    {
                        result.Name = jsonReader.Value.ToString();
                        continue;
                    }

                    if (jsonReader.Token == JsonToken.ArrayStart)
                    {
                        var subFiles = GetFiles(jsonReader);
                        if (!subFiles.IsNullOrEmpty())
                            result.Items = subFiles;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Debug("Failed", ex);
            }
            jsonReader.Close();
            return result;
        }

        /// <summary>
        /// get jsonReader
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        JsonReader GetJsonReader(string content)
        {
            JsonReader jsonReader = null;
            if (content.IsNullOrEmpty())
                return jsonReader;
            try
            {
                jsonReader = new JsonReader(new StringReader(content));
            }
            catch (Exception ex)
            {
                LogHelper.Debug("Failed", ex);
            }
            return jsonReader;
        }

        /// <summary>
        /// get files and folders from jsonreader
        /// </summary>
        /// <param name="jsonReader"></param>
        /// <returns></returns>
        IList<JsonFile> GetFiles(JsonReader jsonReader)
        {
            IList<JsonFile> result = new List<JsonFile>();
            if (jsonReader.IsNull())
                return result;

            while (jsonReader.Token != JsonToken.ArrayEnd)
            {
                jsonReader.Read();
                if (jsonReader.Token == JsonToken.ObjectStart)
                {
                    JsonFile file = ParseFile(jsonReader);
                    if (!file.IsNull())
                    {
                        result.Add(file);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// get file/folder property 
        /// </summary>
        /// <param name="jsonReader"></param>
        /// <returns></returns>
        private JsonFile ParseFile(JsonReader jsonReader)
        {
            JsonFile file = null;
            if (jsonReader.IsNull())
                return file;

            //read name first
            jsonReader.Read();
            string name = jsonReader.Value.ToString();

            if (jsonReader.Token != JsonToken.PropertyName)
            {
                return file;
            }
            file = new JsonFile();
            file.Name = name;
            jsonReader.Read();

            string lastPropertyName = string.Empty;
            //{"Create":1358818109,"LastA":1358818109,"LastW":1358818109,"Attr":16}
            while (jsonReader.Token != JsonToken.ObjectEnd)
            {
                jsonReader.Read();
                if (jsonReader.Token == JsonToken.PropertyName)
                {
                    lastPropertyName = jsonReader.Value.ToString();
                    continue;
                }

                if (lastPropertyName == tagLastWriteTime)
                {
                    file.LastW = ConvertToDatetime(jsonReader.Value);
                    lastPropertyName = string.Empty;
                    continue;
                }

                if (lastPropertyName == tagSize)
                {
                    long size = 0;
                    if (!jsonReader.Value.IsNull())
                    {
                        var nodeValue = jsonReader.Value.ToString();
                        long.TryParse(nodeValue, out size);
                    }
                    file.Size = size;

                    lastPropertyName = string.Empty;
                    continue;
                }

                if (lastPropertyName == tagAttr)
                {
                    int attr = 0;
                    if (!jsonReader.Value.IsNull())
                    {
                        var nodeValue = jsonReader.Value.ToString();
                        Int32.TryParse(nodeValue, out attr);
                    }
                    file.Attr = attr;

                    lastPropertyName = string.Empty;
                    continue;
                }

                if (lastPropertyName == tagData)
                {
                    //don't reader this 
                    skipArray(jsonReader);
                }

                if (lastPropertyName == tagStream)
                {
                    //don't reader this 
                    skipArray(jsonReader);
                }
            }
            return file;
        }

        /// <summary>
        /// convert to datetime
        /// </summary>
        /// <param name="jsonValue"></param>
        /// <returns></returns>
        DateTime ConvertToDatetime(object jsonValue)
        {
            DateTime dt = DateTime.MinValue;
            if (jsonValue.IsNull())
                return dt;
            DateTime.TryParse(jsonValue.ToString(), out dt);
            return dt;
        }

        /// <summary>
        ///  get json header
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        JsonHeader ParseHeader(string line)
        {
            JsonHeader header = null;
            line = line.Replace(tagHeader, "Header");
            try
            {
                header = JsonMapper.ToObject<JsonHeader>(line);
            }
            catch (Exception ex)
            {
                LogHelper.Debug("read head fail,{0}", ex);
            }
            return header;
        }

        /// <summary>
        /// skip read array
        /// </summary>
        /// <param name="reader"></param>
        void skipArray(JsonReader reader)
        {
            if (reader.Token == JsonToken.ArrayStart)
            {
                while (reader.Read())
                {
                    skipObject(reader);
                    if (reader.Token == JsonToken.ArrayEnd)
                        break;
                }
            }
        }

        /// <summary>
        /// skip read object
        /// </summary>
        /// <param name="reader"></param>
        void skipObject(JsonReader reader)
        {
            if (reader.Token == JsonToken.ObjectStart)
            {
                while (reader.Read())
                {
                    skipArray(reader);
                    if (reader.Token == JsonToken.ObjectEnd)
                        break;
                }
            }
        }
    }
}
