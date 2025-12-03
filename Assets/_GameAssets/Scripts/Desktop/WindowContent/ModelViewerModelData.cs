using UnityEngine;

[CreateAssetMenu(fileName = "Model Viewer Model Data", menuName = "ScriptableObjects/Model Viewer Model Data")]
public class ModelViewerModelData : ScriptableObject
{
    [SerializeField] private Mesh mesh;
    [SerializeField] private Vector3 meshPosition;
    [SerializeField] private Vector3 meshRotation;
    [SerializeField] private float meshScale;
    [SerializeField] private float stageScale;
    [SerializeField] private float backgroundScale;
}
