using System.Collections.Generic;
using System.IO;

namespace Assets
{
    public sealed class ClearHistory : Operation
    {
        int count = 0;

        List<string> deleteFiles = new List<string>();
        public override void Start()
        {
            base.Start();
            /// 获取下载目录
            var path = Manager.GetDownloadDataPath("");
            var cur_catlog = Catlog.GetCatlogFile(Manager.BuildDate);
            var temp = Directory.GetFiles(path);
            foreach ( var item in temp)
            {
                if (item.EndsWith(cur_catlog) || Manager.Catlog.IncludeBundle(item))
                    continue;
                deleteFiles.Add(item);
            }
            count = deleteFiles.Count;
        }

        protected override void Update()
        {
            if (status == OperationStatus.Processing)
            {
                while (deleteFiles.Count > 0)
                {
                    progress = (count - deleteFiles.Count) * 1f / count;
                    var file = deleteFiles[0];
                    try
                    {
                        File.Delete(file);
                    }
                    catch { }
                    deleteFiles.RemoveAt(0);
                    if (Updater.Busy()) 
                        break;
                }
                // check finish
                if(deleteFiles.Count == 0)
                    Finish();
            }
        }
    }
}