using UnityEngine;

namespace Utilities.ServiceLocator.Examples
{
    public class ServiceUserExample : MonoBehaviour
    {
        private IGameManagerService _gameManagerService;
        private IPlayerService _playerService;

        private void Start()
        {
            _gameManagerService = ServiceLocator.Global.Get<IGameManagerService>();
            _playerService = ServiceLocator.Global.Get<IPlayerService>();

            //Safe check for service
            if (!ServiceLocator.For(this).TryGet<IFirstSceneServiceOnly>(out var firastSceneService))
            {
                Debug.LogWarning($"Service for {nameof(IFirstSceneServiceOnly)} is not available on that scene!");
            }
            _gameManagerService.StartGame();
            _playerService.InitializePlayer("Player1");

            Invoke(nameof(EndGame), 5f); 
        }

        private void EndGame()
        {
            _gameManagerService.EndGame();
        }
    }
}