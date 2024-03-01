using Snowy.Input;

namespace Systems.FPS_Movement
{
    public class SuperCamera : OnlineClass
    {
        protected InputManager Input;
        protected virtual void Start()
        {
            if (isOffline) Initialize(InputManager.Instance);
        }
        
        protected virtual void Initialize(InputManager input)
        {
            Input = input;
        }
    }
}