using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Rx : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var query = from google in ObservableWWW.Get("http://google.com/")
                    from bing in ObservableWWW.Get("http://bing.com/")
                    from unknown in ObservableWWW.Get(google + bing)
                    select new { google, bing, unknown };

        var cancel = query.Subscribe(x => Debug.Log(x));

        // Call Dispose is cancel.
        cancel.Dispose();
    }


}
