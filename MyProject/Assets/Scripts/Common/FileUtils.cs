using System;
using System.Collections;
using System.IO;
using UnityEngine;

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

            if (Application.platform == RuntimePlatform.Android)
            {
                path = Application.persistentDataPath;
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                path = Application.dataPath;
            }
            else if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                path = Application.dataPath;
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor)
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
#if UNITY_ANDROID
        "jar:file://" + Application.dataPath + "!/assets/";  
#elif UNITY_IPHONE
        Application.dataPath + "/Raw/";
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
    "file://" + Application.dataPath + "/StreamingAssets/";
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
    }
}

