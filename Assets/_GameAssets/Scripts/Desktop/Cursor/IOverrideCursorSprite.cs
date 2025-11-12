using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public interface IOverrideCursorSprite
{
    public Cursor.CursorEvent OverrideOnInputEvent { get; }

    public Sprite CursorSpriteOverride { get; }
}
