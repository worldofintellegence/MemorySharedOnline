using System;
using UnityEngine;

namespace Script
{
    public enum GameState
    {
        Start, Play, PlayAgain, Over,Ranking, ExtraLife, Pause,
      SecondGame,TrampolineJump
            
    }
    public enum GetGame
    {
        FirstGame, SecoundGame
    }
    public static class StateManager
    {
        public static bool Replay;
        private static GameState _state;
        private static GetGame _GetGame;
        public static event Action OnStateChange;

        public static GameState GetState()
        {
            return _state;
        }
        public static GetGame GetGameState()
        {
            return _GetGame;
        }
        public static void SetState(GameState value)
        {
            _state = value;
            OnStateChange?.Invoke();
        }

        public static bool IsPlaying()
        {
            return _state == GameState.Play;
        }
        
        
        public static bool IsOver()
        {
            return _state == GameState.Over;
        }
        public static bool IsExtraLife()
        {
            return _state == GameState.ExtraLife;
        }
        public static bool IsFirstGame()
        {
            return _GetGame == GetGame.FirstGame;
        }
        public static bool IsSecondGame()
        {
            return _state == GameState.SecondGame;
        } public static bool IsTrampoline()
        {
            return _state == GameState.TrampolineJump;
        }
    }
}
