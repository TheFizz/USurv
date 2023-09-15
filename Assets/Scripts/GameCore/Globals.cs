using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals : MonoBehaviour
{
    private static Globals me;

    [SerializeReference] private Transform _playerTransform;
    [SerializeReference] private Camera _mainCamera;
    public static HeatSystem Heat { get; private set; }
    public static InputHandler Input { get; private set; }
    public static UIManager UIManager { get; private set; }
    public static StatModifierTracker StatModTracker { get; private set; }
    public static ActionController ActionController { get; private set; }
    [field: SerializeField] public static Transform PlayerTransform { get; private set; }
    [SerializeField] public static Camera MainCamera { get; private set; }
    void Awake()
    {
        if (me != null)
        {
            Debug.LogError("There is more than one instance if Globals!");
            return;
        }
        Heat = GetComponent<HeatSystem>();
        Input = GetComponent<InputHandler>();
        UIManager = GetComponent<UIManager>();
        StatModTracker = GetComponent<StatModifierTracker>();
        ActionController = GetComponent<ActionController>();

        PlayerTransform = _playerTransform;
        MainCamera = _mainCamera;

        me = this;
    }

    public static float GetLargestValue(Vector3 v3, bool omitY = false)
    {
        if (omitY)
            v3.y = 0;
        return Mathf.Max(Mathf.Max(v3.x, v3.y), v3.z);
    }
}
