using UnityEngine;

public static class CamUtils
{
    public static bool GetWorldSpaceFollowAboveGroundPos(
        Camera cam,
        Vector3 pos_ScreenSpace,
        float targetDistAboveGround,
        LayerMask layerMask,
        float minDistFromCamera,
        out Vector3 pos_WorldSpace)
    {
        const float MaxRaycastDist = 100f;

        //get distance above ground/other objects
        var distFromCamera = targetDistAboveGround;
        if (Physics.Raycast(
            cam.ScreenPointToRay(pos_ScreenSpace), out var hit, MaxRaycastDist, layerMask, QueryTriggerInteraction.Ignore))
        {
            var hitPointWithHeightOffset = hit.point + (Vector3.up * targetDistAboveGround);
            distFromCamera = Mathf.Max(minDistFromCamera, Vector3.Distance(cam.transform.position, hitPointWithHeightOffset));
            pos_WorldSpace = hit.point + ((cam.transform.position - hit.point).normalized * targetDistAboveGround);
            return true;
        }

        pos_WorldSpace = Vector3.zero;
        return false;
    }

    public static bool RaycastFromCamera(Camera cam, Vector3 pos_ScreenSpace, out RaycastHit hit, float maxDistance, LayerMask layerMask)
    {
        return Physics.Raycast(cam.ScreenPointToRay(pos_ScreenSpace), out hit, maxDistance, layerMask);
    }
}
