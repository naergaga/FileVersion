using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FileVersion
{
    public class FileHashProvider
    {
        private bool _isQueryTmp=true;
        private string _queryTmp="?v={0}"; 

        public FileHashProvider()
        {

        }

        public FileHashProvider(bool isQueryTmp,string queryTmp)
        {
            this._isQueryTmp = isQueryTmp;
            _queryTmp =queryTmp;
        }

        SHA256 c1 = SHA256.Create();

        public string Get(string path)
        {
            var hashBuffer = c1.ComputeHash(File.ReadAllBytes(path));
            var result = Convert.ToBase64String(hashBuffer);
            return FilterStr(result.Replace('+', '-').Replace('/', '_').Replace("=", string.Empty));
        }

        private string FilterStr(string str)
        {
            if (_isQueryTmp)
            {
                return string.Format(_queryTmp, str);
            }
            return str;
        }

        public List<FileHashInfo> GetDirInfo(string dir)
        {
            List<FileHashInfo> list = new List<FileHashInfo>();
            if (!Directory.Exists(dir)) return list;
            var files = Directory.GetFiles(dir);
            foreach (var item in files)
            {
                list.Add(new FileHashInfo { Version = Get(item), FileName = item });
            }
            return list;
        }

        public FileHashInfo GetFileHashInfo(string path)
        {
            if (!File.Exists(path)) return null;
           return new FileHashInfo { Version = Get(path), FileName = path };
        }
    }
}
