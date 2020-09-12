using System;
using System.Collections;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

namespace RetroUnity {
    public class GameManager : MonoBehaviour {

        [SerializeField] private string CoreName = "snes9x_libretro";
        [SerializeField] private string RomName = "Chrono Trigger (USA).sfc";
        private LibretroWrapper.Wrapper wrapper;

        private float _frameTimer;

        public Renderer Display;

        private void Awake() {
            LoadRom(Application.streamingAssetsPath + "/" + RomName);
        }

        private void Update() {
            if (wrapper != null) {
                _frameTimer += Time.deltaTime;
                float timePerFrame = 1f / (float)wrapper.GetAVInfo().timing.fps;

                while (_frameTimer >= timePerFrame)
                {
                    wrapper.Update();
                    _frameTimer -= timePerFrame;
                }
            }
            if (LibretroWrapper.tex != null) {
                Display.material.mainTexture = LibretroWrapper.tex;
            }
        }

        private IEnumerator RequestRoutine(string url, Action<byte[]> callback = null)
        {
            // Using the static constructor
            var request = UnityWebRequest.Get(url);
 
            // Wait for the response and then get our data
            yield return request.SendWebRequest();
            var data = request.downloadHandler.data;
 
            // This isn't required, but I prefer to pass in a callback so that I can
            // act on the response data outside of this function
            if (callback != null)
                callback(data);
        }        
        
        public void LoadRom(string romPath) {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
            // If the file doesn't exist the application gets stuck in a loop.
            if (!File.Exists(romPath))
            {
                Debug.LogError(romPath + " not found.");
                return;
            }
#endif

            Display.material.color = Color.white;

            wrapper = new LibretroWrapper.Wrapper(CoreName);
            wrapper.Init();
            
#if UNITY_ANDROID
        Action<byte[]> AfterReadingRomFromAPK = bytes =>
        {
            // Using persistentDataPath
            romPath = Path.Combine(Application.persistentDataPath, RomName);
            // Write
            File.WriteAllBytes(romPath, bytes);
            Debug.Log($"Copied to {romPath}");
            // Load Rom
            wrapper.LoadGame(romPath);
        };
        // Async
        StartCoroutine(RequestRoutine(romPath, AfterReadingRomFromAPK));
#else
            wrapper.LoadGame(romPath);
#endif            
            
        }

        private void OnDestroy() {
            wrapper.DLLHandler.UnloadCore();
        }
    }
}
