using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Text;
using Common;

class AssetBuildBundleInfo
{
	public string fileName;
	public string assetName;
	public string bundleName;
	public List<string> dependencies;

	public void AddDependence(string dep)
	{
		if (dependencies == null) {
			dependencies = new List<string> ();
		}
		dependencies.Add (dep);
	}
}

public class ABBuild {
	// 资源目录
	private static string RES_SRC_PATH = "Assets/Resources/";
	// 打包输出目录
	private static string RES_OUTPUT_PATH = "Assets/StreamingAssets";
    // 打包更新输出目录
    private static string RES_UPDATE_OUTPUT_PATH = "D:/ABBuild_Res";
    // AssetBundle打包后缀
    private static string ASSET_BUNDLE_SUFFIX = ".unity3d";
	// xml文件生成器
	private static XMLDocment doc;
	// bundleName <-> List<AssetBuildBundleInfo>
	private static Dictionary<string, List<AssetBuildBundleInfo>> bundleMap = new Dictionary<string, List<AssetBuildBundleInfo>>();
	// 文件名 <-> AssetBuildBundleInfo
	private static Dictionary<string, AssetBuildBundleInfo> fileMap = new Dictionary<string, AssetBuildBundleInfo>();

	[MenuItem("Custom/打包", false, 351)]
	public static void Pack()
	{
		// 清理输出目录
		CreateOrClearOutPath();

		// 清理之前设置过的bundleName
		ClearAssetBundleName();

		// 设置bunderName
		bundleMap.Clear();
        fileMap.Clear();
        List<string> resList = GetAllResDirs (RES_SRC_PATH);
		foreach (string dir in resList) {
			setAssetBundleName (dir);
		}

		// 打包
		BuildPipeline.BuildAssetBundles(RES_OUTPUT_PATH, BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.StandaloneWindows64);
		AssetDatabase.Refresh ();

        // 构建依赖关系
        if (!System.IO.File.Exists(FileUtils.getPath("StreamingAssets")) ) return;

		AssetBundle assetBundle = AssetBundle.LoadFromFile (FileUtils.getPath ("StreamingAssets"));
		AssetBundleManifest mainfest = assetBundle.LoadAsset<AssetBundleManifest> ("AssetBundleManifest");
		string[] bundleNames = mainfest.GetAllAssetBundles ();
		foreach (string bundleName in bundleNames) {
			string[] deps = mainfest.GetAllDependencies (bundleName);
			foreach (string dep in deps) {
				List<AssetBuildBundleInfo> infoList = null;
				bundleMap.TryGetValue (bundleName, out infoList);
				if (null != infoList) {
					foreach (AssetBuildBundleInfo info in infoList) {
						info.AddDependence (dep);
					}
				}
			}
		}

		// 生成XML
		doc = new XMLDocment();
		doc.startObject ("files");
		foreach (KeyValuePair<string, AssetBuildBundleInfo> pair in fileMap) {
			AssetBuildBundleInfo info = pair.Value;

			doc.startObject ("file");
			doc.createElement ("bundleName", info.bundleName);
			doc.createElement ("fileName", info.fileName);
			doc.createElement ("assetName", info.assetName);

			if (null != info.dependencies) {
				doc.startObject ("deps");
				foreach (string dep in info.dependencies) {
					doc.createElement ("dep", dep);
				}
				doc.endObject ("deps");
			}
			doc.endObject ("file");
		}
		doc.endObject ("files");

		FileStream fs = new FileStream (Path.Combine (RES_OUTPUT_PATH, "StreamingAssets.xml"), FileMode.Create);
		byte[] data = System.Text.Encoding.UTF8.GetBytes (doc.ToString ());
		fs.Write (data, 0, data.Length);
		fs.Flush ();
		fs.Close ();

        if (ResourceManager.GetInstance().ManagerType == ResManagerType.ABB) {
            //创建压缩文件的MD5
            CreateMD5File(".unity3d");

            // 打包后的清理
            ClearOutPath(".xxx");
        }
        else if (ResourceManager.GetInstance().ManagerType == ResManagerType.ABB_UPDATE)
        {
            //压缩文件
            CompressFile();

            //创建压缩文件的MD5
            CreateMD5File(".zip");

            //复制到指定目录
            CopyFileForUpdate();

            // 打包后的清理
            ClearOutPath(".unity3d");
        }
       
    }

	/// <summary>
	/// 设置AssetBundleName
	/// </summary>
	/// <param name="fullpath">Fullpath.</param>
	public static void setAssetBundleName(string fullPath) 
	{
		string[] files = System.IO.Directory.GetFiles (fullPath);
		if (files == null || files.Length == 0) {
			return;
		}

		Debug.Log ("Set AssetBundleName Start......");
		string dirBundleName = fullPath.Substring (RES_SRC_PATH.Length);
        dirBundleName = dirBundleName.Replace ("/", "@") + ASSET_BUNDLE_SUFFIX;
        foreach (string file in files) {
			if (file.EndsWith (".meta")) {
				continue;
			}

            string filePath = file.Replace("\\", "/");
            AssetImporter importer = AssetImporter.GetAtPath (filePath);
			if (importer != null) {
				string ext = System.IO.Path.GetExtension (filePath);
				string bundleName = dirBundleName;
				if (null != ext && (ext.Equals (".prefab")||ext.Equals(".unity")))
                {
					// prefab单个文件打包
					bundleName = filePath.Substring (RES_SRC_PATH.Length);
                    bundleName = bundleName.Replace ("/", "@");
                    if (null != ext) {
						bundleName = bundleName.Replace (ext, ASSET_BUNDLE_SUFFIX);
					} else {
						bundleName += ASSET_BUNDLE_SUFFIX;
					}
				}

				bundleName = bundleName.ToLower ();
				Debug.LogFormat ("Set AssetName Succ, File:{0}, AssetName:{1}", filePath, bundleName);
				importer.assetBundleName = bundleName;
				EditorUtility.UnloadUnusedAssetsImmediate();

				// 存储bundleInfo
				AssetBuildBundleInfo info = new AssetBuildBundleInfo();
				info.assetName = filePath;
				info.fileName = filePath;
				info.bundleName = bundleName;
				if (null != ext) {
					info.fileName = filePath.Substring (0, filePath.IndexOf (ext));
				}
				fileMap.Add (filePath, info);

				List<AssetBuildBundleInfo> infoList = null;
				bundleMap.TryGetValue(info.bundleName, out infoList);
				if (null == infoList) {
					infoList = new List<AssetBuildBundleInfo> ();
					bundleMap.Add (info.bundleName, infoList);
				}
				infoList.Add (info);
			}
            else {
				Debug.LogFormat ("Set AssetName Fail, File:{0}, Msg:Importer is null", file);
			}
		}
		Debug.Log ("Set AssetBundleName End......");


	}

	/// <summary>
	/// 获取所有资源目录
	/// </summary>
	/// <returns>The res all dir path.</returns>
	public static List<string> GetAllResDirs(string fullPath)
	{
		List<string> dirList = new List<string> ();

		// 获取所有子文件
		GetAllSubResDirs (fullPath, dirList);

		return dirList;
	}

	/// <summary>
	/// 递归获取所有子目录文件夹
	/// </summary>
	/// <param name="fullPath">当前路径</param>
	/// <param name="dirList">文件夹列表</param>
	public static void GetAllSubResDirs(string fullPath, List<string> dirList)
	{
		if ((dirList == null) || (string.IsNullOrEmpty(fullPath)))
			return;

		string[] dirs = System.IO.Directory.GetDirectories (fullPath);
		if (dirs != null && dirs.Length > 0) {
			for (int i = 0; i < dirs.Length; ++i) {
				GetAllSubResDirs (dirs [i], dirList);
			}
		}
        else {
            fullPath = fullPath.Replace("\\", "/");
			dirList.Add (fullPath);
		}
	}

	/// <summary>
	/// 创建和清理输出目录
	/// </summary>
	public static void CreateOrClearOutPath()
	{
        if (!System.IO.Directory.Exists (RES_OUTPUT_PATH)) {
			// 不存在创建
			System.IO.Directory.CreateDirectory (RES_OUTPUT_PATH);
		} else {
			// 存在就清理
			System.IO.Directory.Delete(RES_OUTPUT_PATH, true);
			System.IO.Directory.CreateDirectory (RES_OUTPUT_PATH);
		}
	}

	/// <summary>
	/// 清理打包目录
	/// </summary>
	public static void ClearOutPath(string fileExtention)
	{
        string[] files = System.IO.Directory.GetFiles(RES_OUTPUT_PATH, "*", System.IO.SearchOption.AllDirectories);
        foreach (string file in files)
        {
            if (file.EndsWith(".manifest")
                 || file.EndsWith(fileExtention))
            {
                System.IO.File.Delete(file);
            }
        }
    }

	/// <summary>
	/// 清理之前设置的bundleName
	/// </summary>
	public static void ClearAssetBundleName()
	{
		string[] bundleNames = AssetDatabase.GetAllAssetBundleNames ();
		for (int i = 0; i < bundleNames.Length; i++) {
			AssetDatabase.RemoveAssetBundleName (bundleNames [i], true);
		}
	}

    static void CompressFile()
    {
        foreach (KeyValuePair<string, List<AssetBuildBundleInfo>> pair in bundleMap)
        {
            string filePath = Path.Combine(RES_OUTPUT_PATH, pair.Key);
            if (filePath.Length != 0)
            {
                int lastDotIndex = filePath.LastIndexOf(".");
                string outputPath = filePath.Substring(0, lastDotIndex + 1) + "zip";
                FileUtils.CompressFileLZMA(filePath, outputPath);
            }
        }
    }

    static void CreateMD5File(string fileExtension) {

        #region 获取Res文件夹下所有文件的相对路径和MD5值
        // 获取Res文件夹下所有文件的相对路径和MD5值  
        //string[] files = Directory.GetFiles(RES_OUTPUT_PATH, "*", SearchOption.AllDirectories);
        //StringBuilder versions = new StringBuilder();
        //for (int i = 0, len = files.Length; i < len; ++i)
        //{
        //    string filePath = files[i];
        //    int index = files[i].LastIndexOf(".");
        //    if (index == -1) continue;

        //    string extension = filePath.Substring(index);
        //    if (extension == ".unity3d")
        //    {
        //        string relativePath = filePath.Replace(RES_OUTPUT_PATH + "\\", "").Replace("\\", "/");
        //        string md5 = FileUtils.MD5File(filePath);
        //        versions.Append(relativePath).Append(",").Append(md5).Append("\n");
        //    }
        //}
        #endregion

        StringBuilder versions = new StringBuilder();
        foreach (KeyValuePair<string, List<AssetBuildBundleInfo>> pair in bundleMap)
        {
            int lastDotIndex = pair.Key.LastIndexOf(".");
            string fileName = pair.Key.Substring(0, lastDotIndex) + fileExtension;
            string filePath = Path.Combine(RES_OUTPUT_PATH, fileName);
            string md5 = FileUtils.MD5File(filePath);
            versions.Append(fileName).Append(",").Append(md5).Append("\n");
        }
        
        // 生成配置文件  
        FileStream fs = new FileStream(Path.Combine(RES_OUTPUT_PATH , "version.txt"), FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(versions.ToString());
        fs.Write(data, 0, data.Length);
        fs.Flush();
        fs.Close();
    }

    static void CopyFileForUpdate() {
        if (!System.IO.Directory.Exists(RES_UPDATE_OUTPUT_PATH))
        {
            System.IO.Directory.CreateDirectory(RES_UPDATE_OUTPUT_PATH);
        }
        else
        {
            System.IO.Directory.Delete(RES_UPDATE_OUTPUT_PATH, true);
            System.IO.Directory.CreateDirectory(RES_UPDATE_OUTPUT_PATH);
        }

        //复制文件到目录
        string[] files = Directory.GetFiles(RES_OUTPUT_PATH, "*", SearchOption.AllDirectories);
        foreach (string filePath in files)
        {
            string fileName = filePath.Substring(RES_OUTPUT_PATH.Length + 1);
            string destFilePath = Path.Combine(RES_UPDATE_OUTPUT_PATH, fileName);

            if (fileName.EndsWith(".zip")
                 || fileName.EndsWith("StreamingAssets.xml")
                  || fileName.EndsWith("version.txt"))
            {
                if (File.Exists(destFilePath))
                {
                    File.Copy(filePath, destFilePath, true);
                }
                else
                {
                    File.Copy(filePath, destFilePath);
                }
            }
        }
    }
}
