namespace RoR2DevTool.Models
{
    public class RunData
    {
        public string CurrentStage { get; set; }
        public int StageNumber { get; set; }
        public float DifficultyCoefficient { get; set; }
        public float GameTime { get; set; }
        public bool IsPaused { get; set; }
        public float TimeScale { get; set; }
        public string Seed { get; set; }
    }
}
