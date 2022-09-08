using UnityEngine;
using DCL.Interface;

public class WebInterfaceHomeLocationController : IHomeLocationController
{
    public void SetHomeScene(string location)
    {
        WebInterface.SetHomeScene(location);
    }

    public void SetHomeScene(Vector2 location)
    {
        WebInterface.SetHomeScene($"{location.x},{location.y}");
    }
}