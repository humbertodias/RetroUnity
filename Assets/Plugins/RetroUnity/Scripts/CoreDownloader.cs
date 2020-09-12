#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using UnityEditor;
using UnityEngine;

namespace RetroUnity
{
    public class CoreDownloader
    {

        string DownloadFile(string url, string directory)
        {
            using (var webClient = new WebClient())
            {
                var fileName = Path.GetFileName(url);
                var filePath = Path.Combine(directory, fileName);
                Debug.Log($"Downloading at {filePath} from {url}");

                webClient.DownloadFile(new Uri(url), filePath);
                return filePath;
            }         
        }

        [MenuItem("Libretro/Download cores")]
        static void DownloadCores()
        {
            var coreNames = new List<string>()
            {
                "snes9x","blastem", "nestopia", "mgba"
            };
            foreach (var coreName in coreNames)
            {
                DownloadCores(coreName);
            }
        }

        string extractDirectory(BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case BuildTarget.Android:
                case BuildTarget.iOS:
                    return Path.Combine(Application.dataPath, "Plugins");
                default:
                    return Application.streamingAssetsPath;
            }
        }
        
        List<string> unzipCore(string zipPath, string extractDirectory)
        {
            List<string> assets = new List<string>();
            try
            {
                using (var archive = ZipFile.OpenRead(zipPath))
                {
                    foreach (var entry in archive.Entries)
                    {
                        // Gets the full path to ensure that relative segments are removed.
                        var destinationPath = Path.GetFullPath(Path.Combine(extractDirectory, entry.FullName));
                        if (File.Exists(destinationPath))
                        {
                            File.Delete(destinationPath);
                        }

                        entry.ExtractToFile(destinationPath);
                        assets.Add(destinationPath);
                    }
                }
            }
            finally
            {
                File.Delete(zipPath);
            }

            return assets;
        }

        
        static void DownloadCores(string romName)
        {
            // http://buildbot.libretro.com/nightly
            var cores = new Dictionary<BuildTarget, string>()
            {
                // Standalone
                { BuildTarget.StandaloneWindows, $"http://buildbot.libretro.com/nightly/windows/x86_64/latest/{romName}_libretro.dll.zip"  } ,
                { BuildTarget.StandaloneLinux64, $"http://buildbot.libretro.com/nightly/linux/x86_64/latest/{romName}_libretro.so.zip" } ,
                { BuildTarget.StandaloneOSX, $"http://buildbot.libretro.com/nightly/apple/osx/x86_64/latest/{romName}_libretro.dylib.zip"}, 
                // Mobile
                { BuildTarget.Android, $"http://buildbot.libretro.com/nightly/android/latest/armeabi-v7a/{romName}_libretro_android.so.zip" }, 
                { BuildTarget.iOS, $"http://buildbot.libretro.com/nightly/apple/ios/latest/{romName}_libretro_ios.dylib.zip" } 
                
            };
            
            var coreDownloader = new CoreDownloader();
            foreach(var item in cores)
            {
                var buildTarget = item.Key;
                var url = item.Value;
                var extractDirectory = coreDownloader.extractDirectory(buildTarget);
                try
                {
                    var zipPath = coreDownloader.DownloadFile(url, extractDirectory);
                    Debug.Log($"File successfully downloaded and saved to {zipPath}");
                    var unzippedAssets = coreDownloader.unzipCore( zipPath, extractDirectory);
                    Debug.Log($"Unzipping successfully downloaded and saved to {item.Value}");
                    ImportAssets(buildTarget, unzippedAssets);
                }
                catch (Exception e)
                {
                    Debug.Log($"Error downloading: '{e}'");
                }

            }

        }

        
        static void ImportAssets(BuildTarget buildTarget, List<string> assetPaths)
        {
            foreach (var assetPath in assetPaths)
            {
                var relativePath = assetPath.Substring(assetPath.IndexOf("Assets"));
                AssetDatabase.ImportAsset(relativePath);

                // Only for plugins
                if (assetPath.Contains("Plugins"))
                {
                    SetNativePluginLibrary(buildTarget, "ARMv7", relativePath);
                }
                        
            }
            
        }

        static void SetNativePluginLibrary(BuildTarget buildTarget, string cpu, string relativePath)
        {
            // native library, avaiable only for mobile
            var nativePlugin = AssetImporter.GetAtPath(relativePath) as PluginImporter;
            // Exclude
            nativePlugin.SetExcludeEditorFromAnyPlatform(true);
            nativePlugin.SetExcludeFromAnyPlatform(buildTarget, false);
            // Include
            nativePlugin.SetCompatibleWithEditor(false);
            nativePlugin.SetCompatibleWithAnyPlatform(false);
            nativePlugin.SetCompatibleWithPlatform(buildTarget, true);
            // Specific
            nativePlugin.SetPlatformData(buildTarget, "CPU", cpu);
            // Forcing flush
            EditorUtility.SetDirty(nativePlugin);
            AssetDatabase.WriteImportSettingsIfDirty(relativePath);
        }
        
    }
}

#endif