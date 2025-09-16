using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Survivor.Classes.Core.Enums;
namespace Survivor.Classes.Controllers
{
    public class InputController
    {
        public static List<InputState> GetInput()
        {
            var keyboardState = Keyboard.GetState();
            List<InputState> inputs = new List<InputState>();
            if (keyboardState.IsKeyDown(Keys.J))
                inputs.Add(InputState.Attack);
            if (keyboardState.IsKeyDown(Keys.W))
                inputs.Add(InputState.Jump);
            if (keyboardState.IsKeyDown(Keys.A))
                inputs.Add(InputState.MoveLeft);
            if (keyboardState.IsKeyDown(Keys.D))
                inputs.Add(InputState.MoveRight);
            if (keyboardState.IsKeyDown(Keys.K))
                inputs.Add(InputState.Special);
            return inputs;
        }
    }
}