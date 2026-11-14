using UnityEngine;

namespace StarterAssets
{
    public sealed class EnemyAnimationEventReceiver : MonoBehaviour
    {
        // The shared clips emit these events, but enemies have no footstep audio.
        public void OnFootstep(AnimationEvent animationEvent)
        {
        }

        public void OnLand(AnimationEvent animationEvent)
        {
        }
    }
}
