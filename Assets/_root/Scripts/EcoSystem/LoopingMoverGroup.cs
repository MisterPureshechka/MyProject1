using Core;
using UnityEngine;

public class LoopingMoverGroup 
{
    private readonly SpriteRenderer[] _objects;
    private readonly float _spacing;
    private readonly float _rightBoundX;

    public LoopingMoverGroup(SpriteRenderer[] objects, float spacing, float rightBoundX)
    {
        _objects = objects;
        _spacing = spacing;
        _rightBoundX = rightBoundX;

        for (int i = 0; i < _objects.Length; i++)
        {
            _objects[i].transform.position += Vector3.right * _spacing * i;
        }
    }

    public void MoveObjectsLoop(float deltaTime, float speed)
    {
        foreach (var obj in _objects)
        {
            obj.transform.position += Vector3.right * speed * deltaTime;
        }

        for (int i = 0; i < _objects.Length; i++)
        {
            var obj = _objects[i];
            if (obj.transform.position.x > _rightBoundX)
            {
                Transform last = GetLeftmost();

                obj.transform.position = new Vector3(
                    last.position.x - _spacing,
                    obj.transform.position.y,
                    obj.transform.position.z
                );
            }
        }
    }

    private Transform GetLeftmost()
    {
        Transform result = _objects[0].transform;
        foreach (var obj in _objects)
        {
            if (obj.transform.position.x < result.position.x)
                result = obj.transform;
        }
        return result;
    }
}