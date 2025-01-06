using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] Animator animator;
    
    string isWalkingParam = "IsWalking";
    string isRunningParam = "IsRunning";
    string isMiningParam = "IsMining";

    private void Update()
    {
        animator.SetBool(isWalkingParam, playerController.IsWalking);
        animator.SetBool(isRunningParam, playerController.IsRunning);
        animator.SetBool(isMiningParam, Player.Instance.IsMining);
    }
}
