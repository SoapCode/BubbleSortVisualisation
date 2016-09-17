using System;

namespace Algorithms
{ 

    public static class EventManager{

        public static Action SceneEnded = delegate { };
        public static Action CameraMovementEnded = delegate { };
    }
}