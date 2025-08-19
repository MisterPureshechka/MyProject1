using System;
using System.Collections.Generic;
using Core;
using UnityEngine;

public class Controllers : IInitialization, IExecute, IFixedExecute, ICleanUp, IDisposable
{
    private readonly List<IInitialization> _initialize = new();
    private readonly List<IExecute> _execute = new();
    private readonly List<IFixedExecute> _fixedExecute = new();
    private readonly List<ICleanUp> _cleanup = new();
    private bool _disposed;

    public Controllers Add(IController c)
    {
        if (c is IInitialization i) _initialize.Add(i);
        if (c is IExecute e) _execute.Add(e);
        if (c is IFixedExecute f) _fixedExecute.Add(f);
        if (c is ICleanUp cl) _cleanup.Add(cl);
        return this;
    }

    public void Initialization() { for (int i=0;i<_initialize.Count;i++) _initialize[i].Initialize(); }
    public void Execute(float dt) { for (int i=0;i<_execute.Count;i++) _execute[i].Execute(dt); }
    public void FixedExecute(float fdt) { for (int i=0;i<_fixedExecute.Count;i++) _fixedExecute[i].FixedExecute(fdt); }

    public void CleanUp()
    {
        for (int i=_cleanup.Count-1; i>=0; i--)
        {
            try { _cleanup[i].CleanUp(); }
            catch (Exception ex) { Debug.LogException(ex); }
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        CleanUp();
        _initialize.Clear();
        _execute.Clear();
        _fixedExecute.Clear();
        _cleanup.Clear();
    }

    public void Initialize()
    {
        
    }
}
    
