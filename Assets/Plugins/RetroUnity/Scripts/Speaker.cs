using System.Collections.Generic;
using UnityEngine;

namespace RetroUnity {
    [RequireComponent(typeof(AudioSource))]
    public class Speaker : MonoBehaviour {

        public const int AudioBatchSize = 65536;
        public static List<float> AudioBatch = new List<float>(AudioBatchSize);

        private AudioSource _speaker;

        private void Start() {
            _speaker = GetComponent<AudioSource>();
            if (_speaker == null) return;
            _speaker.Play();
        }

        private void OnAudioFilterRead(float[] data, int channels) {
            // wait until enough data is available
            if (AudioBatch.Count < data.Length)
                return;
            for (var i = 0; i < data.Length; i++)
                data[i] = AudioBatch[i];
            // remove data from the beginning
            AudioBatch.RemoveRange(0, data.Length);
        }
        
        public void ProcessSamples(float[] samples)
        {
           foreach (var value in samples)
           {
               AudioBatch.Add(value);
           }
        }        
        

        private void OnGUI() {
            GUI.Label(new Rect(0f, 0f, 300f, 20f), AudioBatch.Count.ToString());
        }
    }
}
