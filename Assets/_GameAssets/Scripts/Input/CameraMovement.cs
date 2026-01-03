using UnityEngine;

public class CameraMovement : MonoBehaviour, ICursorEventListener
{
    [SerializeField] private Camera cam;
    [SerializeField] private Vector2 viewCentre;
    [SerializeField] private Vector2 viewSize;
    [SerializeField] private float moveSpeed = 0.2f;

    private bool isDragging;
    private Vector2 mouseDelta;

    public Vector2 ViewCentre => viewCentre;
    private Vector2 ViewMin => viewCentre - (viewSize / 2f);
    private Vector2 ViewMax => viewCentre + (viewSize / 2f);

    private void Start()
    {
        if(Cursor.InstExists())
        {
            Cursor.Inst.AddCursorEventListener(this);
        }
    }

    private void OnDisable()
    {
        if (Cursor.InstExists())
        {
            Cursor.Inst.RemoveCursorEventListener(this);
        }
    }

    private void Update()
    {
        if(isDragging)
        {
            mouseDelta = Cursor.Inst.RawPositionDelta;
        }
    }

    private void LateUpdate()
    {
        if(isDragging)
        {
            cam.transform.position = cam.transform.position + (Vector3)(mouseDelta * moveSpeed);
        }

        ClampCameraPos();
        
        if(!Cursor.Inst.IsRightClickPressed)
        {
            SetDragging(false);
        }
    }

    private void ClampCameraPos()
    {
        var camHalfSize = cam.rect.size / 2f;
        var camMin = (Vector2)cam.transform.position - camHalfSize;
        var camMax = (Vector2)cam.transform.position + camHalfSize;

        if(camMin.x < ViewMin.x || camMin.y < ViewMin.y)
        {
            var minX = Mathf.Max(camMin.x, ViewMin.x);
            var minY = Mathf.Max(camMin.y, ViewMin.y);

            var centreOffset = camHalfSize;
            cam.transform.position = new Vector3(minX + centreOffset.x, minY + centreOffset.y, cam.transform.position.z);
        }
        else if(camMax.x > ViewMax.x || camMax.y > ViewMax.y)
        {
            var maxX = Mathf.Min(camMax.x, ViewMax.x);
            var maxY = Mathf.Min(camMax.y, ViewMax.y);

            var centreOffset = camHalfSize;
            cam.transform.position = new Vector3(maxX - centreOffset.x, maxY - centreOffset.y, cam.transform.position.z);
        }
    }

    private void SetDragging(bool dragging)
    {
        isDragging = dragging;
        Cursor.Inst.FreezeCursorPos(dragging);
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.green;
        var size = ViewMax - ViewMin;
        Gizmos.DrawWireCube(ViewMin + (size / 2f), size);
    }

    public void OnCursorEvent(Cursor.CursorEvent e)
    {
        if(e == Cursor.CursorEvent.RightClickDown)
        {
            SetDragging(true);
        }
    }
}
