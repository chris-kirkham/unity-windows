using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEditor.Progress;

[System.Serializable]
public class ItemInspectSequence
{
    [SerializeField] private float lerpToInspectPosTime = 1f;
    [SerializeField] private float inspectHoldTime = 1f;

    private Cursor cursor;
    private CraftingItem item;
    private bool isInspecting;
    
    public void OnEnable(Cursor cursor)
    {
        this.cursor = cursor;
        //((ICursorEventListener)this).RegisterListener(cursor);
    }

    public void OnDisable()
    {
        //((ICursorEventListener)this).DeregisterListener(cursor);
    }

    public async Task InspectItem(CraftingItem item)
    {
        this.item = item;
        isInspecting = true;
        item.SetOnInspectVFX(true);
        item.SetState(CraftingItem.State.Draggable);

        var cam = cursor.Cam;
        //item.transform.localScale = Vector3.zero;
        var targetPos = cam.transform.position + (cam.transform.forward * 2f);
        
        item.SetDragPositionOverride(targetPos);

        await Tweening.DoTransform(
            item.transform,
            targetPos,
            cam.transform.rotation * Quaternion.Euler(-90f, 0f, 0f),
            Vector3.one,
            lerpToInspectPosTime).AsyncWaitForCompletion();


        while (isInspecting)
        {
            await Task.Yield();
        }
    }

    public void Cancel()
    {
        item.transform.DORotateQuaternion(Quaternion.identity, lerpToInspectPosTime);
        item.SetDragPositionOverride(null);
        item.SetState(CraftingItem.State.Active);
        item.SetOnInspectVFX(false);
        isInspecting = false;
    }
}
