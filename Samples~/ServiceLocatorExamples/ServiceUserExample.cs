using IKhom.ServiceLocatorSystem.Runtime;
using ServiceLocatorSystem.Samples.ServiceLocatorExamples.MockServices;
using UnityEngine;

namespace ServiceLocatorSystem.Samples.ServiceLocatorExamples
{
    public class ServiceUserExample : MonoBehaviour
    {
        private IGameManagerService _gameManagerService;
        private IPlayerService _playerService;

        private void Start()
        {
            ServiceLocator.For(this)
                .Get(out _gameManagerService)
                .Get(out _playerService);

            //Safe check for service achieving
            if (!ServiceLocator.For(this).TryGet<IFirstSceneServiceOnly>(out _))
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