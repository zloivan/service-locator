namespace ServiceLocatorSystem.Samples.ServiceLocatorExamples.MockServices
{
    public interface IPlayerService
    {
        void InitializePlayer(string playerName);
    }
    public class PlayerServiceExample : IPlayerService
    {
        private readonly ILoggingService _loggingService;

        public PlayerServiceExample(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        public void InitializePlayer(string playerName)
        {
            _loggingService.Log($"Player {playerName} initialized.");
        }
    }
}