using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;

[EditorTool("Grid Creation")]
public class TileTool : EditorTool
{
    [SerializeField] private Texture2D _ToolIcon;

    private GUIContent _IconContent;

    public override GUIContent toolbarIcon => _IconContent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        _IconContent = new GUIContent()
        {
            image = _ToolIcon,
            text = "Grid Creation",
            tooltip = "Grid Creation"
        };
    }

    private void OnDisable()
    {
        
    }
}
