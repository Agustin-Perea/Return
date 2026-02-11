using UnityEngine;

public class TransitionController : MonoBehaviour
{
    EventBinding<CameraEvent> cameraEventBinding;
    public Animator transitionAnimator;
    void OnEnable()
    {
        cameraEventBinding = new EventBinding<CameraEvent>(OnCameraEvent);
        EventBus<CameraEvent>.Register(cameraEventBinding);
    }
    void OnDisable()
    {
        EventBus<CameraEvent>.Deregister(cameraEventBinding);
    }
    void OnCameraEvent(CameraEvent e)
    {
        Debug.Log("Received Camera Event");
        transitionAnimator.Play("TransitionAmination",0);
    }
}
