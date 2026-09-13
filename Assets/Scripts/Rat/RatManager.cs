using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatManager : MonoBehaviour
{
    [Header("References")]
    [Space(10)]
    [SerializeField]
    [Tooltip("The rat prefab to spawn at runtime.")]
    private GameObject ratPrefab;

    [Header("Settings")]
    [Space(10)]
    [SerializeField]
    [Tooltip("The number of rats to spawn.")]
    private int numRats = 5;

    [SerializeField]
    [Tooltip("The X position in world coordinates to start the row of rats.")]
    private int startX = 0;

    [SerializeField]
    [Tooltip("The amount of space between each rat in world units.")]
    private int spacing = 5;

    [SerializeField]
    [Tooltip("The delay (in seconds) between individual rats jumping.")]
    private float ratDelay = 0.5f;

    [SerializeField]
    [Tooltip("The delay (in seconds) between each wave of jumping rats.")]
    private float waveDelay = 0.5f;

    private enum JumpPattern
    {
        LeftToRight,
        RightToLeft,
        OutsideIn,
        InsideOut,
        Alternating,
    }

    [SerializeField]
    [Tooltip("The order the rats jump in.")]
    private JumpPattern jumpPattern;

    private float ratWidth;
    private List<Animator> animators = new List<Animator>();
    private List<int> jumpOrder = new List<int>();

    void Start()
    {
        ratWidth = ratPrefab.GetComponentInChildren<SkinnedMeshRenderer>().bounds.size.x;

        for (int i = 0; i < numRats; i++)
        {
            float x = startX + i * (ratWidth + spacing);
            float y = 0f;
            float z = 0f;
            Vector3 pos = new Vector3(x, y, z);
            Quaternion rot = Quaternion.Euler(0f, 180f, 0f);

            GameObject rat = Instantiate(ratPrefab, pos, rot);
            rat.transform.SetParent(transform, false);

            animators.Add(rat.GetComponent<Animator>());
        }

        BuildJumpOrder();
        StartCoroutine(JumpWave());
    }

    private void BuildJumpOrder()
    {
        int count = animators.Count;

        switch (jumpPattern)
        {
            case JumpPattern.LeftToRight:
            {
                for (int i = 0; i < count; i++)
                {
                    jumpOrder.Add(i);
                }
                break;
            }

            case JumpPattern.RightToLeft:
            {
                for (int i = count - 1; i >= 0; i--)
                {
                    jumpOrder.Add(i);
                }
                break;
            }

            case JumpPattern.OutsideIn:
            {
                int left = 0;
                int right = count - 1;

                while (left <= right)
                {
                    jumpOrder.Add(left++);

                    if (left <= right)
                    {
                        jumpOrder.Add(right--);
                    }
                }
                break;
            }

            case JumpPattern.InsideOut:
            {
                int left = (count - 1) / 2;
                int right = left + 1;

                while (left >= 0 || right < count)
                {
                    if (left >= 0)
                    {
                        jumpOrder.Add(left--);
                    }

                    if (right < count)
                    {
                        jumpOrder.Add(right++);
                    }
                }
                break;
            }

            case JumpPattern.Alternating:
            {
                for (int i = 0; i < count; i += 2)
                {
                    jumpOrder.Add(i);
                }

                for (int i = 1; i < count; i += 2)
                {
                    jumpOrder.Add(i);
                }
                break;
            }
        }
    }

    private IEnumerator JumpWave()
    {
        while (true)
        {
            for (int i = 0; i < jumpOrder.Count; i++)
            {
                animators[jumpOrder[i]].SetTrigger("Jump");
                yield return new WaitForSeconds(ratDelay);
            }

            yield return new WaitForSeconds(waveDelay);
        }
    }
}
