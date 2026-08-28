using UnityEngine;

public class TransitionButton : MonoBehaviour
{
    public void OnClick()
    {
        FadeManager.Instance.StartTransition();
    }
}