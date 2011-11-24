﻿/*
 * Copyright (C) 2011 - Hüseyin Uslu shalafiraistlin@gmail.com
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using VolumetricStudios.VoxlrClient.Screen;
using VolumetricStudios.VoxlrEngine;
using VolumetricStudios.VoxlrEngine.Screen;
using VolumetricStudios.VoxlrEngine.Universe;

namespace VolumetricStudios.VoxlrClient.Input
{
    public class InputManager : GameComponent
    {
        private IWorldService _world;
        private IPlayer _player;
        private IScreenService _screenManager;
        private ICameraControlService _cameraController;
        private InGameDebuggerService _ingameDebuggerService;
        private MouseState _oldMouseState;
        private MouseState _oldMouseClickState;
        private KeyboardState _oldKeyboardState;
        private bool _mouseFocused = true;

        public InputManager(Game game)
            : base(game)
        {
            this.CenterMouse();    
            #if DEBUG 
            this.PrintDebugKeys(); 
            #endif
        }

        public override void Initialize()
        {
            this._world = (IWorldService) this.Game.Services.GetService(typeof (IWorldService));
            this._player = (IPlayer) this.Game.Services.GetService(typeof (IPlayer));
            this._screenManager = (IScreenService) this.Game.Services.GetService(typeof (IScreenService));
            this._cameraController = (ICameraControlService)this.Game.Services.GetService(typeof(ICameraControlService));
            this._ingameDebuggerService = (InGameDebuggerService)this.Game.Services.GetService(typeof(InGameDebuggerService));
            this._oldKeyboardState = Keyboard.GetState();
            this._oldMouseState = Mouse.GetState();
            this._oldMouseClickState = Mouse.GetState();
        }

        public override void Update(GameTime gameTime)
        {
            this.ProcessMouse();
            this.ProcessKeyboard(gameTime);
        }

        private void ProcessMouse()
        {
            MouseState currentState = Mouse.GetState();
            if (currentState == this._oldMouseState || !this._mouseFocused) return;

            float rotation = currentState.X - this._oldMouseState.X;
            if (rotation != 0) _cameraController.RotateCamera(rotation);

            float elevation = currentState.Y - this._oldMouseState.Y;
            if (elevation != 0) _cameraController.ElevateCamera(elevation);

            if (currentState.LeftButton == ButtonState.Pressed && _oldMouseClickState.LeftButton == ButtonState.Released) this._player.Weapon.Use();
            if (currentState.RightButton == ButtonState.Pressed && _oldMouseClickState.RightButton == ButtonState.Released) this._player.Weapon.SecondaryUse();

            this.CenterMouse();
            this._oldMouseClickState = currentState;
        }

        private void ProcessKeyboard(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Up) || keyState.IsKeyDown(Keys.W)) _player.Move(gameTime,MoveDirection.Forward);
            if (keyState.IsKeyDown(Keys.Down) || keyState.IsKeyDown(Keys.S)) _player.Move(gameTime, MoveDirection.Backward);
            if (keyState.IsKeyDown(Keys.Left) || keyState.IsKeyDown(Keys.A)) _player.Move(gameTime, MoveDirection.Left);
            if (keyState.IsKeyDown(Keys.Right) || keyState.IsKeyDown(Keys.D)) _player.Move(gameTime, MoveDirection.Right);
            if (_oldKeyboardState.IsKeyUp(Keys.Space) && keyState.IsKeyDown(Keys.Space)) _player.Jump();  

            if (_oldKeyboardState.IsKeyUp(Keys.F1) && keyState.IsKeyDown(Keys.F1)) this._world.ToggleInfinitiveWorld();
            if (_oldKeyboardState.IsKeyUp(Keys.F2) && keyState.IsKeyDown(Keys.F2)) this._player.ToggleFlyForm();
            if (_oldKeyboardState.IsKeyUp(Keys.F3) && keyState.IsKeyDown(Keys.F3)) this._world.ToggleFog();
            if (_oldKeyboardState.IsKeyUp(Keys.F4) && keyState.IsKeyDown(Keys.F4)) this._mouseFocused = !this._mouseFocused;

            if (_oldKeyboardState.IsKeyUp(Keys.F10) && keyState.IsKeyDown(Keys.F10)) this._ingameDebuggerService.ToggleInGameDebugger();
            if (_oldKeyboardState.IsKeyUp(Keys.F11) && keyState.IsKeyDown(Keys.F11)) this._screenManager.ToggleFPSLimiting();
            if (_oldKeyboardState.IsKeyUp(Keys.F12) && keyState.IsKeyDown(Keys.F12)) ((VoxlrClient) Game).ToggleRasterMode();

            this._oldKeyboardState = keyState;
        }

        private void CenterMouse()
        {
            Mouse.SetPosition(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2);
        }

        private void PrintDebugKeys()
        {
            Debug.WriteLine("Debug keys:");
            Debug.WriteLine("-----------------------------");
            Debug.WriteLine("F1: Infinitive-world: On/Off.");
            Debug.WriteLine("F2: Fly-mode: On/Off.");
            Debug.WriteLine("F3: Fog-mode: None/Near/Far.");
            Debug.WriteLine("F4: Window-focus: On/Off.");
            Debug.WriteLine("F10: In-game Debugger: On/Off.");
            Debug.WriteLine("F11: Frame-limiter: On/Off.");
            Debug.WriteLine("F12: Wireframe mode: On/Off.");
            Debug.WriteLine("-----------------------------");
        }
    }
}
