using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Rooms
{
    public interface IRoomInitializer
    {
        List<IInteractiveObject> GetAllInteravtiveObjects();
        Vector3 GetInitialPosition();
        float GetRoomSize();
        
        
    }
}