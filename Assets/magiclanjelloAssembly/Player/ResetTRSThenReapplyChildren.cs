using System.Collections;
using System.Collections.Generic;
using UKnack;
using UKnack.Commands;
using UnityEngine;

public class ResetTRSThenReapplyChildren : CommandMonoBehaviour
{
    public override void Execute()
    {
        int childCount = transform.childCount;
        Vector3[] positions = new Vector3[childCount];
        Quaternion[] rotations = new Quaternion[childCount];

        SaveChildRotPos(transform, childCount, positions, rotations);
        ResetTransform(transform);
        RestoreChildRotPos(transform, childCount, positions, rotations);
    }

    private static void SaveChildRotPos(Transform transform, int childCount, Vector3[] positions, Quaternion[] rotations)
    {
        for (int i = 0; i < childCount; i++)
        {
            var child = transform.GetChild(i);
            rotations[i] = child.rotation;
            positions[i] = child.position;
        }
    }

    private static void ResetTransform(Transform transform)
    {
        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;
    }

    private static void RestoreChildRotPos(Transform transform, int childCount, Vector3[] positions, Quaternion[] rotations)
    {
        for (int i = 0; i < childCount; i++)
        {
            var child = transform.GetChild(i);
            child.rotation = rotations[i];
            child.position = positions[i];
        }
    }
}
