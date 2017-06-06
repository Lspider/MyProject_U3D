using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using SevenZip.Compression.LZMA;

// #define USE_MD5

namespace Common
{
	/// <summary>
	/// 文件工具类
	/// </summary>
	public class FileUtils
	{

		static string DATA_ROOT_PATH = String.Format("{0}/", Application.streamingAssetsPath);

		/// <summary>
		/// 构造函数
		/// </summary>
		private FileUtils () { }

		/// <summary>
		/// 获取真实路径
		/// </summary>
		/// <returns>真实文件路径</returns>
		/// <param name="file">相对文件路径</param>
		public static string getPath(string file)
		{
			return DATA_ROOT_PATH + file;
		}

		/// <summary>
		/// 获取本地路径
		/// </summary>
		/// <returns>The local path.</returns>
		/// <param name="file">File.</param>
		public static string getLocalPath(string file)
		{
			return file;
		}

        #region 获取设备可写目录
        public static string GetWritePath()
        {
            string path = "";

            if (Application.platform == RuntimePlatform.Android
                || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                //iOS Application/xxxxx/Documents
                //Android /data/data/xxx.xxx.xxx/files
                path = Application.persistentDataPath;
            }
            else if (Application.platform == RuntimePlatform.WindowsPlayer
                || Application.platform == RuntimePlatform.WindowsEditor)
            {
                path = Application.dataPath;
            }

            return path;
        }
        #endregion

        #region StreamingAssets Path
        public static string GetStreamingAssetsPath()
        {
            string PathURL =
#if  UNITY_STANDALONE_WIN || UNITY_EDITOR
    "file://"  + Application.dataPath + "/StreamingAssets/";
#elif UNITY_ANDROID && !UNITY_EDITOR 
        //Application.dataPath + "!/assets/";
            "jar:file://" + Application.dataPath + "!/assets/";
#elif UNITY_IOS && !UNITY_EDITOR 
        Application.dataPath + "/Raw/";
#else
        string.Empty;  
#endif
            return PathURL;
        }
        #endregion

        #region 判断文件是否存在
        public static bool IsExistFile(string path, string fileName)
        {
            FileInfo t = new FileInfo(path + "//" + fileName);
            if (t.Exists)
            {
                return true;
            }

            return false;
        }
        #endregion

        #region 在指定位置创建文件   如果文件已经存在则追加文件内容
        /// <summary>
        /// 在指定位置创建文件   如果文件已经存在则追加文件内容
        /// </summary>
        /// <param name='path'>
        /// 路径
        /// </param>
        /// <param name='name'>
        /// 文件名
        /// </param>
        /// <param name='info'>
        /// 文件内容
        /// </param>
        public static void CreateORwriteConfigFile(string path, string name, string info)
        {
            StreamWriter sw;
            FileInfo t = new FileInfo(path + "//" + name);
            if (!t.Exists)
            {
                sw = t.CreateText();
            }
            else
            {
                sw = t.AppendText();
            }

            sw.WriteLine(info);
            sw.Close();
            sw.Dispose();
        }
        #endregion

        #region  删除文件
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name='path'>
        /// Path.
        /// </param>
        /// <param name='name'>
        /// Name.
        /// </param>
        public static void DeleteFile(string path, string name)
        {
            File.Delete(path + "//" + name);
        }
        #endregion

        #region 读取文件内容  仅读取第一行
        /// <summary>
        /// 读取文件内容  仅读取第一行
        /// </summary>
        /// <param name='path'>
        /// Path.
        /// </param>
        /// <param name='name'>
        /// Name.
        /// </param>
        public static string LoadFile(string path, string name)
        {
            FileInfo t = new FileInfo(path + "//" + name);
            if (!t.Exists)
            {
                return "error";
            }
            StreamReader sr = null;
            sr = File.OpenText(path + "//" + name);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                break;
            }
            sr.Close();
            sr.Dispose();
            return line;
        }
        #endregion

        #region 读取文本文件
        /** 
       * 读取文本文件 
       * path：读取文件的路径 
       * name：读取文件的名称 
       */
        public static ArrayList LoadFileAll(string path, string name)
        {
            //使用流的形式读取  
            StreamReader sr = null;
            try
            {
                sr = File.OpenText(path + "//" + name);
            }
            catch (Exception e)
            {
                //路径与名称未找到文件则直接返回空  
                Debug.Log(e.ToString());
                return null;
            }
            string line;
            ArrayList arrlist = new ArrayList();
            while ((line = sr.ReadLine()) != null)
            {
                //一行一行的读取  
                //将每一行的内容存入数组链表容器中  
                arrlist.Add(line);
            }
            //关闭流  
            sr.Close();
            //销毁流  
            sr.Dispose();
            //将数组链表容器返回  
            return arrlist;
        }

        public static byte[] LoadFile(string filePath)
        {
            //创建文件读取流
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            fileStream.Seek(0, SeekOrigin.Begin);
            //创建文件长度缓冲区
            byte[] bytes = new byte[fileStream.Length];
            //读取文件
            fileStream.Read(bytes, 0, (int)fileStream.Length);
            //释放文件读取流
            fileStream.Close();
            fileStream.Dispose();
            fileStream = null;

            return bytes;
        }

        #endregion



        #region 写入模型到本地 
        //写入模型到本地  
        public static IEnumerator loadAsset(string url)
        {
            WWW w = new WWW(url);
            yield return w;
            if (w.isDone)
            {
                byte[] model = w.bytes;
                int length = model.Length;
                //写入模型到本地  
                CreateModelFile(Application.persistentDataPath, "Model.assetbundle", model, length);
            }
        }

        #endregion

        #region 创建Model
        public static void CreateModelFile(string path, string name, byte[] info, int length)
        {
            //文件流信息   
            Stream sw;
            FileInfo t = new FileInfo(path + "//" + name);
            if (!t.Exists)
            {
                //如果此文件不存在则创建  
                sw = t.Create();
            }
            else
            {
                return;
            }

            //以行的形式写入信息   
            sw.Write(info, 0, length);
            //关闭流  
            sw.Close();
            //销毁流  
            sw.Dispose();
        }

        #endregion

        #region 获取文件的MD5
        /// <summary>
        /// 获取文件的MD5
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string MD5File(string file)
        {
            try
            {
                FileStream fs = new FileStream(file, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(fs);
                fs.Close();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("md5file() fail, error:" + ex.Message);
            }
        }
        #endregion

        #region 使用LZMA算法压缩文件 
        // 使用LZMA算法压缩文件  
        public static void CompressFileLZMA(string inFile, string outFile)
        {
            SevenZip.Compression.LZMA.Encoder coder = new SevenZip.Compression.LZMA.Encoder();
            FileStream input = new FileStream(inFile, FileMode.Open);
            FileStream output = new FileStream(outFile, FileMode.Create);

            coder.WriteCoderProperties(output);

            byte[] data = BitConverter.GetBytes(input.Length);

            output.Write(data, 0, data.Length);

            coder.Code(input, output, input.Length, -1, null);
            output.Flush();
            output.Close();
            input.Close();
        }
        #endregion

        #region 使用LZMA算法解压文件  
        // 使用LZMA算法解压文件  
        public static void DecompressFileLZMA(string inFile, string outFile)
        {
            SevenZip.Compression.LZMA.Decoder coder = new SevenZip.Compression.LZMA.Decoder();
            FileStream input = new FileStream(inFile, FileMode.Open);
            FileStream output = new FileStream(outFile, FileMode.Create);

            byte[] properties = new byte[5];
            input.Read(properties, 0, 5);

            byte[] fileLengthBytes = new byte[8];
            input.Read(fileLengthBytes, 0, 8);
            long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

            coder.SetDecoderProperties(properties);
            coder.Code(input, output, input.Length, fileLength, null);
            output.Flush();
            output.Close();
            input.Close();
        }
        #endregion

        #region 复制源文件夹到目标文件夹  
        // 复制源文件夹到目标文件夹  
        public static void CopyAll(DirectoryInfo source, DirectoryInfo target, ArrayList extensions = null)
        {
            if (source.FullName.ToLower() == target.FullName.ToLower())
            {
                return;
            }

            // 检测目标文件是否存在，如果不存在，则创建  
            if (!Directory.Exists(target.FullName))
            {
                Directory.CreateDirectory(target.FullName);
                target.Refresh();
            }

            // 复制文件至目标文件夹  
            foreach (FileInfo fi in source.GetFiles())
            {
                if (extensions == null || extensions.Count == 0)
                {
                    if (fi.Extension != ".meta")
                    {
                        fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
                    }
                }
                else
                {
                    if (extensions.Contains(fi.Extension))
                    {
                        fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
                    }
                }

            }

            // 使用递归复制子文件夹  
            foreach (DirectoryInfo sourceSubDir in source.GetDirectories())
            {
                DirectoryInfo targetSubDir = target.CreateSubdirectory(sourceSubDir.Name);
                CopyAll(sourceSubDir, targetSubDir, extensions);
            }
        }
        #endregion
    }
}

