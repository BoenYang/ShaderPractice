using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MobaClient client = new MobaClient("127.0.0.1", 1001, MobaData.MyId);
        DontDestroyOnLoad(transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (MobaClient.Instance != null) {
            MobaClient.Instance.Update();
        }
    }
}
