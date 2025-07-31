using System.Runtime.InteropServices;
using UnityEngine;

public class UnityConnector : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void JoinGroupAndSendMessage();

    public static UnityConnector Singleton { get; private set; }

    private void Awake()
    {
        if (Singleton == null)
            Singleton = this;
        else
            Destroy(gameObject);
    }

    public void OnJoinGroupButtonClick()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        JoinGroupAndSendMessage();
#endif
    }
}