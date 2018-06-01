using System.Collections;
using UnityEngine;
using SimpleJSONGameAI;
using System.Text;

using UnityEngine.Networking;
namespace summerschoolai.detail
{
    public class SummerSchoolAIHttpClient : MonoBehaviour
    {
        public static SummerSchoolAIHttpClient instance = null;

		public void sendPost(string apiKey, JSONObject jsonDict)
        {
			StartCoroutine(postJSONRequest(apiKey, jsonDict));
        }
        
		public IEnumerator postJSONRequest( string apiKey, JSONObject jsonDict)
        {
			var content = jsonDict.ToString();
			byte[] byteContentRaw = new UTF8Encoding().GetBytes(content);
            var uploadHandler = (UploadHandler)new UploadHandlerRaw(byteContentRaw);

            UnityWebRequest gwr = new UnityWebRequest();
            using (gwr)
            {
                gwr.method = "POST";
                gwr.url = "https://summerschoolai-e0ae.restdb.io/rest/game";
                gwr.uploadHandler = uploadHandler;
                gwr.SetRequestHeader("Content-Type", "application/json");
				gwr.SetRequestHeader("x-apikey", apiKey);
                gwr.chunkedTransfer = false;
                yield return gwr.SendWebRequest();
                if (gwr.isNetworkError || gwr.isHttpError)
                {
                    Debug.Log("{\"error\": {  \"isHttpError\": \"" + gwr.isHttpError + "\",  \"isNetworkError\": \"" + gwr.isNetworkError + "\" } }");
                }
                yield break;
            }

        }

        void Awake()
        {
            //Check if instance already exists
            if (instance == null)
            {
                //if not, set instance to this
                instance = this;
            }
            //If instance already exists and it's not this:
            else if (instance != this)
            {
                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
                Destroy(gameObject);
            }
            //Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(gameObject);
        }
    }
}


