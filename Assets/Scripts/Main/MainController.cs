using UnityEngine;
using System.Collections;
using Zenject;
using ModestTree;

namespace Algorithms
{
    public enum GameStates
    {
        WaitingToStart,
        Playing,
        GameOver,
		QuittingGame
    }

    public class MainController : IInitializable, ITickable 
    {
		GameStates _state = GameStates.WaitingToStart;

		public GameStates State
		{
			get
			{
				return _state;
			}
		}

        public void Tick()
        {
			switch (_state)
			{
				case GameStates.WaitingToStart:
				{
					break;
				}
				case GameStates.Playing:
				{
					break;
				}
				case GameStates.GameOver:
				{
					break;
				}
				case GameStates.QuittingGame:
				{
					QuitGame ();
					break;
				}
				default:
				{
					Assert.That(false);
					break;
				}
			}
        }

		void QuitGame()
		{
            Assert.That(_state == GameStates.QuittingGame);
			#if UNITY_STANDALONE
			//Quit the application
			Application.Quit();
			#endif
			
			//If we are running in the editor
			#if UNITY_EDITOR
			//Stop playing the scene
			UnityEditor.EditorApplication.isPlaying = false;
			#endif
		}

		//GUI Events

		void StartGame()
		{
            Assert.That(_state == GameStates.WaitingToStart || _state == GameStates.GameOver);

            _state = GameStates.Playing;
		}

		void QuittingGame()
		{
			_state = GameStates.QuittingGame;
		}

        void GameOver()
        {
            Assert.That(_state == GameStates.Playing);

            _state = GameStates.GameOver;
        }

        public void Initialize()
        {
			GUIEventManager.StartButtonAction += StartGame;
			GUIEventManager.QuitButtonAction += QuittingGame;
            GUIEventManager.RetryBtnEvent += StartGame;
            EventManager.SceneEnded += GameOver;
        }
		
    }
}