using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AutoDestroyAnimation : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private IEnumerator Start()
    {
        Debug.Log($"[animation] playing {name} before destroying");
        yield return null;

        var clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        if (clipInfo.Length == 0)
        {
            Debug.LogWarning("[animation] no clips found on " + name);
            yield break;
        }

        var clipLength = clipInfo[0].clip.length;
        yield return new WaitForSeconds(clipLength);
        Debug.Log("[animation] auto destroying " + name);
        Destroy(gameObject);
    }
}
