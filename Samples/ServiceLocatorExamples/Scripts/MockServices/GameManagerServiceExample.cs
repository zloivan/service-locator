namespace Utilities.ServiceLocator.Examples
{
    public interface IGameManagerService
    {
        void StartGame();
        void EndGame();
    }
    
    public class GameManagerServiceExample : IGameManagerService
    {
        private readonly ILoggingService _loggingService;

        public GameManagerServiceExample(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        public void StartGame()
        {
            _loggingService.Log("Game Started");
        }

        public void EndGame()
        {
            _loggingService.Log("Game Ended");
        }
    }
}