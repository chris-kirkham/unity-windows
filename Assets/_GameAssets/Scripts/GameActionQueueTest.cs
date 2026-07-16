using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class GameActionQueueTest : MonoBehaviour, ICursorEventListener
{
    private class SpawnVFXAtPos : IGameAction
    {
        private GameObject vfxPrefab;
        private Vector3 pos;

        public SpawnVFXAtPos(GameObject vfxPrefab, Vector3 cursorPos)
        {
            this.vfxPrefab = vfxPrefab;
            this.pos = cursorPos;
        }

        public async Task Execute()
        {
            GameObject.Instantiate(vfxPrefab, pos, vfxPrefab.transform.rotation);
            await Task.Delay(1000);
        }

        public void Cancel()
        {
            throw new System.NotImplementedException();
        }
    }

    [SerializeField] private GameObject testVFXprefab;
    [SerializeField] private Cursor cursor;

    private GameActionQueue<SpawnVFXAtPos> spawnVFXActionQueue = new GameActionQueue<SpawnVFXAtPos>();

    private void OnEnable()
    {
        if(cursor)
        {
            cursor.AddCursorEventListener(this);
        }

        spawnVFXActionQueue.doDebugLog = true;
    }

    private void OnDisable()
    {
        if(cursor)
        {
            cursor.RemoveCursorEventListener(this);
        }
    }

    public void OnCursorEvent(Cursor.EventID e)
    {
        if(e == Cursor.EventID.LeftClickDown)
        {
            spawnVFXActionQueue.EnqeueAction(new SpawnVFXAtPos(testVFXprefab, cursor.ClampedPosition_WS + (cursor.Cam.transform.forward * 0.5f)));
        }
    }
}

