using UnityEditorInternal;
using UnityEngine;

public class TransitionController : MonoBehaviour
{
    EventBinding<TransitionCameraEvent> cameraEventBinding;
    public Animator transitionAnimator;
    void OnEnable()
    {
        cameraEventBinding = new EventBinding<TransitionCameraEvent>(OnCameraEvent);
        EventBus<TransitionCameraEvent>.Register(cameraEventBinding);
    }
    void OnDisable()
    {
        EventBus<TransitionCameraEvent>.Deregister(cameraEventBinding);
    }
    void OnCameraEvent(TransitionCameraEvent e)
    {
        Debug.Log("Received Transition Camera Event");
        transitionAnimator.Play(e.animationName,0);
    }
}
