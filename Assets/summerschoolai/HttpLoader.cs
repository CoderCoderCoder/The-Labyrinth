using UnityEngine;
using summerschoolai;

public class HttpLoader : MonoBehaviour
{
    public GameObject summerSchoolAIHttpClient; //GameManager prefab to instantiate.

    void Awake()
    {
        //Check if a goedleManager has already been assigned to static variable goedleManager.instance or if it's still null
		if (summerschoolai.detail.SummerSchoolAIHttpClient.instance == null)
            //Instantiate gameManager prefab
			Instantiate(summerSchoolAIHttpClient);
    }
}
