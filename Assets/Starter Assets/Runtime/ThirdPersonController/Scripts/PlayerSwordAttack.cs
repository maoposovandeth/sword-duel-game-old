using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StarterAssets
{
    public class PlayerSwordAttack : MonoBehaviour
    {
        private static readonly int _swordAttackTrigger = Animator.StringToHash("SwordAttack");
        private static readonly int _swordAttackState = Animator.StringToHash("SwordAttack");

        private Animator _animator;
        private Vector3 _handHoldPosition;
        private Quaternion _handHoldRotation;
        private bool _damagePending;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            if (_animator == null || !_animator.isHuman)
            {
                Debug.LogError("PlayerSwordAttack requires a Humanoid Animator.", this);
                enabled = false;
                return;
            }

            Transform rightHand = _animator.GetBoneTransform(HumanBodyBones.RightHand);
            if (rightHand == null)
            {
                Debug.LogError("PlayerSwordAttack could not find the right hand bone.", this);
                enabled = false;
                return;
            }

            _handHoldPosition = transform.InverseTransformPoint(rightHand.position);
            _handHoldRotation = Quaternion.Inverse(transform.rotation) * rightHand.rotation;
        }

        private void OnAnimatorIK(int layerIndex)
        {
            float holdWeight = IsSwordAttackActive() ? 0f : 1f;
            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, holdWeight);
            _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, holdWeight);
            _animator.SetIKPosition(AvatarIKGoal.RightHand, transform.TransformPoint(_handHoldPosition));
            _animator.SetIKRotation(AvatarIKGoal.RightHand, transform.rotation * _handHoldRotation);
        }

        private void Update()
        {
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame &&
                !IsSwordAttackActive())
            {
                _animator.SetTrigger(_swordAttackTrigger);
                if (!_damagePending)
                {
                    StartCoroutine(ApplyAttackDamage());
                }
            }
        }

        private IEnumerator ApplyAttackDamage()
        {
            _damagePending = true;
            yield return new WaitForSeconds(0.3f);

            if (SwordDuelGameManager.Instance != null)
            {
                SwordDuelGameManager.Instance.TryDamageNearestEnemy(transform);
            }

            _damagePending = false;
        }

        private bool IsSwordAttackActive()
        {
            AnimatorStateInfo currentState = _animator.GetCurrentAnimatorStateInfo(0);
            if (currentState.shortNameHash == _swordAttackState)
            {
                return true;
            }

            return _animator.IsInTransition(0) &&
                   _animator.GetNextAnimatorStateInfo(0).shortNameHash == _swordAttackState;
        }
    }
}
