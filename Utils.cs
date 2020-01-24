using UnityEngine;
using System.Collections;

namespace Cirrus.Animations
{
    public static class Utils
    {
        public static float GetClipLength(RuntimeAnimatorController animator, string clipname)
        {
            if (animator == null)
                return -1;

            for (int i = 0; i < animator.animationClips.Length; i++)                 //For all animations
            {
                if (animator.animationClips[i].name == clipname)        //If it has the same name as your clip
                {
                    return animator.animationClips[i].length;
                }
            }

            return -1;
        }
    }
}
