using ClipperLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryButton : ElectricalNode
{
    [Header("Factory Button")]
    [SerializeField] private bool logTrace = false;
    private Animator _animator;
    private Collider2D _buttonCollider;
    private int numObjectsOn;

    private new void Awake()
    {
        base.Awake();
        nodeType = NodeType.INPUT;

        _buttonCollider = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        bool buttonPressed = GetButtonPressed();
        if (buttonPressed != Powered)
        {
            Switch(buttonPressed);
        }
    }

    public void Switch(bool powered)
    {
        StartSignal(powered);
        _animator.SetBool("Powered", powered);
    }

    private bool GetButtonPressed()
    {
        int layerMask = LayerMask.GetMask("Player", "ButtonTrigger");
        var filter = new ContactFilter2D();
        filter.useTriggers = true;
        filter.layerMask = layerMask;
        filter.useLayerMask = true;

        var contacts = new List<Collider2D>();
        _buttonCollider.GetContacts(filter, contacts);

        foreach (Collider2D other in contacts)
        {
            int otherLayer = 1 << other.gameObject.layer;
            LogTrace($"Other Layer: {LayerMask.LayerToName(other.gameObject.layer)}");
            bool correctLayer = (layerMask & otherLayer) != 0;
            if (correctLayer && CheckStandingOn(other))
            {
                return true;
            }
        }

        return false;
    }

    private bool CheckStandingOn(Collider2D other)
    {
        Vector3 floorPoint;
        if (other is BoxCollider2D)
        {
            BoxCollider2D bc = other as BoxCollider2D;
            floorPoint = bc.bounds.center + Vector3.down * bc.bounds.extents.y;
            LogTrace($"Floor Point Box: {floorPoint}");
        } else if (other is CircleCollider2D)
        {
            CircleCollider2D cc = other as CircleCollider2D;
            floorPoint = cc.bounds.center + Vector3.down * cc.radius;
            LogTrace($"Floor Point Circle: {floorPoint}");
        } else
        {
            //L: I'm pretty sure we're only using box colliders and circle colliders for items, but otherwise this will still
            //work
            floorPoint = other.gameObject.transform.position;
            LogTrace($"Floor Point Anything: {floorPoint}");
        }

        return _buttonCollider.OverlapPoint(floorPoint);
    }

    private void LogTrace(string s)
    {
        if (logTrace)
        {
            Debug.Log(s);
        }
    }
}
